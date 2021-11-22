namespace DotNetToolkit.Repository.EntityFrameworkCore
{
    using Internal;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;
    using System;
    using Utility;

    /// <summary>
    /// Contains various extension methods for <see cref="RepositoryOptionsBuilder" />
    /// </summary>
    public static class RepositoryOptionsBuilderExtensions
    {
        /// <summary>
        /// Configures the context to use entity framework core using an IOC container to resolve the <typeparamref name="TDbContext"/>.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseEntityFrameworkCore<TDbContext>([NotNull] this RepositoryOptionsBuilder source) where TDbContext : DbContext
        {
            Guard.NotNull(source, nameof(source));

            source.UseInternalContextFactory(new EfCoreRepositoryContextFactory<TDbContext>());

            return source;
        }

        /// <summary>
        /// Configures the context to use entity framework core with a context options builder for configuring the context.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="optionsAction">The context options builder action.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseEntityFrameworkCore<TDbContext>([NotNull] this RepositoryOptionsBuilder source, [NotNull] Action<DbContextOptionsBuilder> optionsAction) where TDbContext : DbContext
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotNull(optionsAction, nameof(optionsAction));

            var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();

            optionsAction(optionsBuilder);

            return UseEntityFrameworkCore<TDbContext>(source, optionsBuilder.Options);
        }

        /// <summary>
        /// Configures the context to use entity framework core with a context options builder for configuring the context.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="options">The context options.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseEntityFrameworkCore<TDbContext>([NotNull] this RepositoryOptionsBuilder source, [NotNull] DbContextOptions options) where TDbContext : DbContext
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotNull(options, nameof(options));

            source.UseInternalContextFactory(new EfCoreRepositoryContextFactory<TDbContext>(options));

            return source;
        }
    }
}
