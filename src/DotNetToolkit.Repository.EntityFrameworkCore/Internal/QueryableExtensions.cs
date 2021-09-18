namespace DotNetToolkit.Repository.EntityFrameworkCore.Internal
{
    using Configuration.Conventions;
    using Microsoft.EntityFrameworkCore;
    using Query.Strategies;
    using System.Linq;
    using Utility;

    internal static class QueryableExtensions
    {
        public static IQueryable<T> ApplyFetchingOptions<T>(this IQueryable<T> query, IRepositoryConventions conventions, IFetchQueryStrategy<T> fetchStrategy) where T : class
        {
            Guard.NotNull(query, nameof(query));

            var fetchingPaths = fetchStrategy.DefaultIfFetchStrategyEmpty(conventions).PropertyPaths.ToList();

            if (fetchingPaths.Any())
                query = fetchingPaths.Aggregate(query, (current, path) => current.Include(path));

            return query;
        }
    }
}
