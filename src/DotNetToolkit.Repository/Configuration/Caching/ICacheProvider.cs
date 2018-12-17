namespace DotNetToolkit.Repository.Configuration.Caching
{
    using System;

    /// <summary>
    /// Represents a caching provider for caching query data within the repositories.
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// Gets or sets the caching expiration time.
        /// </summary>
        TimeSpan? CacheExpiration { get; set; }

        /// <summary>
        /// Gets the cache.
        /// </summary>
        ICache Cache { get; }
    }
}
