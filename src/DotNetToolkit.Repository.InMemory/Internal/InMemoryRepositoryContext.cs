namespace DotNetToolkit.Repository.InMemory.Internal
{
    using Configuration;
    using Configuration.Conventions;
    using Properties;
    using Queries;
    using Queries.Strategies;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Transactions;

    /// <summary>
    /// Represents an internal repository context for in-memory operations (for testing purposes).
    /// </summary>
    /// <seealso cref="IRepositoryContext" />
    internal class InMemoryRepositoryContext : LinqRepositoryContextBase
    {
        #region Fields

        private const string DefaultDatabaseName = "DotNetToolkit.Repository.InMemory";

        private readonly bool _ignoreTransactionWarning;
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
        /// Initializes a new instance of the <see cref="InMemoryRepositoryContext" /> class.
        /// </summary>
        /// <param name="ignoreTransactionWarning">If a transaction operation is requested, ignore any warnings since the in-memory provider does not support transactions.</param>
        public InMemoryRepositoryContext(bool ignoreTransactionWarning = false)
        {
            DatabaseName = DefaultDatabaseName;
            _ignoreTransactionWarning = ignoreTransactionWarning;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryContext" /> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        /// <param name="ignoreTransactionWarning">If a transaction operation is requested, ignore any warnings since the in-memory provider does not support transactions.</param>
        public InMemoryRepositoryContext(string databaseName, bool ignoreTransactionWarning = false)
        {
            DatabaseName = string.IsNullOrEmpty(databaseName) ? DefaultDatabaseName : databaseName;
            _ignoreTransactionWarning = ignoreTransactionWarning;
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

            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyValueTypeInvalid, entityType.FullName, propertyType));
        }

        #endregion

        #region Implementation of IRepositoryContext

        /// <summary>
        /// Returns the entity's query.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The entity's query.</returns>
        protected override IQueryable<TEntity> AsQueryable<TEntity>(IQueryOptions<TEntity> options)
        {
            var entityType = typeof(TEntity);
            var store = InMemoryCache.Instance.GetDatabaseStore(DatabaseName);

            if (!store.ContainsKey(entityType))
                return Enumerable.Empty<TEntity>().AsQueryable();

            return store[entityType].Select(x => (TEntity)Convert.ChangeType(x.Value, entityType)).AsQueryable();
        }

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database and returns a collection of entities.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public override QueryResult<IEnumerable<TEntity>> ExecuteQuery<TEntity>(string sql, CommandType cmdType, object[] parameters, Func<IDataReader, TEntity> projector)
        {
            throw new NotSupportedException(Resources.QueryExecutionNotSupported);
        }

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        public override QueryResult<int> ExecuteQuery(string sql, CommandType cmdType, object[] parameters)
        {
            throw new NotSupportedException(Resources.QueryExecutionNotSupported);
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <returns>The transaction.</returns>
        public override ITransactionManager BeginTransaction()
        {
            if (!_ignoreTransactionWarning)
                throw new NotSupportedException(Resources.TransactionNotSupported);

            CurrentTransaction = InMemoryNullTransactionManager.Instance;

            return CurrentTransaction;
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be inserted into the database when <see cref="SaveChanges" /> is called..
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public override void Add<TEntity>(TEntity entity)
        {
            _items.Add(new EntitySet(entity, EntityState.Added));
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be updated in the database when <see cref="SaveChanges" /> is called..
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public override void Update<TEntity>(TEntity entity)
        {
            _items.Add(new EntitySet(entity, EntityState.Modified));
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be removed from the database when <see cref="SaveChanges" /> is called..
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public override void Remove<TEntity>(TEntity entity)
        {
            _items.Add(new EntitySet(entity, EntityState.Removed));
        }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        public override int SaveChanges()
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
                                Resources.EntityAlreadyBeingTrackedInStore, entitySet.Entity.GetType()));
                        }

                        var primaryKeyPropertyInfo = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(entityType).First();

                        if (primaryKeyPropertyInfo.IsColumnIdentity())
                        {
                            key = GeneratePrimaryKey(entityType);

                            primaryKeyPropertyInfo.SetValue(entitySet.Entity, key);
                        }
                    }
                    else if (!context.ContainsKey(key))
                    {
                        throw new InvalidOperationException(Resources.EntityNotFoundInStore);
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

        /// <summary>
        /// Finds an entity with the given primary key values in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The entity found in the repository.</returns>
        public override QueryResult<TEntity> Find<TEntity>(IFetchQueryStrategy<TEntity> fetchStrategy, params object[] keyValues)
        {
            if (keyValues == null)
                throw new ArgumentNullException(nameof(keyValues));

            PrimaryKeyConventionHelper.ThrowsIfEntityPrimaryKeyValuesLengthMismatch<TEntity>(keyValues);

            var entityType = typeof(TEntity);
            var store = InMemoryCache.Instance.GetDatabaseStore(DatabaseName);

            if (!store.ContainsKey(entityType))
                return new QueryResult<TEntity>(default(TEntity));

            var key = PrimaryKeyConventionHelper.Combine(keyValues);

            store[entityType].TryGetValue(key, out object entity);

            var result = (TEntity)Convert.ChangeType(entity, entityType);

            return new QueryResult<TEntity>(result);
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            // Clears the collection
            while (_items.Count > 0)
            {
                _items.TryTake(out _);
            }

            base.Dispose();
        }

        #endregion
    }
}