namespace DotNetToolkit.Repository.EntityFrameworkCore
{
    using Configuration;
    using Factories;
    using Internal;
    using Microsoft.EntityFrameworkCore;
    using System;

    /// <summary>
    /// An implementation of <see cref="IRepositoryContextFactory" />.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the database context.</typeparam>
    /// <seealso cref="IRepositoryContextFactory" />
    public class EfCoreRepositoryContextFactory<TDbContext> : IRepositoryContextFactory where TDbContext : DbContext
    {
        #region Fields

        private readonly DbContextOptions _contextOptions;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreRepositoryContextFactory{TDbContext}"/> class.
        /// </summary>
        public EfCoreRepositoryContextFactory() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreRepositoryContextFactory{TDbContext}"/> class.
        /// </summary>
        /// <param name="optionsAction">The context options builder action.</param>
        public EfCoreRepositoryContextFactory(Action<DbContextOptionsBuilder> optionsAction)
        {
            if (optionsAction == null)
                throw new ArgumentNullException(nameof(optionsAction));

            var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();

            optionsAction(optionsBuilder);

            _contextOptions = optionsBuilder.Options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreRepositoryContextFactory{TDbContext}"/> class.
        /// </summary>
        /// <param name="contextOptions">The context options.</param>
        public EfCoreRepositoryContextFactory(DbContextOptions contextOptions)
        {
            if (contextOptions == null)
                throw new ArgumentNullException(nameof(contextOptions));

            _contextOptions = contextOptions;
        }

        #endregion

        #region Implementation of IRepositoryContextFactory

        /// <inheritdoc />
        public IRepositoryContext Create()
        {
            TDbContext underlyingContext;

            try
            {
                underlyingContext = _contextOptions != null
                    ? (TDbContext)Activator.CreateInstance(typeof(TDbContext), _contextOptions)
                    : (TDbContext)Activator.CreateInstance(typeof(TDbContext));
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }

            return new EfCoreRepositoryContext(underlyingContext);

        }

        #endregion
    }
}
