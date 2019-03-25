namespace DotNetToolkit.Repository.Queries
{
    /// <summary>
    /// Represents a paged query result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.Queries.IQueryResult{TResult}" />
    public interface IPagedQueryResult<out TResult> : IQueryResult<TResult>
    {
        /// <summary>
        /// Gets the total number of records.
        /// </summary>
        int Total { get; }
    }
}
