namespace DotNetToolkit.Repository.Configuration.Conventions
{
    using Helpers;
    using Properties;
    using Queries.Strategies;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class PrimaryKeyConventionHelper
    {
        /// <summary>
        /// Gets the composite primary key property information for the specified type.
        /// </summary>
        /// <param name="entityType">The entity type to get the primary key from.</param>
        /// <returns>The composite primary key property infos.</returns>
        /// <remarks>
        /// If the entity type is defined with a composite primary key collection,
        /// then the list of keys found will be returned ordered as defined by their specified <see cref="ColumnAttribute"/> attribute.
        /// </remarks>
        public static IEnumerable<PropertyInfo> GetPrimaryKeyPropertyInfos(Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));

            if (InMemoryCache.Instance.PrimaryKeyMapping.ContainsKey(entityType))
                return InMemoryCache.Instance.PrimaryKeyMapping[entityType];

            // Gets by checking the annotations
            var propertyInfos = entityType
                .GetRuntimeProperties()
                .Where(x => x.IsColumnMapped() && x.GetCustomAttribute<KeyAttribute>() != null)
                .OrderBy(x =>
                {
                    var columnAttribute = x.GetCustomAttribute<ColumnAttribute>();
                    if (columnAttribute != null && columnAttribute.Order > 0)
                        return columnAttribute.Order;

                    return int.MaxValue;
                })
                .ToList();

            // Gets by naming convention
            if (!propertyInfos.Any())
            {
                foreach (var propertyName in GetDefaultPrimaryKeyNameChecks(entityType))
                {
                    var propertyInfo = entityType.GetTypeInfo().GetDeclaredProperty(propertyName);

                    if (propertyInfo != null && propertyInfo.IsColumnMapped())
                    {
                        propertyInfos.Add(propertyInfo);

                        break;
                    }
                }
            }

            if (propertyInfos.Any())
            {
                InMemoryCache.Instance.PrimaryKeyMapping[entityType] = propertyInfos;

                return propertyInfos;
            }

            return Enumerable.Empty<PropertyInfo>();
        }

        /// <summary>
        /// Determines whether the specified entity type has a composite primary key defined.
        /// </summary>
        /// <param name="entityType">The type of the entity.</param>
        public static bool HasCompositePrimaryKey(Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));

            return GetPrimaryKeyPropertyInfos(entityType).Count() > 1;
        }

        /// <summary>
        /// Gets the composite primary key property information for the specified type.
        /// </summary>
        /// <returns>The primary key property infos.</returns>
        public static IEnumerable<PropertyInfo> GetPrimaryKeyPropertyInfos<T>()
        {
            return GetPrimaryKeyPropertyInfos(typeof(T));
        }

        /// <summary>
        /// Gets the collection of primary key values for the specified object.
        /// </summary>
        /// <returns>The primary key value.</returns>
        public static object[] GetPrimaryKeyValues(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var keyValues = GetPrimaryKeyPropertyInfos(obj.GetType())
                .Select(x => x.GetValue(obj, null))
                .ToArray();

            return keyValues;
        }

        /// <summary>
        /// Gets the primary key value for the specified object. If the primary key is defined as a composite key, then the result will be returned as a tuple.
        /// </summary>
        /// <returns>The primary key value.</returns>
        public static object GetPrimaryKeyValue(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            return Combine(GetPrimaryKeyValues(obj));
        }

        /// <summary>
        /// Merges the specified collection of key values into a tuple if there are more than one key; otherwise, it will return the single key object.
        /// </summary>
        /// <returns>The merged primary key value.</returns>
        public static object Combine(object[] keyValues)
        {
            if (keyValues == null)
                throw new ArgumentNullException(nameof(keyValues));

            object key;

            switch (keyValues.Length)
            {
                case 3:
                    {
                        key = Tuple.Create(keyValues[0], keyValues[1], keyValues[2]);
                        break;
                    }
                case 2:
                    {
                        key = Tuple.Create(keyValues[0], keyValues[1]);
                        break;
                    }
                default:
                    {
                        key = keyValues[0];
                        break;
                    }
            }

            return key;
        }

        /// <summary>
        /// Returns a specification for getting an entity by it's primary key.
        /// </summary>
        /// <returns>The new specification.</returns>
        public static ISpecificationQueryStrategy<TEntity> GetByPrimaryKeySpecification<TEntity>(params object[] keyValues) where TEntity : class
        {
            if (keyValues == null)
                throw new ArgumentNullException(nameof(keyValues));

            var propInfos = GetPrimaryKeyPropertyInfos<TEntity>().ToList();

            if (keyValues.Length != propInfos.Count)
                throw new ArgumentException(Resources.EntityPrimaryKeyValuesLengthMismatch, nameof(keyValues));

            var parameter = Expression.Parameter(typeof(TEntity), "x");

            BinaryExpression exp = null;

            for (var i = 0; i < propInfos.Count; i++)
            {
                var propInfo = propInfos[i];
                var keyValue = keyValues[i];

                var x = Expression.Equal(
                    Expression.PropertyOrField(parameter, propInfo.Name),
                    Expression.Constant(keyValue));

                exp = exp == null ? x : Expression.AndAlso(x, exp);
            }

            var lambda = Expression.Lambda<Func<TEntity, bool>>(exp, parameter);

            return new SpecificationQueryStrategy<TEntity>(lambda);
        }

        /// <summary>
        /// Throws an exception if the specified key type collection does not match the ones defined for the entity.
        /// </summary>
        /// <param name="keyTypes">The key type collection to check against.</param>
        public static void ThrowsIfInvalidPrimaryKeyDefinition<TEntity>(params Type[] keyTypes) where TEntity : class
        {
            if (keyTypes == null)
                throw new ArgumentNullException(nameof(keyTypes));

            var definedKeyInfos = GetPrimaryKeyPropertyInfos<TEntity>();

            if (definedKeyInfos.Count() > 1)
            {
                var hasNoKeyOrdering = definedKeyInfos.Any(x =>
                {
                    var columnAttribute = x.GetCustomAttribute<ColumnAttribute>();
                    if (columnAttribute == null)
                        return true;

                    return columnAttribute.Order <= 0;
                });

                if (hasNoKeyOrdering)
                {
                    throw new InvalidOperationException(string.Format(
                        Resources.UnableToDetermineCompositePrimaryKeyOrdering, typeof(TEntity).FullName));
                }
            }

            var definedKeyTypes = definedKeyInfos
                .Select(x => x.PropertyType)
                .ToArray();

            if (keyTypes.Length != definedKeyTypes.Length || definedKeyTypes.Where((t, i) => t != keyTypes[i]).Any())
                throw new InvalidOperationException(Resources.EntityPrimaryKeyTypesMismatch);
        }

        /// <summary>
        /// Gets the primary key name checks.
        /// </summary>
        /// <param name="entityType">The entity type to get the primary key from.</param>
        /// <remarks>Assumes the entity has either an 'Id' property or 'EntityName' + 'Id'.</remarks>
        /// <returns>The list of primary key names to check.</returns>
        private static IEnumerable<string> GetDefaultPrimaryKeyNameChecks(Type entityType)
        {
            const string suffix = "Id";

            return new[] { suffix, entityType.Name + suffix };
        }
    }
}
