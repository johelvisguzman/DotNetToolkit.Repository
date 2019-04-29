namespace DotNetToolkit.Repository.EntityFrameworkCore.Internal
{
    using Extensions;
    using Microsoft.EntityFrameworkCore;
    using Queries;
    using System.Linq;
    using Utility;

    internal static class QueryableExtensions
    {
        public static IQueryable<T> ApplyFetchingOptions<T>(this IQueryable<T> query, IQueryOptions<T> options) where T : class
        {
            Guard.NotNull(query);

            var fetchingPaths = options.DefaultIfFetchStrategyEmpty().PropertyPaths.ToList();

            if (fetchingPaths.Any())
                query = fetchingPaths.Aggregate(query, (current, path) => current.Include(path));

            return query;
        }
    }
}
