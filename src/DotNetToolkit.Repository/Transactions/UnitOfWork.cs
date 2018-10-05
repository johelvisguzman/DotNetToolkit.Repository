namespace DotNetToolkit.Repository.Transactions
{
    using Configuration;
    using Configuration.Interceptors;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Factories;

    /// <summary>
    /// An implementation of <see cref="IUnitOfWork" />.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        #region Fields

        private IRepositoryContext _context;
        private ITransactionManager _transactionManager;
        private readonly IEnumerable<IRepositoryInterceptor> _interceptors;

        private bool _disposed;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork" /> class.
        /// </summary>
        /// <param name="factory">The repository context factory.</param>
        public UnitOfWork(IRepositoryContextFactory factory) : this(new RepositoryConfigurationOptions(factory)) { }

#if !NETSTANDARD
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork" /> class and uses the data from the App.config file to configure the repositories.
        /// </summary>
        public UnitOfWork() : this(Internal.ConfigFile.ConfigurationHelper.GetRequiredConfigurationOptions()) { }
#endif
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork" /> class.
        /// </summary>
        /// <param name="configuration">The repository configuration.</param>
        internal UnitOfWork(IRepositoryConfigurationOptions configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _context = configuration.GetContextFactory().Create();
            _transactionManager = _context.BeginTransaction();
            _interceptors = configuration.GetInterceptors();
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
                if (_transactionManager != null)
                {
                    _transactionManager.Rollback();
                    _transactionManager.Dispose();
                    _transactionManager = null;
                }

                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }

            _disposed = true;
        }

        /// <summary>
        /// Throws if this class has been disposed.
        /// </summary>
        protected virtual void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        #endregion

        #region Implementation of IUnitOfWork

        /// <summary>
        /// Commits all changes made in this unit of work.
        /// </summary>
        public virtual void Commit()
        {
            ThrowIfDisposed();

            if (_transactionManager == null)
                throw new InvalidOperationException("The transaction has already been committed.");

            _transactionManager.Commit();
            _transactionManager = null;
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
            return CreateInstance<Repository<TEntity>>();
        }

        /// <summary>
        /// Creates a new repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity, TKey> Create<TEntity, TKey>() where TEntity : class
        {
            return CreateInstance<Repository<TEntity, TKey>>();
        }

        /// <summary>
        /// Creates a new repository for the specified entity and a composite primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
        /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity, TKey1, TKey2> Create<TEntity, TKey1, TKey2>() where TEntity : class
        {
            return CreateInstance<Repository<TEntity, TKey1, TKey2>>();
        }

        /// <summary>
        /// Creates a new repository for the specified entity and a composite primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
        /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
        /// <typeparam name="TKey3">The type of the third part of the composite primary key.</typeparam>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity, TKey1, TKey2, TKey3> Create<TEntity, TKey1, TKey2, TKey3>() where TEntity : class
        {
            return CreateInstance<Repository<TEntity, TKey1, TKey2, TKey3>>();
        }

        /// <summary>
        /// Creates a new repository for the specified type.
        /// </summary>
        /// <returns>The new repository.</returns>
        public T CreateInstance<T>() where T : class
        {
            ThrowIfDisposed();

            var args = new List<object> { _context };

            if (_interceptors.Any())
                args.Add(_interceptors);

            try
            {
                return (T)Activator.CreateInstance(typeof(T), BindingFlags.NonPublic | BindingFlags.Instance, null, args.ToArray(), null);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        #endregion
    }
}
