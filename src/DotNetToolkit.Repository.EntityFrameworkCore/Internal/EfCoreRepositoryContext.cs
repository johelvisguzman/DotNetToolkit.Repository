namespace DotNetToolkit.Repository.EntityFrameworkCore.Internal
{
    using Configuration;
    using Configuration.Conventions;
    using Configuration.Logging;
    using Extensions;
    using Helpers;
    using Microsoft.EntityFrameworkCore;
    using Queries;
    using Queries.Strategies;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Transactions;

    /// <summary>
    /// Represents an internal entity framework repository context.
    /// </summary>
    /// <seealso cref="IRepositoryContextAsync" />
    internal class EfCoreRepositoryContext : IRepositoryContextAsync
    {
        #region Fields

        private readonly DbContext _context;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the repository context logger.
        /// </summary>
        public ILogger Logger { get; private set; } = NullLogger.Instance;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreRepositoryContext" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public EfCoreRepositoryContext(DbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _context = context;
            _context.ConfigureLogging(s => Logger.Debug(s.TrimEnd(Environment.NewLine.ToCharArray())));
        }

        #endregion

        #region Private Methods

        private void ThrowsIfEntityPrimaryKeyValuesLengthMismatch<TEntity>(object[] keyValues) where TEntity : class
        {
            if (keyValues.Length != PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos<TEntity>().Count())
                throw new ArgumentException(DotNetToolkit.Repository.Properties.Resources.EntityPrimaryKeyValuesLengthMismatch, nameof(keyValues));
        }

        #endregion

        #region Implementation of IRepositoryContext

        /// <inheritdoc />
        public ITransactionManager BeginTransaction()
        {
            return new EfCoreTransactionManager(_context.Database.BeginTransaction());
        }

        /// <inheritdoc />
        public void UseLoggerProvider(ILoggerProvider loggerProvider)
        {
            if (loggerProvider == null)
                throw new ArgumentNullException(nameof(loggerProvider));

            Logger = loggerProvider.Create(typeof(DbContext).FullName);
        }

        /// <inheritdoc />
        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Set<TEntity>().Add(entity);
        }

        /// <inheritdoc />
        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entry = _context.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                var keyValues = PrimaryKeyConventionHelper.GetPrimaryKeyValues(entity);

                var entityInDb = _context.Set<TEntity>().Find(keyValues);

                if (entityInDb != null)
                {
                    _context.Entry(entityInDb).CurrentValues.SetValues(entity);
                }
            }
            else
            {
                entry.State = EntityState.Modified;
            }
        }

        /// <inheritdoc />
        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (_context.Entry(entity).State == EntityState.Detached)
            {
                var keyValues = PrimaryKeyConventionHelper.GetPrimaryKeyValues(entity);

                var entityInDb = _context.Set<TEntity>().Find(keyValues);

                if (entityInDb != null)
                {
                    _context.Set<TEntity>().Remove(entityInDb);
                }
            }
            else
            {
                _context.Set<TEntity>().Remove(entity);
            }
        }

        /// <inheritdoc />
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        /// <inheritdoc />
        public QueryResult<TEntity> Find<TEntity>(IFetchQueryStrategy<TEntity> fetchStrategy, params object[] keyValues) where TEntity : class
        {
            if (keyValues == null)
                throw new ArgumentNullException(nameof(keyValues));

            ThrowsIfEntityPrimaryKeyValuesLengthMismatch<TEntity>(keyValues);

            if (fetchStrategy == null)
            {
                var result = _context.Set<TEntity>().Find(keyValues);

                return new QueryResult<TEntity>(result);
            }

            var options = new QueryOptions<TEntity>()
                .Include(PrimaryKeyConventionHelper.GetByPrimaryKeySpecification<TEntity>(keyValues))
                .Include(fetchStrategy);

            return Find<TEntity, TEntity>(options, IdentityExpression<TEntity>.Instance);
        }

        /// <inheritdoc />
        public QueryResult<TResult> Find<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var result = _context.Set<TEntity>()
                .AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplyFetchingOptions(options)
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

            var query = _context.Set<TEntity>()
                .AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplyFetchingOptions(options)
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
        public QueryResult<int> Count<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            var result = _context.Set<TEntity>()
                .AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplyFetchingOptions(options)
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

            var result = _context.Set<TEntity>()
                .AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplyFetchingOptions(options)
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

            var query = _context.Set<TEntity>()
                .AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplyFetchingOptions(options)
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

            var query = _context.Set<TEntity>()
                .AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplyFetchingOptions(options)
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

        #region Implementation of IRepositoryContextAsync

        /// <inheritdoc />
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<QueryResult<TEntity>> FindAsync<TEntity>(CancellationToken cancellationToken, IFetchQueryStrategy<TEntity> fetchStrategy, params object[] keyValues) where TEntity : class
        {
            if (keyValues == null)
                throw new ArgumentNullException(nameof(keyValues));

            ThrowsIfEntityPrimaryKeyValuesLengthMismatch<TEntity>(keyValues);

            if (fetchStrategy == null)
            {
                var result = await _context.Set<TEntity>().FindAsync(keyValues, cancellationToken);

                return new QueryResult<TEntity>(result);
            }

            var options = new QueryOptions<TEntity>()
                .Include(PrimaryKeyConventionHelper.GetByPrimaryKeySpecification<TEntity>(keyValues))
                .Include(fetchStrategy);

            return await FindAsync<TEntity, TEntity>(options, IdentityExpression<TEntity>.Instance, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<QueryResult<TResult>> FindAsync<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var result = await _context.Set<TEntity>()
                .AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplyFetchingOptions(options)
                .ApplySortingOptions(options)
                .ApplyPagingOptions(options)
                .Select(selector)
                .FirstOrDefaultAsync(cancellationToken);

            return new QueryResult<TResult>(result);
        }

        /// <inheritdoc />
        public async Task<QueryResult<IEnumerable<TResult>>> FindAllAsync<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var query = _context.Set<TEntity>()
                .AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplyFetchingOptions(options)
                .ApplySortingOptions(options);

            var data = await query
                .ApplyPagingOptions(options)
                .Select(selector)
                .Select(x => new
                {
                    Result = x,
                    Total = query.Count()
                })
                .ToListAsync(cancellationToken);

            var result = data.Select(x => x.Result);
            var total = data.FirstOrDefault()?.Total ?? 0;

            return new QueryResult<IEnumerable<TResult>>(result, total);
        }

        /// <inheritdoc />
        public async Task<QueryResult<int>> CountAsync<TEntity>(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            var result = await _context.Set<TEntity>()
                .AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplyFetchingOptions(options)
                .ApplySortingOptions(options)
                .ApplyPagingOptions(options)
                .CountAsync(cancellationToken);

            return new QueryResult<int>(result);
        }

        /// <inheritdoc />
        public async Task<QueryResult<bool>> ExistsAsync<TEntity>(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var result = await _context.Set<TEntity>()
                .AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplyFetchingOptions(options)
                .ApplySortingOptions(options)
                .ApplyPagingOptions(options)
                .AnyAsync(cancellationToken);

            return new QueryResult<bool>(result);
        }

        /// <inheritdoc />
        public async Task<QueryResult<Dictionary<TDictionaryKey, TElement>>> ToDictionaryAsync<TEntity, TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            var keySelectFunc = keySelector.Compile();
            var elementSelectorFunc = elementSelector.Compile();

            var query = _context.Set<TEntity>()
                .AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplyFetchingOptions(options)
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

                result = await data.Select(x => x.Result).ToDictionaryAsync(keySelectFunc, elementSelectorFunc, cancellationToken);
                total = (await data.FirstOrDefaultAsync(cancellationToken))?.Total ?? 0;
            }
            else
            {
                // Gets the total count from memory
                result = await query.ToDictionaryAsync(keySelectFunc, elementSelectorFunc, cancellationToken);
                total = result.Count;
            }

            return new QueryResult<Dictionary<TDictionaryKey, TElement>>(result, total);
        }

        /// <inheritdoc />
        public Task<QueryResult<IEnumerable<TResult>>> GroupByAsync<TEntity, TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            return Task.FromResult<QueryResult<IEnumerable<TResult>>>(GroupBy<TEntity, TGroupKey, TResult>(options, keySelector, resultSelector));
        }

        #endregion

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            _context.Dispose();
        }

        #endregion

    }
}