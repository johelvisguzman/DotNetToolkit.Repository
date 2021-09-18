namespace DotNetToolkit.Repository.Extensions
{
    using JetBrains.Annotations;
    using Query;
    using System;
    using System.Linq.Expressions;
    using Utility;

    /// <summary>
    /// Contains various utility methods for applying options to the specified <see cref="IQueryOptions{T}" />.
    /// </summary>
    public static class QueryOptionsExtensions
    {
        /// <summary>
        /// Converts the specified predicate to <see cref="IQueryOptions{T}" />.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The new query options instance.</returns>
        public static IQueryOptions<T> ToQueryOptions<T>([NotNull] this Expression<Func<T, bool>> predicate)
        {
            return new QueryOptions<T>().SatisfyBy(Guard.NotNull(predicate, nameof(predicate)));
        }
    }
}
