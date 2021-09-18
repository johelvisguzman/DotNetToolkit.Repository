namespace DotNetToolkit.Repository.Extensions
{
    using Configuration.Conventions;
    using Configuration.Conventions.Internal;
    using Extensions.Internal;
    using JetBrains.Annotations;
    using Query;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Utility;

    /// <summary>
    /// Contains various utility methods for applying options to the specified <see cref="IQueryable{T}" />.
    /// </summary>
    public static class QueryableExtensions
    {
        private static readonly MethodInfo CastMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast));
        private static readonly MethodInfo ToListMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList));

        /// <summary>
        /// Apply a specification strategy options to the specified entity's query.
        /// </summary>
        /// <returns>The entity's query with the applied options.</returns>
        public static IQueryable<T> ApplySpecificationOptions<T>([NotNull] this IQueryable<T> query, [CanBeNull] IQueryOptions<T> options) where T : class
        {
            Guard.NotNull(query, nameof(query));

            if (options != null && options.SpecificationStrategy != null)
                query = options.SpecificationStrategy.SatisfyingEntitiesFrom(query);

            return query;
        }

        /// <summary>
        /// Apply a sorting options to the specified entity's query.
        /// </summary>
        /// <returns>The entity's query with the applied options.</returns>
        public static IOrderedQueryable<T> ApplySortingOptions<T>([NotNull] this IQueryable<T> query, [NotNull] IRepositoryConventions conventions, [CanBeNull] IQueryOptions<T> options) where T : class
        {
            Guard.NotNull(query, nameof(query));

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
                ? query.OrderByDescending(primarySortingProperty)
                : query.OrderBy(primarySortingProperty);

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
        public static IQueryable<T> ApplyPagingOptions<T>([NotNull] this IQueryable<T> query, [CanBeNull] IQueryOptions<T> options)
        {
            Guard.NotNull(query, nameof(query));

            if (options != null && options.PageSize != -1)
            {
                query = query.Skip((options.PageIndex - 1) * options.PageSize).Take(options.PageSize);
            }

            return query;
        }

        private static IQueryable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(this IQueryable<TOuter> outer, IEnumerable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        {
            return outer.GroupJoin(
                    inner,
                    outerKeySelector,
                    innerKeySelector,
                    (o, i) => new { outer = o, innerCollection = i })
                .SelectMany(
                    a => a.innerCollection.DefaultIfEmpty(),
                    (a, i) => resultSelector(a.outer, i));
        }

        private static IList CastToList(Type type, IEnumerable items)
        {
            var castItems = CastMethod
                .MakeGenericMethod(new Type[] { type })
                .Invoke(null, new object[] { items });

            var list = ToListMethod
                .MakeGenericMethod(new Type[] { type })
                .Invoke(null, new object[] { castItems });

            return (IList)list;
        }

        private static IOrderedQueryable<T> ApplyOrder<T>([NotNull] IQueryable<T> source, [NotNull] string propertyName, [NotNull] string methodName)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotEmpty(propertyName, nameof(propertyName));
            Guard.NotEmpty(methodName, nameof(methodName));

            var lambda = ExpressionHelper.GetExpression<T>(propertyName);
            var type = ExpressionHelper.GetMemberExpression(lambda).Type;

            var result = typeof(Queryable)
                .GetRuntimeMethods()
                .Single(method => method.Name == methodName &&
                                  method.IsGenericMethodDefinition &&
                                  method.GetGenericArguments().Length == 2 &&
                                  method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), type)
                .Invoke(null, new object[] { source, lambda });

            return (IOrderedQueryable<T>)result;
        }

        private static IOrderedQueryable<T> OrderBy<T>([NotNull] this IQueryable<T> query, [NotNull] string propertyName)
        {
            return ApplyOrder<T>(query, propertyName, nameof(Queryable.OrderBy));
        }

        private static IOrderedQueryable<T> OrderByDescending<T>([NotNull] this IQueryable<T> query, [NotNull] string propertyName)
        {
            return ApplyOrder<T>(query, propertyName, nameof(Queryable.OrderByDescending));
        }

        private static IOrderedQueryable<T> ThenBy<T>([NotNull] this IOrderedQueryable<T> query, [NotNull] string propertyName)
        {
            return ApplyOrder<T>(query, propertyName, nameof(Queryable.ThenBy));
        }

        private static IOrderedQueryable<T> ThenByDescending<T>([NotNull] this IOrderedQueryable<T> query, [NotNull] string propertyName)
        {
            return ApplyOrder<T>(query, propertyName, nameof(Queryable.ThenByDescending));
        }
    }
}
