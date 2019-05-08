namespace DotNetToolkit.Repository.EntityFramework
{
    using Configuration.Options;
    using Internal;
    using JetBrains.Annotations;
    using System.Data.Common;
    using System.Data.Entity;
    using Utility;

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
        public static RepositoryOptionsBuilder UseEntityFramework<TDbContext>([NotNull] this RepositoryOptionsBuilder source) where TDbContext : DbContext
        {
            Guard.NotNull(source, nameof(source));

            source.UseInternalContextFactory(new EfRepositoryContextFactory<TDbContext>());

            return source;
        }

        /// <summary>
        /// Configures the context to use entity framework with a connection string.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseEntityFramework<TDbContext>([NotNull] this RepositoryOptionsBuilder source, [NotNull] string nameOrConnectionString) where TDbContext : DbContext
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotEmpty(nameOrConnectionString, nameof(nameOrConnectionString));

            source.UseInternalContextFactory(new EfRepositoryContextFactory<TDbContext>(nameOrConnectionString));

            return source;
        }

        /// <summary>
        /// Configures the context to use entity framework with an existing connection.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="existingConnection">The existing connection.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseEntityFramework<TDbContext>([NotNull] this RepositoryOptionsBuilder source, [NotNull] DbConnection existingConnection) where TDbContext : DbContext
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotNull(existingConnection, nameof(existingConnection));

            source.UseInternalContextFactory(new EfRepositoryContextFactory<TDbContext>(existingConnection));

            return source;
        }

        /// <summary>
        /// Configures the context to use entity framework with the <see cref="RepositoryDependencyResolver"/> using an IOC container to resolve the <see cref="DbContext"/>.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseEntityFramework([NotNull] this RepositoryOptionsBuilder source)
        {
            Guard.NotNull(source, nameof(source));

            source.UseInternalContextFactory(new EfRepositoryContextFactory());

            return source;
        }
    }
}
