﻿namespace DotNetToolkit.Repository.InMemory.Internal
{
    using Configuration;
    using Configuration.Conventions;
    using Configuration.Logging;
    using Extensions;
    using Helpers;
    using Properties;
    using Queries;
    using Queries.Strategies;
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

        /// <summary>
        /// Gets the repository context logger.
        /// </summary>
        public ILogger Logger { get; private set; } = NullLogger.Instance;

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

        #region Public Methods

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

        #region Private Methods

        private void ThrowsIfEntityPrimaryKeyValuesLengthMismatch<TEntity>(object[] keyValues) where TEntity : class
        {
            if (keyValues.Length != PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos<TEntity>().Count())
                throw new ArgumentException(DotNetToolkit.Repository.Properties.Resources.EntityPrimaryKeyValuesLengthMismatch, nameof(keyValues));
        }

        private IQueryable<TEntity> AsQueryable<TEntity>() where TEntity : class
        {
            var entityType = typeof(TEntity);
            var store = InMemoryCache.Instance.GetDatabaseStore(DatabaseName);

            if (!store.ContainsKey(entityType))
                return Enumerable.Empty<TEntity>().AsQueryable();

            return store[entityType].Select(x => (TEntity)Convert.ChangeType(x.Value, entityType)).AsQueryable();
        }

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

        private object GeneratePrimaryKey(Type entityType)
        {
            var propertyInfo = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(entityType).First();
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

            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.InMemoryContext_EntityKeyValueTypeInvalid, entityType.FullName, propertyType));
        }

        #endregion

        #region Implementation of IRepositoryContext

        /// <inheritdoc />
        public ITransactionManager BeginTransaction()
        {
            throw new NotSupportedException(Resources.InMemoryContext_TransactionNotSupported);
        }

        /// <inheritdoc />
        public void UseLoggerProvider(ILoggerProvider loggerProvider)
        {
            if (loggerProvider == null)
                throw new ArgumentNullException(nameof(loggerProvider));

            Logger = loggerProvider.Create(GetType().FullName);
        }

        /// <inheritdoc />
        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            _items.Add(new EntitySet(entity, EntityState.Added));
        }

        /// <inheritdoc />
        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            _items.Add(new EntitySet(entity, EntityState.Modified));
        }

        /// <inheritdoc />
        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            _items.Add(new EntitySet(entity, EntityState.Removed));
        }

        /// <inheritdoc />
        public virtual int SaveChanges()
        {
            var store = InMemoryCache.Instance.GetDatabaseStore(DatabaseName);
            var count = 0;

            try
            {
                while (_items.TryTake(out var entitySet))
                {
                    var entityType = entitySet.Entity.GetType();
                    var key = PrimaryKeyConventionHelper.GetPrimaryKeyValue(entitySet.Entity);

                    if (!store.ContainsKey(entityType))
                        store[entityType] = new ConcurrentDictionary<object, object>();

                    var context = store[entityType];

                    if (entitySet.State == EntityState.Added)
                    {
                        if (context.ContainsKey(key))
                        {
                            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                Resources.InMemoryContext_EntityAlreadyBeingTrackedInStore, entitySet.Entity.GetType()));
                        }

                        var primeryKeyPropertyInfo = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(entityType).First();

                        if (primeryKeyPropertyInfo.IsColumnIdentity())
                        {
                            key = GeneratePrimaryKey(entityType);

                            primeryKeyPropertyInfo.SetValue(entitySet.Entity, key);
                        }
                    }
                    else if (!context.ContainsKey(key))
                    {
                        throw new InvalidOperationException(Resources.InMemoryContext_EntityNotFoundInStore);
                    }

                    if (entitySet.State == EntityState.Removed)
                    {
                        context.TryRemove(key, out _);
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

        /// <inheritdoc />
        public virtual QueryResult<TEntity> Find<TEntity>(IFetchQueryStrategy<TEntity> fetchStrategy, params object[] keyValues) where TEntity : class
        {
            if (keyValues == null)
                throw new ArgumentNullException(nameof(keyValues));

            ThrowsIfEntityPrimaryKeyValuesLengthMismatch<TEntity>(keyValues);

            var entityType = typeof(TEntity);
            var store = InMemoryCache.Instance.GetDatabaseStore(DatabaseName);

            if (!store.ContainsKey(entityType))
                return new QueryResult<TEntity>(default(TEntity));

            var key = PrimaryKeyConventionHelper.Combine(keyValues);

            store[entityType].TryGetValue(key, out object entity);

            var result = (TEntity)Convert.ChangeType(entity, entityType);

            return new QueryResult<TEntity>(result);
        }

        /// <inheritdoc />
        public QueryResult<TResult> Find<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var result = AsQueryable<TEntity>()
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(options)
                .ApplyPagingOptions(options)
                .Select(selector)
                .FirstOrDefault();

            return new QueryResult<TResult>(result);
        }

        /// <inheritdoc />
        public QueryResult<IEnumerable<TResult>> FindAll<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var query = AsQueryable<TEntity>()
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(options);

            var data = query
                .ApplyPagingOptions(options)
                .Select(selector)
                .Select(x => new
                {
                    Result = x,
                    Total = query.Count()
                })
                .ToList();

            var result = data.Select(x => x.Result);
            var total = data.FirstOrDefault()?.Total ?? 0;

            return new QueryResult<IEnumerable<TResult>>(result, total);
        }

        /// <inheritdoc />
        public QueryResult<IEnumerable<TEntity>> FindAll<TEntity>() where TEntity : class
        {
            return FindAll<TEntity, TEntity>((IQueryOptions<TEntity>)null, IdentityExpression<TEntity>.Instance);
        }

        /// <inheritdoc />
        public QueryResult<int> Count<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            var result = AsQueryable<TEntity>()
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(options)
                .ApplyPagingOptions(options)
                .Count();

            return new QueryResult<int>(result);
        }

        /// <inheritdoc />
        public QueryResult<bool> Exists<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var result = AsQueryable<TEntity>()
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(options)
                .ApplyPagingOptions(options)
                .Any();

            return new QueryResult<bool>(result);
        }

        /// <inheritdoc />
        public QueryResult<Dictionary<TDictionaryKey, TElement>> ToDictionary<TEntity, TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector) where TEntity : class
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            var keySelectFunc = keySelector.Compile();
            var elementSelectorFunc = elementSelector.Compile();

            var query = AsQueryable<TEntity>()
                .AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(options);

            Dictionary<TDictionaryKey, TElement> result;
            int total;

            if (options != null && options.PageSize != -1)
            {
                // Tries to get the count in one query
                var data = query
                    .ApplyPagingOptions(options)
                    .Select(x => new
                    {
                        Result = x,
                        Total = query.Count()
                    });

                result = data.Select(x => x.Result).ToDictionary(keySelectFunc, elementSelectorFunc);
                total = data.FirstOrDefault()?.Total ?? 0;
            }
            else
            {
                // Gets the total count from memory
                result = query.ToDictionary(keySelectFunc, elementSelectorFunc);
                total = result.Count;
            }

            return new QueryResult<Dictionary<TDictionaryKey, TElement>>(result, total);
        }

        /// <inheritdoc />
        public QueryResult<IEnumerable<TResult>> GroupBy<TEntity, TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector) where TEntity : class
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            var keySelectFunc = keySelector.Compile();
            var resultSelectorFunc = resultSelector.Compile();

            var query = AsQueryable<TEntity>()
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(options);

            var data = query
                .ApplyPagingOptions(options)
                .Select(x => new
                {
                    Result = x,
                    Total = query.Count()
                });

            var result = data.Select(x => x.Result).GroupBy(keySelectFunc, resultSelectorFunc).ToList();
            var total = data.FirstOrDefault()?.Total ?? 0;

            return new QueryResult<IEnumerable<TResult>>(result, total);
        }

        #endregion

        #region IDisposable

        /// <inheritdoc />
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