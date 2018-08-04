namespace DotNetToolkit.Repository.AdoNet.Internal
{
    using Configuration.Conventions;
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
        private readonly ConcurrentDictionary<object, object> _entityDataReaderMapping = new ConcurrentDictionary<object, object>();

        #endregion

        #region Constructors

        public Mapper(Type entityType)
        {
            SqlNavigationPropertiesMapping = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

            SqlPropertiesMapping = entityType
                .GetRuntimeProperties()
                .Where(x => x.IsPrimitive() && x.IsColumnMapped())
                .OrderBy(x => x.GetColumnOrder())
                .ToDictionary(x => x.GetColumnName(), x => x);
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
            var tableName = tableType.GetTableName();

            _tableAliasMapping.TryAdd(tableName, tableAlias);
            _tableNameAndTypeMapping.TryAdd(tableName, tableType);
            _tableTypeAndNameMapping.TryAdd(tableType, tableName);
            _tableColumnAliasMapping[tableName] = new Dictionary<string, string>();

            return tableAlias;
        }

        public string GenerateColumnAlias(PropertyInfo pi)
        {
            var columnName = pi.GetColumnName();
            var columnAlias = columnName;
            var tableName = pi.DeclaringType.GetTableName();

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
            var columnName = pi.GetColumnName();
            var tableName = pi.DeclaringType.GetTableName();
            var columnMapping = _tableColumnAliasMapping[tableName];

            if (!columnMapping.ContainsKey(columnName))
                throw new InvalidOperationException(string.Format(Resources.InvalidColumnName, columnName));

            return columnMapping[columnName];
        }

        public string NormalizeColumnAlias(string columnAlias)
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
            if (r == null)
                throw new ArgumentNullException(nameof(r));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            var key = GetKey<T>(r);
            var entity = _entityDataReaderMapping.ContainsKey(key) ? (T)_entityDataReaderMapping[key] : Activator.CreateInstance<T>();
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
                        if (columnPropertyInfosMapping.ContainsKey(name))
                        {
                            columnPropertyInfosMapping[name].SetValue(joinTableInstances[joinTableType], value);
                        }
                        else
                        {
                            columnPropertyInfosMapping[NormalizeColumnAlias(name)].SetValue(joinTableInstances[joinTableType], value);
                        }
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
                    var isJoinPropertyCollection = false;

                    // Sets the main table property in the join table
                    var mainTablePropertyInfo = joinTableType.GetRuntimeProperties().Single(x => x.PropertyType == entityType);

                    mainTablePropertyInfo.SetValue(joinTableInstance, entity);

                    // Sets the join table property in the main table
                    var joinTablePropertyInfo = mainTableProperties.Single(x =>
                    {
                        isJoinPropertyCollection = x.PropertyType.IsGenericCollection();

                        var type = isJoinPropertyCollection
                            ? x.PropertyType.GetGenericArguments().First()
                            : x.PropertyType;

                        return type == joinTableType;
                    });

                    if (isJoinPropertyCollection)
                    {
                        var collection = joinTablePropertyInfo.GetValue(entity, null);

                        if (collection == null)
                        {
                            var collectionTypeParam = joinTablePropertyInfo.PropertyType.GetGenericArguments().First();

                            collection = Activator.CreateInstance(typeof(List<>).MakeGenericType(collectionTypeParam));

                            joinTablePropertyInfo.SetValue(entity, collection);
                        }

                        collection.GetType().GetMethod("Add").Invoke(collection, new[] { joinTableInstance });
                    }
                    else
                    {
                        joinTablePropertyInfo.SetValue(entity, joinTableInstance);
                    }
                }
            }

            _entityDataReaderMapping[key] = entity;

            return elementSelector(entity);
        }

        private static object GetKey<T>(DbDataReader r)
        {
            if (r == null)
                throw new ArgumentNullException(nameof(r));

            var primaryKeyValues = PrimaryKeyConventionHelper
                .GetPrimaryKeyPropertyInfos<T>()
                .Select(x => r[x.GetColumnName()])
                .ToList();

            object key = null;

            switch (primaryKeyValues.Count)
            {
                case 3:
                    {
                        key = Tuple.Create(primaryKeyValues[0], primaryKeyValues[1], primaryKeyValues[2]);
                        break;
                    }
                case 2:
                    {
                        key = Tuple.Create(primaryKeyValues[0], primaryKeyValues[1]);
                        break;
                    }
                case 1:
                    {
                        key = primaryKeyValues[0];
                        break;
                    }
                default:
                    {
                        key = Guid.NewGuid();
                        break;
                    }
            }

            return key;
        }

        #endregion
    }

    internal class Mapper<T> where T : class
    {
        public static T Map(DbDataReader r)
        {
            if (r == null)
                throw new ArgumentNullException(nameof(r));

            return new Mapper(typeof(T)).Map<T, T>(r, IdentityFunction<T>.Instance);
        }
    }
}
