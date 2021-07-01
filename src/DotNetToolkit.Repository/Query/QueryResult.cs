namespace DotNetToolkit.Repository.Query
{
    using JetBrains.Annotations;

    /// <summary>
    /// Represents an internal paged query result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class PagedQueryResult<TResult>
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
    }
}
