namespace DotNetToolkit.Repository.Caching.Couchbase
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
        /// Configures the caching provider to use couchbase.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseCouchbase([NotNull] this RepositoryOptionsBuilder source, [NotNull] Action<CouchbaseCacheOptions> optionsAction)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotNull(optionsAction, nameof(optionsAction));

            var options = new CouchbaseCacheOptions();

            optionsAction(options);

            var provider = new CouchbaseCacheProvider(
                options.Host,
                options.BucketName,
                options.Username,
                options.Password,
                options.Expiry,
                options.Serializer);

            source.UseCachingProvider(provider);

            return source;
        }
    }
}
