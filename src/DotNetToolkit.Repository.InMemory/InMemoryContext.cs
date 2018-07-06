namespace DotNetToolkit.Repository.InMemory
{
    using Helpers;
    using Properties;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents an internal in-memory context.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    internal class InMemoryContext : IDisposable
    {
        #region Fields

        private const string DefaultDatabaseName = "DotNetToolkit.Repository.InMemory";

        private readonly BlockingCollection<EntitySet> _items = new BlockingCollection<EntitySet>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        public string DatabaseName { get; internal set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryContext"/> class.
        /// </summary>
        public InMemoryContext() : this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryContext"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        public InMemoryContext(string databaseName)
        {
            DatabaseName = string.IsNullOrEmpty(databaseName) ? DefaultDatabaseName : databaseName;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Tracks the specified entity in memory and will be inserted into the database when <see cref="SaveChanges" /> is called..
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            _items.Add(new EntitySet(entity, EntityState.Added));
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be updated in the database when <see cref="SaveChanges" /> is called..
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            _items.Add(new EntitySet(entity, EntityState.Modified));
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be removed from the database when <see cref="SaveChanges" /> is called..
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            _items.Add(new EntitySet(entity, EntityState.Removed));
        }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        public int SaveChanges()
        {
            var context = InMemoryCache.Instance.GetContext(DatabaseName);
            var count = 0;

            foreach (var entitySet in _items)
            {
                var entityType = entitySet.Entity.GetType();
                var keyPropertyInfo = ConventionHelper.GetPrimaryKeyPropertyInfos(entityType).First();
                var key = GetPrimaryKey(entitySet.Entity);
                var existInDb = context.Any(x => x.Value.GetType() == entityType && x.Key.Equals(key));

                if (entitySet.State == EntityState.Added)
                {
                    if (ConventionHelper.IsIdentity(keyPropertyInfo) && !ConventionHelper.HasCompositePrimaryKey(entityType))
                    {
                        key = GeneratePrimaryKey(entityType);

                        keyPropertyInfo.SetValue(entitySet.Entity, key);
                    }
                    else if (existInDb)
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityAlreadyBeingTrackedInStore, entitySet.Entity.GetType()));
                    }
                }
                else if (!existInDb)
                {
                    throw new InvalidOperationException(Resources.EntityNotFoundInStore);
                }

                if (entitySet.State == EntityState.Removed)
                {
                    context.TryRemove(key, out object entity);
                }
                else
                {
                    context[key] = DeepCopy(entitySet.Entity);
                }

                count++;
            }

            // Clears the collection
            while (_items.Count > 0)
            {
                _items.TryTake(out _);
            }

            return count;
        }

        /// <summary>
        /// Finds an entity in the in-memory context with the specified key values and entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="keyValues">The key values.</param>
        /// <returns>The entity found.</returns>
        public TEntity Find<TEntity>(params object[] keyValues) where TEntity : class
        {
            if (keyValues.Length != ConventionHelper.GetPrimaryKeyPropertyInfos<TEntity>().Count())
                throw new ArgumentException(DotNetToolkit.Repository.Properties.Resources.EntityPrimaryKeyValuesLengthMismatch, nameof(keyValues));

            object key;

            switch (keyValues.Length)
            {
                case 3:
                    {
                        key = Tuple.Create(keyValues[0], keyValues[1], keyValues[2]);
                        break;
                    }
                case 2:
                    {
                        key = Tuple.Create(keyValues[0], keyValues[1]);
                        break;
                    }
                default:
                    {
                        key = keyValues[0];
                        break;
                    }
            }

            InMemoryCache.Instance.GetContext(DatabaseName).TryGetValue(key, out object entity);

            return (TEntity)Convert.ChangeType(entity, typeof(TEntity));
        }

        /// <summary>
        /// Finds a collection of entities in the in-memory context of the specified type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>The collection of entities in the in-memory context of the specified type.</returns>
        public IEnumerable<TEntity> FindAll<TEntity>() where TEntity : class
        {
            return InMemoryCache.Instance.GetContext(DatabaseName).Select(y => (TEntity)Convert.ChangeType(y.Value, typeof(TEntity)));
        }

        /// <summary>
        /// Ensures the in-memory store is completely deleted.
        /// </summary>
        public void EnsureDeleted()
        {
            // Clears the collection
            while (_items.Count > 0)
            {
                _items.TryTake(out _);
            }

            InMemoryCache.Instance.GetContext(DatabaseName).Clear();
        }

        #endregion

        #region	Private Methods

        private static object DeepCopy(object entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var newItem = Activator.CreateInstance(entity.GetType());

            foreach (var propInfo in entity.GetType().GetRuntimeProperties())
            {
                if (propInfo.CanWrite)
                    propInfo.SetValue(newItem, propInfo.GetValue(entity, null), null);
            }

            return newItem;
        }

        private object GetPrimaryKey(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var entityType = obj.GetType();
            var propInfos = ConventionHelper.GetPrimaryKeyPropertyInfos(entityType).ToList();

            switch (propInfos.Count)
            {
                case 3:
                    {
                        var key1 = propInfos[0].GetValue(obj, null);
                        var key2 = propInfos[1].GetValue(obj, null);
                        var key3 = propInfos[2].GetValue(obj, null);

                        return Tuple.Create(key1, key2, key3);
                    }
                case 2:
                    {
                        var key1 = propInfos[0].GetValue(obj, null);
                        var key2 = propInfos[1].GetValue(obj, null);

                        return Tuple.Create(key1, key2);
                    }
                default:
                    {
                        return propInfos[0].GetValue(obj, null);
                    }

            }
        }

        private object GeneratePrimaryKey(Type entityType)
        {
            var propertyInfo = ConventionHelper.GetPrimaryKeyPropertyInfos(entityType).First();
            var propertyType = propertyInfo.PropertyType;

            if (propertyType == typeof(Guid))
                return Guid.NewGuid();

            if (propertyType == typeof(string))
                return Guid.NewGuid().ToString("N");

            if (propertyType == typeof(int))
            {
                var key = InMemoryCache.Instance.GetContext(DatabaseName)
                    .Where(x => x.Value.GetType() == entityType)
                    .Select(x => propertyInfo.GetValue(x.Value, null))
                    .OrderByDescending(x => x)
                    .FirstOrDefault();

                return Convert.ToInt32(key) + 1;
            }

            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyValueTypeInvalid, entityType.FullName, propertyType));
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Clears the collection
            while (_items.Count > 0)
            {
                _items.TryTake(out _);
            }
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
            public EntitySet(object entity, EntityState state)
            {
                Entity = entity;
                State = state;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the entity.
            /// </summary>
            public object Entity { get; }

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
            private readonly ConcurrentDictionary<string, ConcurrentDictionary<object, object>> _storage;

            #endregion

            #region Constructors

            /// <summary>
            /// Prevents a default instance of the <see cref="InMemoryCache"/> class from being created.
            /// </summary>
            private InMemoryCache()
            {
                _storage = new ConcurrentDictionary<string, ConcurrentDictionary<object, object>>();
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
            public ConcurrentDictionary<object, object> GetContext(string name)
            {
                if (!_storage.ContainsKey(name))
                {
                    _storage[name] = new ConcurrentDictionary<object, object>();
                }

                return _storage[name];
            }

            #endregion
        }

        #endregion
    }
}
