﻿namespace DotNetToolkit.Repository.AzureStorageBlob
{
    using Internal;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
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
        /// <param name="serializerSettings">The serializer options to use when serializing to JSON.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseAzureStorageBlob([NotNull] this RepositoryOptionsBuilder source, [NotNull] string connectionString, bool createIfNotExists = false, JsonSerializerSettings serializerSettings = null)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotEmpty(connectionString, nameof(connectionString));

            source.UseInternalContextFactory(
                new AzureStorageBlobRepositoryContextFactory(connectionString, createIfNotExists, serializerSettings));

            return source;
        }

        /// <summary>
        /// Configures the context to use the azure storage blob service with a connection string.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="containerNameBuilder">The name of the container builder.</param>
        /// <param name="createIfNotExists">Creates the container if it does not exist.</param>
        /// <param name="serializerSettings">The serializer options to use when serializing to JSON.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseAzureStorageBlob([NotNull] this RepositoryOptionsBuilder source, [NotNull] string connectionString, IAzureStorageBlobContainerNameBuilder containerNameBuilder, bool createIfNotExists = false, JsonSerializerSettings serializerSettings = null)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotEmpty(connectionString, nameof(connectionString));
            Guard.NotNull(containerNameBuilder, nameof(containerNameBuilder));

            source.UseInternalContextFactory(
                new AzureStorageBlobRepositoryContextFactory(connectionString, containerNameBuilder, createIfNotExists, serializerSettings));

            return source;
        }
    }
}
