﻿namespace DotNetToolkit.Repository.InMemory
{
    using FetchStrategies;
    using Logging;
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

        private static readonly object _syncRoot = new object();
        private ConcurrentDictionary<TKey, EntitySet<TEntity, TKey>> _context;
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
        /// Initializes a new instance of the <see cref="InMemoryRepositoryBase{TEntity,TKey}"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        protected InMemoryRepositoryBase(string databaseName = null) : this(databaseName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryBase{TEntity,TKey}"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected InMemoryRepositoryBase(ILogger logger) : this(null, logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryBase{TEntity,TKey}"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        /// <param name="logger">The logger.</param>
        protected InMemoryRepositoryBase(string databaseName, ILogger logger) : base(logger)
        {
            DatabaseName = string.IsNullOrEmpty(databaseName) ? DefaultDatabaseName : databaseName;
            _context = new ConcurrentDictionary<TKey, EntitySet<TEntity, TKey>>();
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
                _context.Clear();
                _context = null;
            }

            _disposed = true;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Ensures the in-memory store is completely deleted.
        /// </summary>
        internal void EnsureDeleted()
        {
            _context.Clear();
            InMemoryCache<TEntity, TKey>.Instance.GetContext(DatabaseName).Clear();
        }

        /// <summary>
        /// Sets the time stamp.
        /// </summary>
        /// <param name="time">The time.</param>
        internal void SetTimeStamp(DateTime time)
        {
            InMemoryCache<TEntity, TKey>.Instance.SetTimeStamp(DatabaseName, time);
        }

        /// <summary>
        /// Gets the time stamp.
        /// </summary>
        /// <returns>The time stamp.</returns>
        internal DateTime GetTimeStamp()
        {
            return InMemoryCache<TEntity, TKey>.Instance.GetTimeStamp(DatabaseName);
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

        /// <summary>
        /// Generates a new temporary primary id for the entity.
        /// </summary>
        /// <returns>The new generated primary id.</returns>
        protected virtual TKey GenerateTemporaryPrimaryKey()
        {
            var propertyInfo = GetPrimaryKeyPropertyInfo();
            var propertyType = propertyInfo.PropertyType;
            if (propertyType == typeof(int))
            {
                var key = _context.OrderByDescending(x => x.Key).Select(x => x.Key).FirstOrDefault();

                return (TKey)Convert.ChangeType(Convert.ToInt32(key) + 1, typeof(TKey));
            }

            return GeneratePrimaryKey();
        }

        #endregion

        #region Overrides of RepositoryBase<TEntity,TKey>

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// A protected overridable method for adding the specified <paramref name="entity" /> into the repository.
        /// </summary>
        protected override void AddItem(TEntity entity)
        {
            // Ensures the last entity of the same reference is updated by the current one added
            var key = _context
                .Where(x => x.Value.Entity.Equals(entity))
                .Select(x => x.Key)
                .SingleOrDefault();

            var hasTemporaryKey = false;

            if (key != null && key.Equals(default(TKey)))
            {
                key = GetPrimaryKey(entity);

                if (key != null && key.Equals(default(TKey)))
                {
                    key = GenerateTemporaryPrimaryKey();
                    hasTemporaryKey = true;
                }
            }

            _context[key] = new EntitySet<TEntity, TKey>(entity, key, EntityState.Added)
            {
                HasTemporaryKey = hasTemporaryKey
            };
        }

        /// <summary>
        /// A protected overridable method for deleting the specified <paramref name="entity" /> from the repository.
        /// </summary>
        protected override void DeleteItem(TEntity entity)
        {
            var key = GetPrimaryKey(entity);
            var hasTemporaryKey = false;

            if (key != null && key.Equals(default(TKey)))
            {
                key = GenerateTemporaryPrimaryKey();
                hasTemporaryKey = true;
            }

            _context[key] = new EntitySet<TEntity, TKey>(entity, key, EntityState.Removed)
            {
                HasTemporaryKey = hasTemporaryKey
            };
        }

        /// <summary>
        /// A protected overridable method for updating the specified <paramref name="entity" /> in the repository.
        /// </summary>
        protected override void UpdateItem(TEntity entity)
        {
            var key = GetPrimaryKey(entity);
            var hasTemporaryKey = false;

            if (key != null && key.Equals(default(TKey)))
            {
                key = GenerateTemporaryPrimaryKey();
                hasTemporaryKey = true;
            }

            _context[key] = new EntitySet<TEntity, TKey>(entity, key, EntityState.Modified)
            {
                HasTemporaryKey = hasTemporaryKey
            };
        }

        /// <summary>
        /// A protected overridable method for saving changes made in the current unit of work in the repository.
        /// </summary>
        protected override void SaveChanges()
        {
            lock (_syncRoot)
            {
                var context = InMemoryCache<TEntity, TKey>.Instance.GetContext(DatabaseName);

                foreach (var entitySet in _context.Select(y => y.Value))
                {
                    var key = entitySet.Key;

                    if (entitySet.State == EntityState.Added)
                    {
                        if (entitySet.HasTemporaryKey)
                        {
                            key = GeneratePrimaryKey();
                            SetPrimaryKey(entitySet.Entity, key);
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
                        context.Remove(key);
                    }
                    else
                    {
                        context[key] = DeepCopy(entitySet.Entity);
                    }
                }

                SetTimeStamp(DateTime.Now);

                _context.Clear();
            }
        }

        /// <summary>
        /// A protected overridable method for getting an entity query that supplies the specified fetching strategy from the repository.
        /// </summary>
        protected override IQueryable<TEntity> GetQuery(IFetchStrategy<TEntity> fetchStrategy = null)
        {
            return InMemoryCache<TEntity, TKey>.Instance
                .GetContext(DatabaseName)
                .Select(y => y.Value)
                .AsQueryable();
        }

        /// <summary>
        /// Gets an entity query with the given primary key value from the repository.
        /// </summary>
        protected override TEntity GetEntity(TKey key, IFetchStrategy<TEntity> fetchStrategy)
        {
            InMemoryCache<TEntity, TKey>.Instance
                .GetContext(DatabaseName)
                .TryGetValue(key, out TEntity entity);

            return entity;
        }

        #endregion

        #region Nested type: EntitySet<TEntity, TKey>

        /// <summary>
        /// Represents an internal entity set in the in-memory store, which holds the entity and it's state representing the operation that was performed at the time.
        /// </summary>
        private class EntitySet<TEntity, TKey> where TEntity : class
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="EntitySet{TEntity, TKey}"/> class.
            /// </summary>
            /// <param name="entity">The entity.</param>
            /// <param name="key">The entity primary key value.</param>
            /// <param name="state">The state.</param>
            public EntitySet(TEntity entity, TKey key, EntityState state)
            {
                Entity = entity;
                Key = key;
                State = state;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the entity.
            /// </summary>
            public TEntity Entity { get; }

            /// <summary>
            /// Gets the primary key value.
            /// </summary>
            public TKey Key { get; }

            /// <summary>
            /// Gets the state.
            /// </summary>
            public EntityState State { get; }

            /// <summary>
            /// Gets or sets a value indicating whether this instance has a temporary key.
            /// </summary>
            public bool HasTemporaryKey { get; set; }

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

        #region Nested type: InMemoryCache<TEntity, TKey>

        /// <summary>
        /// Represents an internal thread safe database storage which will store any information for the in-memory
        /// store that is needed through the life time of the application.
        /// </summary>
        private class InMemoryCache<TEntity, TKey> where TEntity : class
        {
            #region Fields

            private static volatile InMemoryCache<TEntity, TKey> _instance;
            private static readonly object _syncRoot = new object();
            private readonly ConcurrentDictionary<string, SortedDictionary<TKey, TEntity>> _storage;
            private readonly ConcurrentDictionary<string, DateTime> _timestamp;

            #endregion

            #region Constructors

            /// <summary>
            /// Prevents a default instance of the <see cref="InMemoryCache{TEntity, TKey}"/> class from being created.
            /// </summary>
            private InMemoryCache()
            {
                _storage = new ConcurrentDictionary<string, SortedDictionary<TKey, TEntity>>();
                _timestamp = new ConcurrentDictionary<string, DateTime>();
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the instance.
            /// </summary>
            public static InMemoryCache<TEntity, TKey> Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        lock (_syncRoot)
                        {
                            if (_instance == null)
                                _instance = new InMemoryCache<TEntity, TKey>();
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
            public SortedDictionary<TKey, TEntity> GetContext(string name)
            {
                if (!_storage.ContainsKey(name))
                {
                    _storage[name] = new SortedDictionary<TKey, TEntity>();
                }

                return _storage[name];
            }

            /// <summary>
            /// Sets the time stamp.
            /// </summary>
            /// <param name="name">The database name.</param>
            /// <param name="time">The time.</param>
            public void SetTimeStamp(string name, DateTime time)
            {
                _timestamp[name] = time;
            }

            /// <summary>
            /// Gets the time stamp.
            /// </summary>
            /// <param name="name">The database name.</param>
            /// <returns>The time stamp.</returns>
            public DateTime GetTimeStamp(string name)
            {
                _timestamp.TryGetValue(name, out DateTime time);

                return time;
            }

            #endregion
        }

        #endregion
    }
}