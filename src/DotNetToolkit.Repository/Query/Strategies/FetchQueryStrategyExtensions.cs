namespace DotNetToolkit.Repository.Query.Strategies
{
    using Configuration.Conventions;
    using JetBrains.Annotations;
    using System.Linq;
    using Utility;

    /// <summary>
    /// Contains various utility methods for applying options to the specified <see cref="IFetchQueryStrategy{T}" />.
    /// </summary>
    public static class FetchQueryStrategyExtensions
    {
        /// <summary>
        /// Returns the fetch strategy, or a default valued if the sequence is empty.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="conventions">The configurable conventions.</param>
        /// <returns>The fetching strategy.</returns>
        public static IFetchQueryStrategy<T> DefaultIfFetchStrategyEmpty<T>([CanBeNull] this IFetchQueryStrategy<T> source, [NotNull] IRepositoryConventions conventions)
        {
            Guard.NotNull(conventions, nameof(conventions));

            return source != null && source.PropertyPaths.Any()
                ? source
                : FetchQueryStrategy<T>.Default(conventions);
        }
    }
}
