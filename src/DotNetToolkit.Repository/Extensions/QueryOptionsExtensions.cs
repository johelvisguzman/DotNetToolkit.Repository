namespace DotNetToolkit.Repository.Extensions
{
    using Queries;
    using Queries.Strategies;
    using System.Linq;

    /// <summary>
    /// Contains various utility methods for applying options to the specified <see cref="IQueryOptions{T}" />.
    /// </summary>
    public static class QueryOptionsExtensions
    {
        /// <summary>
        /// Returns the fetch strategy, or a default valued if the sequence is empty.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>The fetching strategy.</returns>
        public static IFetchQueryStrategy<T> DefaultIfFetchStrategyEmpty<T>(this IQueryOptions<T> source)
        {
            return source?.FetchStrategy != null && source.FetchStrategy.PropertyPaths.Any()
                ? source.FetchStrategy
                : FetchQueryStrategy<T>.Default();
        }
    }
}
