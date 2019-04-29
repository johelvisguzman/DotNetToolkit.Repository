namespace DotNetToolkit.Repository.Extensions
{
    using JetBrains.Annotations;
    using Queries;
    using Queries.Strategies;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Utility;

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
        public static IFetchQueryStrategy<T> DefaultIfFetchStrategyEmpty<T>([CanBeNull] this IQueryOptions<T> source)
        {
            return source?.FetchStrategy != null && source.FetchStrategy.PropertyPaths.Any()
                ? source.FetchStrategy
                : FetchQueryStrategy<T>.Default();
        }

        /// <summary>
        /// Converts the specified predicate to <see cref="IQueryOptions{T}" />.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The new query options instance.</returns>
        public static IQueryOptions<T> ToQueryOptions<T>([NotNull] this Expression<Func<T, bool>> predicate)
        {
            Guard.NotNull(predicate);

            return new QueryOptions<T>().SatisfyBy(predicate);
        }
    }
}
