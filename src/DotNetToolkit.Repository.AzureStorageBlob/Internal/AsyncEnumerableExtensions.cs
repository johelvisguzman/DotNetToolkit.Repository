namespace DotNetToolkit.Repository.AzureStorageBlob.Internal
{
    using Configuration.Conventions;
    using Extensions;
    using Query;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Utility;

    internal static class AsyncEnumerableExtensions
    {
        public static IAsyncEnumerable<T> ApplySpecificationOptions<T>(this IAsyncEnumerable<T> source, IQueryOptions<T> options) where T : class
        {
            Guard.NotNull(source, nameof(source));

            var predicate = options?.SpecificationStrategy?.Predicate;
            if (predicate != null)
            {
                source = source.Where(predicate.Compile());
            }

            return source;
        }

        public static IOrderedAsyncEnumerable<T> ApplySortingOptions<T>(this IAsyncEnumerable<T> source, IRepositoryConventions conventions, IQueryOptions<T> options) where T : class
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

        public static IAsyncEnumerable<T> ApplyPagingOptions<T>(this IAsyncEnumerable<T> source, IQueryOptions<T> options)
        {
            Guard.NotNull(source, nameof(source));

            if (options != null && options.PageSize != -1)
            {
                source = source.Skip((options.PageIndex - 1) * options.PageSize).Take(options.PageSize);
            }

            return source;
        }

        private static IOrderedAsyncEnumerable<T> ApplyOrder<T>(this IAsyncEnumerable<T> source, string propertyName, string methodName)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotEmpty(propertyName, nameof(propertyName));
            Guard.NotEmpty(methodName, nameof(methodName));

            var lambda = ExpressionHelper.GetExpression<T>(propertyName);
            var type = ExpressionHelper.GetMemberExpression(lambda).Type;
            var func = lambda.Compile();

            var result = typeof(AsyncEnumerable)
                .GetRuntimeMethods()
                .Single(method => method.Name == methodName &&
                                  method.IsGenericMethodDefinition &&
                                  method.GetGenericArguments().Length == 2 &&
                                  method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), type)
                .Invoke(null, new object[] { source, func });

            return (IOrderedAsyncEnumerable<T>)result;
        }

        private static IOrderedAsyncEnumerable<T> OrderBy<T>(this IAsyncEnumerable<T> source, string propertyName)
        {
            return ApplyOrder<T>(source, propertyName, nameof(AsyncEnumerable.OrderBy));
        }

        private static IOrderedAsyncEnumerable<T> OrderByDescending<T>(this IAsyncEnumerable<T> source, string propertyName)
        {
            return ApplyOrder<T>(source, propertyName, nameof(AsyncEnumerable.OrderByDescending));
        }

        private static IOrderedAsyncEnumerable<T> ThenBy<T>(this IOrderedAsyncEnumerable<T> source, string propertyName)
        {
            return ApplyOrder<T>(source, propertyName, nameof(AsyncEnumerable.ThenBy));
        }

        private static IOrderedAsyncEnumerable<T> ThenByDescending<T>(this IOrderedAsyncEnumerable<T> source, string propertyName)
        {
            return ApplyOrder<T>(source, propertyName, nameof(AsyncEnumerable.ThenByDescending));
        }
    }
}
