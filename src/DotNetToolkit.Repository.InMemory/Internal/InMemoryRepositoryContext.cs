namespace DotNetToolkit.Repository.InMemory.Internal
{
    using Configuration;
    using Configuration.Conventions;
    using Extensions;
    using Properties;
    using Queries.Strategies;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using Transactions;
    using Transactions.Internal;
    using Utility;

    /// <summary>
    /// Represents an internal repository context for in-memory operations (for testing purposes).
    /// </summary>
    /// <seealso cref="IRepositoryContext" />
    internal class InMemoryRepositoryContext : LinqRepositoryContextBase
    {
        #region Fields

        private const string DefaultDatabaseName = "DotNetToolkit.Repository.InMemory";

        private readonly bool _ignoreTransactionWarning;
        private readonly bool _ignoreSqlQueryWarning;
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
        /// <param name="ignoreSqlQueryWarning">If a SQL query is executed, ignore any warnings since the in-memory provider does not support SQL query execution.</param>
        public InMemoryRepositoryContext(bool ignoreTransactionWarning = false, bool ignoreSqlQueryWarning = false) : this(DefaultDatabaseName, ignoreTransactionWarning, ignoreSqlQueryWarning) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryContext" /> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        /// <param name="ignoreTransactionWarning">If a transaction operation is requested, ignore any warnings since the in-memory provider does not support transactions.</param>
        /// <param name="ignoreSqlQueryWarning">If a SQL query is executed, ignore any warnings since the in-memory provider does not support SQL query execution.</param>
        public InMemoryRepositoryContext(string databaseName, bool ignoreTransactionWarning = false, bool ignoreSqlQueryWarning = false)
        {
            DatabaseName = string.IsNullOrEmpty(databaseName) ? DefaultDatabaseName : databaseName;
            Conventions = RepositoryConventions.Default;

            _ignoreTransactionWarning = ignoreTransactionWarning;
            _ignoreSqlQueryWarning = ignoreSqlQueryWarning;
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

        private object GeneratePrimaryKey(Type entityType)
        {
            var propertyInfo = Conventions.GetPrimaryKeyPropertyInfos(entityType).First();
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

                var key = store[entityType].Keys.LastOrDefault();

                return Convert.ToInt32(key) + 1;
            }

            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyValueTypeInvalid, entityType.FullName, propertyType));
        }

        private object GetPrimaryKeyValue(object obj)
        {
            return Combine(Conventions.GetPrimaryKeyValues(Guard.NotNull(obj, nameof(obj))));
        }

        private static object Combine(object[] keyValues)
        {
            Guard.NotEmpty(keyValues, nameof(keyValues));

            return keyValues.Length == 1 ? keyValues[0] : string.Join(":", keyValues);
        }

        #endregion

        #region Implementation of IRepositoryContext

        /// <summary>
        /// Returns the entity's query.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <returns>The entity's query.</returns>
        protected override IQueryable<TEntity> AsQueryable<TEntity>()
        {
            var entityType = typeof(TEntity);
            var store = InMemoryCache.Instance.GetDatabaseStore(DatabaseName);

            if (!store.ContainsKey(entityType))
                return Enumerable.Empty<TEntity>().AsQueryable();

            var context = store[entityType];
            var query = context
                .Select(x => (TEntity)Convert.ChangeType(CloneableHelper.DeepCopy(x.Value), entityType))
                .AsQueryable();

            return query;
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <returns>The transaction.</returns>
        public override ITransactionManager BeginTransaction()
        {
            if (!_ignoreTransactionWarning)
                throw new NotSupportedException(DotNetToolkit.Repository.Properties.Resources.TransactionNotSupported);

            CurrentTransaction = NullTransactionManager.Instance;

            return CurrentTransaction;
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be inserted into the database when <see cref="SaveChanges" /> is called.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public override void Add<TEntity>(TEntity entity)
        {
            _items.Add(new EntitySet(Guard.NotNull(entity, nameof(entity)), EntityState.Added));
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be updated in the database when <see cref="SaveChanges" /> is called.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public override void Update<TEntity>(TEntity entity)
        {
            _items.Add(new EntitySet(Guard.NotNull(entity, nameof(entity)), EntityState.Modified));
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be removed from the database when <see cref="SaveChanges" /> is called.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public override void Remove<TEntity>(TEntity entity)
        {
            _items.Add(new EntitySet(Guard.NotNull(entity, nameof(entity)), EntityState.Removed));
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
                    var key = GetPrimaryKeyValue(entitySet.Entity);

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

                        var primaryKeyPropertyInfo = Conventions.GetPrimaryKeyPropertyInfos(entityType).First();

                        if (Conventions.IsColumnIdentity(primaryKeyPropertyInfo))
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
                        context[key] = CloneableHelper.DeepCopy(entitySet.Entity);
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
        /// Creates a raw SQL query that is executed directly in the database and returns a collection of entities.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public override IEnumerable<TEntity> ExecuteSqlQuery<TEntity>(string sql, CommandType cmdType, Dictionary<string, object> parameters, Func<IDataReader, IRepositoryConventions, TEntity> projector)
        {
            if (!_ignoreSqlQueryWarning)
                throw new NotSupportedException(Repository.Properties.Resources.QueryExecutionNotSupported);

            Guard.NotEmpty(sql, nameof(sql));
            Guard.NotNull(projector, nameof(projector));

            return Enumerable.Empty<TEntity>();
        }

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database and returns a collection of entities.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public override IEnumerable<TEntity> ExecuteSqlQuery<TEntity>(string sql, CommandType cmdType, Dictionary<string, object> parameters, Func<IDataReader, TEntity> projector)
        {
            if (!_ignoreSqlQueryWarning)
                throw new NotSupportedException(Repository.Properties.Resources.QueryExecutionNotSupported);

            Guard.NotEmpty(sql, nameof(sql));
            Guard.NotNull(projector, nameof(projector));

            return Enumerable.Empty<TEntity>();
        }

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <returns>The number of rows affected.</returns>
        public override int ExecuteSqlCommand(string sql, CommandType cmdType, Dictionary<string, object> parameters)
        {
            if (!_ignoreSqlQueryWarning)
                throw new NotSupportedException(Repository.Properties.Resources.QueryExecutionNotSupported);

            Guard.NotEmpty(sql, nameof(sql));

            return 0;
        }

        /// <summary>
        /// Finds an entity with the given primary key values in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity.</param>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The entity found in the repository.</returns>
        public override TEntity Find<TEntity>(IFetchQueryStrategy<TEntity> fetchStrategy, params object[] keyValues)
        {
            Guard.NotEmpty(keyValues, nameof(keyValues));

            if (fetchStrategy == null)
            {
                var entityType = typeof(TEntity);
                var store = InMemoryCache.Instance.GetDatabaseStore(DatabaseName);

                if (!store.ContainsKey(entityType))
                    return default(TEntity);

                var key = Combine(keyValues);

                store[entityType].TryGetValue(key, out object entity);

                var result = (TEntity)Convert.ChangeType(entity, entityType);

                return result;
            }

            return base.Find(fetchStrategy, keyValues);
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