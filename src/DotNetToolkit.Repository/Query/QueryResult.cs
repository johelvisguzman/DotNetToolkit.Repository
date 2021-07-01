namespace DotNetToolkit.Repository.Query
{
    using JetBrains.Annotations;
    using Utility;

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

    /// <summary>
    /// Represents an internal cache query result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class CacheQueryResult<TResult>
    {
        /// <summary>
        /// Gets the hashed key associated to this cached result.
        /// </summary>
        public string HashedKey { get; }

        /// <summary>
        /// Gets the result.
        /// </summary>
        public TResult Result { get; }

        /// <summary>
        /// Gets a value indicating whether the executed query result was retrieved from the cache.
        /// </summary>
        public bool CacheUsed { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheQueryResult{TResult}"/> class.
        /// </summary>
        /// <param name="hashedKey">The hashed key.</param>
        /// <param name="result">The result.</param>
        /// <param name="cacheUsed">Indicates whether the executed query result was retrieved from the cache.</param>
        internal CacheQueryResult([NotNull] string hashedKey, [CanBeNull] TResult result, bool cacheUsed)
        {
            HashedKey = Guard.NotEmpty(hashedKey, nameof(hashedKey));
            Result = result;
            CacheUsed = cacheUsed;
        }
    }

    /// <summary>
    /// Represents an internal cache paged query result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class CachePagedQueryResult<TResult> : CacheQueryResult<PagedQueryResult<TResult>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CachePagedQueryResult{TResult}"/> class.
        /// </summary>
        /// <param name="hashedKey">The hashed key.</param>
        /// <param name="result">The result.</param>
        /// <param name="cacheUsed">Indicates whether the executed query result was retrieved from the cache.</param>
        internal CachePagedQueryResult([NotNull] string hashedKey, [CanBeNull] PagedQueryResult<TResult> result, bool cacheUsed)
            : base(hashedKey, result, cacheUsed)
        {
        }
    }
}
