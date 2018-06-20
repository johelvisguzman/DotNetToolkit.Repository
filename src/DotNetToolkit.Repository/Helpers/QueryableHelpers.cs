namespace DotNetToolkit.Repository.Helpers
{
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

            if (options.Specification != null)
                query = options.Specification.SatisfyingEntitiesFrom(query);

            if (options.SortingPropertiesMapping.Any())
            {
                var primarySorting = options.SortingPropertiesMapping.ElementAt(0);
                var primarySortingOrder = primarySorting.Value;
                var primarySortingProperty = primarySorting.Key;

                var sortedQuery = primarySortingOrder == SortOrder.Descending
                    ? query.OrderByDescending(primarySortingProperty)
                    : query.OrderBy(primarySortingProperty);

                for (var i = 1; i < options.SortingPropertiesMapping.Count; i++)
                {
                    var sorting = options.SortingPropertiesMapping.ElementAt(i);
                    var sortingOrder = sorting.Value;
                    var sortingProperty = sorting.Key;

                    sortedQuery = sortingOrder == SortOrder.Descending
                        ? sortedQuery.ThenByDescending(sortingProperty)
                        : sortedQuery.ThenBy(sortingProperty);
                }

                query = sortedQuery;
            }
            else
            {
                // Sorts on the Id key by default if no sorting is provided
                var primaryKeyPropertyInfo = ConventionHelper.GetPrimaryKeyPropertyInfo<T>();
                var primaryKeyPropertyName = primaryKeyPropertyInfo.Name;

                query = query.OrderBy(primaryKeyPropertyName);
            }

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
