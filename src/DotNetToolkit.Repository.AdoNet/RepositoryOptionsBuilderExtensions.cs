namespace DotNetToolkit.Repository.AdoNet
{
    using Configuration.Options;
    using System;
    using System.Data.Common;

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
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseAdoNet(this RepositoryOptionsBuilder source, string nameOrConnectionString)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (nameOrConnectionString == null)
                throw new ArgumentNullException(nameof(nameOrConnectionString));

            source.Options.With(new AdoNetRepositoryContextFactory(nameOrConnectionString));

            return source;
        }

        /// <summary>
        /// Configures the context to use ado.net with a provider and a connection string.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseAdoNet(this RepositoryOptionsBuilder source, string providerName, string connectionString)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (providerName == null)
                throw new ArgumentNullException(nameof(providerName));

            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            source.Options.With(new AdoNetRepositoryContextFactory(providerName, connectionString));

            return source;
        }

        /// <summary>
        /// Configures the context to use ado.net with an existing connection.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="existingConnection">The existing connection.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseAdoNet(this RepositoryOptionsBuilder source, DbConnection existingConnection)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (existingConnection == null)
                throw new ArgumentNullException(nameof(existingConnection));

            source.Options.With(new AdoNetRepositoryContextFactory(existingConnection));

            return source;
        }
    }
}
