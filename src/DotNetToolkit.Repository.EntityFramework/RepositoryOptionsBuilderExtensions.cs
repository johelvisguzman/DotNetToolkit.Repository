namespace DotNetToolkit.Repository.EntityFramework
{
    using Configuration.Options;
    using System;
    using System.Data.Common;
    using System.Data.Entity;

    /// <summary>
    /// Contains various extension methods for <see cref="RepositoryOptionsBuilder" />
    /// </summary>
    public static class RepositoryOptionsBuilderExtensions
    {
        /// <summary>
        /// Configures the context to use entity framework.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseEntityFramework<TDbContext>(this RepositoryOptionsBuilder source) where TDbContext : DbContext
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Options.AddInternalContextFactory(new EfRepositoryContextFactory<TDbContext>());

            return source;
        }

        /// <summary>
        /// Configures the context to use entity framework with a connection string.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseEntityFramework<TDbContext>(this RepositoryOptionsBuilder source, string nameOrConnectionString) where TDbContext : DbContext
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Options.AddInternalContextFactory(new EfRepositoryContextFactory<TDbContext>(nameOrConnectionString));

            return source;
        }

        /// <summary>
        /// Configures the context to use entity framework with an existing connection.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="existingConnection">The existing connection.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseEntityFramework<TDbContext>(this RepositoryOptionsBuilder source, DbConnection existingConnection) where TDbContext : DbContext
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Options.AddInternalContextFactory(new EfRepositoryContextFactory<TDbContext>(existingConnection));

            return source;
        }
    }
}
