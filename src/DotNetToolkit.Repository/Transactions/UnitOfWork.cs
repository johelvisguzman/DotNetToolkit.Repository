namespace DotNetToolkit.Repository.Transactions
{
    using Configuration;
    using Configuration.Options;
    using Configuration.Options.Internal;
    using Factories;
    using JetBrains.Annotations;
    using System;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="IUnitOfWork" />.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        #region Fields

        private IRepositoryOptions _options;
        private IRepositoryContext _context;
        private ITransactionManager _transactionManager;

        private bool _disposed;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork" /> class.
        /// </summary>
        /// <param name="optionsAction">A builder action used to create or modify options for this unit of work.</param>
        public UnitOfWork([NotNull] Action<RepositoryOptionsBuilder> optionsAction)
        {
            Guard.NotNull(optionsAction, nameof(optionsAction));

            var optionsBuilder = new RepositoryOptionsBuilder();

            optionsAction(optionsBuilder);

            Initialize(optionsBuilder.Options);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork" /> class.
        /// </summary>
        /// <param name="options">The repository options.</param>
        public UnitOfWork([NotNull] IRepositoryOptions options)
        {
            Initialize(Guard.NotNull(options, nameof(options)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class with the <see cref="RepositoryDependencyResolver"/> using an IOC container to resolve the <see cref="IRepositoryOptions"/>.
        /// </summary>
        public UnitOfWork() : this(RepositoryDependencyResolver.Current.Resolve<IRepositoryOptions>()) { }

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

        #region Private Methods

        private void Initialize([NotNull] IRepositoryOptions options)
        {
            Guard.NotNull(options, nameof(options));

            var contextFactory = Guard.EnsureNotNull(options.ContextFactory, "No context provider has been configured for this unit of work.");

            _context = contextFactory.Create();
            _transactionManager = _context.BeginTransaction();

            // Re-configures the options and adds a shared context for the repositories to use
            _options = new RepositoryOptions(options).With(new RepositoryContextFactory(_context));
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
            ThrowIfDisposed();

            return new Repository<TEntity>(_options);
        }

        /// <summary>
        /// Creates a new repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity, TKey> Create<TEntity, TKey>() where TEntity : class
        {
            ThrowIfDisposed();

            return new Repository<TEntity, TKey>(_options);
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
            ThrowIfDisposed();

            return new Repository<TEntity, TKey1, TKey2>(_options);
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
            ThrowIfDisposed();

            return new Repository<TEntity, TKey1, TKey2, TKey3>(_options);
        }

        /// <summary>
        /// Creates a new repository for the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The new repository.</returns>
        public T CreateInstance<T>() where T : class
        {
            ThrowIfDisposed();

            return (T)Activator.CreateInstance(typeof(T), new object[] { _options });
        }

        #endregion

        #region Nested Type: RepositoryContextFactory

        private class RepositoryContextFactory : IRepositoryContextFactory
        {
            private readonly IRepositoryContext _context;

            public RepositoryContextFactory(IRepositoryContext context)
            {
                _context = Guard.NotNull(context, nameof(context));
            }

            public IRepositoryContext Create() => _context;
        }

        #endregion
    }
}
