namespace DotNetToolkit.Repository.Internal
{
    using Configuration;
    using Configuration.Conventions;
    using Configuration.Logging;
    using JetBrains.Annotations;
    using Queries;
    using Queries.Strategies;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq.Expressions;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Transactions;
    using Utility;

    /// <summary>
    /// Represents an internal asynchronous repository context wrapper.
    /// </summary>
    /// <seealso cref="IRepositoryContextAsync" />
    [ComVisible(false)]
    internal class RepositoryContextAsyncWrapper : IRepositoryContextAsync
    {
        #region Fields

        private readonly IRepositoryContext _underlyingContext;

        #endregion

        #region Constructors

        public RepositoryContextAsyncWrapper([NotNull] IRepositoryContext context)
        {
            _underlyingContext = Guard.NotNull(context, nameof(context));
        }

        #endregion

        #region Private Methods

        private Task<TResult> RunAsync<TResult>(Func<TResult> function, CancellationToken cancellationToken)
        {
            if (function == null)
                throw new ArgumentNullException(nameof(function));

            return Task.FromResult<TResult>(function());
        }

        #endregion

        #region Implementation of IRepositoryContext

        public IEnumerable<TEntity> ExecuteSqlQuery<TEntity>(string sql, CommandType cmdType, Dictionary<string, object> parameters, Func<IDataReader, IRepositoryConventions, TEntity> projector) where TEntity : class
        {
            return _underlyingContext.ExecuteSqlQuery(sql, cmdType, parameters, projector);
        }

        public IEnumerable<TEntity> ExecuteSqlQuery<TEntity>(string sql, CommandType cmdType, Dictionary<string, object> parameters, Func<IDataReader, TEntity> projector) where TEntity : class
        {
            return _underlyingContext.ExecuteSqlQuery(sql, cmdType, parameters, projector);
        }

        public int ExecuteSqlCommand(string sql, CommandType cmdType, Dictionary<string, object> parameters)
        {
            return _underlyingContext.ExecuteSqlCommand(sql, cmdType, parameters);
        }

        public ITransactionManager BeginTransaction()
        {
            return _underlyingContext.BeginTransaction();
        }

        public ITransactionManager CurrentTransaction { get { return _underlyingContext.CurrentTransaction; } }

        public IRepositoryConventions Conventions
        {
            get { return _underlyingContext.Conventions; }
            set { _underlyingContext.Conventions = value; }
        }

        public ILogger Logger
        {
            get { return _underlyingContext.Logger; }
            set { _underlyingContext.Logger = value; }
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            _underlyingContext.Add<TEntity>(entity);
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            _underlyingContext.Update<TEntity>(entity);
        }

        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            _underlyingContext.Remove<TEntity>(entity);
        }

        public int SaveChanges()
        {
            return _underlyingContext.SaveChanges();
        }

        public TEntity Find<TEntity>(IFetchQueryStrategy<TEntity> fetchStrategy, params object[] keyValues) where TEntity : class
        {
            return _underlyingContext.Find<TEntity>(fetchStrategy, keyValues);
        }

