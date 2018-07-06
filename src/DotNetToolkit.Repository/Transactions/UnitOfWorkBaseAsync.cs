namespace DotNetToolkit.Repository.Transactions
{
    using Factories;

    /// <summary>
    /// An implementation of <see cref="IUnitOfWorkAsync" />.
    /// </summary>
    public abstract class UnitOfWorkBaseAsync : UnitOfWorkBase, IUnitOfWorkAsync
    {
        #region Fields

        private readonly IRepositoryFactoryAsync _factory;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkBaseAsync"/> class.
        /// </summary>
        /// <param name="transactionManager">The transaction.</param>
        /// <param name="factory">The asynchronous underlying repository factory.</param>
        protected UnitOfWorkBaseAsync(ITransactionManager transactionManager, IRepositoryFactoryAsync factory) : base(transactionManager, factory)
        {
            _factory = factory;
        }

        #endregion

        #region Implementation of IRepositoryFactoryAsync

        /// <summary>
        /// Creates a new asynchronous repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>The new asynchronous repository.</returns>
        public IRepositoryAsync<TEntity> CreateAsync<TEntity>() where TEntity : class
        {
            return _factory.CreateAsync<TEntity>();
        }

        /// <summary>
        /// Creates a new asynchronous repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <returns>The new asynchronous repository.</returns>
        public IRepositoryAsync<TEntity, TKey> CreateAsync<TEntity, TKey>() where TEntity : class
        {
            return _factory.CreateAsync<TEntity, TKey>();
        }

        /// <summary>
        /// Creates a new asynchronous repository for the specified entity and a composite primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
        /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
        /// <returns>The new asynchronous repository.</returns>
        public IRepositoryAsync<TEntity, TKey1, TKey2> CreateAsync<TEntity, TKey1, TKey2>() where TEntity : class
        {
            return _factory.CreateAsync<TEntity, TKey1, TKey2>();
        }

        /// <summary>
        /// Creates a new asynchronous repository for the specified entity and a composite primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
        /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
        /// <typeparam name="TKey3">The type of the third part of the composite primary key.</typeparam>
        /// <returns>The new asynchronous repository.</returns>
        public IRepositoryAsync<TEntity, TKey1, TKey2, TKey3> CreateAsync<TEntity, TKey1, TKey2, TKey3>() where TEntity : class
        {
            return _factory.CreateAsync<TEntity, TKey1, TKey2, TKey3>();
        }

        #endregion
    }
}
