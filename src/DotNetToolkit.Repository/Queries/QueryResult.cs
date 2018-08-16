namespace DotNetToolkit.Repository.Queries
{
    /// <summary>
    /// Represents a query result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class QueryResult<TResult>
    {
        /// <summary>
        /// Gets the result.
        /// </summary>
        public TResult Result { get; }

        /// <summary>
        /// Gets a value indicating whether the executed query has a result.
        /// </summary>
        public bool HasResult
        {
            get { return Result != null && !Result.Equals(default(TResult)); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResult{TResult}"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public QueryResult(TResult result)
        {
            Result = result;
        }
    }
}
