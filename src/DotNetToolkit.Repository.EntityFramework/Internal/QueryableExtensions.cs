namespace DotNetToolkit.Repository.EntityFramework.Internal
{
    using Configuration.Conventions;
    using Extensions;
    using Query;
    using System.Data.Entity;
    using System.Linq;
    using Utility;

    internal static class QueryableExtensions
    {
        public static IQueryable<T> ApplyFetchingOptions<T>(this IQueryable<T> query, IRepositoryConventions conventions, IQueryOptions<T> options) where T : class
        {
            Guard.NotNull(query, nameof(query));

            var fetchingPaths = options.DefaultIfFetchStrategyEmpty(conventions).PropertyPaths.ToList();

            if (fetchingPaths.Any())
                query = fetchingPaths.Aggregate(query, (current, path) => current.Include(path));

            return query;
        }
    }
}
