namespace DotNetToolkit.Repository.EntityFrameworkCore
{
    using Configuration.Options;
    using Microsoft.EntityFrameworkCore;
    using System;

    /// <summary>
    /// Contains various extension methods for <see cref="RepositoryOptionsBuilder" />
    /// </summary>
    public static class RepositoryOptionsBuilderExtensions
    {
        /// <summary>
        /// Configures the context to use entity framework core.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseEntityFrameworkCore<TDbContext>(this RepositoryOptionsBuilder source) where TDbContext : DbContext
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var extension = source.Options.FindExtension<EfCoreRepositoryOptionsExtension<TDbContext>>() ?? new EfCoreRepositoryOptionsExtension<TDbContext>();

            source.Options.AddOrUpdateExtension<EfCoreRepositoryOptionsExtension<TDbContext>>(extension);

            return source;
        }

        /// <summary>
        /// Configures the context to use entity framework core with a context options builder for configuring the context.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="optionsAction">The context options builder action.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseEntityFrameworkCore<TDbContext>(this RepositoryOptionsBuilder source, Action<DbContextOptionsBuilder> optionsAction) where TDbContext : DbContext
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (optionsAction == null)
                throw new ArgumentNullException(nameof(optionsAction));

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
        public static RepositoryOptionsBuilder UseEntityFrameworkCore<TDbContext>(this RepositoryOptionsBuilder source, DbContextOptions options) where TDbContext : DbContext
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var extension = (source.Options.FindExtension<EfCoreRepositoryOptionsExtension<TDbContext>>() ?? new EfCoreRepositoryOptionsExtension<TDbContext>()).WithDbContextOptions(options);

            source.Options.AddOrUpdateExtension<EfCoreRepositoryOptionsExtension<TDbContext>>(extension);

            return source;
        }
    }
}
