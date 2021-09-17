namespace DotNetToolkit.Repository.InMemory.Internal
{
    using Configuration;
    using Configuration.Conventions;
    using Extensions;
    using Query.Strategies;
    using System;
    using System.Collections.Generic;
    using System.Data;
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

        private readonly InMemoryUnderlyingDbContext _underlyingContext;

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

            _underlyingContext = new InMemoryUnderlyingDbContext(databaseName, Conventions);
        }

        #endregion

        #region Implementation of IInMemoryRepositoryContext

        public string DatabaseName { get; set; }

        public void EnsureDeleted()
        {
            _underlyingContext.ClearDatabase();
        }

        #endregion

        #region Implementation of IRepositoryContext

        protected override IQueryable<TEntity> AsQueryable<TEntity>()
        {
            return _underlyingContext.FindAll<TEntity>().AsQueryable();
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
            _underlyingContext.Add(Guard.NotNull(entity, nameof(entity)));
        }

        public override void Update<TEntity>(TEntity entity)
        {
            _underlyingContext.Update(Guard.NotNull(entity, nameof(entity)));
        }

        public override void Remove<TEntity>(TEntity entity)
        {
            _underlyingContext.Remove(Guard.NotNull(entity, nameof(entity)));
        }

        public override int SaveChanges()
        {
            return _underlyingContext.SaveChanges();
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
                var result = _underlyingContext.Find<TEntity>(keyValues);

                return result;
            }

            return base.Find(fetchStrategy, keyValues);
        }

        #endregion

        #region Implementation of IDisposable

        public override void Dispose()
        {
            _underlyingContext.Dispose();
            base.Dispose();
        }

        #endregion
    }
}