namespace DotNetToolkit.Repository.InMemory
{
    using FetchStrategies;
    using Helpers;
    using Interceptors;
    using Properties;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents a repository for in-memory operations (for testing purposes).
    /// </summary>
    public abstract class InMemoryRepositoryBase<TEntity, TKey> : RepositoryBase<TEntity, TKey> where TEntity : class
    {
        #region Fields

        private const string DefaultDatabaseName = "DotNetToolkit.Repository.InMemory";

        private List<EntitySet> _items;
        private bool _disposed;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        internal string DatabaseName { get; set; }

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
            DatabaseName = string.IsNullOrEmpty(databaseName) ? DefaultDatabaseName : databaseName;
            _items = new List<EntitySet>();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Ensures the in-memory store is completely deleted.
        /// </summary>
        internal void EnsureDeleted()
        {
            _items.Clear();
            InMemoryCache.Instance.GetContext(DatabaseName).Clear();
        }

        #endregion

        #region	Private Methods

        /// <summary>
        /// Returns a deep copy of the specified object. This method does not require the object to be marked as serializable.
        /// </summary>
        /// <param name="entity">The object to be copy.</param>
        /// <returns>The deep copy of the specified object.</returns>
        private static TEntity DeepCopy(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var newItem = (TEntity)Activator.CreateInstance(typeof(TEntity));

            foreach (var propInfo in entity.GetType().GetRuntimeProperties())
            {
                if (propInfo.CanWrite)
                    propInfo.SetValue(newItem, propInfo.GetValue(entity, null), null);
            }

            return newItem;
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
                _items.Clear();
                _items = null;
            }

            _disposed = true;
        }

        /// <summary>
        /// A protected overridable method for adding the specified <paramref name="entity" /> into the repository.
        /// </summary>
        protected override void AddItem(TEntity entity)
        {
            _items.Add(new EntitySet(entity, EntityState.Added));
        }

        /// <summary>
        /// A protected overridable method for deleting the specified <paramref name="entity" /> from the repository.
        /// </summary>
        protected override void DeleteItem(TEntity entity)
        {
            _items.Add(new EntitySet(entity, EntityState.Removed));
        }

        /// <summary>
        /// A protected overridable method for updating the specified <paramref name="entity" /> in the repository.
        /// </summary>
        protected override void UpdateItem(TEntity entity)
        {
            _items.Add(new EntitySet(entity, EntityState.Modified));
        }

        /// <summary>
        /// A protected overridable method for saving changes made in the current unit of work in the repository.
        /// </summary>
        protected override void SaveChanges()
        {
            var context = InMemoryCache.Instance.GetContext(DatabaseName);

            foreach (var entitySet in _items)
            {
                var entityType = entitySet.Entity.GetType();
                var keyPropertyInfo = ConventionHelper.GetPrimaryKeyPropertyInfos(entityType).First();
                var key = (TKey)Convert.ChangeType(keyPropertyInfo.GetValue(entitySet.Entity, null), typeof(TKey));

                if (entitySet.State == EntityState.Added)
                {
                    var isKeyNullOrDefault = key != null && key.Equals(default(TKey));

                    if (isKeyNullOrDefault)
                    {
                        key = GeneratePrimaryKey();

                        keyPropertyInfo.SetValue(entitySet.Entity, key);
                    }
                    else if (context.ContainsKey(key))
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityAlreadyBeingTrackedInStore, entitySet.Entity.GetType()));
                    }
                }
                else if (!context.ContainsKey(key))
                {
                    throw new InvalidOperationException(Resources.EntityNotFoundInStore);
                }

                if (entitySet.State == EntityState.Removed)
                {
                    context.TryRemove(key, out TEntity entity);
                }
                else
                {
                    context[key] = DeepCopy(entitySet.Entity);
                }
            }

            _items.Clear();
        }

        /// <summary>
        /// A protected overridable method for getting an entity query that supplies the specified fetching strategy from the repository.
        /// </summary>
        protected override IQueryable<TEntity> GetQuery(IFetchStrategy<TEntity> fetchStrategy = null)
        {
            return InMemoryCache.Instance.GetContext(DatabaseName).Select(y => y.Value).AsQueryable();
        }

        /// <summary>
        /// Gets an entity query with the given primary key value from the repository.
        /// </summary>
        protected override TEntity GetEntity(TKey key, IFetchStrategy<TEntity> fetchStrategy)
        {
            InMemoryCache.Instance.GetContext(DatabaseName).TryGetValue(key, out TEntity entity);

            return entity;
        }

        #endregion

        #region Nested type: EntitySet

        /// <summary>
        /// Represents an internal entity set in the in-memory store, which holds the entity and it's state representing the operation that was performed at the time.
        /// </summary>
        private class EntitySet
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="EntitySet"/> class.
            /// </summary>
            /// <param name="entity">The entity.</param>
            /// <param name="state">The state.</param>
            public EntitySet(TEntity entity, EntityState state)
            {
                Entity = entity;
                State = state;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the entity.
            /// </summary>
            public TEntity Entity { get; }

            /// <summary>
            /// Gets the state.
            /// </summary>
            public EntityState State { get; }

            #endregion
        }

        #endregion

        #region Nested type: EntityState

        /// <summary>
        /// Represents an internal state for an entity in the in-memory store.
        /// </summary>
        private enum EntityState
        {
            Added,
            Removed,
            Modified
        }

        #endregion

        #region Nested type: InMemoryCache

        /// <summary>
        /// Represents an internal thread safe database storage which will store any information for the in-memory
        /// store that is needed through the life time of the application.
        /// </summary>
        private class InMemoryCache
        {
            #region Fields

            private static volatile InMemoryCache _instance;
            private static readonly object _syncRoot = new object();
            private readonly ConcurrentDictionary<string, ConcurrentDictionary<TKey, TEntity>> _storage;

            #endregion

            #region Constructors

            /// <summary>
            /// Prevents a default instance of the <see cref="InMemoryCache"/> class from being created.
            /// </summary>
            private InMemoryCache()
            {
                _storage = new ConcurrentDictionary<string, ConcurrentDictionary<TKey, TEntity>>();
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the instance.
            /// </summary>
            public static InMemoryCache Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        lock (_syncRoot)
                        {
                            if (_instance == null)
                                _instance = new InMemoryCache();
                        }
                    }

                    return _instance;
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Gets the scoped database context by the specified name.
            /// </summary>
            /// <param name="name">The database name.</param>
            /// <returns>The scoped database context by the specified database name.</returns>
            public ConcurrentDictionary<TKey, TEntity> GetContext(string name)
            {
                if (!_storage.ContainsKey(name))
                {
                    _storage[name] = new ConcurrentDictionary<TKey, TEntity>();
                }

                return _storage[name];
            }

            #endregion
        }

        #endregion
    }
}