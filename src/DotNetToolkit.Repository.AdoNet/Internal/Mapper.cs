﻿namespace DotNetToolkit.Repository.AdoNet.Internal
{
    using Configuration.Conventions;
    using Extensions;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Utility;

    internal class Mapper<T>
    {
        #region Fields

        private readonly Dictionary<string, PropertyInfo> _properties;
        private readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _navigationProperties;
        private readonly Func<string, Type> _getTableTypeByColumnAliasCallback;
        private readonly ConcurrentDictionary<object, object> _entityDataReaderMapping = new ConcurrentDictionary<object, object>();
        private readonly IRepositoryConventions _conventions;

        #endregion

        #region Constructors

        public Mapper(IRepositoryConventions conventions)
        {
            _conventions = Guard.NotNull(conventions, nameof(conventions));

            _properties = typeof(T).GetRuntimeProperties()
                .Where(x => x.IsPrimitive() && _conventions.IsColumnMapped(x))
                .OrderBy(_conventions.GetColumnOrderOrDefault)
                .ToDictionary(_conventions.GetColumnName, x => x);

            _navigationProperties = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
        }

        public Mapper(IRepositoryConventions conventions, Dictionary<Type, Dictionary<string, PropertyInfo>> navigationProperties, Func<string, Type> getTableTypeByColumnAliasCallback)
        {
            _conventions = Guard.NotNull(conventions, nameof(conventions));

            _properties = typeof(T).GetRuntimeProperties()
                .Where(x => x.IsPrimitive() && _conventions.IsColumnMapped(x))
                .OrderBy(_conventions.GetColumnOrderOrDefault)
                .ToDictionary(_conventions.GetColumnName, x => x);

            _navigationProperties = navigationProperties ?? new Dictionary<Type, Dictionary<string, PropertyInfo>>();
            _getTableTypeByColumnAliasCallback = Guard.NotNull(getTableTypeByColumnAliasCallback, nameof(getTableTypeByColumnAliasCallback));
        }

        #endregion

        #region Public Methods

        public TElement Map<TElement>(DbDataReader r, Func<T, TElement> elementSelector)
        {
            if (r == null)
                throw new ArgumentNullException(nameof(r));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            var key = GetDataReaderPrimaryKey<T>(r);

            var entity = _entityDataReaderMapping.ContainsKey(key)
                ? (T)_entityDataReaderMapping[key]
                : Activator.CreateInstance<T>();

            var entityType = typeof(T);
            var joinTableInstances = _navigationProperties.Keys.ToDictionary(x => x, Activator.CreateInstance);

            for (var i = 0; i < r.FieldCount; i++)
            {
                var name = r.GetName(i);
                var value = r[name];

                if (value == DBNull.Value)
                    value = null;

                if (_properties.ContainsKey(name))
                {
                    if (!r.IsDBNull(r.GetOrdinal(name)))
                        _properties[name].SetValue(entity, value);
                }
                else if (joinTableInstances.Any())
                {
                    var joinTableProperty = _getTableTypeByColumnAliasCallback(name);

                    if (joinTableProperty != null)
                    {
                        var joinTableType = _getTableTypeByColumnAliasCallback(name);

                        if (joinTableType != null)
                        {
                            var columnPropertyInfosMapping = _navigationProperties.Single(x => x.Key == joinTableType).Value;

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
            }
            
            if (joinTableInstances.Any())
            {
                var mainTablePrimaryKeyPropertyInfo = _conventions.GetPrimaryKeyPropertyInfos<T>().First();
                var mainTablePrimaryKeyValue = mainTablePrimaryKeyPropertyInfo.GetValue(entity);

                // Needs to make sure we are not dealing with navigation properties that are not actually linked to this entity
                var validJoinTableInstances = joinTableInstances
                    .Where(x =>
                    {
                        var joinTableForeignKeyPropertyInfo = _conventions
                            .GetForeignKeyPropertyInfos(x.Key, entityType)
                            .First();

                        var joinTableForeignKeyValue = joinTableForeignKeyPropertyInfo.GetValue(x.Value);

                        return mainTablePrimaryKeyValue.Equals(joinTableForeignKeyValue);
                    })
                    .ToDictionary(x => x.Key, x => x.Value);

                if (validJoinTableInstances.Any())
                {
                    var mainTableProperties = entityType.GetRuntimeProperties().ToList();

                    foreach (var item in validJoinTableInstances)
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
            }

            _entityDataReaderMapping[key] = entity;

            return elementSelector(entity);
        }

        public T Map(DbDataReader r) => Map<T>(r, IdentityFunction<T>.Instance);

        #endregion

        #region Private Methods

        private object GetDataReaderPrimaryKey<T>(DbDataReader r)
        {
            if (r == null)
                throw new ArgumentNullException(nameof(r));

            var primaryKeyValues = _conventions
                .GetPrimaryKeyPropertyInfos<T>()
                .Select(x => r[_conventions.GetColumnName(x)])
                .ToList();

            switch (primaryKeyValues.Count)
            {
                case 0:
                    return Guid.NewGuid();
                case 1:
                    return primaryKeyValues[0];
                default:
                    return string.Join(":", primaryKeyValues);
            }
        }

        private string NormalizeColumnAlias(string columnAlias)
        {
            return Regex.Replace(columnAlias, @"[\d-]", string.Empty);
        }

        #endregion
    }
}
