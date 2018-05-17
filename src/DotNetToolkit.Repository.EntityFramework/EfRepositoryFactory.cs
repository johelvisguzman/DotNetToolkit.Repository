namespace DotNetToolkit.Repository.EntityFramework
{
    using System;
    using System.Data.Entity;

    /// <summary>
    /// An implementation of <see cref="IRepositoryFactory" />.
    /// </summary>
    public class EfRepositoryFactory : IRepositoryFactory
    {
        #region Fields

        private readonly IRepositoryFactoryOptions _options;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfRepositoryFactory"/> class.
        /// </summary>
        public EfRepositoryFactory()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfRepositoryFactory"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public EfRepositoryFactory(IRepositoryFactoryOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _options = options;
        }

        #endregion

        #region Private Methods

        private static DbContext GetDbContext(IRepositoryFactoryOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (options.DbContextType == null)
                throw new InvalidOperationException($"The repository options must provide a '{nameof(options.DbContextType)}'.");

            DbContext context;

            if (options.DbContextArgs == null)
                context = (DbContext)Activator.CreateInstance(options.DbContextType);
            else
                context = (DbContext)Activator.CreateInstance(options.DbContextType, options.DbContextArgs);

            return context;
        }

        #endregion

        #region Implementation of IRepositoryFactory

        /// <summary>
        /// Creates a new repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity> Create<TEntity>() where TEntity : class
        {
            if (_options == null)
                throw new InvalidOperationException("No options have been provided.");

            return Create<TEntity>(_options);
        }

        /// <summary>
        /// Creates a new repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity, TKey> Create<TEntity, TKey>() where TEntity : class
        {
            if (_options == null)
                throw new InvalidOperationException("No options have been provided.");

            return Create<TEntity, TKey>(_options);
        }

        /// <summary>
        /// Creates a new repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="options">The options.</param>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity> Create<TEntity>(IRepositoryFactoryOptions options) where TEntity : class
        {
            return new EfRepository<TEntity>(GetDbContext(options), options.Logger);
        }

        /// <summary>
        /// Creates a new repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <param name="options">The options.</param>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity, TKey> Create<TEntity, TKey>(IRepositoryFactoryOptions options) where TEntity : class
        {
            return new EfRepository<TEntity, TKey>(GetDbContext(options), options.Logger);
        }

        #endregion
    }
}
