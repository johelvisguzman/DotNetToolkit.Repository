namespace DotNetToolkit.Repository.Extensions
{
    using Configuration.Conventions;
    using JetBrains.Annotations;
    using Properties;
    using Queries.Strategies;
    using System;
    using System.Globalization;
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
            => EnsureCallback(
                   Guard.NotNull(source).PrimaryKeysCallback,
                   nameof(source.PrimaryKeysCallback))(Guard.NotNull(type)) ?? new PropertyInfo[0];

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
            => GetPrimaryKeyPropertyInfos(source, Guard.NotNull(obj).GetType())
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
            Guard.NotNull(source);
            Guard.NotEmpty(keyValues);

            var propInfos = source.GetPrimaryKeyPropertyInfos<T>();

            if (keyValues.Length != propInfos.Length)
                throw new ArgumentException(Resources.EntityPrimaryKeyValuesLengthMismatch, nameof(keyValues));

            var parameter = Expression.Parameter(typeof(T), "x");

            BinaryExpression exp = null;

            for (var i = 0; i < propInfos.Length; i++)
            {
                var propInfo = propInfos[i];
                var keyValue = keyValues[i];

                var x = Expression.Equal(
                    Expression.PropertyOrField(parameter, propInfo.Name),
                    Expression.Constant(keyValue));

                exp = exp == null ? x : Expression.AndAlso(x, exp);
            }

            Guard.EnsureNotNull(exp, "The expression cannot be null.");

            var lambda = Expression.Lambda<Func<T, bool>>(exp, parameter);

            return new SpecificationQueryStrategy<T>(lambda);
        }

        /// <summary>
        /// Gets a collection of foreign key properties that matches the specified foreign type.
        /// </summary>
        /// <param name="source">The repository conventions.</param>
        /// <param name="sourceType">The source type.</param>
        /// <param name="foreignType">The foreign type to match.</param>
        /// <returns>The collection of foreign key properties that matches the specified foreign type.</returns>
        public static PropertyInfo[] GetForeignKeyPropertyInfos([NotNull] this IRepositoryConventions source, [NotNull] Type sourceType, [NotNull] Type foreignType)
            => EnsureCallback(
                   Guard.NotNull(source).ForeignKeysCallback,
                   nameof(source.ForeignKeysCallback))(Guard.NotNull(sourceType), Guard.NotNull(foreignType)) ?? new PropertyInfo[0];

        /// <summary>
        /// Gets a table name for the specified type.
        /// </summary>
        /// <param name="source">The configurable conventions.</param>
        /// <param name="type">The type.</param>
        /// <returns>The table name for the specified type.</returns>
        public static string GetTableName([NotNull] this IRepositoryConventions source, [NotNull] Type type)
            => EnsureCallback(
                Guard.NotNull(source).TableNameCallback,
                nameof(source.TableNameCallback))(Guard.NotNull(type));

        /// <summary>
        /// Gets a table name for the specified type.
        /// </summary>
        /// <param name="source">The configurable conventions.</param>
        /// <returns>The table name for the specified type.</returns>
        public static string GetTableName<T>([NotNull] this IRepositoryConventions source)
            => GetTableName(source, typeof(T));

        /// <summary>
        /// Gets a column name for the specified property.
        /// </summary>
        /// <param name="source">The configurable conventions.</param>
        /// <param name="pi">The property.</param>
        /// <returns>The column name for the specified property.</returns>
        public static string GetColumnName([NotNull] this IRepositoryConventions source, [NotNull] PropertyInfo pi)
            => EnsureCallback(
                Guard.NotNull(source).ColumnNameCallback,
                nameof(source.ColumnNameCallback))(Guard.NotNull(pi));

        /// <summary>
        /// Gets a column order for the specified property.
        /// </summary>
        /// <param name="source">The configurable conventions.</param>
        /// <param name="pi">The property.</param>
        /// <returns>The column order for the specified property.</returns>
        public static int? GetColumnOrder([NotNull] this IRepositoryConventions source, [NotNull] PropertyInfo pi)
            => EnsureCallback(
                Guard.NotNull(source).ColumnOrderCallback,
               nameof(source.ColumnOrderCallback))(Guard.NotNull(pi));

        /// <summary>
        /// Gets a column order for the specified property or a default value of <see cref="Int32.MaxValue" />.
        /// </summary>
        /// <param name="source">The configurable conventions.</param>
        /// <param name="pi">The property.</param>
        /// <returns>The column order for the specified property or a default value of <see cref="Int32.MaxValue"/>.</returns>
        public static int GetColumnOrderOrDefault([NotNull] this IRepositoryConventions source, [NotNull] PropertyInfo pi)
            => GetColumnOrder(source, pi) ?? Int32.MaxValue;

        /// <summary>
        /// Determines whether the specified property is mapped.
        /// </summary>
        /// <param name="source">The configurable conventions.</param>
        /// <param name="pi">The property.</param>
        /// <returns><c>true</c> if column is mapped; otherwise, <c>false</c>.</returns>
        public static bool IsColumnMapped([NotNull] this IRepositoryConventions source, [NotNull] PropertyInfo pi)
            => EnsureCallback(
                Guard.NotNull(source).IsColumnMappedCallback,
                nameof(source.IsColumnMappedCallback))(Guard.NotNull(pi));

        /// <summary>
        /// Determines whether the specified property is defined as identity.
        /// </summary>
        /// <param name="source">The configurable conventions.</param>
        /// <param name="pi">The property.</param>
        /// <returns><c>true</c> if column is defined as identity.; otherwise, <c>false</c>.</returns>
        public static bool IsColumnIdentity([NotNull] this IRepositoryConventions source, [NotNull] PropertyInfo pi)
            => EnsureCallback(
                Guard.NotNull(source).IsColumnIdentityCallback,
                nameof(source.IsColumnIdentityCallback))(Guard.NotNull(pi));

        /// <summary>
        /// Throws an exception if the specified key type collection does not match the ones defined for the entity.
        /// </summary>
        /// <param name="source">The configurable conventions.</param>
        /// <param name="keyTypes">The key type collection to check against.</param>
        public static void ThrowsIfInvalidPrimaryKeyDefinition<T>([NotNull] this IRepositoryConventions source, [NotNull] params Type[] keyTypes) where T : class
        {
            Guard.NotNull(source);
            Guard.NotEmpty(keyTypes);

            var definedKeyInfos = source.GetPrimaryKeyPropertyInfos<T>().ToList();

            if (!definedKeyInfos.Any())
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    Resources.EntityRequiresPrimaryKey,
                    typeof(T).FullName));
            }

            if (definedKeyInfos.Count > 1)
            {
                var hasNoKeyOrdering = definedKeyInfos.Any(x =>
                {
                    var columnOrdering = source.GetColumnOrder(x);

                    if (!columnOrdering.HasValue)
                        return true;

                    return columnOrdering.Value <= 0;
                });

                if (hasNoKeyOrdering)
                {
                    throw new InvalidOperationException(string.Format(
                        Resources.UnableToDetermineCompositePrimaryKeyOrdering, typeof(T).FullName));
                }
            }

            var definedKeyTypes = definedKeyInfos
                .Select(x => x.PropertyType)
                .ToArray();

            if (keyTypes.Length != definedKeyTypes.Length || definedKeyTypes.Where((t, i) => t != keyTypes[i]).Any())
                throw new InvalidOperationException(Resources.EntityPrimaryKeyTypesMismatch);
        }

        /// <summary>
        /// Applies the specified conventions to the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        public static void Apply<T>([NotNull] this T target, [NotNull] T source) where T : IRepositoryConventions
        {
            Guard.NotNull(source);
            Guard.NotNull(target);

            var properties = typeof(T).GetRuntimeProperties().ToArray();

            foreach (var pi in properties)
            {
                if (pi.CanWrite)
                {
                    var value = pi.GetValue(source, null);

                    if (value != null)
                    {
                        pi.SetValue(target, value);
                    }
                }
            }
        }

        private static T EnsureCallback<T>([ValidatedNotNull] [NoEnumeration]T value, [InvokerParameterName] string parameterName) where T : class
            => Guard.EnsureNotNull(
                value,
                "The conventions callback function '{0}.{1}' cannot be null.",
                typeof(IRepositoryConventions).Name,
                parameterName);
    }
}
