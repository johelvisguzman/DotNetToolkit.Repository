namespace DotNetToolkit.Repository
{
    using FetchStrategies;
    using Helpers;
    using Interceptors;
    using Properties;
    using Queries;
    using Specifications;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// An implementation of <see cref="IRepository{TEntity, TKey}" />.
    /// </summary>
    public abstract class RepositoryBase<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class
    {
        #region Fields

        private readonly IEnumerable<IRepositoryInterceptor> _interceptors;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity, TKey}"/> class.
        /// </summary>
        protected RepositoryBase() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="interceptors">The interceptors.</param>
        protected RepositoryBase(IEnumerable<IRepositoryInterceptor> interceptors)
        {
            _interceptors = interceptors ?? Enumerable.Empty<IRepositoryInterceptor>();
        }

        #endregion

        #region Internal Methods

        internal void InterceptAddItem(TEntity entity)
        {
            Intercept(x => x.AddExecuting(entity));

            AddItem(entity);

            Intercept(x => x.AddExecuted(entity));
        }

        internal void InterceptAddItem(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                InterceptAddItem(entity);
            }
        }

        internal void InterceptUpdateItem(TEntity entity)
        {
            Intercept(x => x.UpdateExecuting(entity));

            UpdateItem(entity);

            Intercept(x => x.UpdateExecuted(entity));
        }

        internal void InterceptUpdateItem(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                InterceptUpdateItem(entity);
            }
        }

        internal void InterceptDeleteItem(TEntity entity)
        {
            Intercept(x => x.DeleteExecuting(entity));

            DeleteItem(entity);

            Intercept(x => x.DeleteExecuted(entity));
        }

        internal void InterceptDeleteItem(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                InterceptDeleteItem(entity);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected abstract void Dispose(bool disposing);

        /// <summary>
        /// A protected overridable method for adding the specified <paramref name="entity" /> into the repository.
        /// </summary>
        protected abstract void AddItem(TEntity entity);

        /// <summary>
        /// A protected overridable method for deleting the specified <paramref name="entity" /> from the repository.
        /// </summary>
        protected abstract void DeleteItem(TEntity entity);

        /// <summary>
        /// A protected overridable method for updating the specified <paramref name="entity" /> in the repository.
        /// </summary>
        protected abstract void UpdateItem(TEntity entity);

        /// <summary>
        /// A protected overridable method for saving changes made in the current unit of work in the repository.
        /// </summary>
        protected abstract void SaveChanges();

        /// <summary>
        /// A protected overridable method for getting an entity query that supplies the specified fetching strategy from the repository.
        /// </summary>
        protected abstract IQueryable<TEntity> GetQuery(IFetchStrategy<TEntity> fetchStrategy = null);

        /// <summary>
        /// Gets an entity query that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected IQueryable<TEntity> GetQuery(IQueryOptions<TEntity> options)
        {
            return options != null ? options.Apply(GetQuery(options.FetchStrategy)) : GetQuery();
        }

        /// <summary>
        /// Gets an entity query with the given primary key value from the repository.
        /// </summary>
        protected virtual TEntity GetEntity(TKey key, IFetchStrategy<TEntity> fetchStrategy)
        {
            var options = new QueryOptions<TEntity>().SatisfyBy(GetByPrimaryKeySpecification(key));

            if (fetchStrategy != null)
                options.Fetch(fetchStrategy);

            return GetEntity<TEntity>(options, IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Gets an entity that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected virtual TResult GetEntity<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return GetQuery(options).Select(selector).FirstOrDefault();
        }

        /// <summary>
        /// Gets a collection of entities that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected virtual IEnumerable<TResult> GetEntities<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return GetQuery(options).Select(selector).ToList();
        }

        /// <summary>
        /// Gets the number of entities that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected virtual int GetCount(IQueryOptions<TEntity> options)
        {
            return GetQuery(options).Count();
        }

        /// <summary>
        /// Determining whether the repository contains an entity that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected virtual bool GetExist(IQueryOptions<TEntity> options)
        {
            return GetQuery(options).Any();
        }

        /// <summary>
        /// Gets a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, an element selector.
        /// </summary>
        protected virtual Dictionary<TDictionaryKey, TElement> GetDictionary<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            var keySelectFunc = keySelector.Compile();
            var elementSelectorFunc = elementSelector.Compile();

            return GetQuery(options).ToDictionary(keySelectFunc, elementSelectorFunc, EqualityComparer<TDictionaryKey>.Default);
        }

        /// <summary>
        /// Gets a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, an element selector.
        /// </summary>
        protected virtual IEnumerable<TResult> GetGroupBy<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<IGrouping<TGroupKey, TEntity>, TResult>> resultSelector)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            var keySelectFunc = keySelector.Compile();
            var resultSelectorFunc = resultSelector.Compile();

            return GetQuery(options).GroupBy(keySelectFunc, EqualityComparer<TGroupKey>.Default).Select(resultSelectorFunc).ToList();
        }

        /// <summary>
        /// Returns a specification for getting an entity by it's primary key.
        /// </summary>
        /// <param name="key">The entity's key.</param>
        /// <returns>The new specification.</returns>
        // https://github.com/SharpRepository/SharpRepository/blob/develop/SharpRepository.Repository/RepositoryBase.cs
        protected virtual ISpecification<TEntity> GetByPrimaryKeySpecification(TKey key)
        {
            var propInfo = ConventionHelper.GetPrimaryKeyPropertyInfos<TEntity>().First();
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var lambda = Expression.Lambda<Func<TEntity, bool>>(
                Expression.Equal(
                    Expression.PropertyOrField(parameter, propInfo.Name),
                    Expression.Constant(key)
                ),
                parameter
            );

            return new Specification<TEntity>(lambda);
        }

        /// <summary>
        /// Executes the specified interceptor activity.
        /// </summary>
        /// <param name="action">The action to be executed.</param>
        protected void Intercept(Action<IRepositoryInterceptor> action)
        {
            foreach (var interceptor in _interceptors)
            {
                action(interceptor);
            }
        }

        /// <summary>
        /// Intercepts any errors that occurred while performing the specified action.
        /// </summary>
        /// <typeparam name="T">The type of the result returned by the specified action.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>The result of the performed action.</returns>
        protected T InterceptError<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        #endregion

        #region Implementation of IRepositoryQueryable<out TEntity>

        /// <summary>
        /// Returns the entity <see cref="System.Linq.IQueryable{TEntity}" />.
        /// </summary>
        public virtual IQueryable<TEntity> AsQueryable()
        {
            return InterceptError<IQueryable<TEntity>>(() => GetQuery());
        }

        #endregion

        #region Implementation of ICanAggregate<TEntity>

        /// <summary>
        /// Returns the number of entities contained in the repository.
        /// </summary>
        /// <returns>The number of entities contained in the repository.</returns>
        public int Count()
        {
            return Count((IQueryOptions<TEntity>)null);
        }

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return Count(InterceptError<IQueryOptions<TEntity>>(() => new QueryOptions<TEntity>().SatisfyBy(predicate)));
        }

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public int Count(IQueryOptions<TEntity> options)
        {
            return InterceptError<int>(() => GetCount(options));
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Dictionary<TDictionaryKey, TEntity> ToDictionary<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            return ToDictionary<TDictionaryKey>((IQueryOptions<TEntity>)null, keySelector);
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public Dictionary<TDictionaryKey, TEntity> ToDictionary<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            return ToDictionary<TDictionaryKey, TEntity>(options, keySelector, IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Dictionary<TDictionaryKey, TElement> ToDictionary<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            return ToDictionary<TDictionaryKey, TElement>((IQueryOptions<TEntity>)null, keySelector, elementSelector);
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, and an element selector function with entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public Dictionary<TDictionaryKey, TElement> ToDictionary<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            try
            {
                if (keySelector == null)
                    throw new ArgumentNullException(nameof(keySelector));

                if (elementSelector == null)
                    throw new ArgumentNullException(nameof(elementSelector));

                return GetDictionary<TDictionaryKey, TElement>(options, keySelector, elementSelector);
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A transform function to produce a result value from each element.</param>
        /// <returns>A new <see cref="IGrouping{TGroupKey, TEntity}" /> that contains keys and values.</returns>
        public IEnumerable<TResult> GroupBy<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<IGrouping<TGroupKey, TEntity>, TResult>> resultSelector)
        {
            return GroupBy<TGroupKey, TResult>((IQueryOptions<TEntity>)null, keySelector, resultSelector);
        }

        /// <summary>
        /// Returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A transform function to produce a result value from each element.</param>
        /// <returns>A new <see cref="IGrouping{TGroupKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public IEnumerable<TResult> GroupBy<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<IGrouping<TGroupKey, TEntity>, TResult>> resultSelector)
        {
            try
            {
                if (keySelector == null)
                    throw new ArgumentNullException(nameof(keySelector));

                if (resultSelector == null)
                    throw new ArgumentNullException(nameof(resultSelector));

                return GetGroupBy<TGroupKey, TResult>(options, keySelector, resultSelector);
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        #endregion

        #region Implementation of ICanAdd<in TEntity>

        /// <summary>
        /// Adds the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public void Add(TEntity entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                InterceptAddItem(entity);

                SaveChanges();
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Adds the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        public void Add(IEnumerable<TEntity> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                if (!entities.Any())
                    return;

                InterceptAddItem(entities);

                SaveChanges();
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        #endregion

        #region Implementation of ICanUpdate<in TEntity>

        /// <summary>
        /// Updates the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public void Update(TEntity entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                InterceptUpdateItem(entity);

                SaveChanges();
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Updates the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        public void Update(IEnumerable<TEntity> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                if (!entities.Any())
                    return;

                InterceptUpdateItem(entities);

                SaveChanges();
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        #endregion

        #region Implementation of ICanDelete<in TEntity,in TKey>

        /// <summary>
        /// Deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        public void Delete(TKey key)
        {
            var entity = Get(key);

            if (entity == null)
            {
                var ex = new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyNotFound, key));

                Intercept(x => x.Error(ex));

                throw ex;
            }

            Delete(entity);
        }

        /// <summary>
        /// Deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public void Delete(TEntity entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                InterceptDeleteItem(entity);

                SaveChanges();
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Deletes all the entities in the repository that satisfies the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            Delete(FindAll(predicate));
        }

        /// <summary>
        /// Deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        public void Delete(IQueryOptions<TEntity> options)
        {
            Delete(FindAll(options));
        }

        /// <summary>
        /// Deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        public void Delete(IEnumerable<TEntity> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                if (!entities.Any())
                    return;

                InterceptDeleteItem(entities);

                SaveChanges();
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        #endregion

        #region Implementation of ICanGet<TEntity,in TKey>

        /// <summary>
        /// Gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <return>The entity found.</return>
        public TEntity Get(TKey key)
        {
            return Get<TEntity>(key, IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <return>The entity found.</return>
        public TEntity Get(TKey key, IFetchStrategy<TEntity> fetchStrategy)
        {
            return Get<TEntity>(key, IdentityExpression<TEntity>.Instance, fetchStrategy);
        }

        /// <summary>
        /// Gets a specific projected entity result with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public TResult Get<TResult>(TKey key, Expression<Func<TEntity, TResult>> selector)
        {
            return Get<TResult>(key, selector, (IFetchStrategy<TEntity>)null);
        }

        /// <summary>
        /// Gets a specific projected entity result with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public TResult Get<TResult>(TKey key, Expression<Func<TEntity, TResult>> selector, IFetchStrategy<TEntity> fetchStrategy)
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

                return GetEntity<TResult>(options, selector);
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Determines whether the repository contains an entity with the given primary key value
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public bool Exists(TKey key)
        {
            return Get(key) != null;
        }

        #endregion

        #region Implementation of ICanFind<TEntity>

        /// <summary>
        /// Finds the first entity in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The entity that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public TEntity Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Find<TEntity>(predicate, IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Finds the first entity in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The entity that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public TEntity Find(IQueryOptions<TEntity> options)
        {
            return Find<TEntity>(options, IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public TResult Find<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return Find<TResult>(InterceptError<IQueryOptions<TEntity>>(() => new QueryOptions<TEntity>().SatisfyBy(predicate)), selector);
        }

        /// <summary>
        /// Finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public TResult Find<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            try
            {
                if (options == null)
                    throw new ArgumentNullException(nameof(options));

                if (selector == null)
                    throw new ArgumentNullException(nameof(selector));

                return GetEntity<TResult>(options, selector);
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Finds the collection of entities in the repository.
        /// </summary>
        /// <returns>The collection of entities in the repository.</returns>
        public IEnumerable<TEntity> FindAll()
        {
            return FindAll<TEntity>(IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate)
        {
            return FindAll<TEntity>(predicate, IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public IEnumerable<TEntity> FindAll(IQueryOptions<TEntity> options)
        {
            return FindAll<TEntity>(options, IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository.
        /// </summary>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository.</returns>
        public IEnumerable<TResult> FindAll<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return FindAll<TResult>((IQueryOptions<TEntity>)null, selector);
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public IEnumerable<TResult> FindAll<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return FindAll<TResult>(InterceptError<IQueryOptions<TEntity>>(() => new QueryOptions<TEntity>().SatisfyBy(predicate)), selector);
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public IEnumerable<TResult> FindAll<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            try
            {
                if (selector == null)
                    throw new ArgumentNullException(nameof(selector));

                return GetEntities<TResult>(options, selector);
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">The predicate used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified predicate; otherwise, <c>false</c>.</returns>
        public bool Exists(Expression<Func<TEntity, bool>> predicate)
        {
            return Exists(InterceptError<IQueryOptions<TEntity>>(() => new QueryOptions<TEntity>().SatisfyBy(predicate)));
        }

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        public bool Exists(IQueryOptions<TEntity> options)
        {
            try
            {
                if (options == null)
                    throw new ArgumentNullException(nameof(options));

                return GetExist(options);
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        #endregion

        #region Nested type: IdentityExpression<TElement>

        protected class IdentityExpression<TElement>
        {
            public static Expression<Func<TElement, TElement>> Instance
            {
                get { return x => x; }
            }
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}