namespace DotNetToolkit.Repository.AdoNet.Internal
{
    using Configuration.Conventions;
    using Extensions;
    using Extensions.Internal;
    using JetBrains.Annotations;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;
    using Utility;

    internal class Mapper<T>
    {
        #region Fields

        private readonly IRepositoryConventions _conventions;
        private readonly Dictionary<string, MappingProperty> _mappings;
        private readonly Dictionary<object, object> _entityDataReaderMapping;

        #endregion

        #region Constructors

        public Mapper([NotNull] IRepositoryConventions conventions, [NotNull] IEnumerable<MappingProperty> mappings)
        {
            _conventions = Guard.NotNull(conventions, nameof(conventions));
            _mappings = Guard.NotNull(mappings, nameof(mappings)).ToDictionary(x => x.ColumnAlias);
            _entityDataReaderMapping = new Dictionary<object, object>();
        }

        #endregion

        #region Public Methods

        public T Map(DbDataReader r)
        {
            var entityType = typeof(T);
            var entityMappingKey = string.Join(":", GetPrimaryKeyValues(entityType, r));
            var entity = _entityDataReaderMapping.ContainsKey(entityMappingKey)
                ? (T)_entityDataReaderMapping[entityMappingKey]
                : Activator.CreateInstance<T>();

            var joinEntityMapping = _mappings.Values
               .Where(x => x.PropertyInfo.DeclaringType != entityType)
               .GroupBy(x => x.TableAlias)
               .ToDictionary(
                   grp => grp.Key,
                   grp => new Lazy<object>(
                       () => Activator.CreateInstance(grp.First().PropertyInfo.DeclaringType)));

            for (var i = 0; i < r.FieldCount; i++)
            {
                var name = r.GetName(i);

                if (_mappings.ContainsKey(name))
                {
                    var mappingProperty = _mappings[name];
                    var pi = mappingProperty.PropertyInfo;
                    var value = r[name] == DBNull.Value ? null : r[name];

                    // Map properties in main entity table
                    if (entityType.IsSubclassOf(pi.DeclaringType) || pi.DeclaringType == entityType)
                    {
                        if (!r.IsDBNull(r.GetOrdinal(name)))
                        {
                            pi.SetValue(entity, value);
                        }
                    }
                    // Assumes this is a foreign property column
                    else
                    {
                        if (!r.IsDBNull(r.GetOrdinal(name)))
                        {
                            var joinTablePropertyInfo = mappingProperty.JoinTablePropertyInfo;
                            var joinEntityFactory = joinEntityMapping[mappingProperty.TableAlias];
                            var joinEntityIsNew = !joinEntityFactory.IsValueCreated;
                            var joinEntity = joinEntityFactory.Value;

                            // Map property in foreign entity
                            pi.DeclaringType.GetProperty(pi.Name).SetValue(joinEntity, value);

                            // Do the following if the entity is newly created
                            if (joinEntityIsNew)
                            {
                                // Sets the main table property in the join table
                                var mainTablePropertyInfo = joinEntity
                                    .GetType()
                                    .GetRuntimeProperties()
                                    .FirstOrDefault(x => x.PropertyType == entityType);

                                if (mainTablePropertyInfo != null)
                                {
                                    mainTablePropertyInfo.SetValue(joinEntity, entity);
                                }

                                // Sets the join table property in the main table
                                if (joinTablePropertyInfo.PropertyType.IsGenericCollection())
                                {
                                    var collection = joinTablePropertyInfo.GetValue(entity, null);

                                    if (collection == null)
                                    {
                                        var collectionTypeParam = joinTablePropertyInfo.PropertyType.GetGenericArguments().First();

                                        collection = Activator.CreateInstance(typeof(List<>).MakeGenericType(collectionTypeParam));

                                        joinTablePropertyInfo.SetValue(entity, collection);
                                    }

                                    collection.GetType().GetMethod("Add").Invoke(collection, new[] { joinEntity });
                                }
                                else
                                {
                                    joinTablePropertyInfo.SetValue(entity, joinEntity);
                                }
                            }
                        }
                    }
                }
            }

            _entityDataReaderMapping[entityMappingKey] = entity;

            return entity;
        }

        #endregion

        #region Private Methods

        private IEnumerable<object> GetPrimaryKeyValues(Type type, DbDataReader r)
        {
            foreach (var pi in _conventions.GetPrimaryKeyPropertyInfos(type))
            {
                var mappingProperty = _mappings.Values.First(x => x.PropertyInfo.DeclaringType == type && x.PropertyInfo.Name.Equals(pi.Name));
                var value = r[mappingProperty.ColumnAlias];

                yield return r[mappingProperty.ColumnAlias];
            }
        }

        #endregion
    }
}
