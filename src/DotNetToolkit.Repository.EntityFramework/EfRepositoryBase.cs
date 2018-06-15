namespace DotNetToolkit.Repository.EntityFramework
{
    using FetchStrategies;
    using Interceptors;
    using Queries;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a repository for entity framework.
    /// </summary>
    public abstract class EfRepositoryBase<TEntity, TKey> : RepositoryBaseAsync<TEntity, TKey> where TEntity : class
    {
        #region Fields

        private bool _disposed;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the entity set.
        /// </summary>
        protected DbSet<TEntity> DbSet { get; private set; }

        /// <summary>
        /// Gets the database context.
        /// </summary>
        protected DbContext Context { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfRepositoryBase{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        protected EfRepositoryBase(DbContext context) : this(context, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfRepositoryBase{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="interceptors">The interceptors.</param>
        protected EfRepositoryBase(DbContext context, IEnumerable<IRepositoryInterceptor> interceptors) : base(interceptors)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            Context = context;
            DbSet = Context.Set<TEntity>();
        }

        #endregion

        #region Overrides of RepositoryBase<TEntity, TKey>

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (Context != null)
                {
                    Context.Dispose();
                }
            }

            _disposed = true;
        }

        /// <summary>
        /// A protected overridable method for adding the specified <paramref name="entity" /> into the repository.
        /// </summary>
        protected override void AddItem(TEntity entity)
        {
            DbSet.Add(entity);
        }

        /// <summary>
        /// A protected overridable method for deleting the specified <paramref name="entity" /> from the repository.
        /// </summary>
        protected override void DeleteItem(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        /// <summary>
        /// A protected overridable method for updating the specified <paramref name="entity" /> in the repository.
        /// </summary>
        protected override void UpdateItem(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// A protected overridable method for saving changes made in the current unit of work in the repository.
        /// </summary>
        protected override void SaveChanges()
        {
            Context.SaveChanges();
        }

        /// <summary>
        /// A protected overridable method for getting an entity query that supplies the specified fetching strategy from the repository.
        /// </summary>
        protected override IQueryable<TEntity> GetQuery(IFetchStrategy<TEntity> fetchStrategy = null)
        {
            var query = DbSet.AsQueryable();
            return fetchStrategy == null ? query : fetchStrategy.IncludePaths.Aggregate(query, (current, path) => current.Include(path));
        }

        /// <summary>
        /// Gets an entity query with the given primary key value from the repository.
        /// </summary>
        protected override TEntity GetEntity(TKey key, IFetchStrategy<TEntity> fetchStrategy)
        {
            return fetchStrategy == null ? DbSet.Find(key) : base.GetEntity(key, fetchStrategy);
        }

        #endregion

        #region Overrides of RepositoryBaseAsync<TEntity, TKey>

        /// <summary>
        /// A protected asynchronous overridable method for saving changes made in the current unit of work in the repository.
        /// </summary>
        protected override Task SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return Context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Gets an entity query with the given primary key value from the repository.
        /// </summary>
        protected override Task<TEntity> GetEntityAsync(TKey key, IFetchStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            return fetchStrategy == null ? DbSet.FindAsync(cancellationToken, key) : base.GetEntityAsync(key, fetchStrategy, cancellationToken);
        }

        /// <summary>
        /// A protected asynchronous overridable method for getting an entity that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected override Task<TResult> GetEntityAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return GetQuery(options).Select(selector).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// A protected asynchronous overridable method for getting a collection of entities that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected override async Task<IEnumerable<TResult>> GetEntitiesAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return await GetQuery(options).Select(selector).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// A protected asynchronous overridable method for getting a the number of entities that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected override Task<int> GetCountAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return GetQuery(options).CountAsync(cancellationToken);
        }

        /// <summary>
        /// A protected asynchronous overridable method for determining whether the repository contains an entity that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected override Task<bool> GetExistAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return GetQuery(options).AnyAsync(cancellationToken);
        }

        /// <summary>
        /// A protected asynchronous overridable method for getting a new <see cref="IDictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, an element selector.
        /// </summary>
        protected override Task<Dictionary<TDictionaryKey, TElement>> GetDictionaryAsync<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            var keySelectFunc = keySelector.Compile();
            var elementSelectorFunc = elementSelector.Compile();

            return GetQuery(options).ToDictionaryAsync(keySelectFunc, elementSelectorFunc, EqualityComparer<TDictionaryKey>.Default, cancellationToken);
        }

        /// <summary>
        /// A protected asynchronous overridable method for getting a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, an element selector.
        /// </summary>
        protected override async Task<IEnumerable<TResult>> GetGroupByAsync<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<IGrouping<TGroupKey, TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            return await GetQuery(options).GroupBy(keySelector, EqualityComparer<TGroupKey>.Default).Select(resultSelector).ToListAsync(cancellationToken);
        }

        #endregion
    }
}
