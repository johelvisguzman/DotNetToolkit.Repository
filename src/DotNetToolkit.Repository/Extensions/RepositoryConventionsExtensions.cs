namespace DotNetToolkit.Repository.Extensions
{
    using Configuration.Conventions;
    using JetBrains.Annotations;
    using Properties;
    using Query.Strategies;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Utility;

    /// <summary>
    /// Contains various extension methods for <see cref="IRepositoryConventions" />
    /// </summary>
    public static class RepositoryConventionsExtensions
    {
        /// <summary>
        /// Gets a collection of primary keys for the specified type.
        /// </summary>
        /// <param name="source">The repository conventions.</param>
        /// <param name="type">The type.</param>
        /// <returns>The collection of primary keys for the specified type.</returns>
        public static PropertyInfo[] GetPrimaryKeyPropertyInfos([NotNull] this IRepositoryConventions source, [NotNull] Type type)
        {
            Guard.NotNull(type, nameof(type));

            var result = EnsureCallback(
                source.PrimaryKeysCallback,
                nameof(source.PrimaryKeysCallback))(type) ?? new PropertyInfo[0];

            return result;
        }

        /// <summary>
        /// Gets a collection of primary keys for the specified type.
        /// </summary>
        /// <param name="source">The repository conventions.</param>
        /// <returns>The collection of primary keys for the specified type.</returns>
        public static PropertyInfo[] GetPrimaryKeyPropertyInfos<T>([NotNull] this IRepositoryConventions source)
            => GetPrimaryKeyPropertyInfos(source, typeof(T));

        /// <summary>
        /// Determines whether the specified entity type has a composite primary key defined.
        /// </summary>
        /// <param name="source">The repository conventions.</param>
        /// <param name="entityType">The type of the entity.</param>
        /// <returns><c>true</c> if the specified entity type has a composite primary key defined; otherwise, <c>false</c>.</returns>
        public static bool HasCompositePrimaryKey([NotNull] this IRepositoryConventions source, [NotNull] Type entityType)
            => GetPrimaryKeyPropertyInfos(source, entityType).Length > 1;

        /// <summary>
        /// Gets the collection of primary key values for the specified object.
        /// </summary>
        /// <param name="source">The repository conventions.</param>
        /// <param name="obj">The entity to get the key values from.</param>
        /// <returns>The primary key value.</returns>
        public static object[] GetPrimaryKeyValues([NotNull] this IRepositoryConventions source, [NotNull] object obj)
            => GetPrimaryKeyPropertyInfos(source, Guard.NotNull(obj, nameof(obj)).GetType())
                .Select(x => x.GetValue(obj, null))
                .ToArray();

        /// <summary>
        /// Returns a specification query strategy for getting an entity by it's primary key values.
        /// </summary>
        /// <param name="source">The repository conventions.</param>
        /// <param name="keyValues">The primary key values.</param>
        /// <returns>The new specification.</returns>
        public static ISpecificationQueryStrategy<T> GetByPrimaryKeySpecification<T>([NotNull] this IRepositoryConventions source, [NotNull] params object[] keyValues) where T : class
        {
            Guard.NotEmpty(keyValues, nameof(keyValues));

            var propInfos = source.GetPrimaryKeyPropertyInfos<T>();

            if (keyValues.Length != propInfos.Length)
                throw new ArgumentException(Resources.EntityPrimaryKeyValuesLengthMismatch, nameof(keyValues));

            var lambda = ExpressionHelper.Combine<T>(propInfos, keyValues, (left, right) => Expression.AndAlso(left, right));

            return new SpecificationQueryStrategy<T>(lambda);
        }

        /// <summary>
        /// Throws an exception if the specified entity type has an invalid primary key definition.
        /// </summary>
        /// <param name="source">The configurable conventions.</param>
        internal static void ThrowsIfInvalidPrimaryKeyDefinition<T>([NotNull] this IRepositoryConventions source) where T : class
        {
            var definedKeyInfos = source.GetPrimaryKeyPropertyInfos<T>();

            if (!definedKeyInfos.Any())
            {
                throw new InvalidOperationException(string.Format(
                    Resources.EntityRequiresPrimaryKey,
                    typeof(T).FullName));
            }
        }

        private static T EnsureCallback<T>([NotNull] T value, [InvokerParameterName] string parameterName) where T : class
            => Guard.EnsureNotNull(
                value,
                string.Format("The conventions callback function '{0}' cannot be null.", parameterName));
    }
}
