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
        TimeSpan? Expiry { get; set; }

        /// <summary>
        /// Gets the cache.
        /// </summary>
        ICache Cache { get; }
    }

    /// <summary>
    /// Represents a caching provider for caching query data within the repositories.
    /// </summary>
    /// <typeparam name="TCache">The type of the cache.</typeparam>
    /// <seealso cref="ICacheProvider" />
    public interface ICacheProvider<out TCache> : ICacheProvider where TCache : ICache
    {
        /// <summary>
        /// Gets the cache.
        /// </summary>
        new TCache Cache { get; }
    }
}
