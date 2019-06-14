namespace DotNetToolkit.Repository.Queries
{
    /// <summary>
    /// Represents an internal cache query result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface ICacheQueryResult<out TResult>
    {
        /// <summary>
        /// Gets the hashed key associated to this cached result.
        /// </summary>
        string HashedKey { get; }

        /// <summary>
        /// Gets the result.
        /// </summary>
        TResult Result { get; }

        /// <summary>
        /// Gets a value indicating whether the executed query result was retrieved from the cache.
        /// </summary>
        bool CacheUsed { get; }
    }
}
