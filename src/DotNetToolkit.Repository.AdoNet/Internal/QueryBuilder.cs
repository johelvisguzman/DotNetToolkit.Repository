namespace DotNetToolkit.Repository.AdoNet.Internal
{
    using Configuration.Conventions.Internal;
    using Extensions;
    using Queries;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Utility;

    /// <summary>
    /// Represents an internal query builder for building various queries.
    /// </summary>
    internal class QueryBuilder
    {
        public static void CreateInsertStatement(object entity, out string sql, out Dictionary<string, object> parameters)
        {
            Guard.NotNull(entity);

            parameters = new Dictionary<string, object>();

            var entityType = entity.GetType();
            var tableName = entityType.GetTableName();

            var primaryKeyPropertyInfos = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(entityType).ToList();
            var properties = GetProperties(entityType);

            if (primaryKeyPropertyInfos.Count == 1)
            {
                var primaryKeyPropertyInfo = primaryKeyPropertyInfos.First();
                var primaryKeyColumnName = primaryKeyPropertyInfo.GetColumnName();

                if (primaryKeyPropertyInfo.IsColumnIdentity())
                {
                    properties.Remove(primaryKeyColumnName);
                    parameters.Add($"@{primaryKeyColumnName}", primaryKeyPropertyInfo.GetValue(entity, null));
                }
            }

            foreach (var x in properties)
            {
                parameters.Add($"@{x.Key}", x.Value.GetValue(entity, null));
            }

            var columnNames = string.Join(", ", properties.Select(x => x.Key)).TrimEnd();
            var values = string.Join(", ", properties.Select(x => $"@{x.Key}")).TrimEnd();

            sql = $"INSERT INTO [{tableName}] ({columnNames}){Environment.NewLine}VALUES ({values})";
        }

        public static void CreateUpdateStatement(object entity, out string sql, out Dictionary<string, object> parameters)
        {
            Guard.NotNull(entity);

            parameters = new Dictionary<string, object>();

            var entityType = entity.GetType();
            var tableName = entityType.GetTableName();

            var primaryKeyPropertyInfos = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(entityType).ToList();
            var primaryKeyColumnNamesDict = primaryKeyPropertyInfos.ToDictionary(x => x.GetColumnName(), x => x);
            var properties = GetProperties(entityType);

            if (primaryKeyPropertyInfos.Count == 1)
            {
                var primaryKeyPropertyInfo = primaryKeyPropertyInfos.First();
                var primaryKeyColumnName = primaryKeyPropertyInfo.GetColumnName();

                if (primaryKeyPropertyInfo.IsColumnIdentity())
                {
                    properties.Remove(primaryKeyColumnName);
                    parameters.Add($"@{primaryKeyColumnName}", primaryKeyPropertyInfo.GetValue(entity, null));
                }
            }

            var values = string.Join($",{Environment.NewLine}\t", properties.Select(x => x.Key + " = " + $"@{x.Key}"));
            var condition = string.Join(" AND ", primaryKeyColumnNamesDict.Select(kv => "(" + kv.Key + " = @" + kv.Key + ")"));

            sql = $"UPDATE [{tableName}]{Environment.NewLine}SET {values}{Environment.NewLine}WHERE ({condition})";

            foreach (var x in properties)
            {
                parameters.Add($"@{x.Key}", x.Value.GetValue(entity, null));
            }
        }

        public static void CreateDeleteStatement(object entity, out string sql, out Dictionary<string, object> parameters)
        {
            Guard.NotNull(entity);

            parameters = new Dictionary<string, object>();

            var entityType = entity.GetType();
            var tableName = entityType.GetTableName();

            var primaryKeyColumnNamesDict = PrimaryKeyConventionHelper
                .GetPrimaryKeyPropertyInfos(entityType)
                .ToDictionary(x => x.GetColumnName(), x => x);

            var condition = string.Join(" AND ", primaryKeyColumnNamesDict.Select(kv => "(" + kv.Key + " = @" + kv.Key + ")"));

            sql = $"DELETE FROM [{tableName}]{Environment.NewLine}WHERE ({condition})";

            foreach (var x in primaryKeyColumnNamesDict)
            {
                parameters.Add($"@{x.Key}", x.Value.GetValue(entity, null));
            }
        }

        public static void CreateSelectStatement<T>(IQueryOptions<T> options, string defaultSelect, bool applyFetchOptions, out string sql, out Dictionary<string, object> parameters, out Dictionary<Type, Dictionary<string, PropertyInfo>> navigationProperties, out Func<string, Type> getTableTypeByColumnAliasCallback)
        {
            parameters = new Dictionary<string, object>();
            navigationProperties = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

            var tableAliasCount = 1;
            var tableAliasMapping = new Dictionary<string, string>();
            var tableNameAndTypeMapping = new Dictionary<string, Type>();
            var tableTypeAndNameMapping = new Dictionary<Type, string>();
            var tableColumnAliasMapping = new Dictionary<string, Dictionary<string, string>>();
            var columnAliasMappingCount = new Dictionary<string, int>();
            var columnAliasTableNameMapping = new Dictionary<string, string>();
            var crossJoinCountColumnAlias = string.Empty;
            var crossJoinTableAlias = string.Empty;
            var select = string.Empty;

            string GenerateTableAlias(Type tableType)
            {
                if (tableType == null)
                    throw new ArgumentNullException(nameof(tableType));

                var tableAlias = $"Extent{tableAliasCount++}";
                var tableName = tableType.GetTableName();

                tableAliasMapping.Add(tableName, tableAlias);
                tableNameAndTypeMapping.Add(tableName, tableType);
                tableTypeAndNameMapping.Add(tableType, tableName);
                tableColumnAliasMapping[tableName] = new Dictionary<string, string>();

                return tableAlias;
            }

            string GenerateColumnAlias(PropertyInfo pi)
            {
                if (pi == null)
                    throw new ArgumentNullException(nameof(pi));

                var columnName = pi.GetColumnName();
                var columnAlias = columnName;
                var tableName = pi.DeclaringType.GetTableName();

                if (columnAliasMappingCount.TryGetValue(columnName, out int columnAliasCount))
                {
                    ++columnAliasCount;
                    columnAlias = $"{columnAlias}{columnAliasCount}";

                    columnAliasMappingCount[columnName] = columnAliasCount;
                }
                else
                {
                    columnAliasMappingCount.Add(columnName, 0);
                }

                tableColumnAliasMapping[tableName][columnName] = columnAlias;
                columnAliasTableNameMapping[columnAlias] = tableName;

                return columnAlias;
            }

            string EnsureColumnAlias(string prefix)
            {
                var columnAlias = prefix;

                while (columnAliasTableNameMapping.ContainsKey(columnAlias))
                {
                    var count = new string(columnAlias.Where(char.IsDigit).ToArray());

                    columnAlias = columnAlias + count + 1;
                }

                return columnAlias;
            }

            string EnsureTableAlias(string prefix)
            {
                var tableAlias = prefix;

                while (tableAliasMapping.ContainsValue(tableAlias))
                {
                    var count = new string(tableAlias.Where(char.IsDigit).ToArray());

                    tableAlias = tableAlias + count + 1;
                }

                return tableAlias;
            }

            string GetTableNameFromType(Type tableType)
                => tableTypeAndNameMapping[tableType];

            string GetTableAliasFromName(string tableName)
                => tableAliasMapping[tableName];

            string GetTableAliasFromType(Type tableType)
                => GetTableAliasFromName(GetTableNameFromType(tableType));

            string GetColumnAliasFromProperty(PropertyInfo pi)
            {
                if (pi == null)
                    throw new ArgumentNullException(nameof(pi));

                var columnName = pi.GetColumnName();
                var tableName = pi.DeclaringType.GetTableName();
                var columnMapping = tableColumnAliasMapping[tableName];

                if (!columnMapping.ContainsKey(columnName))
                    return null;

                return columnMapping[columnName];
            }

            getTableTypeByColumnAliasCallback = columnAlias =>
            {
                if (columnAliasTableNameMapping.ContainsKey(columnAlias))
                {
                    var tableName = columnAliasTableNameMapping[columnAlias];

                    return tableNameAndTypeMapping[tableName];
                }

                return null;
            };

            var sb = new StringBuilder();
            var joinStatementSb = new StringBuilder();

            var mainTableType = typeof(T);
            var mainTableName = mainTableType.GetTableName();
            var mainTableAlias = GenerateTableAlias(mainTableType);
            var mainTableProperties = mainTableType.GetRuntimeProperties().ToList();
            var mainTablePrimaryKeyPropertyInfo = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos<T>().First();
            var mainTablePrimaryKeyName = mainTablePrimaryKeyPropertyInfo.GetColumnName();
            var fetchingPaths = applyFetchOptions
                ? options.DefaultIfFetchStrategyEmpty().PropertyPaths.ToList()
                : Enumerable.Empty<string>().ToList();

            const string DEFAULT_CROSS_JOIN_COLUMN_ALIAS = "C1";
            const string DEFAULT_CROSS_JOIN_TABLE_ALIAS = "GroupBy1";

            var properties = GetProperties(mainTableType);

            foreach (var pi in properties.Values)
            {
                GenerateColumnAlias(pi);
            }

            // -----------------------------------------------------------------------------------------------------------
            // Select clause
            // -----------------------------------------------------------------------------------------------------------

            if (string.IsNullOrEmpty(defaultSelect))
            {
                // Default select
                select = string.Join($",{Environment.NewLine}\t", properties.Select(x =>
                {
                    var colAlias = GetColumnAliasFromProperty(x.Value);
                    var colName = x.Key;

                    return $"[{mainTableAlias}].[{colName}] AS [{colAlias}]";
                }));
            }
            else
            {
                select = defaultSelect;
            }

            // Append join tables from fetchStrategy
            // Only supports a one to one table join for now...
            if (fetchingPaths.Any())
            {
                sb.Append($"SELECT{Environment.NewLine}\t{select}");

                foreach (var path in fetchingPaths)
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
                        var joinTableAlias = GenerateTableAlias(joinTableType);
                        var joinTableColumnNames = string.Join($",{Environment.NewLine}\t",
                            joinTableProperties
                                .Where(Extensions.PropertyInfoExtensions.IsPrimitive)
                                .Select(x =>
                                {
                                    var colAlias = GenerateColumnAlias(x);
                                    var colName = x.GetColumnName();

                                    return $"[{joinTableAlias}].[{colName}] AS [{colAlias}]";
                                }));

                        if (string.IsNullOrEmpty(defaultSelect))
                        {
                            sb.Append($",{Environment.NewLine}\t");
                            sb.Append(joinTableColumnNames);
                        }

                        joinStatementSb.Append(Environment.NewLine);
                        joinStatementSb.Append($"LEFT OUTER JOIN [{joinTableName}] AS [{joinTableAlias}] ON [{mainTableAlias}].[{mainTablePrimaryKeyName}] = [{joinTableAlias}].[{joinTableForeignKeyName}]");

                        navigationProperties.Add(joinTableType, joinTableProperties.ToDictionary(ModelConventionHelper.GetColumnName, x => x));
                    }
                }

                if (options != null && options.PageSize != -1 && string.IsNullOrEmpty(defaultSelect))
                {
                    crossJoinCountColumnAlias = EnsureColumnAlias(DEFAULT_CROSS_JOIN_COLUMN_ALIAS);
                    crossJoinTableAlias = EnsureTableAlias(DEFAULT_CROSS_JOIN_TABLE_ALIAS);

                    // Cross join counter column
                    sb.Append(",");
                    sb.Append(Environment.NewLine);
                    sb.Append($"\t[{crossJoinTableAlias}].[{crossJoinCountColumnAlias}] AS [{crossJoinCountColumnAlias}]");
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
                sb.Append($"SELECT{Environment.NewLine}\t{select}{Environment.NewLine}FROM [{mainTableName}] AS [{mainTableAlias}]");
            }

            if (options != null)
            {
                // -----------------------------------------------------------------------------------------------------------
                // Cross Join clause
                // -----------------------------------------------------------------------------------------------------------

                if (options.PageSize != -1 && string.IsNullOrEmpty(defaultSelect))
                {
                    if (string.IsNullOrEmpty(crossJoinTableAlias))
                    {
                        crossJoinTableAlias = EnsureTableAlias(DEFAULT_CROSS_JOIN_TABLE_ALIAS);
                        crossJoinCountColumnAlias = EnsureColumnAlias(DEFAULT_CROSS_JOIN_COLUMN_ALIAS);
                    }

                    sb.Append(Environment.NewLine);
                    sb.Append("CROSS JOIN (");
                    sb.Append(Environment.NewLine);
                    sb.Append($"\tSELECT COUNT(*) AS [{crossJoinCountColumnAlias}]");
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
                            GetTableAliasFromType,
                            GetColumnAliasFromProperty,
                            out var expSql,
                            out _);

                        sb.Append(Environment.NewLine);
                        sb.Append($"\tWHERE {expSql}");
                    }

                    sb.Append(Environment.NewLine);
                    sb.Append($") AS [{crossJoinTableAlias}]");
                }

                // -----------------------------------------------------------------------------------------------------------
                // Where clause
                // -----------------------------------------------------------------------------------------------------------

                if (options.SpecificationStrategy != null)
                {
                    new ExpressionTranslator().Translate(
                        options.SpecificationStrategy.Predicate,
                        GetTableAliasFromType,
                        GetColumnAliasFromProperty,
                        out var expSql,
                        out var expParameters);

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

                if (string.IsNullOrEmpty(defaultSelect))
                {
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
                        var tableName = GetTableNameFromType(tableType);
                        var tableAlias = GetTableAliasFromName(tableName);
                        var sortingPropertyInfo = ExpressionHelper.GetPropertyInfo(lambda);
                        var columnAlias = GetColumnAliasFromProperty(sortingPropertyInfo);

                        sb.Append($"[{tableAlias}].[{columnAlias}] {(sortOrder == SortOrder.Descending ? "DESC" : "ASC")}, ");
                    }

                    sb.Remove(sb.Length - 2, 2);
                }

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

            sql = sb.ToString();
        }

        public static void CreateSelectStatement<T>(IQueryOptions<T> options, bool applyFetchOptions, out string sql, out Dictionary<string, object> parameters, out Dictionary<Type, Dictionary<string, PropertyInfo>> navigationProperties, out Func<string, Type> getTableTypeByColumnAliasCallback)
        {
            CreateSelectStatement<T>(
                options,
                null,
                applyFetchOptions,
                out sql,
                out parameters,
                out navigationProperties,
                out getTableTypeByColumnAliasCallback);
        }

        public static void CreateSelectStatement<T>(IQueryOptions<T> options, out string sql, out Dictionary<string, object> parameters, out Dictionary<Type, Dictionary<string, PropertyInfo>> navigationProperties, out Func<string, Type> getTableTypeByColumnAliasCallback)
        {
            CreateSelectStatement<T>(
                options,
                true,
                out sql,
                out parameters,
                out navigationProperties,
                out getTableTypeByColumnAliasCallback);
        }

        public static void CreateSelectStatement<T>(IQueryOptions<T> options, out string sql, out Dictionary<string, object> parameters)
        {
            CreateSelectStatement<T>(
                options,
                false,
                out sql,
                out parameters,
                out var navigationProperties,
                out var getPropertyFromColumnAliasCallback);
        }
        
        public static void CreateSelectStatement<T>(IQueryOptions<T> options, string select, out string sql, out Dictionary<string, object> parameters)
        {
            CreateSelectStatement<T>(
                options,
                select,
                false,
                out sql,
                out parameters,
                out var navigationProperties,
                out var getPropertyFromColumnAliasCallback);
        }

        public static void CreateSelectStatement<T>(string select, out string sql)
        {
            CreateSelectStatement<T>(null, select, out sql, out var parameters);
        }

        public static void CreateSelectStatement<T>(out string sql)
        {
            CreateSelectStatement<T>(null, out sql, out var parameters);
        }

        public static void ExtractCrossJoinColumnName(string sql, out string columnName)
        {
            Guard.NotEmpty(sql);

            columnName = string.Empty;

            var i = sql.IndexOf("CROSS JOIN", StringComparison.InvariantCultureIgnoreCase);

            if (i > 0)
            {
                i = sql.IndexOf("SELECT COUNT(*) AS ", i, StringComparison.InvariantCultureIgnoreCase);
                i = sql.IndexOf("[", i, StringComparison.Ordinal);

                var j = sql.IndexOf("]", i, StringComparison.Ordinal);

                columnName = sql.Substring(i + 1, j - i - 1);
            }
        }

        private static Dictionary<string, PropertyInfo> GetProperties(Type entityType)
        {
            Guard.NotNull(entityType);

            return entityType
                .GetRuntimeProperties()
                .Where(x => x.IsPrimitive() && x.IsColumnMapped())
                .OrderBy(x => x.GetColumnOrder())
                .ToDictionary(x => x.GetColumnName(), x => x);
        }
    }
}