        public TResult Find<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            return _underlyingContext.Find<TEntity, TResult>(options, selector);
        }

        public IPagedQueryResult<IEnumerable<TResult>> FindAll<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            return _underlyingContext.FindAll<TEntity, TResult>(options, selector);
        }

        public int Count<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            return _underlyingContext.Count<TEntity>(options);
        }

        public bool Exists<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            return _underlyingContext.Exists<TEntity>(options);
        }

        public IPagedQueryResult<Dictionary<TDictionaryKey, TElement>> ToDictionary<TEntity, TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector) where TEntity : class
        {
            return _underlyingContext.ToDictionary<TEntity, TDictionaryKey, TElement>(options, keySelector, elementSelector);
        }

        public IPagedQueryResult<IEnumerable<TResult>> GroupBy<TEntity, TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector) where TEntity : class
        {
            return _underlyingContext.GroupBy<TEntity, TGroupKey, TResult>(options, keySelector, resultSelector);
        }

        #endregion

        #region Implementation of IRepositoryContextAsync

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync<TEntity>(string sql, CommandType cmdType, Dictionary<string, object> parameters, Func<IDataReader, IRepositoryConventions, TEntity> projector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (_underlyingContext is IRepositoryContextAsync contextAsync)
            {
                return contextAsync.ExecuteSqlQueryAsync(sql, cmdType, parameters, projector, cancellationToken);
            }

            return RunAsync<IEnumerable<TEntity>>(() => ExecuteSqlQuery(sql, cmdType, parameters, projector), cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync<TEntity>(string sql, CommandType cmdType, Dictionary<string, object> parameters, Func<IDataReader, TEntity> projector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (_underlyingContext is IRepositoryContextAsync contextAsync)
            {
                return contextAsync.ExecuteSqlQueryAsync(sql, cmdType, parameters, projector, cancellationToken);
            }

            return RunAsync<IEnumerable<TEntity>>(() => ExecuteSqlQuery(sql, cmdType, parameters, projector), cancellationToken);
        }

        public Task<int> ExecuteSqlCommandAsync(string sql, CommandType cmdType, Dictionary<string, object> parameters, CancellationToken cancellationToken = new CancellationToken())
        {
            if (_underlyingContext is IRepositoryContextAsync contextAsync)
            {
                return contextAsync.ExecuteSqlCommandAsync(sql, cmdType, parameters, cancellationToken);
            }

            return RunAsync<int>(() => ExecuteSqlCommand(sql, cmdType, parameters), cancellationToken);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            if (_underlyingContext is IRepositoryContextAsync contextAsync)
            {
                return contextAsync.SaveChangesAsync(cancellationToken);
            }

            return RunAsync<int>(() => SaveChanges(), cancellationToken);
        }

        public Task<TEntity> FindAsync<TEntity>(CancellationToken cancellationToken, IFetchQueryStrategy<TEntity> fetchStrategy, params object[] keyValues) where TEntity : class
        {
            if (_underlyingContext is IRepositoryContextAsync contextAsync)
            {
                return contextAsync.FindAsync(cancellationToken, fetchStrategy, keyValues);
            }

            return RunAsync<TEntity>(() => Find(fetchStrategy, keyValues), cancellationToken);
        }

        public Task<TResult> FindAsync<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (_underlyingContext is IRepositoryContextAsync contextAsync)
            {
                return contextAsync.FindAsync<TEntity, TResult>(options, selector, cancellationToken);
            }

            return RunAsync<TResult>(() => Find<TEntity, TResult>(options, selector), cancellationToken);
        }

        public Task<IPagedQueryResult<IEnumerable<TResult>>> FindAllAsync<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (_underlyingContext is IRepositoryContextAsync contextAsync)
            {
                return contextAsync.FindAllAsync<TEntity, TResult>(options, selector, cancellationToken);
            }

            return RunAsync<IPagedQueryResult<IEnumerable<TResult>>>(() => FindAll<TEntity, TResult>(options, selector), cancellationToken);
        }

        public Task<int> CountAsync<TEntity>(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (_underlyingContext is IRepositoryContextAsync contextAsync)
            {
                return contextAsync.CountAsync<TEntity>(options, cancellationToken);
            }

            return RunAsync<int>(() => Count<TEntity>(options), cancellationToken);
        }

        public Task<bool> ExistsAsync<TEntity>(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (_underlyingContext is IRepositoryContextAsync contextAsync)
            {
                return contextAsync.ExistsAsync<TEntity>(options, cancellationToken);
            }

            return RunAsync<bool>(() => Exists<TEntity>(options), cancellationToken);
        }

        public Task<IPagedQueryResult<Dictionary<TDictionaryKey, TElement>>> ToDictionaryAsync<TEntity, TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (_underlyingContext is IRepositoryContextAsync contextAsync)
            {
                return contextAsync.ToDictionaryAsync<TEntity, TDictionaryKey, TElement>(options, keySelector, elementSelector, cancellationToken);
            }

            return RunAsync<IPagedQueryResult<Dictionary<TDictionaryKey, TElement>>>(() => ToDictionary<TEntity, TDictionaryKey, TElement>(options, keySelector, elementSelector), cancellationToken);
        }

        public Task<IPagedQueryResult<IEnumerable<TResult>>> GroupByAsync<TEntity, TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (_underlyingContext is IRepositoryContextAsync contextAsync)
            {
                return contextAsync.GroupByAsync<TEntity, TGroupKey, TResult>(options, keySelector, resultSelector, cancellationToken);
            }

            return RunAsync<IPagedQueryResult<IEnumerable<TResult>>>(() => GroupBy<TEntity, TGroupKey, TResult>(options, keySelector, resultSelector), cancellationToken);
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            _underlyingContext.Dispose();
        }

        #endregion
    }
}
