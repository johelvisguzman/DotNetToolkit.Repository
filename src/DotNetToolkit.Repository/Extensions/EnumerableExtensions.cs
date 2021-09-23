namespace DotNetToolkit.Repository.Extensions
{
    using Configuration.Conventions;
    using Configuration.Conventions.Internal;
    using Extensions.Internal;
    using JetBrains.Annotations;
    using Query;
    using Query.Strategies;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Utility;

    /// <summary>
    /// Contains various utility methods for applying options to the specified <see cref="IEnumerable{T}" />.
    /// </summary>
    public static class EnumerableExtensions
    {
        private static readonly MethodInfo _castMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast));
        private static readonly MethodInfo _toListMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList));

        /// <summary>
        /// Apply a specification strategy options to the specified entity's query.
        /// </summary>
        /// <returns>The entity's query with the applied options.</returns>
        public static IEnumerable<T> ApplySpecificationOptions<T>([NotNull] this IEnumerable<T> source, [CanBeNull] IQueryOptions<T> options) where T : class
        {
            Guard.NotNull(source, nameof(source));

            var predicate = options?.SpecificationStrategy?.Predicate;
            if (predicate != null)
            {
                source = source.Where(predicate.Compile());
            }

            return source;
        }

        /// <summary>
        /// Apply a sorting options to the specified entity's query.
        /// </summary>
        /// <returns>The entity's query with the applied options.</returns>
        public static IOrderedEnumerable<T> ApplySortingOptions<T>([NotNull] this IEnumerable<T> source, [NotNull] IRepositoryConventions conventions, [CanBeNull] IQueryOptions<T> options) where T : class
        {
            Guard.NotNull(source, nameof(source));

            var sorting = new Dictionary<string, SortOrder>();

            if (options != null)
                sorting = options.SortingProperties.ToDictionary(x => x.Key, x => x.Value);

            // Sorts on the composite key by default if no sorting is provided
            if (!sorting.Any())
            {
                foreach (var primaryKeyPropertyInfo in conventions.GetPrimaryKeyPropertyInfos<T>())
                {
                    sorting.Add(primaryKeyPropertyInfo.Name, SortOrder.Ascending);
                }
            }

            var primarySorting = sorting.ElementAt(0);
            var primarySortingOrder = primarySorting.Value;
            var primarySortingProperty = primarySorting.Key;

            var sortedQuery = primarySortingOrder == SortOrder.Descending
                ? source.OrderByDescending(primarySortingProperty)
                : source.OrderBy(primarySortingProperty);

            for (var i = 1; i < sorting.Count; i++)
            {
                var sort = sorting.ElementAt(i);
                var sortOrder = sort.Value;
                var sortProperty = sort.Key;

                sortedQuery = sortOrder == SortOrder.Descending
                    ? sortedQuery.ThenByDescending(sortProperty)
                    : sortedQuery.ThenBy(sortProperty);
            }

            return sortedQuery;
        }

        /// <summary>
        /// Apply a paging options to the specified entity's query.
        /// </summary>
        /// <returns>The entity's query with the applied options.</returns>
        public static IEnumerable<T> ApplyPagingOptions<T>([NotNull] this IEnumerable<T> source, [CanBeNull] IQueryOptions<T> options)
        {
            Guard.NotNull(source, nameof(source));

            if (options != null && options.PageSize != -1)
            {
                source = source.Skip((options.PageIndex - 1) * options.PageSize).Take(options.PageSize);
            }

            return source;
        }

        /// <summary>
        /// Apply a fetching options to the specified entity's query.
        /// </summary>
        /// <returns>The entity's query with the applied options.</returns>
        public static IEnumerable<T> ApplyFetchingOptions<T>([NotNull] this IEnumerable<T> query, [CanBeNull] IFetchQueryStrategy<T> fetchStrategy, [NotNull] Func<Type, IEnumerable<object>> innerQueryCallback) where T : class
        {
            Guard.NotNull(query, nameof(query));
            Guard.NotNull(innerQueryCallback, nameof(innerQueryCallback));

            var mainTableType = typeof(T);
            var mainTablePropertiesMap = mainTableType.GetRuntimeProperties().ToDictionary(x => x.Name);
            var fetchingPaths = fetchStrategy.DefaultIfFetchStrategyEmpty().PropertyPaths.ToList();

            foreach (var path in fetchingPaths)
            {
                // Only do a join when the primary table has a foreign key property for the join table
                var joinTablePropertyInfo = mainTablePropertiesMap[path];
                var joinTableForeignKeyPropertyInfos = ForeignKeyConventionHelper.GetForeignKeyPropertyInfos(joinTablePropertyInfo);

                if (joinTableForeignKeyPropertyInfos != null && joinTableForeignKeyPropertyInfos.Length > 0)
                {
                    var joinTableType = joinTablePropertyInfo.PropertyType.TryGetGenericTypeOrDefault(out bool isJoinPropertyCollection);
                    var innerQuery = innerQueryCallback(joinTableType);

                    var mainTablePrimaryKeyPropertyInfos = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(mainTableType);
                    var mainTablePropertyInfo = joinTableType.GetRuntimeProperties().FirstOrDefault(x => x.PropertyType == mainTableType);

                    var comparer = new ObjectArrayComparer();

                    // key selector functions
                    object[] outerKeySelectorFunc(T outer)
                    {
                        return mainTablePrimaryKeyPropertyInfos.Select(pi => pi.GetValue(outer)).ToArray();
                    }

                    object[] innerKeySelectorFunc(object inner)
                    {
                        return joinTableForeignKeyPropertyInfos.Select(pi => pi.GetValue(inner)).ToArray();
                    }

                    if (isJoinPropertyCollection)
                    {
                        query = query.GroupJoin(
                            innerQuery,
                            outerKeySelectorFunc,
                            innerKeySelectorFunc,
                            (outer, inner) =>
                            {
                                if (inner != null)
                                {
                                    var items = inner.Select(item =>
                                    {
                                        if (mainTablePropertyInfo != null)
                                        {
                                            // Sets the main table property in the join table
                                            mainTablePropertyInfo.SetValue(item, outer);
                                        }

                                        return item;
                                    }).ToList(joinTableType);

                                    // Sets the join table property in the main table
                                    joinTablePropertyInfo.SetValue(outer, items);
                                }

                                return outer;
                            }, comparer);
                    }
                    else
                    {
                        query = query.LeftJoin(
                            innerQuery,
                            outerKeySelectorFunc,
                            innerKeySelectorFunc,
                            (outer, inner) =>
                            {
                                if (inner != null)
                                {
                                    if (mainTablePropertyInfo != null)
                                    {
                                        // Sets the main table property in the join table
                                        mainTablePropertyInfo.SetValue(inner, outer);
                                    }

                                    // Sets the join table property in the main table
                                    joinTablePropertyInfo.SetValue(outer, inner);
                                }

                                return outer;
                            }, comparer);
                    }
                }
            }

            return query;
        }

        private static ICollection ToList(this IEnumerable items, Type type)
        {
            var castItems = _castMethod
                .MakeGenericMethod(new Type[] { type })
                .Invoke(null, new object[] { items });

            var list = _toListMethod
                .MakeGenericMethod(new Type[] { type })
                .Invoke(null, new object[] { castItems });

            return (ICollection)list;
        }

        private static IEnumerable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            return outer.GroupJoin(
                    inner,
                    outerKeySelector,
                    innerKeySelector,
                    (o, i) => new { outer = o, innerCollection = i },
                    comparer)
                .SelectMany(
                    a => a.innerCollection.DefaultIfEmpty(),
                    (a, i) => resultSelector(a.outer, i));
        }

        private static IOrderedEnumerable<T> ApplyOrder<T>(this IEnumerable<T> source, string propertyName, string methodName)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotEmpty(propertyName, nameof(propertyName));
            Guard.NotEmpty(methodName, nameof(methodName));

            var lambda = ExpressionHelper.GetExpression<T>(propertyName);
            var type = ExpressionHelper.GetMemberExpression(lambda).Type;
            var func = lambda.Compile();

            var result = typeof(Enumerable)
                .GetRuntimeMethods()
                .Single(method => method.Name == methodName &&
                                  method.IsGenericMethodDefinition &&
                                  method.GetGenericArguments().Length == 2 &&
                                  method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), type)
                .Invoke(null, new object[] { source, func });

            return (IOrderedEnumerable<T>)result;
        }

        private static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> source, string propertyName)
        {
            return ApplyOrder<T>(source, propertyName, nameof(Enumerable.OrderBy));
        }

        private static IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> source, string propertyName)
        {
            return ApplyOrder<T>(source, propertyName, nameof(Enumerable.OrderByDescending));
        }

        private static IOrderedEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> source, string propertyName)
        {
            return ApplyOrder<T>(source, propertyName, nameof(Enumerable.ThenBy));
        }

        private static IOrderedEnumerable<T> ThenByDescending<T>(this IOrderedEnumerable<T> source, string propertyName)
        {
            return ApplyOrder<T>(source, propertyName, nameof(Enumerable.ThenByDescending));
        }

        private class ObjectArrayComparer : IEqualityComparer<object[]>
        {
            public bool Equals(object[] x, object[] y)
            {
                return x.Length == y.Length && Enumerable.SequenceEqual(x, y);
            }

            public int GetHashCode(object[] o)
            {
                var result = o.Aggregate((a, b) => a.GetHashCode() ^ b.GetHashCode());
                return result.GetHashCode();
            }
        }
    }
}