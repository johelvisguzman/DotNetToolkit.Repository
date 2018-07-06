namespace DotNetToolkit.Repository
{
    using FetchStrategies;
    using Interceptors;
    using Properties;
    using Queries;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Wrappers;

    /// <summary>
    /// An implementation of <see cref="IRepositoryAsync{TEntity, TKey}" />.
    /// </summary>
    public abstract class RepositoryBaseAsync<TEntity, TKey> : RepositoryBase<TEntity, TKey>, IRepositoryAsync<TEntity, TKey> where TEntity : class
    {
        #region Fields

        private IReadOnlyRepositoryAsync<TEntity, TKey> _wrapper;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBaseAsync{TEntity, TKey}"/> class.
        /// </summary>
        protected RepositoryBaseAsync() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBaseAsync{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="interceptors">The interceptors.</param>
        protected RepositoryBaseAsync(IEnumerable<IRepositoryInterceptor> interceptors) : base(interceptors) { }

        #endregion

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
            var options = new QueryOptions<TEntity>().SatisfyBy(GetByPrimaryKeySpecification(key));

            if (fetchStrategy != null)
                options.Fetch(fetchStrategy);

            return GetEntityAsync(options, IdentityExpression<TEntity>.Instance, cancellationToken);
        }

        /// <summary>
        /// A protected asynchronous overridable method for getting an entity that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected abstract Task<TResult> GetEntityAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// A protected asynchronous overridable method for getting a collection of entities that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected abstract Task<IEnumerable<TResult>> GetEntitiesAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// A protected asynchronous overridable method for getting the number of entities that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected abstract Task<int> GetCountAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// A protected asynchronous overridable method for determining whether the repository contains an entity that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected abstract Task<bool> GetExistAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// A protected asynchronous overridable method for getting a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, an element selector.
        /// </summary>
        protected abstract Task<Dictionary<TDictionaryKey, TElement>> GetDictionaryAsync<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// A protected asynchronous overridable method for getting a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, an element selector.
        /// </summary>
        protected abstract Task<IEnumerable<TResult>> GetGroupByAsync<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<IGrouping<TGroupKey, TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken());

        #endregion

        #region Implementation of IRepositoryAsync<TEntity, TKey>

        /// <summary>
        /// Asynchronously returns the number of entities contained in the repository.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The number of entities contained in the repository.</returns>
        public Task<int> CountAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return CountAsync((IQueryOptions<TEntity>)null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously returns the number of entities that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of entities that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return CountAsync(InterceptError<IQueryOptions<TEntity>>(() => new QueryOptions<TEntity>().SatisfyBy(predicate)), cancellationToken);
        }

        /// <summary>
        /// Asynchronously returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public Task<int> CountAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return InterceptError<Task<int>>(() => GetCountAsync(options, cancellationToken));
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Task<Dictionary<TDictionaryKey, TEntity>> ToDictionaryAsync<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ToDictionaryAsync<TDictionaryKey>((IQueryOptions<TEntity>)null, keySelector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public Task<Dictionary<TDictionaryKey, TEntity>> ToDictionaryAsync<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ToDictionaryAsync<TDictionaryKey, TEntity>(options, keySelector, IdentityExpression<TEntity>.Instance, cancellationToken);
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Task<Dictionary<TDictionaryKey, TElement>> ToDictionaryAsync<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ToDictionaryAsync<TDictionaryKey, TElement>((IQueryOptions<TEntity>)null, keySelector, elementSelector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, and an element selector function with entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public Task<Dictionary<TDictionaryKey, TElement>> ToDictionaryAsync<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                if (keySelector == null)
                    throw new ArgumentNullException(nameof(keySelector));

                if (elementSelector == null)
                    throw new ArgumentNullException(nameof(elementSelector));

                return GetDictionaryAsync<TDictionaryKey, TElement>(options, keySelector, elementSelector, cancellationToken);
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A transform function to produce a result value from each element.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="IGrouping{TGroupKey, TEntity}" /> that contains keys and values.</returns>
        public Task<IEnumerable<TResult>> GroupByAsync<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<IGrouping<TGroupKey, TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return GroupByAsync<TGroupKey, TResult>((IQueryOptions<TEntity>)null, keySelector, resultSelector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A transform function to produce a result value from each element.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="IGrouping{TGroupKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public Task<IEnumerable<TResult>> GroupByAsync<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<IGrouping<TGroupKey, TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                if (keySelector == null)
                    throw new ArgumentNullException(nameof(keySelector));

                if (resultSelector == null)
                    throw new ArgumentNullException(nameof(resultSelector));

                return GetGroupByAsync<TGroupKey, TResult>(options, keySelector, resultSelector, cancellationToken);
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Returns a read-only <see cref="IReadOnlyRepositoryAsync{TEntity, TKey}" /> wrapper for the current repository.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper around the current repository.</returns>
        public IReadOnlyRepositoryAsync<TEntity, TKey> AsReadOnlyAsync()
        {
            return _wrapper ?? (_wrapper = new ReadOnlyRepositoryAsync<TEntity, TKey>(this));
        }

        /// <summary>
        /// Asynchronously adds the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                cancellationToken.ThrowIfCancellationRequested();

                InterceptAddItem(entity);

                await SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Asynchronously adds the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                if (!entities.Any())
                    return;

                cancellationToken.ThrowIfCancellationRequested();

                InterceptAddItem(entities);

                await SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Asynchronously updates the specified <paramref name="entity" /> in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                cancellationToken.ThrowIfCancellationRequested();

                InterceptUpdateItem(entity);

                await SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Asynchronously updates the specified <paramref name="entities" /> collection in the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                if (!entities.Any())
                    return;

                cancellationToken.ThrowIfCancellationRequested();

                InterceptUpdateItem(entities);

                await SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Asynchronously deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var entity = await GetAsync(key, cancellationToken);

            if (entity == null)
            {
                var ex = new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyNotFound, key));

                Intercept(x => x.Error(ex));

                throw ex;
            }

            await DeleteAsync(entity, cancellationToken);
        }

        /// <summary>
        /// Asynchronously deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                cancellationToken.ThrowIfCancellationRequested();

                InterceptDeleteItem(entity);

                await SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Asynchronously deletes all the entities in the repository that satisfies the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            var entitiesInDb = await FindAllAsync(predicate, cancellationToken);

            await DeleteAsync(entitiesInDb, cancellationToken);
        }

        /// <summary>
        /// Asynchronously deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            var entitiesInDb = await FindAllAsync(options, cancellationToken);

            await DeleteAsync(entitiesInDb, cancellationToken);
        }

        /// <summary>
        /// Asynchronously deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                if (!entities.Any())
                    return;

                cancellationToken.ThrowIfCancellationRequested();

                InterceptDeleteItem(entities);

                await SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Asynchronously gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <return>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</return>
        public Task<TEntity> GetAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            return GetAsync<TEntity>(key, IdentityExpression<TEntity>.Instance, cancellationToken);
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
            return GetAsync<TEntity>(key, IdentityExpression<TEntity>.Instance, fetchStrategy, cancellationToken);
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
            return GetAsync<TResult>(key, selector, (IFetchStrategy<TEntity>)null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously gets a specific projected entity result with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public Task<TResult> GetAsync<TResult>(TKey key, Expression<Func<TEntity, TResult>> selector, IFetchStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                if (key == null)
                    throw new ArgumentNullException(nameof(key));

                if (selector == null)
                    throw new ArgumentNullException(nameof(selector));

                var options = new QueryOptions<TEntity>().SatisfyBy(GetByPrimaryKeySpecification(key));

                if (fetchStrategy != null)
                    options.Fetch(fetchStrategy);

                return GetEntityAsync<TResult>(options, selector, cancellationToken);
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity with the given primary key value
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                if (key == null)
                    throw new ArgumentNullException(nameof(key));

                return GetExistAsync(new QueryOptions<TEntity>().SatisfyBy(GetByPrimaryKeySpecification(key)), cancellationToken);
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Asynchronously finds the first entity in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return FindAsync<TEntity>(predicate, IdentityExpression<TEntity>.Instance, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds the first entity in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public Task<TEntity> FindAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return FindAsync<TEntity>(options, IdentityExpression<TEntity>.Instance, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public Task<TResult> FindAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return FindAsync<TResult>(InterceptError<IQueryOptions<TEntity>>(() => new QueryOptions<TEntity>().SatisfyBy(predicate)), selector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public Task<TResult> FindAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                if (options == null)
                    throw new ArgumentNullException(nameof(options));

                if (selector == null)
                    throw new ArgumentNullException(nameof(selector));

                return GetEntityAsync<TResult>(options, selector, cancellationToken);
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the collection of entities in the repository.</returns>
        public Task<IEnumerable<TEntity>> FindAllAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return FindAllAsync<TEntity>(IdentityExpression<TEntity>.Instance, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return FindAllAsync<TEntity>(predicate, IdentityExpression<TEntity>.Instance, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public Task<IEnumerable<TEntity>> FindAllAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return FindAllAsync<TEntity>(options, IdentityExpression<TEntity>.Instance, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository.
        /// </summary>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the collection of projected entity results in the repository.</returns>
        public Task<IEnumerable<TResult>> FindAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return FindAllAsync<TResult>((IQueryOptions<TEntity>)null, selector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public Task<IEnumerable<TResult>> FindAllAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return FindAllAsync<TResult>(InterceptError<IQueryOptions<TEntity>>(() => new QueryOptions<TEntity>().SatisfyBy(predicate)), selector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public Task<IEnumerable<TResult>> FindAllAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                if (selector == null)
                    throw new ArgumentNullException(nameof(selector));

                return GetEntitiesAsync<TResult>(options, selector, cancellationToken);
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">The predicate used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the conditions defined by the specified predicate; otherwise, <c>false</c>.</returns>
        public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExistsAsync(InterceptError<IQueryOptions<TEntity>>(() => new QueryOptions<TEntity>().SatisfyBy(predicate)), cancellationToken);
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        public Task<bool> ExistsAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                if (options == null)
                    throw new ArgumentNullException(nameof(options));

                return GetExistAsync(options, cancellationToken);
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        #endregion
    }
}
