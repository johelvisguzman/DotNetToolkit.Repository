namespace DotNetToolkit.Repository.Helpers
{
    using Configuration.Conventions;
    using Queries;
    using System;
    using System.Linq;
    using System.Reflection;

    internal static class QueryableHelpers
    {
        public static IQueryable<T> Apply<T>(this IQueryOptions<T> options, IQueryable<T> query) where T : class
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (options.SpecificationStrategy != null)
                query = options.SpecificationStrategy.SatisfyingEntitiesFrom(query);

            var sortings = options.SortingPropertiesMapping.ToDictionary(x => x.Key, x => x.Value);

            // Sorts on the composite key by default if no sorting is provided
            if (!sortings.Any())
            {
                foreach (var primaryKeyPropertyInfo in PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos<T>())
                {
                    sortings.Add(primaryKeyPropertyInfo.Name, SortOrder.Ascending);
                }
            }

            var primarySorting = sortings.ElementAt(0);
            var primarySortingOrder = primarySorting.Value;
            var primarySortingProperty = primarySorting.Key;

            var sortedQuery = primarySortingOrder == SortOrder.Descending
                ? query.OrderByDescending(primarySortingProperty)
                : query.OrderBy(primarySortingProperty);

            for (var i = 1; i < sortings.Count; i++)
            {
                var sorting = sortings.ElementAt(i);
                var sortingOrder = sorting.Value;
                var sortingProperty = sorting.Key;

                sortedQuery = sortingOrder == SortOrder.Descending
                    ? sortedQuery.ThenByDescending(sortingProperty)
                    : sortedQuery.ThenBy(sortingProperty);
            }

            query = sortedQuery;

            if (options.PageSize != -1)
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
