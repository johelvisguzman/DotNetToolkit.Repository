namespace DotNetToolkit.Repository.Extensions
{
    using Configuration;
    using Configuration.Conventions;
    using Extensions.Internal;
    using JetBrains.Annotations;
    using Properties;
    using Queries.Strategies;
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Utility;

    /// <summary>
    /// Contains various extension methods for <see cref="IRepositoryConventions" />
    /// </summary>
    public static class RepositoryConventionsExtensions
    {
        private readonly static ConcurrentDictionary<Type, ConcurrentDictionary<Type, PropertyInfo[]>> _primaryKeyPropertyInfosCache
            = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, PropertyInfo[]>>();
        private readonly static ConcurrentDictionary<Type, ConcurrentDictionary<Tuple<Type, string>, Tuple<BinaryExpression, ParameterExpression>>> _primaryKeySpecificationsCache
            = new ConcurrentDictionary<Type, ConcurrentDictionary<Tuple<Type, string>, Tuple<BinaryExpression, ParameterExpression>>>();
        private readonly static ConcurrentDictionary<Type, ConcurrentDictionary<Tuple<Type, Type>, PropertyInfo[]>> _foreignKeyPropertyInfosCache
            = new ConcurrentDictionary<Type, ConcurrentDictionary<Tuple<Type, Type>, PropertyInfo[]>>();
        private readonly static ConcurrentDictionary<Type, ConcurrentDictionary<Type, string>> _tableNameCache
            = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, string>>();
        private readonly static ConcurrentDictionary<Type, ConcurrentDictionary<PropertyInfo, string>> _columnNameCache
            = new ConcurrentDictionary<Type, ConcurrentDictionary<PropertyInfo, string>>();
        private readonly static ConcurrentDictionary<Type, ConcurrentDictionary<PropertyInfo, int?>> _columnOrderCache
            = new ConcurrentDictionary<Type, ConcurrentDictionary<PropertyInfo, int?>>();
        private readonly static ConcurrentDictionary<Type, ConcurrentDictionary<PropertyInfo, bool>> _isColumnMappedCache
            = new ConcurrentDictionary<Type, ConcurrentDictionary<PropertyInfo, bool>>();
        private readonly static ConcurrentDictionary<Type, ConcurrentDictionary<PropertyInfo, bool>> _isColumnIdentityCallback
            = new ConcurrentDictionary<Type, ConcurrentDictionary<PropertyInfo, bool>>();

        /// <summary>
        /// Gets a collection of primary keys for the specified type.
        /// </summary>
        /// <param name="source">The repository conventions.</param>
        /// <param name="type">The type.</param>
        /// <returns>The collection of primary keys for the specified type.</returns>
        public static PropertyInfo[] GetPrimaryKeyPropertyInfos([NotNull] this IRepositoryConventions source, [NotNull] Type type)
        {
            EnsureOwner(source);
            Guard.NotNull(type, nameof(type));

            var store = _primaryKeyPropertyInfosCache.GetOrAdd(source.Owner, new ConcurrentDictionary<Type, PropertyInfo[]>());

            if (!store.TryGetValue(type, out var result))
            {
                store[type] = result = EnsureCallback(
                    source.PrimaryKeysCallback,
                    nameof(source.PrimaryKeysCallback))(type) ?? new PropertyInfo[0];
            }

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
            EnsureOwner(source);
            Guard.NotEmpty(keyValues, nameof(keyValues));

            var type = typeof(T);
            var store = _primaryKeySpecificationsCache.GetOrAdd(source.Owner, new ConcurrentDictionary<Tuple<Type, string>, Tuple<BinaryExpression, ParameterExpression>>());
            var key = Tuple.Create(type, string.Join(":", keyValues));

            if (!store.TryGetValue(key, out var result))
            {
                var propInfos = source.GetPrimaryKeyPropertyInfos(type);

                if (keyValues.Length != propInfos.Length)
                    throw new ArgumentException(Resources.EntityPrimaryKeyValuesLengthMismatch, nameof(keyValues));

                var parameter = Expression.Parameter(type, "x");

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

                store[key] = result = Tuple.Create(exp, parameter);
            }

            var lambda = Expression.Lambda<Func<T, bool>>(result.Item1, result.Item2);

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
        {
            EnsureOwner(source);
            Guard.NotNull(sourceType, nameof(sourceType));
            Guard.NotNull(foreignType, nameof(foreignType));

            var store = _foreignKeyPropertyInfosCache.GetOrAdd(source.Owner, new ConcurrentDictionary<Tuple<Type, Type>, PropertyInfo[]>());
            var key = Tuple.Create(sourceType, foreignType);

            if (!store.TryGetValue(key, out var result))
            {
                store[key] = result = EnsureCallback(
                    source.ForeignKeysCallback,
                    nameof(source.ForeignKeysCallback))(sourceType, foreignType) ?? new PropertyInfo[0];
            }

            return result;
        }

        /// <summary>
        /// Gets a table name for the specified type.
        /// </summary>
        /// <param name="source">The configurable conventions.</param>
        /// <param name="type">The type.</param>
        /// <returns>The table name for the specified type.</returns>
        public static string GetTableName([NotNull] this IRepositoryConventions source, [NotNull] Type type)
        {
            EnsureOwner(source);
            Guard.NotNull(type, nameof(type));

            var store = _tableNameCache.GetOrAdd(source.Owner, new ConcurrentDictionary<Type, string>());

            if (!store.TryGetValue(type, out var result))
            {
                store[type] = result = EnsureCallback(
                   source.TableNameCallback,
                   nameof(source.TableNameCallback))(type);
            }

            return result;
        }

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
        {
            EnsureOwner(source);
            Guard.NotNull(pi, nameof(pi));

            var store = _columnNameCache.GetOrAdd(source.Owner, new ConcurrentDictionary<PropertyInfo, string>());

            if (!store.TryGetValue(pi, out var result))
            {
                store[pi] = result = EnsureCallback(
                   source.ColumnNameCallback,
                   nameof(source.ColumnNameCallback))(pi);
            }

            return result;
        }

        /// <summary>
        /// Gets a column order for the specified property.
        /// </summary>
        /// <param name="source">The configurable conventions.</param>
        /// <param name="pi">The property.</param>
        /// <returns>The column order for the specified property.</returns>
        public static int? GetColumnOrder([NotNull] this IRepositoryConventions source, [NotNull] PropertyInfo pi)
        {
            EnsureOwner(source);
            Guard.NotNull(pi, nameof(pi));

            var store = _columnOrderCache.GetOrAdd(source.Owner, new ConcurrentDictionary<PropertyInfo, int?>());

            if (!store.TryGetValue(pi, out var result))
            {
                store[pi] = result = EnsureCallback(
                   source.ColumnOrderCallback,
                   nameof(source.ColumnOrderCallback))(pi);
            }

            return result;
        }

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
        {
            EnsureOwner(source);
            Guard.NotNull(pi, nameof(pi));

            var store = _isColumnMappedCache.GetOrAdd(source.Owner, new ConcurrentDictionary<PropertyInfo, bool>());

            if (!store.TryGetValue(pi, out var result))
            {
                store[pi] = result = EnsureCallback(
                   source.IsColumnMappedCallback,
                   nameof(source.IsColumnMappedCallback))(pi);
            }

            return result;
        }

        /// <summary>
        /// Determines whether the specified property is defined as identity.
        /// </summary>
        /// <param name="source">The configurable conventions.</param>
        /// <param name="pi">The property.</param>
        /// <returns><c>true</c> if column is defined as identity.; otherwise, <c>false</c>.</returns>
        public static bool IsColumnIdentity([NotNull] this IRepositoryConventions source, [NotNull] PropertyInfo pi)
        {
            EnsureOwner(source);
            Guard.NotNull(pi, nameof(pi));

            var store = _isColumnIdentityCallback.GetOrAdd(source.Owner, new ConcurrentDictionary<PropertyInfo, bool>());

            if (!store.TryGetValue(pi, out var result))
            {
                store[pi] = result = EnsureCallback(
                   source.IsColumnIdentityCallback,
                   nameof(source.IsColumnIdentityCallback))(pi);
            }

            return result;
        }

        /// <summary>
        /// Throws an exception if the specified entity type has an invalid primary key definition.
        /// </summary>
        /// <param name="source">The configurable conventions.</param>
        internal static void ThrowsIfInvalidPrimaryKeyDefinition<T>([NotNull] this IRepositoryConventions source) where T : class
        {
            EnsureOwner(source);

            var definedKeyInfos = source.GetPrimaryKeyPropertyInfos<T>();

            if (!definedKeyInfos.Any())
            {
                throw new InvalidOperationException(string.Format(
                    Resources.EntityRequiresPrimaryKey,
                    typeof(T).FullName));
            }
        }

        /// <summary>
        /// Applies the specified conventions to the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        public static void Apply<T>([NotNull] this T target, [NotNull] T source) where T : IRepositoryConventions
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotNull(target, nameof(target));

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

        private static Type EnsureOwner([NotNull] IRepositoryConventions source)
        {
            Guard.NotNull(source, nameof(source));

            var owner = Guard.EnsureNotNull(
                source.Owner,
                string.Format("The conventions '{0}' cannot be null.", nameof(source.Owner)));

            if (!owner.ImplementsInterface(typeof(IRepositoryContext)))
                throw new InvalidOperationException(string.Format(
                    "The conventions '{0}' is not an instance of type '{1}'.", nameof(source.Owner), typeof(IRepositoryContext).FullName));

            return owner;
        }

        private static T EnsureCallback<T>([NotNull] T value, [InvokerParameterName] string parameterName) where T : class
            => Guard.EnsureNotNull(
                value,
                string.Format("The conventions callback function '{0}' cannot be null.", parameterName));
    }
}
