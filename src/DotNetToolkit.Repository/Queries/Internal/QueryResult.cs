namespace DotNetToolkit.Repository.Queries.Internal
{
    using JetBrains.Annotations;

    /// <summary>
    /// An implementation of <see cref="IQueryResult{TResult}" />.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    internal class QueryResult<TResult> : IQueryResult<TResult>
    {
        private readonly TResult _result;

        /// <summary>
        /// Gets the result.
        /// </summary>
        public TResult Result
        {
            get { return HasResult ? _result : default(TResult); }
        }

        /// <summary>
        /// Gets a value indicating whether the executed query has a result.
        /// </summary>
        public bool HasResult
        {
            get { return _result != null && !_result.Equals(default(TResult)); }
        }

        /// <summary>
        /// Gets a value indicating whether the executed query result was retrieved from the cache.
        /// </summary>
        public bool CacheUsed { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResult{TResult}"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public QueryResult([CanBeNull] TResult result)
        {
            _result = result;
        }
    }
}
