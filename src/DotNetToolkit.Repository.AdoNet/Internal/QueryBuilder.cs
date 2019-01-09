namespace DotNetToolkit.Repository.AdoNet.Internal
{
    using Configuration.Conventions;
    using Extensions;
    using Helpers;
    using Properties;
    using Queries;
    using Queries.Strategies;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    internal class QueryBuilder
    {
        #region Fields

        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, PropertyInfo>> _sqlPropertiesMapping = new ConcurrentDictionary<Type, ConcurrentDictionary<string, PropertyInfo>>();

        // TODO: NEEDS TO FIGURE OUT A BETTER WAY TO DO THIS
        private static readonly Lazy<string> _crossJoinCountColumnName = new Lazy<string>(() => "Counter_" + Guid.NewGuid().ToString("N"));
        private readonly DataAccessProviderType _providerType;
        
        #endregion

        #region Preperties

        public static string CrossJoinCountColumnName { get { return _crossJoinCountColumnName.Value; } }

        #endregion

        #region Constructors

        public QueryBuilder(DataAccessProviderType providerType)
        {
            _providerType = providerType;
        }

        #endregion

        #region Public Methods

        public void PrepareCountQuery<T>(IQueryOptions<T> options, out Mapper mapper) where T : class
        {
            var parameters = new Dictionary<string, object>();
            var sb = new StringBuilder();
            var joinStatementSb = new StringBuilder();
            var mainTableType = typeof(T);
            var mainTableName = mainTableType.GetTableName();
            var m = new Mapper(mainTableType);
            var mainTableAlias = m.GetTableAlias(mainTableName);
            var mainTableProperties = mainTableType.GetRuntimeProperties().ToList();
            var mainTablePrimaryKeyPropertyInfo = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos<T>().First();
            var mainTablePrimaryKeyName = mainTablePrimaryKeyPropertyInfo.GetColumnName();
            var fetchStrategy = options?.FetchStrategy;

            // -----------------------------------------------------------------------------------------------------------
            // Select clause
            // -----------------------------------------------------------------------------------------------------------
            sb.Append("SELECT COUNT(*)");
            sb.Append(Environment.NewLine);
            sb.Append($"FROM [{mainTableName}] AS [{mainTableAlias}]");

            // Check to see if we can automatically include some navigation properties (this seems to be the behavior of entity framework as well).
            // Only supports a one to one table join for now...
            if (fetchStrategy == null || !fetchStrategy.PropertyPaths.Any())
            {
                // Assumes we want to perform a join when the navigation property from the primary table has also a navigation property of
                // the same type as the primary table
                // Only do a join when the primary table has a foreign key property for the join table
                var paths = mainTableProperties
                    .Where(x => x.IsComplex() && PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(x.PropertyType).Any())
                    .Select(x => x.Name)
                    .ToList();

                if (paths.Count > 0)
                {
                    if (fetchStrategy == null)
                        fetchStrategy = new FetchQueryStrategy<T>();

                    foreach (var path in paths)
                    {
                        fetchStrategy.Fetch(path);
                    }
                }
            }

            // Append join tables from fetchStrategy
            // Only supports a one to one table join for now...
            if (fetchStrategy != null && fetchStrategy.PropertyPaths.Any())
            {
                foreach (var path in fetchStrategy.PropertyPaths)
                {
                    var joinTablePropertyInfo = mainTableProperties.Single(x => x.Name.Equals(path));
                    var joinTableType = joinTablePropertyInfo.PropertyType.IsGenericCollection()
                        ? joinTablePropertyInfo.PropertyType.GetGenericArguments().First()
                        : joinTablePropertyInfo.PropertyType;
                    var joinTableForeignKeyPropertyInfo = ForeignKeyConventionHelper.GetForeignKeyPropertyInfos(joinTableType, mainTableType).FirstOrDefault();

                    // Only do a join when the primary table has a foreign key property for the join table
                    if (joinTableForeignKeyPropertyInfo != null)
                    {
                        var joinTableForeignKeyName = joinTableForeignKeyPropertyInfo.GetColumnName();
                        var joinTableProperties = joinTableType.GetRuntimeProperties().ToList();
                        var joinTableName = joinTableType.GetTableName();
                        var joinTableAlias = m.GenerateTableAlias(joinTableType);

                        foreach (var x in joinTableProperties.Where(Extensions.PropertyInfoExtensions.IsPrimitive))
                        {
                            m.GenerateColumnAlias(x);
                        }

                        joinStatementSb.Append(Environment.NewLine);
                        joinStatementSb.Append($"LEFT OUTER JOIN [{joinTableName}] AS [{joinTableAlias}] ON [{mainTableAlias}].[{mainTablePrimaryKeyName}] = [{joinTableAlias}].[{joinTableForeignKeyName}]");

                        m.SqlNavigationPropertiesMapping.Add(joinTableType, joinTableProperties.ToDictionary(ModelConventionHelper.GetColumnName, x => x));
                    }
                }

                if (joinStatementSb.Length > 0)
                {
                    joinStatementSb.Remove(0, Environment.NewLine.Length);

                    sb.Append(Environment.NewLine);
                    sb.Append(joinStatementSb);
                }
            }

            if (options != null)
            {
                // -----------------------------------------------------------------------------------------------------------
                // Where clause
                // -----------------------------------------------------------------------------------------------------------

                if (options.SpecificationStrategy != null)
                {
                    new ExpressionTranslator().Translate(
                        options.SpecificationStrategy.Predicate,
                        m,
                        out string expSql,
                        out Dictionary<string, object> expParameters);

                    sb.Append($"{Environment.NewLine}WHERE ");
                    sb.Append(expSql);

                    foreach (var item in expParameters)
                    {
                        parameters.Add(item.Key, item.Value);
                    }
                }
            }

            // Setup mapper object
            m.Sql = sb.ToString();
            m.Parameters = parameters;

            mapper = m;
        }

        public void PrepareDefaultSelectQuery<T>(IQueryOptions<T> options, out Mapper mapper, bool allowCrossJoinTotalCount = false) where T : class
        {
            var parameters = new Dictionary<string, object>();
            var sb = new StringBuilder();
            var joinStatementSb = new StringBuilder();
            var mainTableType = typeof(T);
            var mainTableName = mainTableType.GetTableName();
            var m = new Mapper(mainTableType);
            var mainTableAlias = m.GetTableAlias(mainTableName);
            var mainTableProperties = mainTableType.GetRuntimeProperties().ToList();
            var mainTablePrimaryKeyPropertyInfo = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos<T>().First();
            var mainTablePrimaryKeyName = mainTablePrimaryKeyPropertyInfo.GetColumnName();
            var fetchStrategy = options?.FetchStrategy;

            const string CrossJoinTableName = "temp1";

            // Default select
            var columns = string.Join($",{Environment.NewLine}\t", m.SqlPropertiesMapping.Select(x =>
            {
                var colAlias = m.GetColumnAlias(x.Value);
                var colName = x.Value.GetColumnName();

                return $"[{mainTableAlias}].[{colName}] AS [{colAlias}]";
            }));

            // Check to see if we can automatically include some navigation properties (this seems to be the behavior of entity framework as well).
            // Only supports a one to one table join for now...
            if (fetchStrategy == null || !fetchStrategy.PropertyPaths.Any())
            {
                // Assumes we want to perform a join when the navigation property from the primary table has also a navigation property of
                // the same type as the primary table
                // Only do a join when the primary table has a foreign key property for the join table
                var paths = mainTableProperties
                    .Where(x => x.IsComplex() && PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(x.PropertyType).Any())
                    .Select(x => x.Name)
                    .ToList();

                if (paths.Count > 0)
                {
                    if (fetchStrategy == null)
                        fetchStrategy = new FetchQueryStrategy<T>();

                    foreach (var path in paths)
                    {
                        fetchStrategy.Fetch(path);
                    }
                }
            }

            // -----------------------------------------------------------------------------------------------------------
            // Select clause
            // -----------------------------------------------------------------------------------------------------------

            // Append join tables from fetchStrategy
            // Only supports a one to one table join for now...
            if (fetchStrategy != null && fetchStrategy.PropertyPaths.Any())
            {
                sb.Append($"SELECT{Environment.NewLine}\t{columns}");

                foreach (var path in fetchStrategy.PropertyPaths)
                {
                    var joinTablePropertyInfo = mainTableProperties.Single(x => x.Name.Equals(path));
                    var joinTableType = joinTablePropertyInfo.PropertyType.IsGenericCollection()
                        ? joinTablePropertyInfo.PropertyType.GetGenericArguments().First()
                        : joinTablePropertyInfo.PropertyType;
                    var joinTableForeignKeyPropertyInfo = ForeignKeyConventionHelper.GetForeignKeyPropertyInfos(joinTableType, mainTableType).FirstOrDefault();

                    // Only do a join when the primary table has a foreign key property for the join table
                    if (joinTableForeignKeyPropertyInfo != null)
                    {
                        var joinTableForeignKeyName = joinTableForeignKeyPropertyInfo.GetColumnName();
                        var joinTableProperties = joinTableType.GetRuntimeProperties().ToList();
                        var joinTableName = joinTableType.GetTableName();
                        var joinTableAlias = m.GenerateTableAlias(joinTableType);
                        var joinTableColumnNames = string.Join($",{Environment.NewLine}\t",
                            joinTableProperties
                                .Where(Extensions.PropertyInfoExtensions.IsPrimitive)
                                .Select(x =>
                                {
                                    var colAlias = m.GenerateColumnAlias(x);
                                    var colName = x.GetColumnName();

                                    return $"[{joinTableAlias}].[{colName}] AS [{colAlias}]";
                                }));


                        sb.Append($",{Environment.NewLine}\t");
                        sb.Append(joinTableColumnNames);

                        joinStatementSb.Append(Environment.NewLine);
                        joinStatementSb.Append($"LEFT OUTER JOIN [{joinTableName}] AS [{joinTableAlias}] ON [{mainTableAlias}].[{mainTablePrimaryKeyName}] = [{joinTableAlias}].[{joinTableForeignKeyName}]");

                        m.SqlNavigationPropertiesMapping.Add(joinTableType, joinTableProperties.ToDictionary(ModelConventionHelper.GetColumnName, x => x));
                    }
                }

                if (options != null && options.PageSize != -1 && allowCrossJoinTotalCount)
                {
                    // Cross join counter column
                    sb.Append(",");
                    sb.Append(Environment.NewLine);
                    sb.Append($"\t[{CrossJoinTableName}].[{CrossJoinCountColumnName}] AS [{CrossJoinCountColumnName}]");
                }

                sb.Append(Environment.NewLine);
                sb.Append($"FROM [{mainTableName}] AS [{mainTableAlias}]");

                if (joinStatementSb.Length > 0)
                {
                    joinStatementSb.Remove(0, Environment.NewLine.Length);

                    sb.Append(Environment.NewLine);
                    sb.Append(joinStatementSb);
                }
            }
            else
            {
                sb.Append($"SELECT{Environment.NewLine}\t{columns}{Environment.NewLine}FROM [{mainTableName}] AS [{mainTableAlias}]");
            }

            if (options != null)
            {
                // -----------------------------------------------------------------------------------------------------------
                // Cross Join clause
                // -----------------------------------------------------------------------------------------------------------

                if (options.PageSize != -1 && allowCrossJoinTotalCount)
                {
                    sb.Append(Environment.NewLine);
                    sb.Append("CROSS JOIN (");
                    sb.Append(Environment.NewLine);
                    sb.Append($"\tSELECT COUNT(*) AS [{CrossJoinCountColumnName}]");
                    sb.Append(Environment.NewLine);
                    sb.Append($"\tFROM [{mainTableName}] AS [{mainTableAlias}]");

                    if (joinStatementSb.Length > 0)
                    {
                        sb.Append(Environment.NewLine);
                        sb.Append("\t");
                        sb.Append(joinStatementSb);
                    }

                    if (options.SpecificationStrategy != null)
                    {
                        new ExpressionTranslator().Translate(
                            options.SpecificationStrategy.Predicate,
                            m,
                            out string expSql,
                            out _);

                        sb.Append(Environment.NewLine);
                        sb.Append($"\tWHERE {expSql}");
                    }

                    sb.Append(Environment.NewLine);
                    sb.Append($") AS [{CrossJoinTableName}]");
                }

                // -----------------------------------------------------------------------------------------------------------
                // Where clause
                // -----------------------------------------------------------------------------------------------------------

                if (options.SpecificationStrategy != null)
                {
                    new ExpressionTranslator().Translate(
                        options.SpecificationStrategy.Predicate,
                        m,
                        out string expSql,
                        out Dictionary<string, object> expParameters);

                    sb.Append($"{Environment.NewLine}WHERE ");
                    sb.Append(expSql);

                    foreach (var item in expParameters)
                    {
                        parameters.Add(item.Key, item.Value);
                    }
                }

                // -----------------------------------------------------------------------------------------------------------
                // Sorting clause
                // -----------------------------------------------------------------------------------------------------------

                var sorting = options.SortingPropertiesMapping.ToDictionary(x => x.Key, x => x.Value);

                if (!sorting.Any())
                {
                    // Sorts on the Id key by default if no sorting is provided
                    foreach (var primaryKeyPropertyInfo in PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos<T>())
                    {
                        sorting.Add(primaryKeyPropertyInfo.Name, SortOrder.Ascending);
                    }
                }

                sb.Append(Environment.NewLine);
                sb.Append("ORDER BY ");

                foreach (var sort in sorting)
                {
                    var sortOrder = sort.Value;
                    var sortProperty = sort.Key;
                    var lambda = ExpressionHelper.GetExpression<T>(sortProperty);
                    var tableType = ExpressionHelper.GetMemberExpression(lambda).Expression.Type;
                    var tableName = m.GetTableName(tableType);
                    var tableAlias = m.GetTableAlias(tableName);
                    var sortingPropertyInfo = ExpressionHelper.GetPropertyInfo(lambda);
                    var columnAlias = m.GetColumnAlias(sortingPropertyInfo);

                    sb.Append($"[{tableAlias}].[{columnAlias}] {(sortOrder == SortOrder.Descending ? "DESC" : "ASC")}, ");
                }

                sb.Remove(sb.Length - 2, 2);

                // -----------------------------------------------------------------------------------------------------------
                // Paging clause
                // -----------------------------------------------------------------------------------------------------------

                if (options.PageSize != -1)
                {
                    sb.Append(Environment.NewLine);
                    sb.Append($"OFFSET {options.PageSize} * ({options.PageIndex} - 1) ROWS");
                    sb.Append(Environment.NewLine);
                    sb.Append($"FETCH NEXT {options.PageSize} ROWS ONLY");
                }
            }

            // Setup mapper object
            m.Sql = sb.ToString();
            m.Parameters = parameters;

            mapper = m;
        }

        public void PrepareEntitySetQuery(EntitySet entitySet, bool existInDb, bool isIdentity, PropertyInfo primeryKeyPropertyInfo, out string sql, out Dictionary<string, object> parameters)
        {
            var entityType = entitySet.Entity.GetType();
            var tableName = entityType.GetTableName();
            var primaryKeyColumnName = primeryKeyPropertyInfo.GetColumnName();

            if (!_sqlPropertiesMapping.ContainsKey(entityType))
            {
                var dict = entityType
                    .GetRuntimeProperties()
                    .Where(x => x.IsPrimitive() && x.IsColumnMapped())
                    .OrderBy(x => x.GetColumnOrder())
                    .ToDictionary(x => x.GetColumnName(), x => x);

                _sqlPropertiesMapping[entityType] = new ConcurrentDictionary<string, PropertyInfo>(dict);
            }

            var sqlPropertiesMapping = _sqlPropertiesMapping[entityType];
            var properties = (isIdentity ? sqlPropertiesMapping.Where(x => !x.Key.Equals(primaryKeyColumnName)) : sqlPropertiesMapping).ToList();

            sql = string.Empty;
            parameters = new Dictionary<string, object>();

            switch (entitySet.State)
            {
                case EntityState.Added:
                    {
                        if (existInDb)
                            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityAlreadyBeingTrackedInStore, entitySet.Entity.GetType()));

                        var columnNames = string.Join(", ", properties.Select(x => x.Value.GetColumnName())).TrimEnd();
                        var values = string.Join(", ", properties.Select(x => $"@{x.Value.GetColumnName()}")).TrimEnd();

                        sql = $"INSERT INTO [{tableName}] ({columnNames}){Environment.NewLine}VALUES ({values})";

                        var canGetScopeIdentity = true;

#if NETFULL
                        if (_providerType == DataAccessProviderType.SqlServerCompact)
                            canGetScopeIdentity = false;
#endif

                        if (canGetScopeIdentity)
                            sql += $"{Environment.NewLine}SELECT SCOPE_IDENTITY()";

                        foreach (var pi in properties)
                        {
                            parameters.Add($"@{pi.Value.GetColumnName()}", pi.Value.GetValue(entitySet.Entity, null));
                        }

                        if (isIdentity)
                            parameters.Add($"@{primaryKeyColumnName}", primeryKeyPropertyInfo.GetValue(entitySet.Entity, null));

                        break;
                    }
                case EntityState.Removed:
                    {
                        if (!existInDb)
                            throw new InvalidOperationException(Resources.EntityNotFoundInStore);

                        sql = $"DELETE FROM [{tableName}]{Environment.NewLine}WHERE {primaryKeyColumnName} = @{primaryKeyColumnName}";

                        parameters.Add($"@{primaryKeyColumnName}", primeryKeyPropertyInfo.GetValue(entitySet.Entity, null));

                        break;
                    }
                case EntityState.Modified:
                    {
                        if (!existInDb)
                            throw new InvalidOperationException(Resources.EntityNotFoundInStore);

                        var values = string.Join($",{Environment.NewLine}\t", properties.Select(x =>
                        {
                            var columnName = x.Value.GetColumnName();
                            return columnName + " = " + $"@{columnName}";
                        }));

                        sql = $"UPDATE [{tableName}]{Environment.NewLine}SET {values}{Environment.NewLine}WHERE {primaryKeyColumnName} = @{primaryKeyColumnName}";

                        foreach (var pi in properties)
                        {
                            parameters.Add($"@{pi.Value.GetColumnName()}", pi.Value.GetValue(entitySet.Entity, null));
                        }

                        if (isIdentity)
                            parameters.Add($"@{primaryKeyColumnName}", primeryKeyPropertyInfo.GetValue(entitySet.Entity, null));

                        break;
                    }
            }
        }

        #endregion
    }
}
