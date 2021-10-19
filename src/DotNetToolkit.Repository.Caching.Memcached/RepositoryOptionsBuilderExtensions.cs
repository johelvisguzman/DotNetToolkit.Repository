namespace DotNetToolkit.Repository.Caching.Memcached
{
    using Configuration.Options;
    using Utility;
    using JetBrains.Annotations;
    using System;

    /// <summary>
    /// Contains various extension methods for <see cref="RepositoryOptionsBuilder" />
    /// </summary>
    public static class RepositoryOptionsBuilderExtensions
    {
        /// <summary>
        /// Configures the caching provider to use memcached.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseMemcached([NotNull] this RepositoryOptionsBuilder source, [NotNull] Action<MemcachedCacheOptions> optionsAction)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotNull(optionsAction, nameof(optionsAction));

            var options = new MemcachedCacheOptions();

            optionsAction(options);

            var provider = new MemcachedCacheProvider(
                options.Host,
                options.Username,
                options.Password,
                options.Protocal,
                options.AuthenticationType,
                options.Expiry);

            source.UseCachingProvider(provider);

            return source;
        }
    }
}
