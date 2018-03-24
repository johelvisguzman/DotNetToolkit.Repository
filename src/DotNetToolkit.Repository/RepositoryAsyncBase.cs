namespace DotNetToolkit.Repository
{
    using FetchStrategies;
    using Properties;
    using Queries;
    using Specifications;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// An implementation of <see cref="IRepositoryAsync{TEntity, TKey}" />.
    /// </summary>
    public abstract class RepositoryAsyncBase<TEntity, TKey> : RepositoryBase<TEntity, TKey>, IRepositoryAsync<TEntity, TKey> where TEntity : class
    {
        #region Protected Methods

        /// <summary>
        /// A protected asynchronous overridable method for saving changes made in the current unit of work in the repository.
        /// </summary>
        protected abstract Task SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously gets an entity query with the given primary key value from the repository.
        /// </summary>
        protected virtual Task<TEntity> GetEntityAsync(TKey key, IFetchStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            return GetEntityAsync(GetByPrimaryKeySpecification(key, fetchStrategy), (IQueryOptions<TEntity>)null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously gets an entity query with the given primary key value from the repository.
        /// </summary>
        protected virtual Task<TResult> GetEntityAsync<TResult>(TKey key, IFetchStrategy<TEntity> fetchStrategy, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return GetEntityAsync(GetByPrimaryKeySpecification(key, fetchStrategy), (IQueryOptions<TEntity>)null, selector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously gets an entity query with the given primary key value from the repository.
        /// </summary>
        protected virtual Task<TEntity> GetEntityAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            return GetAsync(key, (IFetchStrategy<TEntity>)null, cancellationToken);
        }

        /// <summary>
        /// A protected asynchronous overridable method for getting an entity that satisfies the criteria specified by the <paramref name="criteria" /> from the repository.
        /// </summary>
        protected abstract Task<TEntity> GetEntityAsync(ISpecification<TEntity> criteria, IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// A protected asynchronous overridable method for getting an entity that satisfies the criteria specified by the <paramref name="criteria" /> from the repository.
        /// </summary>
        protected abstract Task<TResult> GetEntityAsync<TResult>(ISpecification<TEntity> criteria, IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// A protected asynchronous overridable method for getting a collection of entities that satisfies the criteria specified by the <paramref name="criteria" /> from the repository.
        /// </summary>
        protected abstract Task<List<TEntity>> GetEntitiesAsync(ISpecification<TEntity> criteria, IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// A protected asynchronous overridable method for getting a collection of entities that satisfies the criteria specified by the <paramref name="criteria" /> from the repository.
        /// </summary>
        protected abstract Task<List<TResult>> GetEntitiesAsync<TResult>(ISpecification<TEntity> criteria, IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// A protected asynchronous overridable method for getting the number of entities that satisfies the criteria specified by the <paramref name="criteria" /> from the repository.
        /// </summary>
        protected abstract Task<int> GetCountAsync(ISpecification<TEntity> criteria, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// A protected asynchronous overridable method for determining whether the repository contains an entity that satisfies the criteria specified by the <paramref name="criteria" /> from the repository.
        /// </summary>
        protected abstract Task<bool> GetExistAsync(ISpecification<TEntity> criteria, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// A protected asynchronous overridable method for getting a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, an element selector.
        /// </summary>
        protected abstract Task<Dictionary<TDictionaryKey, TElement>> GetDictionaryAsync<TDictionaryKey, TElement>(ISpecification<TEntity> criteria, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken());

        #endregion

        #region Implementation of ICanAggregateAsync<TEntity>

        /// <summary>
        /// Asynchronously returns the number of entities contained in the repository.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of entities contained in the repository.</returns>
        public Task<int> CountAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return CountAsync((ISpecification<TEntity>)null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously returns the number of entities that satisfies the criteria specified by the <paramref name="criteria" /> in the repository.
        /// </summary>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of entities that satisfied the criteria specified by the <paramref name="criteria" /> in the repository..</returns>
        public Task<int> CountAsync(ISpecification<TEntity> criteria, CancellationToken cancellationToken = new CancellationToken())
        {
            return GetCountAsync(criteria, cancellationToken);
        }

        /// <summary>
        /// Asynchronously returns the number of entities that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of entities that satisfied the criteria specified by the <paramref name="predicate" /> in the repository..</returns>
        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return CountAsync(new Specification<TEntity>(predicate), cancellationToken);
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Task<Dictionary<TDictionaryKey, TEntity>> ToDictionaryAsync<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector, IQueryOptions<TEntity> options = null, CancellationToken cancellationToken = new CancellationToken())
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            return ToDictionaryAsync((ISpecification<TEntity>)null, keySelector, options, cancellationToken);
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TElemen}" /> according to the specified <paramref name="keySelector" />, a comparer, and an element selector function..
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Task<Dictionary<TDictionaryKey, TElement>> ToDictionaryAsync<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, IQueryOptions<TEntity> options = null, CancellationToken cancellationToken = new CancellationToken())
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            return ToDictionaryAsync((ISpecification<TEntity>)null, keySelector, elementSelector, options, cancellationToken);
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Task<Dictionary<TDictionaryKey, TEntity>> ToDictionaryAsync<TDictionaryKey>(ISpecification<TEntity> criteria, Expression<Func<TEntity, TDictionaryKey>> keySelector, IQueryOptions<TEntity> options = null, CancellationToken cancellationToken = new CancellationToken())
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            return ToDictionaryAsync(criteria, keySelector, IdentityExpression<TEntity>.Instance, options, cancellationToken);
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TElemen}" /> according to the specified <paramref name="keySelector" />, a comparer, and an element selector function..
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Task<Dictionary<TDictionaryKey, TElement>> ToDictionaryAsync<TDictionaryKey, TElement>(ISpecification<TEntity> criteria, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, IQueryOptions<TEntity> options = null, CancellationToken cancellationToken = new CancellationToken())
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            return GetDictionaryAsync(criteria, keySelector, elementSelector, options, cancellationToken);
        }

        #endregion

        #region Implementation of ICanAddAsync<in TEntity>

        /// <summary>
        /// Asynchronously adds the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            cancellationToken.ThrowIfCancellationRequested();

            AddItem(entity);
            await SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Asynchronously adds the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var entity in entities)
            {
                AddItem(entity);
            }

            await SaveChangesAsync(cancellationToken);
        }

        #endregion

        #region Implementation of ICanUpdateAsync<in TEntity>

        /// <summary>
        /// Asynchronously updates the specified <paramref name="entity" /> in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            cancellationToken.ThrowIfCancellationRequested();

            UpdateItem(entity);
            await SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Asynchronously updates the specified <paramref name="entities" /> collection in the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var entity in entities)
            {
                UpdateItem(entity);
            }

            await SaveChangesAsync(cancellationToken);
        }

        #endregion

        #region Implementation of ICanDeleteAsync<TEntity,in TKey>

        /// <summary>
        /// Asynchronously deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            cancellationToken.ThrowIfCancellationRequested();

            var entity = await GetAsync(key, cancellationToken);
            if (entity == null)
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyNotFound, key));

            DeleteItem(entity);
            await SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Asynchronously deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            cancellationToken.ThrowIfCancellationRequested();

            DeleteItem(entity);
            await SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Asynchronously deletes all entities in the repository that satisfied the criteria specified by the <paramref name="criteria" />.
        /// </summary>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(ISpecification<TEntity> criteria, CancellationToken cancellationToken = new CancellationToken())
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            await DeleteAsync(await FindAllAsync(criteria, (IQueryOptions<TEntity>)null, cancellationToken), cancellationToken);
        }

        /// <summary>
        /// Asynchronously deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var entity in entities)
            {
                DeleteItem(entity);
            }

            await SaveChangesAsync(cancellationToken);
        }

        #endregion

        #region Implementation of ICanGetAsync<TEntity,in TKey>

        /// <summary>
        /// Asynchronously gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <return>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</return>
        public Task<TEntity> GetAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return GetEntityAsync(key, cancellationToken);
        }

        /// <summary>
        /// Asynchronously gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <return>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</return>
        public Task<TEntity> GetAsync(TKey key, IFetchStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return GetEntityAsync(key, fetchStrategy, cancellationToken);
        }

        /// <summary>
        /// Asynchronously gets a specific projected entity result with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public Task<TResult> GetAsync<TResult>(TKey key, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return GetAsync(key, selector, (IFetchStrategy<TEntity>)null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously gets a specific projected entity result with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public Task<TResult> GetAsync<TResult>(TKey key, Expression<Func<TEntity, TResult>> selector, IFetchStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (fetchStrategy == null)
                throw new ArgumentNullException(nameof(fetchStrategy));

            return GetEntityAsync(key, fetchStrategy, selector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity with the given primary key value
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return GetExistAsync(GetByPrimaryKeySpecification(key, (IFetchStrategy<TEntity>)null), cancellationToken);
        }

        #endregion

        #region Implementation of ICanFindAsync<TEntity>

        /// <summary>
        /// Asynchronously finds the first entity in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, IQueryOptions<TEntity> options = null, CancellationToken cancellationToken = new CancellationToken())
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return FindAsync(new Specification<TEntity>(predicate), options, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds the first entity in the repository that satisfies the criteria specified by the <paramref name="criteria" /> in the repository.
        /// </summary>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity that satisfied the criteria specified by the <paramref name="criteria" /> in the repository.</returns>
        public Task<TEntity> FindAsync(ISpecification<TEntity> criteria, IQueryOptions<TEntity> options = null, CancellationToken cancellationToken = new CancellationToken())
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            return GetEntityAsync(criteria, options, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public Task<TResult> FindAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, IQueryOptions<TEntity> options = null, CancellationToken cancellationToken = new CancellationToken())
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return FindAsync(new Specification<TEntity>(predicate), selector, options, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="criteria" /> in the repository.
        /// </summary>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public Task<TResult> FindAsync<TResult>(ISpecification<TEntity> criteria, Expression<Func<TEntity, TResult>> selector, IQueryOptions<TEntity> options = null, CancellationToken cancellationToken = new CancellationToken())
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return GetEntityAsync(criteria, options, selector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository.
        /// </summary>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the collection of entities in the repository.</returns>
        public Task<List<TEntity>> FindAllAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetEntitiesAsync(null, null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, IQueryOptions<TEntity> options = null, CancellationToken cancellationToken = new CancellationToken())
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return FindAllAsync(new Specification<TEntity>(predicate), options, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="criteria" />.
        /// </summary>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the collection of entities in the repository that satisfied the criteria specified by the <paramref name="criteria" />.</returns>
        public Task<List<TEntity>> FindAllAsync(ISpecification<TEntity> criteria, IQueryOptions<TEntity> options = null, CancellationToken cancellationToken = new CancellationToken())
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            return GetEntitiesAsync(criteria, options, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public Task<List<TResult>> FindAllAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, IQueryOptions<TEntity> options = null, CancellationToken cancellationToken = new CancellationToken())
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return FindAllAsync(new Specification<TEntity>(predicate), selector, options, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="criteria" />.
        /// </summary>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="criteria" />.</returns>
        public Task<List<TResult>> FindAllAsync<TResult>(ISpecification<TEntity> criteria, Expression<Func<TEntity, TResult>> selector, IQueryOptions<TEntity> options = null, CancellationToken cancellationToken = new CancellationToken())
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return GetEntitiesAsync(criteria, options, selector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">The predicate used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the conditions defined by the specified predicate; otherwise, <c>false</c>.</returns>
        public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return ExistsAsync(new Specification<TEntity>(predicate), cancellationToken);
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="criteria" />.
        /// </summary>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        public Task<bool> ExistsAsync(ISpecification<TEntity> criteria, CancellationToken cancellationToken = new CancellationToken())
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            return GetExistAsync(criteria, cancellationToken);
        }

        #endregion
    }
}
