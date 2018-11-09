namespace DotNetToolkit.Repository.Queries
{
    /// <summary>
    /// An implementation of <see cref="IQueryResult{TResult}" />.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class QueryResult<TResult> : IQueryResult<TResult>
    {
        private readonly TResult _result;

        /// <inheritdoc />
        public TResult Result
        {
            get { return HasResult ? _result : default(TResult); }
        }

        /// <inheritdoc />
        public int? Total { get; }

        /// <summary>
        /// Gets a value indicating whether the executed query has a result.
        /// </summary>
        public bool HasResult
        {
            get { return _result != null && !_result.Equals(default(TResult)); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResult{TResult}"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public QueryResult(TResult result)
        {
            _result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResult{TResult}"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="total">The total number of records.</param>
        public QueryResult(TResult result, int total)
        {
            _result = result;
            Total = total;
        }
    }
}
