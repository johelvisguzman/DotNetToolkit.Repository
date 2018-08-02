namespace DotNetToolkit.Repository.Configuration.Conventions
{
    using Helpers;
    using Properties;
    using Specifications;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
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
        /// Returns a specification for getting an entity by it's primary key.
        /// </summary>
        /// <returns>The new specification.</returns>
        public static ISpecification<TEntity> GetByPrimaryKeySpecification<TEntity>(params object[] keyValues) where TEntity : class
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

            return new Specification<TEntity>(lambda);
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
