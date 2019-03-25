namespace DotNetToolkit.Repository.EntityFrameworkCore.Internal
{
    using Extensions;
    using Microsoft.EntityFrameworkCore;
    using Queries;
    using System;
    using System.Linq;

    internal static class QueryableExtensions
    {
        public static IQueryable<T> ApplyFetchingOptions<T>(this IQueryable<T> query, IQueryOptions<T> options) where T : class
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var fetchingPaths = options.DefaultIfFetchStrategyEmpty().PropertyPaths.ToList();

            if (fetchingPaths.Any())
                query = fetchingPaths.Aggregate(query, (current, path) => current.Include(path));

            return query;
        }
    }
}
