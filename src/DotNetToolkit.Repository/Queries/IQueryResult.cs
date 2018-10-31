namespace DotNetToolkit.Repository.Queries
{

    /// <summary>
    /// Represents a query result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface IQueryResult<out TResult>
    {
        /// <summary>
        /// Gets the result.
        /// </summary>
        TResult Result { get; }

        /// <summary>
        /// Gets the total number of records.
        /// </summary>
        int? Total { get; }
    }
}
