namespace DotNetToolkit.Repository.AdoNet.Internal
{
    using Helpers;
    using Properties;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    internal class Mapper
    {
        #region Fields

        private int _tableAliasCount = 1;

        private readonly ConcurrentDictionary<string, string> _tableAliasMapping = new ConcurrentDictionary<string, string>();
        private readonly ConcurrentDictionary<string, Type> _tableNameAndTypeMapping = new ConcurrentDictionary<string, Type>();
        private readonly ConcurrentDictionary<Type, string> _tableTypeAndNameMapping = new ConcurrentDictionary<Type, string>();
        private readonly ConcurrentDictionary<string, Dictionary<string, string>> _tableColumnAliasMapping = new ConcurrentDictionary<string, Dictionary<string, string>>();
        private readonly ConcurrentDictionary<string, string> _columnAliasTableNameMapping = new ConcurrentDictionary<string, string>();
        private readonly ConcurrentDictionary<string, int> _columnAliasMappingCount = new ConcurrentDictionary<string, int>();

        #endregion

        #region Constructors

        public Mapper(Type entityType)
        {
            SqlNavigationPropertiesMapping = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

            SqlPropertiesMapping = entityType
                .GetRuntimeProperties()
                .Where(x => ConventionHelper.IsPrimitive(x) && ConventionHelper.IsMapped(x))
                .OrderBy(ConventionHelper.GetColumnOrder)
                .ToDictionary(ConventionHelper.GetColumnName, x => x);
        }

        #endregion

        #region Properties

        public string Sql { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        public Dictionary<Type, Dictionary<string, PropertyInfo>> SqlNavigationPropertiesMapping { get; }
        public Dictionary<string, PropertyInfo> SqlPropertiesMapping { get; }

        #endregion

        #region Public Methods

        public string GenerateTableAlias(Type tableType)
        {
            var tableAlias = $"Extent{_tableAliasCount++}";
            var tableName = ConventionHelper.GetTableName(tableType);

            _tableAliasMapping.TryAdd(tableName, tableAlias);
            _tableNameAndTypeMapping.TryAdd(tableName, tableType);
            _tableTypeAndNameMapping.TryAdd(tableType, tableName);
            _tableColumnAliasMapping[tableName] = new Dictionary<string, string>();

            return tableAlias;
        }

        public string GenerateColumnAlias(PropertyInfo pi)
        {
            var columnName = ConventionHelper.GetColumnName(pi);
            var columnAlias = columnName;
            var tableName = ConventionHelper.GetTableName(pi.DeclaringType);

            if (_columnAliasMappingCount.TryGetValue(columnName, out int columnAliasCount))
            {
                ++columnAliasCount;
                columnAlias = $"{columnAlias}{columnAliasCount}";

                _columnAliasMappingCount[columnName] = columnAliasCount;
            }
            else
            {
                _columnAliasMappingCount.TryAdd(columnName, 0);
            }

            _tableColumnAliasMapping[tableName][columnName] = columnAlias;

            _columnAliasTableNameMapping[columnAlias] = tableName;

            return columnAlias;
        }

        public string GetTableAlias(string tableName)
        {
            return _tableAliasMapping[tableName];
        }

        public string GetTableName(string tableAlias)
        {
            return !_tableAliasMapping.Values.Contains(tableAlias)
                ? null
                : _tableAliasMapping.SingleOrDefault(x => x.Value == tableAlias).Key;
        }

        public string GetTableName(Type tableType)
        {
            return _tableTypeAndNameMapping[tableType];
        }

        public string GetColumnAlias(PropertyInfo pi)
        {
            var columnName = ConventionHelper.GetColumnName(pi);
            var tableName = ConventionHelper.GetTableName(pi.DeclaringType);
            var columnMapping = _tableColumnAliasMapping[tableName];

            if (!columnMapping.ContainsKey(columnName))
                throw new InvalidOperationException(string.Format(Resources.InvalidColumnName, columnName));

            return columnMapping[columnName];
        }

        public string GetColumnName(string columnAlias)
        {
            return Regex.Replace(columnAlias, @"[\d-]", string.Empty);
        }

        public Type GetTableTypeByColumnAlias(string columnAlias)
        {
            var tableName = _columnAliasTableNameMapping[columnAlias];

            return _tableNameAndTypeMapping[tableName];
        }

        public TElement Map<T, TElement>(DbDataReader r, Func<T, TElement> elementSelector)
        {
            var entity = Activator.CreateInstance<T>();
            var entityType = typeof(T);
            var joinTableInstances = SqlNavigationPropertiesMapping.Keys.ToDictionary(x => x, Activator.CreateInstance);

            for (var i = 0; i < r.FieldCount; i++)
            {
                var name = r.GetName(i);
                var value = r[name];

                if (value == DBNull.Value)
                    value = null;

                if (SqlPropertiesMapping.ContainsKey(name) && !r.IsDBNull(r.GetOrdinal(name)))
                {
                    SqlPropertiesMapping[name].SetValue(entity, value);
                }
                else if (joinTableInstances.Any())
                {
                    var joinTableType = GetTableTypeByColumnAlias(name);

                    if (joinTableType != null)
                    {
                        var columnPropertyInfosMapping = SqlNavigationPropertiesMapping.Single(x => x.Key == joinTableType).Value;
                        var columnName = GetColumnName(name);

                        columnPropertyInfosMapping[columnName].SetValue(joinTableInstances[joinTableType], value);
                    }
                }
            }

            if (joinTableInstances.Any())
            {
                var mainTableProperties = entityType.GetRuntimeProperties().ToList();

                foreach (var item in joinTableInstances)
                {
                    var joinTableInstance = item.Value;
                    var joinTableType = item.Key;

                    // Sets the main table property in the join table
                    var mainTablePropertyInfo = joinTableType.GetRuntimeProperties().Single(x => x.PropertyType == entityType);

                    mainTablePropertyInfo.SetValue(joinTableInstance, entity);

                    // Sets the join table property in the main table
                    var joinTablePropertyInfo = mainTableProperties.Single(x => x.PropertyType == joinTableType);

                    joinTablePropertyInfo.SetValue(entity, joinTableInstance);
                }
            }

            return elementSelector(entity);
        }

        #endregion
    }
}
