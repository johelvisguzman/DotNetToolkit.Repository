﻿namespace DotNetToolkit.Repository.InMemory
{
    using Configuration;
    using FetchStrategies;
    using Helpers;
    using Properties;
    using Queries;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Transactions;

    /// <summary>
    /// Represents an internal repository context for in-memory operations (for testing purposes).
    /// </summary>
    /// <seealso cref="IRepositoryContext" />
    internal class InMemoryRepositoryContext : IRepositoryContext
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

        /// <summary>
        /// Gets the item types in-memory that have not yet been saved. This collection is cleared after the context is saved.
        /// </summary>
        internal IEnumerable<Type> ItemTypes { get { return _items.Select(x => x.Entity.GetType()); } }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryContext" /> class.
        /// </summary>
        public InMemoryRepositoryContext() : this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryContext" /> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        public InMemoryRepositoryContext(string databaseName)
        {
            DatabaseName = string.IsNullOrEmpty(databaseName) ? DefaultDatabaseName : databaseName;
        }

        #endregion

        #region Private Methods

        private void ThrowsIfEntityPrimaryKeyValuesLengthMismatch<TEntity>(object[] keyValues) where TEntity : class
        {
            if (keyValues.Length != ConventionHelper.GetPrimaryKeyPropertyInfos<TEntity>().Count())
                throw new ArgumentException(DotNetToolkit.Repository.Properties.Resources.EntityPrimaryKeyValuesLengthMismatch, nameof(keyValues));
        }

        private IQueryable<TEntity> GetQuery<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            return options != null ? options.Apply(AsQueryable<TEntity>(options.FetchStrategy)) : AsQueryable<TEntity>();
        }

        #endregion

        #region Implementation of IContext

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <returns>The transaction.</returns>
        public ITransactionManager BeginTransaction()
        {
            throw new NotSupportedException(Resources.TransactionNotSupported);
        }

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
        public virtual int SaveChanges()
        {
            var store = InMemoryCache.Instance.GetDatabaseStore(DatabaseName);
            var count = 0;

            try
            {
                foreach (var entitySet in _items)
                {
                    var entityType = entitySet.Entity.GetType();
                    var key = GetPrimaryKey(entitySet.Entity);

                    if (!store.ContainsKey(entityType))
                        store[entityType] = new ConcurrentDictionary<object, object>();

                    var context = store[entityType];

                    if (entitySet.State == EntityState.Added)
                    {
                        if (context.ContainsKey(key))
                        {
                            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                Resources.EntityAlreadyBeingTrackedInStore, entitySet.Entity.GetType()));
                        }

                        var primeryKeyPropertyInfo = ConventionHelper.GetPrimaryKeyPropertyInfos(entityType).First();

                        if (ConventionHelper.IsIdentity(primeryKeyPropertyInfo))
                        {
                            key = GeneratePrimaryKey(entityType);

                            primeryKeyPropertyInfo.SetValue(entitySet.Entity, key);
                        }
                    }
                    else if (!context.ContainsKey(key))
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
            }
            finally
            {
                // Clears the collection
                while (_items.Count > 0)
                {
                    _items.TryTake(out _);
                }
            }

            return count;
        }

        /// <summary>
        /// Returns the entity <see cref="T:System.Linq.IQueryable`1" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <returns>The entity <see cref="T:System.Linq.IQueryable`1" />.</returns>
        public virtual IQueryable<TEntity> AsQueryable<TEntity>() where TEntity : class
        {
            var entityType = typeof(TEntity);
            var store = InMemoryCache.Instance.GetDatabaseStore(DatabaseName);

            if (!store.ContainsKey(entityType))
                return Enumerable.Empty<TEntity>().AsQueryable();

            return store[entityType].Select(x => (TEntity)Convert.ChangeType(x.Value, entityType)).AsQueryable();
        }

        /// <summary>
        /// Returns the entity <see cref="T:System.Linq.IQueryable`1" /> using the specified fetching strategy.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="fetchStrategy"></param>
        /// <returns>The entity <see cref="T:System.Linq.IQueryable`1" />.</returns>
        public IQueryable<TEntity> AsQueryable<TEntity>(IFetchStrategy<TEntity> fetchStrategy) where TEntity : class
        {
            return AsQueryable<TEntity>();
        }

        /// <summary>
        /// Finds an entity with the given primary key values in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The entity found in the repository.</returns>
        public virtual TEntity Find<TEntity>(IFetchStrategy<TEntity> fetchStrategy, params object[] keyValues) where TEntity : class
        {
            if (keyValues == null)
                throw new ArgumentNullException(nameof(keyValues));

            ThrowsIfEntityPrimaryKeyValuesLengthMismatch<TEntity>(keyValues);

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

            var entityType = typeof(TEntity);
            var store = InMemoryCache.Instance.GetDatabaseStore(DatabaseName);

            if (!store.ContainsKey(entityType))
                return default(TEntity);

            store[entityType].TryGetValue(key, out object entity);

            return (TEntity)Convert.ChangeType(entity, entityType);
        }

        /// <summary>
        /// Finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public TResult Find<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return GetQuery(options).Select(selector).FirstOrDefault();
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public IEnumerable<TResult> FindAll<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return GetQuery(options).Select(selector).ToList();
        }

        /// <summary>
        /// Finds the collection of entities in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <returns>The collection of entities in the repository.</returns>
        public IEnumerable<TEntity> FindAll<TEntity>() where TEntity : class
        {
            return FindAll<TEntity, TEntity>((IQueryOptions<TEntity>)null, IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public int Count<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            return GetQuery(options).Count();
        }

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        public bool Exists<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return GetQuery(options).Any();
        }

        /// <summary>
        /// Returns a new <see cref="T:System.Collections.Generic.Dictionary`2" /> according to the specified <paramref name="keySelector" />, and an element selector function with entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public Dictionary<TDictionaryKey, TElement> ToDictionary<TEntity, TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector) where TEntity : class
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            var keySelectFunc = keySelector.Compile();
            var elementSelectorFunc = elementSelector.Compile();

            return GetQuery(options).ToDictionary(keySelectFunc, elementSelectorFunc);
        }

        /// <summary>
        /// Returns a new <see cref="T:System.Collections.Generic.IEnumerable`1" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <returns>A new <see cref="T:System.Linq.IGrouping`2" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public IEnumerable<TResult> GroupBy<TEntity, TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector) where TEntity : class
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            var keySelectFunc = keySelector.Compile();
            var resultSelectorFunc = resultSelector.Compile();

            return GetQuery(options).GroupBy(keySelectFunc, resultSelectorFunc).ToList();
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

            InMemoryCache.Instance.GetDatabaseStore(DatabaseName).Clear();
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
                var store = InMemoryCache.Instance.GetDatabaseStore(DatabaseName);

                if (!store.ContainsKey(entityType))
                    return 1;

                var key = store[entityType]
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
            /// Initializes a new instance of the <see cref="EntitySet" /> class.
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
            private readonly ConcurrentDictionary<string, ConcurrentDictionary<Type, ConcurrentDictionary<object, object>>> _storage;

            #endregion

            #region Constructors

            /// <summary>
            /// Prevents a default instance of the <see cref="InMemoryCache" /> class from being created.
            /// </summary>
            private InMemoryCache()
            {
                _storage = new ConcurrentDictionary<string, ConcurrentDictionary<Type, ConcurrentDictionary<object, object>>>();
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
            public ConcurrentDictionary<Type, ConcurrentDictionary<object, object>> GetDatabaseStore(string name)
            {
                if (!_storage.ContainsKey(name))
                {
                    _storage[name] = new ConcurrentDictionary<Type, ConcurrentDictionary<object, object>>();
                }

                return _storage[name];
            }

            #endregion
        }

        #endregion
    }
}