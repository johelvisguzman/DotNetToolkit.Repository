namespace DotNetToolkit.Repository.InMemory.Internal
{
    using Configuration;
    using Configuration.Conventions;
    using Extensions;
    using InMemory.Properties;
    using Query.Strategies;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using Transactions;
    using Transactions.Internal;
    using Utility;

    internal class InMemoryRepositoryContext : LinqEnumerableRepositoryContextBase, IInMemoryRepositoryContext
    {
        #region Fields

        private const string DefaultDatabaseName = "DotNetToolkit.Repository.InMemory";

        private readonly bool _ignoreTransactionWarning;
        private readonly bool _ignoreSqlQueryWarning;

        private readonly InMemoryDatabase _db;

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

            _db = InMemoryDatabasesCache.Instance.GetDatabase(databaseName);
        }

        #endregion

        #region Overrides of LinqEnumerableRepositoryContextBase

        protected override IEnumerable<TEntity> AsEnumerable<TEntity>(IFetchQueryStrategy<TEntity> fetchStrategy)
        {
            return _db
                .FindAll<TEntity>()
                .ApplyFetchingOptions(fetchStrategy, _db.FindAll);
        }

        #endregion

        #region Implementation of IInMemoryRepositoryContext

        public string DatabaseName { get; set; }

        public void EnsureDeleted()
        {
            _db.Clear();
        }

        #endregion

        #region Implementation of IRepositoryContext

        public override ITransactionManager BeginTransaction()
        {
            if (!_ignoreTransactionWarning)
                throw new NotSupportedException(DotNetToolkit.Repository.Properties.Resources.TransactionNotSupported);

            CurrentTransaction = NullTransactionManager.Instance;

            return CurrentTransaction;
        }

        public override void Add<TEntity>(TEntity entity)
        {
            Guard.NotNull(entity, nameof(entity));

            var entityType = typeof(TEntity);
            var keyValues = Conventions.GetPrimaryKeyValues(entity);

            if (_db.TryFind<TEntity>(keyValues, out object _))
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.EntityAlreadyBeingTrackedInStore,
                        entityType));
            }

            if (TryGeneratePrimaryKey<TEntity>(entity, out var newKey))
            {
                // assumes we only have a single key since
                // we cannot generated for a composite key anyways
                keyValues[0] = newKey;
            }

            _db.AddOrUpdate<TEntity>(entity, keyValues);
        }

        public override void Update<TEntity>(TEntity entity)
        {
            Guard.NotNull(entity, nameof(entity));

            var keyValues = Conventions.GetPrimaryKeyValues(entity);

            if (!_db.TryFind<TEntity>(keyValues, out object _))
            {
                throw new InvalidOperationException(Resources.EntityNotFoundInStore);
            }

            _db.AddOrUpdate<TEntity>(entity, keyValues);
        }

        public override void Remove<TEntity>(TEntity entity)
        {
            Guard.NotNull(entity, nameof(entity));

            var keyValues = Conventions.GetPrimaryKeyValues(entity);

            if (!_db.TryFind<TEntity>(keyValues, out object _))
            {
                throw new InvalidOperationException(Resources.EntityNotFoundInStore);
            }

            _db.Remove<TEntity>(keyValues);
        }

        public override int SaveChanges()
        {
            return -1;
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
                if (_db.TryFind<TEntity>(keyValues, out object entity))
                {
                    return (TEntity)Convert.ChangeType(entity, typeof(TEntity));
                }

                return default(TEntity);
            }

            return base.Find(fetchStrategy, keyValues);
        }

        #endregion

        #region Implementation of IDisposable

        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion
    }
}