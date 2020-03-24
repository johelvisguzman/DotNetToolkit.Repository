namespace DotNetToolkit.Repository.EntityFrameworkCore.Internal
{
    using Configuration;
    using Microsoft.EntityFrameworkCore;
    using System;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="IRepositoryContextFactory" />.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the database context.</typeparam>
    /// <seealso cref="IRepositoryContextFactory" />
    internal class EfCoreRepositoryContextFactory<TDbContext> : IRepositoryContextFactory where TDbContext : DbContext
    {
        #region Fields

        private readonly DbContextOptions _contextOptions;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreRepositoryContextFactory{TDbContext}"/> using an IOC container to resolve the <typeparamref name="TDbContext"/>.
        /// </summary>
        public EfCoreRepositoryContextFactory() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreRepositoryContextFactory{TDbContext}"/> class.
        /// </summary>
        /// <param name="contextOptions">The context options.</param>
        public EfCoreRepositoryContextFactory(DbContextOptions contextOptions)
        {
            _contextOptions = Guard.NotNull(contextOptions, nameof(contextOptions));
        }

        #endregion

        #region Implementation of IRepositoryContextFactory

        /// <summary>
        /// Create a new repository context.
        /// </summary>
        /// <returns>The new repository context.</returns>
        public IRepositoryContext Create()
        {
            var underlyingContext = _contextOptions != null
                ? (TDbContext)Activator.CreateInstance(typeof(TDbContext), _contextOptions)
                : RepositoryDependencyResolver.Current.Resolve<TDbContext>();

            return new EfCoreRepositoryContext(underlyingContext);

        }

        #endregion
    }
}
