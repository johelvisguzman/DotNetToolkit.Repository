namespace DotNetToolkit.Repository.Internal
{
    using Configuration;
    using Configuration.Logging;
    using Queries;
    using Queries.Strategies;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Transactions;

    /// <summary>
    /// Represents an internal asynchronous repository context wrapper.
    /// </summary>
    /// <seealso cref="IRepositoryContextAsync" />
    [ComVisible(false)]
    internal class RepositoryContextAsyncWrapper : IRepositoryContextAsync
    {
        #region Fields

        private readonly IRepositoryContext _context;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryContextAsyncWrapper"/> class.
        /// </summary>
        /// <param name="context">The underlying repository context.</param>
        public RepositoryContextAsyncWrapper(IRepositoryContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _context = context;
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

        /// <inheritdoc />
        public ITransactionManager BeginTransaction()
        {
            return _context.BeginTransaction();
        }

        /// <inheritdoc />
        public void UseLoggerProvider(ILoggerProvider loggerProvider)
        {
            _context.UseLoggerProvider(loggerProvider);
        }

        /// <inheritdoc />
        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            _context.Add<TEntity>(entity);
        }

        /// <inheritdoc />
        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            _context.Update<TEntity>(entity);
        }

        /// <inheritdoc />
        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            _context.Remove<TEntity>(entity);
        }

        /// <inheritdoc />
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        /// <inheritdoc />
        public QueryResult<TEntity> Find<TEntity>(IFetchQueryStrategy<TEntity> fetchStrategy, params object[] keyValues) where TEntity : class
        {
            return _context.Find<TEntity>(fetchStrategy, keyValues);
        }

        /// <inheritdoc />
        public QueryResult<TResult> Find<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            return _context.Find<TEntity, TResult>(options, selector);
        }

        /// <inheritdoc />
        public QueryResult<IEnumerable<TResult>> FindAll<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            return _context.FindAll<TEntity, TResult>(options, selector);
        }

        /// <inheritdoc />
        public QueryResult<int> Count<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            return _context.Count<TEntity>(options);
        }

        /// <inheritdoc />
        public QueryResult<bool> Exists<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            return _context.Exists<TEntity>(options);
        }

        /// <inheritdoc />
        public QueryResult<Dictionary<TDictionaryKey, TElement>> ToDictionary<TEntity, TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector) where TEntity : class
        {
            return _context.ToDictionary<TEntity, TDictionaryKey, TElement>(options, keySelector, elementSelector);
        }

        /// <inheritdoc />
        public QueryResult<IEnumerable<TResult>> GroupBy<TEntity, TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector) where TEntity : class
        {
            return _context.GroupBy<TEntity, TGroupKey, TResult>(options, keySelector, resultSelector);
        }

        #endregion

        #region Implementation of IRepositoryContextAsync

        /// <inheritdoc />
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            if (_context is IRepositoryContextAsync contextAsync)
            {
                return contextAsync.SaveChangesAsync(cancellationToken);
            }

            return RunAsync<int>(() => SaveChanges(), cancellationToken);
        }

        /// <inheritdoc />
        public Task<QueryResult<TEntity>> FindAsync<TEntity>(CancellationToken cancellationToken, IFetchQueryStrategy<TEntity> fetchStrategy, params object[] keyValues) where TEntity : class
        {
            if (_context is IRepositoryContextAsync contextAsync)
            {
                return contextAsync.FindAsync(cancellationToken, fetchStrategy, keyValues);
            }

            return RunAsync<QueryResult<TEntity>>(() => Find(fetchStrategy, keyValues), cancellationToken);
        }

        /// <inheritdoc />
        public Task<QueryResult<TResult>> FindAsync<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (_context is IRepositoryContextAsync contextAsync)
            {
                return contextAsync.FindAsync<TEntity, TResult>(options, selector, cancellationToken);
            }

            return RunAsync<QueryResult<TResult>>(() => Find<TEntity, TResult>(options, selector), cancellationToken);
        }

        /// <inheritdoc />
        public Task<QueryResult<IEnumerable<TResult>>> FindAllAsync<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (_context is IRepositoryContextAsync contextAsync)
            {
                return contextAsync.FindAllAsync<TEntity, TResult>(options, selector, cancellationToken);
            }

            return RunAsync<QueryResult<IEnumerable<TResult>>>(() => FindAll<TEntity, TResult>(options, selector), cancellationToken);
        }

        /// <inheritdoc />
        public Task<QueryResult<int>> CountAsync<TEntity>(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (_context is IRepositoryContextAsync contextAsync)
            {
                return contextAsync.CountAsync<TEntity>(options, cancellationToken);
            }

            return RunAsync<QueryResult<int>>(() => Count<TEntity>(options), cancellationToken);
        }

        /// <inheritdoc />
        public Task<QueryResult<bool>> ExistsAsync<TEntity>(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (_context is IRepositoryContextAsync contextAsync)
            {
                return contextAsync.ExistsAsync<TEntity>(options, cancellationToken);
            }

            return RunAsync<QueryResult<bool>>(() => Exists<TEntity>(options), cancellationToken);
        }

        /// <inheritdoc />
        public Task<QueryResult<Dictionary<TDictionaryKey, TElement>>> ToDictionaryAsync<TEntity, TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (_context is IRepositoryContextAsync contextAsync)
            {
                return contextAsync.ToDictionaryAsync<TEntity, TDictionaryKey, TElement>(options, keySelector, elementSelector, cancellationToken);
            }

            return RunAsync<QueryResult<Dictionary<TDictionaryKey, TElement>>>(() => ToDictionary<TEntity, TDictionaryKey, TElement>(options, keySelector, elementSelector), cancellationToken);
        }

        /// <inheritdoc />
        public Task<QueryResult<IEnumerable<TResult>>> GroupByAsync<TEntity, TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (_context is IRepositoryContextAsync contextAsync)
            {
                return contextAsync.GroupByAsync<TEntity, TGroupKey, TResult>(options, keySelector, resultSelector, cancellationToken);
            }

            return RunAsync<QueryResult<IEnumerable<TResult>>>(() => GroupBy<TEntity, TGroupKey, TResult>(options, keySelector, resultSelector), cancellationToken);
        }

        #endregion

        #region Implementation of IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            _context.Dispose();
        }

        #endregion
    }
}
