namespace DotNetToolkit.Repository.InMemory.Internal
{
    using Configuration;
    using Configuration.Conventions;
    using Configuration.Conventions.Internal;
    using Extensions;
    using Properties;
    using Query.Strategies;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using Transactions;
    using Transactions.Internal;
    using Utility;

    internal class InMemoryRepositoryContext : LinqRepositoryContextBase, IInMemoryRepositoryContext
    {
        #region Fields

        private const string DefaultDatabaseName = "DotNetToolkit.Repository.InMemory";

        private readonly bool _ignoreTransactionWarning;
        private readonly bool _ignoreSqlQueryWarning;
        private readonly BlockingCollection<EntitySet> _items = new BlockingCollection<EntitySet>();

        #endregion

        #region Constructors

        public InMemoryRepositoryContext(bool ignoreTransactionWarning = false, bool ignoreSqlQueryWarning = false) 
            : this(DefaultDatabaseName, ignoreTransactionWarning, ignoreSqlQueryWarning)
        { }

        public InMemoryRepositoryContext(string databaseName, bool ignoreTransactionWarning = false, bool ignoreSqlQueryWarning = false)
        {
            DatabaseName = string.IsNullOrEmpty(databaseName) ? DefaultDatabaseName : databaseName;
            Conventions = RepositoryConventions.Default();

            _ignoreTransactionWarning = ignoreTransactionWarning;
            _ignoreSqlQueryWarning = ignoreSqlQueryWarning;
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

        #region Implementation of IInMemoryRepositoryContext

        public string DatabaseName { get; set; }

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

        #region Implementation of IRepositoryContext

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

        public override ITransactionManager BeginTransaction()
        {
            if (!_ignoreTransactionWarning)
                throw new NotSupportedException(DotNetToolkit.Repository.Properties.Resources.TransactionNotSupported);

            CurrentTransaction = NullTransactionManager.Instance;

            return CurrentTransaction;
        }

        public override void Add<TEntity>(TEntity entity)
        {
            _items.Add(new EntitySet(Guard.NotNull(entity, nameof(entity)), EntityState.Added));
        }

        public override void Update<TEntity>(TEntity entity)
        {
            _items.Add(new EntitySet(Guard.NotNull(entity, nameof(entity)), EntityState.Modified));
        }

        public override void Remove<TEntity>(TEntity entity)
        {
            _items.Add(new EntitySet(Guard.NotNull(entity, nameof(entity)), EntityState.Removed));
        }

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

                        if (ModelConventionHelper.IsColumnIdentity(Conventions, primaryKeyPropertyInfo))
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

        public override IEnumerable<TEntity> ExecuteSqlQuery<TEntity>(string sql, CommandType cmdType, Dictionary<string, object> parameters, Func<IDataReader, TEntity> projector)
        {
            if (!_ignoreSqlQueryWarning)
                throw new NotSupportedException(Repository.Properties.Resources.QueryExecutionNotSupported);

            Guard.NotEmpty(sql, nameof(sql));
            Guard.NotNull(projector, nameof(projector));

            return Enumerable.Empty<TEntity>();
        }

        public override int ExecuteSqlCommand(string sql, CommandType cmdType, Dictionary<string, object> parameters)
        {
            if (!_ignoreSqlQueryWarning)
                throw new NotSupportedException(Repository.Properties.Resources.QueryExecutionNotSupported);

            Guard.NotEmpty(sql, nameof(sql));

            return 0;
        }

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

        #region Implementation of IDisposable

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

        #region Nested Type: EntitySet

        class EntitySet
        {
            public EntitySet(object entity, EntityState state)
            {
                Entity = entity;
                State = state;
            }

            public object Entity { get; }

            public EntityState State { get; }
        }

        #endregion

        #region Nested Type: EntityState

        enum EntityState
        {
            Added,
            Removed,
            Modified
        }

        #endregion
    }
}