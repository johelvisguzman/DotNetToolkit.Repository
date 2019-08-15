namespace DotNetToolkit.Repository.AzureStorageTable
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
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        /// <param name="createIfNotExists">Creates the container if it does not exist.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseAzureStorageTable([NotNull] this RepositoryOptionsBuilder source, [NotNull] string nameOrConnectionString, bool createIfNotExists = false)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotEmpty(nameOrConnectionString, nameof(nameOrConnectionString));

            source.UseInternalContextFactory(new AzureStorageTableRepositoryContextFactory(nameOrConnectionString, createIfNotExists));

            return source;
        }

        /// <summary>
        /// Configures the context to use the azure storage blob service with a connection string.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="createIfNotExists">Creates the container if it does not exist.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseAzureStorageTable([NotNull] this RepositoryOptionsBuilder source, [NotNull] string nameOrConnectionString, string tableName, bool createIfNotExists = false)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotEmpty(nameOrConnectionString, nameof(nameOrConnectionString));
            Guard.NotEmpty(tableName, nameof(tableName));

            source.UseInternalContextFactory(new AzureStorageTableRepositoryContextFactory(nameOrConnectionString, tableName, createIfNotExists));

            return source;
        }
    }
}
