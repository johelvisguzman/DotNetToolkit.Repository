namespace DotNetToolkit.Repository.AdoNet
{
    using FetchStrategies;
    using Interceptors;
    using Queries;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a repository for ado.net.
    /// </summary>
    public abstract class AdoNetRepositoryBase<TEntity, TKey> : RepositoryBaseAsync<TEntity, TKey> where TEntity : class
    {
        #region Fields

        private bool _disposed;
        private AdoNetContext _context;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is identity.
        /// </summary>
        protected bool IsIdentity { get; private set; }

        /// <summary>
        /// Gets the name of the identity property.
        /// </summary>
        protected PropertyInfo SqlIdentityPropertyInfo { get; private set; }

        /// <summary>
        /// Gets the SQL properties.
        /// </summary>
        protected Dictionary<string, PropertyInfo> SqlPropertiesMapping { get; private set; }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        protected string TableName { get; private set; }

        /// <summary>
        /// Gets the current transaction.
        /// </summary>
        protected DbTransaction CurrentTransaction { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        protected AdoNetRepositoryBase(AdoNetContext context) : this(context, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="interceptors">The interceptors.</param>
        protected AdoNetRepositoryBase(AdoNetContext context, IEnumerable<IRepositoryInterceptor> interceptors) : base(interceptors)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _context = context;
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
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }

            _disposed = true;
        }

        /// <summary>
        /// A protected overridable method for adding the specified <paramref name="entity" /> into the repository.
        /// </summary>
        protected override void AddItem(TEntity entity)
        {
            _context.Add<TEntity>(entity);
        }

        /// <summary>
        /// A protected overridable method for deleting the specified <paramref name="entity" /> from the repository.
        /// </summary>
        protected override void DeleteItem(TEntity entity)
        {
            _context.Remove<TEntity>(entity);
        }

        /// <summary>
        /// A protected overridable method for updating the specified <paramref name="entity" /> in the repository.
        /// </summary>
        protected override void UpdateItem(TEntity entity)
        {
            _context.Update<TEntity>(entity);
        }

        /// <summary>
        /// A protected overridable method for saving changes made in the current unit of work in the repository.
        /// </summary>
        protected override void SaveChanges()
        {
            _context.SaveChanges();
        }

        /// <summary>
        /// A protected overridable method for getting an entity query that supplies the specified fetching strategy from the repository.
        /// </summary>
        protected override IQueryable<TEntity> GetQuery(IFetchStrategy<TEntity> fetchStrategy = null)
        {
            var options = fetchStrategy != null ? new QueryOptions<TEntity>().Fetch(fetchStrategy) : null;

            return _context.ExecuteList<TEntity>(options).AsQueryable();
        }

        /// <summary>
        /// Gets an entity query with the given primary key value from the repository.
        /// </summary>
        protected override TEntity GetEntity(TKey key, IFetchStrategy<TEntity> fetchStrategy)
        {
            var options = new QueryOptions<TEntity>().SatisfyBy(GetByPrimaryKeySpecification(key));

            if (fetchStrategy != null)
                options.Fetch(fetchStrategy);

            return _context.ExecuteObject<TEntity>(options);
        }

        /// <summary>
        /// Gets an entity that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected override TResult GetEntity<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var selectorFunc = selector.Compile();

            return _context.ExecuteObject<TEntity, TResult>(options, selectorFunc);
        }

        /// <summary>
        /// Goptionsollection of entities that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected override IEnumerable<TResult> GetEntities<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var selectorFunc = selector.Compile();

            return _context.ExecuteList<TEntity, TResult>(options, selectorFunc);
        }

        /// <summary>
        /// Gets the number of entities that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected override int GetCount(IQueryOptions<TEntity> options)
        {
            return _context.ExecuteCount<TEntity>(options);
        }

        /// <summary>
        /// A protected overridable method for determining whether the repository contains an entity that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected override bool GetExist(IQueryOptions<TEntity> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return _context.ExecuteExist<TEntity>(options);
        }

        /// <summary>
        /// Gets a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, an element selector.
        /// </summary>
        protected override Dictionary<TDictionaryKey, TElement> GetDictionary<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            var keySelectFunc = keySelector.Compile();
            var elementSelectFync = elementSelector.Compile();

            return _context.ExecuteDictionary(options, keySelectFunc, elementSelectFync);
        }

        /// <summary>
        /// Gets a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, an element selector.
        /// </summary>
        protected override IEnumerable<TResult> GetGroupBy<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<IGrouping<TGroupKey, TEntity>, TResult>> resultSelector)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            var keySelectFunc = keySelector.Compile();
            var resultSelectorFunc = resultSelector.Compile();

            return _context.ExecuteList(options)
                .GroupBy(keySelectFunc, EqualityComparer<TGroupKey>.Default)
                .Select(resultSelectorFunc)
                .ToList();
        }

        #endregion

        #region Overrides of RepositoryBaseAsync<TEntity, TKey>

        /// <summary>
        /// A protected asynchronous overridable method for saving changes made in the current unit of work in the repository.
        /// </summary>
        protected override Task SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Gets an entity query with the given primary key value from the repository.
        /// </summary>
        protected override Task<TEntity> GetEntityAsync(TKey key, IFetchStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            var options = new QueryOptions<TEntity>().SatisfyBy(GetByPrimaryKeySpecification(key));

            if (fetchStrategy != null)
                options.Fetch(fetchStrategy);

            return _context.ExecuteObjectAsync<TEntity>(options, cancellationToken);
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

            var selectorFunc = selector.Compile();

            return _context.ExecuteObjectAsync(options, selectorFunc, cancellationToken);
        }

        /// <summary>
        /// A protected asynchronous overridable method for getting a collection of entities that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected override Task<IEnumerable<TResult>> GetEntitiesAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var selectorFunc = selector.Compile();

            return _context.ExecuteListAsync(options, selectorFunc, cancellationToken);
        }

        /// <summary>
        /// A protected asynchronous overridable method for getting a the number of entities that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected override Task<int> GetCountAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _context.ExecuteCountAsync(options, cancellationToken);
        }

        /// <summary>
        /// A protected asynchronous overridable method for determining whether the repository contains an entity that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected override Task<bool> GetExistAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return _context.ExecuteExistAsync(options, cancellationToken);
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
            var elementSelectFync = elementSelector.Compile();

            return _context.ExecuteDictionaryAsync(options, keySelectFunc, elementSelectFync, cancellationToken);
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

            var keySelectFunc = keySelector.Compile();
            var resultSelectorFunc = resultSelector.Compile();

            return (await _context.ExecuteListAsync(options, cancellationToken))
                .GroupBy(keySelectFunc, EqualityComparer<TGroupKey>.Default)
                .Select(resultSelectorFunc)
                .ToList();
        }

        #endregion

        #region Nested type: EntitySet

        /// <summary>
        /// Represents an internal entity set in the in-memory store, which holds the entity and it's state representing the operation that was performed at the time.
        /// </summary>
        private class EntitySet
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="EntitySet"/> class.
            /// </summary>
            /// <param name="entity">The entity.</param>
            /// <param name="state">The state.</param>
            public EntitySet(TEntity entity, EntityState state)
            {
                Entity = entity;
                State = state;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the entity.
            /// </summary>
            public TEntity Entity { get; }

            /// <summary>
            /// Gets the state.
            /// </summary>
            public EntityState State { get; }

            #endregion
        }

        #endregion

        #region Nested type: EntityState

        /// <summary>
        /// Represents an internal state for an entity in the in-memory store.
        /// </summary>
        private enum EntityState
        {
            Added,
            Removed,
            Modified
        }

        #endregion
    }
}
