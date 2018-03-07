namespace DotNetToolkit.Repository.Queries
{
    using System.Linq;

    /// <summary>
    /// Represents a query options which can be used for sorting or paging on queries.
    /// </summary>
    /// <typeparam name="T">The entity type of the repository.</typeparam>
    public interface IQueryOptions<T>
    {
        /// <summary>
        /// Applies the current options to the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The new query with the defined options applied.</returns>
        IQueryable<T> Apply(IQueryable<T> query);
    }
}
