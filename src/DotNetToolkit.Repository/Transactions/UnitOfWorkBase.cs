namespace DotNetToolkit.Repository.Transactions
{
    using Factories;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An implementation of <see cref="IUnitOfWork" />.
    /// </summary>
    public abstract class UnitOfWorkBase : IUnitOfWork
    {
        #region Fields

        private readonly IRepositoryFactory _factory;

        private bool _disposed;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the transaction manager.
        /// </summary>
        protected ITransactionManager TransactionManager { get; private set; }

        /// <summary>
        /// Gets the repositories.
        /// </summary>
        internal Dictionary<Type, object> Repositories { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkBase"/> class.
        /// </summary>
        /// <param name="transactionManager">The transaction.</param>
        /// <param name="factory">The underlying repository factory.</param>
        protected UnitOfWorkBase(ITransactionManager transactionManager, IRepositoryFactory factory)
        {
            if (transactionManager == null)
                throw new ArgumentNullException(nameof(transactionManager));

            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            TransactionManager = transactionManager;
            _factory = factory;
            Repositories = new Dictionary<Type, object>();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (TransactionManager != null)
                {
                    TransactionManager.Rollback();
                    TransactionManager.Dispose();
                    TransactionManager = null;
                }
            }

            _disposed = true;
        }

        #endregion

        #region Implementation of IUnitOfWork

        /// <summary>
        /// Commits all changes made in this unit of work.
        /// </summary>
        public virtual void Commit()
        {
            if (TransactionManager == null)
                throw new InvalidOperationException("The transaction has already been committed or disposed.");

            TransactionManager.Commit();
            TransactionManager = null;
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
            return _factory.Create<TEntity>();
        }

        /// <summary>
        /// Creates a new repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity, TKey> Create<TEntity, TKey>() where TEntity : class
        {
            return _factory.Create<TEntity, TKey>();
        }

        /// <summary>
        /// Creates a new repository for the specified type.
        /// </summary>
        /// <returns>The new repository.</returns>
        public T CreateInstance<T>() where T : class
        {
            return _factory.CreateInstance<T>();
        }

        #endregion
    }
}
