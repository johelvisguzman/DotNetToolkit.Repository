namespace DotNetToolkit.Repository.Extensions
{
    using Configuration.Conventions;
    using Helpers;
    using Queries;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal static class QueryableExtensions
    {
        public static IQueryable<T> ApplySpecificationOptions<T>(this IQueryable<T> query, IQueryOptions<T> options) where T : class
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (options != null && options.SpecificationStrategy != null)
                query = options.SpecificationStrategy.SatisfyingEntitiesFrom(query);

            return query;
        }

        public static IOrderedQueryable<T> ApplySortingOptions<T>(this IQueryable<T> query, IQueryOptions<T> options) where T : class
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var sorting = new Dictionary<string, SortOrder>();

            if (options != null)
                sorting = options.SortingPropertiesMapping.ToDictionary(x => x.Key, x => x.Value);

            // Sorts on the composite key by default if no sorting is provided
            if (!sorting.Any())
            {
                foreach (var primaryKeyPropertyInfo in PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos<T>())
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

        public static IQueryable<T> ApplyPagingOptions<T>(this IQueryable<T> query, IQueryOptions<T> options)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (options != null && options.PageSize != -1)
            {
                query = query.Skip((options.PageIndex - 1) * options.PageSize).Take(options.PageSize);
            }

            return query;
        }

        private static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string propertyName, string methodName)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

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

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
        {
            return ApplyOrder<T>(source, propertyName, "OrderBy");
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            return ApplyOrder<T>(source, propertyName, "OrderByDescending");
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string propertyName)
        {
            return ApplyOrder<T>(source, propertyName, "ThenBy");
        }

        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string propertyName)
        {
            return ApplyOrder<T>(source, propertyName, "ThenByDescending");
        }
    }
}
