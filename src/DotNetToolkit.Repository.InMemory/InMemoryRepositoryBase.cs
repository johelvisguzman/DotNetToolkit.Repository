namespace DotNetToolkit.Repository.InMemory
{
    using FetchStrategies;
    using Interceptors;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a repository for in-memory operations with a composite primary key (for testing purposes).
    /// </summary>
    public abstract class InMemoryRepositoryBase<TEntity, TKey1, TKey2, TKey3> : RepositoryBase<TEntity, TKey1, TKey2, TKey3> where TEntity : class
    {
        #region Fields

        private bool _disposed;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the context.
        /// </summary>
        internal InMemoryContext Context { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryBase{TEntity, TKey1, TKey2, TKey3}"/> class.
        /// </summary>
        protected InMemoryRepositoryBase() : this(null, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryBase{TEntity, TKey1, TKey2, TKey3}"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        protected InMemoryRepositoryBase(string databaseName) : this(databaseName, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryBase{TEntity, TKey1, TKey2, TKey3}"/> class.
        /// </summary>
        /// <param name="interceptors">The interceptors.</param>
        protected InMemoryRepositoryBase(IEnumerable<IRepositoryInterceptor> interceptors) : this(null, interceptors) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryBase{TEntity, TKey1, TKey2, TKey3}"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        /// <param name="interceptors">The interceptors.</param>
        protected InMemoryRepositoryBase(string databaseName, IEnumerable<IRepositoryInterceptor> interceptors) : base(interceptors)
        {
            Context = new InMemoryContext(databaseName);
        }

        #endregion

        #region Overrides of RepositoryBase<TEntity, TKey1, TKey2, TKey3>

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                Context.Dispose();
                Context = null;
            }

            _disposed = true;
        }

        /// <summary>
        /// A protected overridable method for adding the specified <paramref name="entity" /> into the repository.
        /// </summary>
        protected override void AddItem(TEntity entity)
        {
            Context.Add(entity);
        }

        /// <summary>
        /// A protected overridable method for deleting the specified <paramref name="entity" /> from the repository.
        /// </summary>
        protected override void DeleteItem(TEntity entity)
        {
            Context.Remove(entity);
        }

        /// <summary>
        /// A protected overridable method for updating the specified <paramref name="entity" /> in the repository.
        /// </summary>
        protected override void UpdateItem(TEntity entity)
        {
            Context.Update(entity);
        }

        /// <summary>
        /// A protected overridable method for saving changes made in the current unit of work in the repository.
        /// </summary>
        protected override void SaveChanges()
        {
            Context.SaveChanges();
        }

        /// <summary>
        /// A protected overridable method for getting an entity query that supplies the specified fetching strategy from the repository.
        /// </summary>
        protected override IQueryable<TEntity> GetQuery(IFetchStrategy<TEntity> fetchStrategy = null)
        {
            return Context.FindAll<TEntity>().AsQueryable();
        }

        /// <summary>
        /// Gets an entity query with the given primary key value from the repository.
        /// </summary>
        protected override TEntity GetEntity(object[] keyValues, IFetchStrategy<TEntity> fetchStrategy)
        {
            ThrowsIfEntityPrimaryKeyValuesLengthMismatch(keyValues);

            return Context.Find<TEntity>(keyValues);
        }

        #endregion
    }

    /// <summary>
    /// Represents a repository for in-memory operations with a composite primary key (for testing purposes).
    /// </summary>
    public abstract class InMemoryRepositoryBase<TEntity, TKey1, TKey2> : RepositoryBase<TEntity, TKey1, TKey2> where TEntity : class
    {
        #region Fields

        private bool _disposed;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the context.
        /// </summary>
        internal InMemoryContext Context { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryBase{TEntity, TKey1, TKey2}"/> class.
        /// </summary>
        protected InMemoryRepositoryBase() : this(null, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryBase{TEntity, TKey1, TKey2}"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        protected InMemoryRepositoryBase(string databaseName) : this(databaseName, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryBase{TEntity, TKey1, TKey2}"/> class.
        /// </summary>
        /// <param name="interceptors">The interceptors.</param>
        protected InMemoryRepositoryBase(IEnumerable<IRepositoryInterceptor> interceptors) : this(null, interceptors) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryBase{TEntity, TKey1, TKey2}"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        /// <param name="interceptors">The interceptors.</param>
        protected InMemoryRepositoryBase(string databaseName, IEnumerable<IRepositoryInterceptor> interceptors) : base(interceptors)
        {
            Context = new InMemoryContext(databaseName);
        }

        #endregion

        #region Overrides of RepositoryBase<TEntity, TKey1, TKey2>

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                Context.Dispose();
                Context = null;
            }

            _disposed = true;
        }

        /// <summary>
        /// A protected overridable method for adding the specified <paramref name="entity" /> into the repository.
        /// </summary>
        protected override void AddItem(TEntity entity)
        {
            Context.Add(entity);
        }

        /// <summary>
        /// A protected overridable method for deleting the specified <paramref name="entity" /> from the repository.
        /// </summary>
        protected override void DeleteItem(TEntity entity)
        {
            Context.Remove(entity);
        }

        /// <summary>
        /// A protected overridable method for updating the specified <paramref name="entity" /> in the repository.
        /// </summary>
        protected override void UpdateItem(TEntity entity)
        {
            Context.Update(entity);
        }

        /// <summary>
        /// A protected overridable method for saving changes made in the current unit of work in the repository.
        /// </summary>
        protected override void SaveChanges()
        {
            Context.SaveChanges();
        }

        /// <summary>
        /// A protected overridable method for getting an entity query that supplies the specified fetching strategy from the repository.
        /// </summary>
        protected override IQueryable<TEntity> GetQuery(IFetchStrategy<TEntity> fetchStrategy = null)
        {
            return Context.FindAll<TEntity>().AsQueryable();
        }

        /// <summary>
        /// Gets an entity query with the given primary key value from the repository.
        /// </summary>
        protected override TEntity GetEntity(object[] keyValues, IFetchStrategy<TEntity> fetchStrategy)
        {
            ThrowsIfEntityPrimaryKeyValuesLengthMismatch(keyValues);

            return Context.Find<TEntity>(keyValues);
        }

        #endregion
    }

    /// <summary>
    /// Represents a repository for in-memory operations (for testing purposes).
    /// </summary>
    public abstract class InMemoryRepositoryBase<TEntity, TKey> : RepositoryBase<TEntity, TKey> where TEntity : class
    {
        #region Fields

        private bool _disposed;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the context.
        /// </summary>
        internal InMemoryContext Context { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryBase{TEntity, TKey}"/> class.
        /// </summary>
        protected InMemoryRepositoryBase() : this(null, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        protected InMemoryRepositoryBase(string databaseName) : this(databaseName, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="interceptors">The interceptors.</param>
        protected InMemoryRepositoryBase(IEnumerable<IRepositoryInterceptor> interceptors) : this(null, interceptors) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        /// <param name="interceptors">The interceptors.</param>
        protected InMemoryRepositoryBase(string databaseName, IEnumerable<IRepositoryInterceptor> interceptors) : base(interceptors)
        {
            Context = new InMemoryContext(databaseName);
        }

        #endregion

        #region Overrides of RepositoryBase<TEntity, TKey>

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                Context.Dispose();
                Context = null;
            }

            _disposed = true;
        }

        /// <summary>
        /// A protected overridable method for adding the specified <paramref name="entity" /> into the repository.
        /// </summary>
        protected override void AddItem(TEntity entity)
        {
            Context.Add(entity);
        }

        /// <summary>
        /// A protected overridable method for deleting the specified <paramref name="entity" /> from the repository.
        /// </summary>
        protected override void DeleteItem(TEntity entity)
        {
            Context.Remove(entity);
        }

        /// <summary>
        /// A protected overridable method for updating the specified <paramref name="entity" /> in the repository.
        /// </summary>
        protected override void UpdateItem(TEntity entity)
        {
            Context.Update(entity);
        }

        /// <summary>
        /// A protected overridable method for saving changes made in the current unit of work in the repository.
        /// </summary>
        protected override void SaveChanges()
        {
            Context.SaveChanges();
        }

        /// <summary>
        /// A protected overridable method for getting an entity query that supplies the specified fetching strategy from the repository.
        /// </summary>
        protected override IQueryable<TEntity> GetQuery(IFetchStrategy<TEntity> fetchStrategy = null)
        {
            return Context.FindAll<TEntity>().AsQueryable();
        }

        /// <summary>
        /// Gets an entity query with the given primary key value from the repository.
        /// </summary>
        protected override TEntity GetEntity(object[] keyValues, IFetchStrategy<TEntity> fetchStrategy)
        {
            ThrowsIfEntityPrimaryKeyValuesLengthMismatch(keyValues);

            return Context.Find<TEntity>(keyValues);
        }

        #endregion
    }
}