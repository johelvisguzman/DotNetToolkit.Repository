namespace DotNetToolkit.Repository.Query.Internal
{
    using JetBrains.Annotations;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="ICacheQueryResult{TResult}" />.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    internal class CacheQueryResult<TResult> : ICacheQueryResult<TResult>
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
        public bool CacheUsed { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheQueryResult{TResult}"/> class.
        /// </summary>
        /// <param name="hashedKey">The hashed key.</param>
        /// <param name="result">The result.</param>
        public CacheQueryResult([NotNull] string hashedKey, [CanBeNull] TResult result)
        {
            HashedKey = Guard.NotEmpty(hashedKey, nameof(hashedKey));
            Result = result;
        }
    }
}
