namespace DotNetToolkit.Repository.EntityFramework
{
    using FetchStrategies;
    using Logging;
    using Queries;
    using Specifications;
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
    public abstract class EfRepositoryBase<TEntity, TKey> : RepositoryAsyncBase<TEntity, TKey> where TEntity : class
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
        protected EfRepositoryBase(DbContext context) : this (context, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfRepositoryBase{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger.</param>
        protected EfRepositoryBase(DbContext context, ILogger logger) : base(logger)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            Context = context;
            DbSet = Context.Set<TEntity>();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
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

        #endregion

        #region Overrides of RepositoryBase<TEntity, TKey>

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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Overrides of RepositoryAsyncBase<TEntity, TKey>

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
        /// A protected asynchronous overridable method for getting an entity that satisfies the criteria specified by the <paramref name="criteria" /> from the repository.
        /// </summary>
        protected override Task<TEntity> GetEntityAsync(ISpecification<TEntity> criteria, IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            return GetQuery(criteria, options).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// A protected asynchronous overridable method for getting an entity that satisfies the criteria specified by the <paramref name="criteria" /> from the repository.
        /// </summary>
        protected override Task<TResult> GetEntityAsync<TResult>(ISpecification<TEntity> criteria, IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return GetQuery(criteria, options).Select(selector).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// A protected asynchronous overridable method for getting a collection of entities that satisfies the criteria specified by the <paramref name="criteria" /> from the repository.
        /// </summary>
        protected override async Task<IEnumerable<TEntity>> GetEntitiesAsync(ISpecification<TEntity> criteria, IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return await GetQuery(criteria, options).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// A protected asynchronous overridable method for getting a collection of entities that satisfies the criteria specified by the <paramref name="criteria" /> from the repository.
        /// </summary>
        protected override async Task<IEnumerable<TResult>> GetEntitiesAsync<TResult>(ISpecification<TEntity> criteria, IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return await GetQuery(criteria, options).Select(selector).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// A protected asynchronous overridable method for getting a the number of entities that satisfies the criteria specified by the <paramref name="criteria" /> from the repository.
        /// </summary>
        protected override Task<int> GetCountAsync(ISpecification<TEntity> criteria, CancellationToken cancellationToken = new CancellationToken())
        {
            var predicate = criteria?.Predicate;

            return predicate == null ? GetQuery().CountAsync(cancellationToken) : GetQuery().CountAsync(predicate, cancellationToken);
        }

        /// <summary>
        /// A protected asynchronous overridable method for determining whether the repository contains an entity that satisfies the criteria specified by the <paramref name="criteria" /> from the repository.
        /// </summary>
        protected override Task<bool> GetExistAsync(ISpecification<TEntity> criteria, CancellationToken cancellationToken = new CancellationToken())
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            var predicate = criteria?.Predicate;

            return predicate == null ? GetQuery().AnyAsync(cancellationToken) : GetQuery().AnyAsync(predicate, cancellationToken);
        }

        /// <summary>
        /// A protected asynchronous overridable method for getting a new <see cref="IDictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, an element selector.
        /// </summary>
        protected override Task<Dictionary<TDictionaryKey, TElement>> GetDictionaryAsync<TDictionaryKey, TElement>(ISpecification<TEntity> criteria, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            var keySelectFunc = keySelector.Compile();
            var elementSelectorFunc = elementSelector.Compile();

            return GetQuery(criteria, options).ToDictionaryAsync(keySelectFunc, elementSelectorFunc, EqualityComparer<TDictionaryKey>.Default, cancellationToken);
        }

        /// <summary>
        /// A protected asynchronous overridable method for getting a new <see cref="IGrouping{TGroupKey, TElemen}" /> according to the specified <paramref name="keySelector" />, an element selector.
        /// </summary>
        protected override async Task<IEnumerable<IGrouping<TGroupKey, TElement>>> GetGroupByAsync<TGroupKey, TElement>(ISpecification<TEntity> criteria, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            return await GetQuery(criteria, options).GroupBy(keySelector, elementSelector, EqualityComparer<TGroupKey>.Default).ToListAsync(cancellationToken);
        }

        #endregion
    }
}
