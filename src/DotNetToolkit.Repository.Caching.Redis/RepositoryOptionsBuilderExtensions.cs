namespace DotNetToolkit.Repository.Caching.Redis
{
    using Internal;
    using JetBrains.Annotations;
    using System;
    using Utility;

    /// <summary>
    /// Contains various extension methods for <see cref="RepositoryOptionsBuilder" />
    /// </summary>
    public static class RepositoryOptionsBuilderExtensions
    {
        /// <summary>
        /// Configures the caching provider to use redis.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <returns>The same builder instance.</returns>
        /// <remarks>This will connect to a single server on the local machine using the default redis port (6379).</remarks>
        public static RepositoryOptionsBuilder UseRedis([NotNull] this RepositoryOptionsBuilder source)
        {
            Guard.NotNull(source, nameof(source));

            source.UseCachingProvider(new RedisCacheProvider());

            return source;
        }

        /// <summary>
        /// Configures the caching provider to use redis.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseRedis([NotNull] this RepositoryOptionsBuilder source, [NotNull] Action<RedisCacheOptions> optionsAction)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotNull(optionsAction, nameof(optionsAction));

            var options = new RedisCacheOptions();

            optionsAction(options);

            var provider = new RedisCacheProvider(
                options.Host,
                options.Username,
                options.Password,
                options.Ssl,
                options.AllowAdmin,
                options.DefaultDatabase,
                options.Expiry,
                options.SerializerSettings);

            source.UseCachingProvider(provider);

            return source;
        }
    }
}
