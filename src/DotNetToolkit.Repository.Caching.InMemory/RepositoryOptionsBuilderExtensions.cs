namespace DotNetToolkit.Repository.Caching.InMemory
{
    using Configuration.Options;
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
        /// Configures the caching provider to use microsoft's in-memory cache.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseInMemoryCache([NotNull] this RepositoryOptionsBuilder source)
        {
            Guard.NotNull(source, nameof(source));

            source.UseCachingProvider(new InMemoryCacheProvider());

            return source;
        }

        /// <summary>
        /// Configures the caching provider to use the in-memory cache.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseInMemoryCache([NotNull] this RepositoryOptionsBuilder source, [NotNull] Action<InMemoryCacheOptions> optionsAction)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotNull(optionsAction, nameof(optionsAction));

            var options = new InMemoryCacheOptions();

            optionsAction(options);

            var provider = new InMemoryCacheProvider(
                options.Clock,
                options.ExpirationScanFrequency,
                options.Expiry);

            source.UseCachingProvider(provider);

            return source;
        }
    }
}
