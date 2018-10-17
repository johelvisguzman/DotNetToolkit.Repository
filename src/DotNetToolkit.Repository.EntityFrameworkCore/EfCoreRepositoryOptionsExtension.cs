namespace DotNetToolkit.Repository.EntityFrameworkCore
{
    using Configuration.Options;
    using Factories;
    using Microsoft.EntityFrameworkCore;
    using System;

    /// <summary>
    /// An implementation of <see cref="IRepositoryOptionsContextFactoryExtensions" />.
    /// </summary>
    public class EfCoreRepositoryOptionsExtension<TDbContext> : IRepositoryOptionsContextFactoryExtensions where TDbContext : DbContext
    {
        private DbContextOptions _contextOptions;
        private EfCoreRepositoryContextFactory<TDbContext> _contextFactory;

        /// <summary>
        /// Returns the extension instance with a context options builder to configure the context.
        /// </summary>
        /// <param name="options">The context options.</param>
        /// <returns>The same extension instance.</returns>
        public EfCoreRepositoryOptionsExtension<TDbContext> WithDbContextOptions(DbContextOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _contextOptions = options;

            return this;
        }

        /// <summary>
        /// Gets the configured repository context factory.
        /// </summary>
        public IRepositoryContextFactory ContextFactory
        {
            get
            {
                if (_contextFactory == null)
                {
                    _contextFactory = _contextOptions != null
                        ? new EfCoreRepositoryContextFactory<TDbContext>(_contextOptions)
                        : new EfCoreRepositoryContextFactory<TDbContext>();
                }

                return _contextFactory;
            }
        }

        /// <summary>
        /// Gets the type of the repository context factory.
        /// </summary>
        public Type ContextFactoryType { get { return typeof(EfCoreRepositoryContextFactory<TDbContext>); } }
    }
}
