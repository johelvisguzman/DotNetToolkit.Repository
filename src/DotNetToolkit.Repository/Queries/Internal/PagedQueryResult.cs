namespace DotNetToolkit.Repository.Queries.Internal
{
    using JetBrains.Annotations;

    /// <summary>
    /// An implementation of <see cref="IPagedQueryResult{TResult}" />.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    internal class PagedQueryResult<TResult> : IPagedQueryResult<TResult>
    {
        /// <summary>
        /// Gets the result.
        /// </summary>
        public TResult Result { get; }

        /// <summary>
        /// Gets the total number of records.
        /// </summary>
        public int Total { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedQueryResult{TResult}"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="total">The total number of records.</param>
        public PagedQueryResult([CanBeNull] TResult result, int total)
        {
            Result = result;
            Total = total;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedQueryResult{TResult}"/> class.
        /// </summary>
        /// <param name="result">The paged result.</param>
        internal PagedQueryResult(IPagedQueryResult<TResult> result) : this(result.Result, result.Total) { }
    }
}
