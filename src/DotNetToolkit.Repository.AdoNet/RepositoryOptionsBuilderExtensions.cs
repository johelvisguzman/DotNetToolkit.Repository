namespace DotNetToolkit.Repository.AdoNet
{
    using Configuration.Options;
    using Internal;
    using JetBrains.Annotations;
    using System.Data.Common;
    using Utility;

    /// <summary>
    /// Contains various extension methods for <see cref="RepositoryOptionsBuilder" />
    /// </summary>
    public static class RepositoryOptionsBuilderExtensions
    {
        /// <summary>
        /// Configures the context to use ado.net with a connection string.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        /// <param name="ensureDatabaseCreated">
        /// Ensures that the database for the context exists. If it exists, no action is taken.
        /// If it does not exist then the database and all its schema are created.
        /// If the database exists, then no effort is made to ensure it is compatible with the model for this context.
        /// </param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseAdoNet([NotNull] this RepositoryOptionsBuilder source, [NotNull] string nameOrConnectionString, bool ensureDatabaseCreated = false)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotEmpty(nameOrConnectionString, nameof(nameOrConnectionString));

            source.UseInternalContextFactory(new AdoNetRepositoryContextFactory(nameOrConnectionString, ensureDatabaseCreated));

            return source;
        }

        /// <summary>
        /// Configures the context to use ado.net with a provider and a connection string.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="ensureDatabaseCreated">
        /// Ensures that the database for the context exists. If it exists, no action is taken.
        /// If it does not exist then the database and all its schema are created.
        /// If the database exists, then no effort is made to ensure it is compatible with the model for this context.
        /// </param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseAdoNet([NotNull] this RepositoryOptionsBuilder source, [NotNull] string providerName, [NotNull] string connectionString, bool ensureDatabaseCreated = false)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotEmpty(providerName, nameof(providerName));
            Guard.NotEmpty(connectionString, nameof(connectionString));

            source.UseInternalContextFactory(new AdoNetRepositoryContextFactory(providerName, connectionString, ensureDatabaseCreated));

            return source;
        }

        /// <summary>
        /// Configures the context to use ado.net with an existing connection.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="existingConnection">The existing connection.</param>
        /// <param name="ensureDatabaseCreated">
        /// Ensures that the database for the context exists. If it exists, no action is taken.
        /// If it does not exist then the database and all its schema are created.
        /// If the database exists, then no effort is made to ensure it is compatible with the model for this context.
        /// </param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseAdoNet([NotNull] this RepositoryOptionsBuilder source, [NotNull] DbConnection existingConnection, bool ensureDatabaseCreated = false)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotNull(existingConnection, nameof(existingConnection));

            source.UseInternalContextFactory(new AdoNetRepositoryContextFactory(existingConnection, ensureDatabaseCreated));

            return source;
        }
    }
}
