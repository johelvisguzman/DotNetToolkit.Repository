namespace DotNetToolkit.Repository.AzureStorageBlob
{
    using Configuration.Options;
    using Internal;
    using JetBrains.Annotations;
    using Utility;

    /// <summary>
    /// Contains various extension methods for <see cref="RepositoryOptionsBuilder" />
    /// </summary>
    public static class RepositoryOptionsBuilderExtensions
    {
        /// <summary>
        /// Configures the context to use the azure storage blob service with a connection string.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="createIfNotExists">Creates the container if it does not exist.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseAzureStorageBlob([NotNull] this RepositoryOptionsBuilder source, [NotNull] string connectionString, bool createIfNotExists = false)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotEmpty(connectionString, nameof(connectionString));

            source.UseInternalContextFactory(new AzureStorageBlobRepositoryContextFactory(connectionString, createIfNotExists));

            return source;
        }

        /// <summary>
        /// Configures the context to use the azure storage blob service with a connection string.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="container">The name of the container.</param>
        /// <param name="createIfNotExists">Creates the container if it does not exist.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseAzureStorageBlob([NotNull] this RepositoryOptionsBuilder source, [NotNull] string connectionString, string container, bool createIfNotExists = false)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotEmpty(connectionString, nameof(connectionString));
            Guard.NotEmpty(container, nameof(container));

            source.UseInternalContextFactory(new AzureStorageBlobRepositoryContextFactory(connectionString, container, createIfNotExists));

            return source;
        }
    }
}
