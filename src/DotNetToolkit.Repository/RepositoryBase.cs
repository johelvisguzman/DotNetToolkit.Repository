namespace DotNetToolkit.Repository
{
    using FetchStrategies;
    using Helpers;
    using Logging;
    using Properties;
    using Queries;
    using Specifications;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// An implementation of <see cref="IRepository{TEntity, TKey}" />.
    /// </summary>
    public abstract class RepositoryBase<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class
    {
        #region Properties

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity,TKey}"/> class.
        /// </summary>
        protected RepositoryBase()
        {
            ThrowIfEntityKeyValueTypeMismatch();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected RepositoryBase(ILogger logger) : this()
        {
            Logger = logger;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public abstract void Dispose();

        #endregion

        #region Protected Methods

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
        /// Gets an entity query that satisfies the criteria specified by the <paramref name="criteria" /> from the repository.
        /// </summary>
        protected IQueryable<TEntity> GetQuery(ISpecification<TEntity> criteria, IQueryOptions<TEntity> options)
        {
            IQueryable<TEntity> query;

            if (criteria != null)
            {
                query = GetQuery(criteria.FetchStrategy);
                query = criteria.SatisfyingEntitiesFrom(query);
            }
            else
            {
                query = GetQuery();
            }

            if (options != null)
            {
                query = options.Apply(query);
            }

            return query;
        }

        /// <summary>
        /// Gets an entity query with the given primary key value from the repository.
        /// </summary>
        protected virtual TEntity GetEntity(TKey key, IFetchStrategy<TEntity> fetchStrategy)
        {
            return GetEntity(GetByPrimaryKeySpecification(key, fetchStrategy), (IQueryOptions<TEntity>)null);
        }

        /// <summary>
        /// Gets an entity query with the given primary key value from the repository.
        /// </summary>
        protected TEntity GetEntity(TKey key)
        {
            return Get(key, (IFetchStrategy<TEntity>)null);
        }

        /// <summary>
        /// Gets an entity that satisfies the criteria specified by the <paramref name="criteria" /> from the repository.
        /// </summary>
        protected TEntity GetEntity(ISpecification<TEntity> criteria, IQueryOptions<TEntity> options)
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            return GetQuery(criteria, options).FirstOrDefault();
        }

        /// <summary>
        /// Gets an entity that satisfies the criteria specified by the <paramref name="criteria" /> from the repository.
        /// </summary>
        protected TResult GetEntity<TResult>(ISpecification<TEntity> criteria, IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            return GetQuery(criteria, options).Select(selector).FirstOrDefault();
        }

        /// <summary>
        /// Gets a collection of entities that satisfies the criteria specified by the <paramref name="criteria" /> from the repository.
        /// </summary>
        protected IEnumerable<TEntity> GetEntities(ISpecification<TEntity> criteria, IQueryOptions<TEntity> options)
        {
            return GetQuery(criteria, options).ToList();
        }

        /// <summary>
        /// Gets a collection of entities that satisfies the criteria specified by the <paramref name="criteria" /> from the repository.
        /// </summary>
        protected IEnumerable<TResult> GetEntities<TResult>(ISpecification<TEntity> criteria, IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            return GetQuery(criteria, options).Select(selector).ToList();
        }

        /// <summary>
        /// Gets the number of entities that satisfies the criteria specified by the <paramref name="criteria" /> from the repository.
        /// </summary>
        protected virtual int GetCount(ISpecification<TEntity> criteria)
        {
            var predicate = criteria?.Predicate?.Compile();

            return predicate == null ? GetQuery().Count() : GetQuery().Count(predicate);
        }

        /// <summary>
        /// Determining whether the repository contains an entity that satisfies the criteria specified by the <paramref name="criteria" /> from the repository.
        /// </summary>
        protected virtual bool GetExist(ISpecification<TEntity> criteria)
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            return Find(criteria) != null;
        }

        /// <summary>
        /// Gets a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, an element selector.
        /// </summary>
        protected virtual Dictionary<TDictionaryKey, TElement> GetDictionary<TDictionaryKey, TElement>(ISpecification<TEntity> criteria, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, IQueryOptions<TEntity> options)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            var keySelectFunc = keySelector.Compile();
            var elementSelectorFunc = elementSelector.Compile();

            return GetQuery(criteria, options).ToDictionary(keySelectFunc, elementSelectorFunc, EqualityComparer<TDictionaryKey>.Default);
        }

        /// <summary>
        /// Gets a new <see cref="IGrouping{TGroupKey, TElement}" /> according to the specified <paramref name="keySelector" />, an element selector.
        /// </summary>
        protected virtual IEnumerable<IGrouping<TGroupKey, TElement>> GetGroupBy<TGroupKey, TElement>(ISpecification<TEntity> criteria, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, IQueryOptions<TEntity> options)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            var keySelectFunc = keySelector.Compile();
            var elementSelectorFunc = elementSelector.Compile();

            return GetQuery(criteria, options).AsEnumerable().GroupBy(keySelectFunc, elementSelectorFunc, EqualityComparer<TGroupKey>.Default);
        }

        /// <summary>
        /// Returns a specification for getting an entity by it's primary key.
        /// </summary>
        /// <param name="key">The entity's key.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <returns>The new specification.</returns>
        // https://github.com/SharpRepository/SharpRepository/blob/develop/SharpRepository.Repository/RepositoryBase.cs
        protected virtual ISpecification<TEntity> GetByPrimaryKeySpecification(TKey key, IFetchStrategy<TEntity> fetchStrategy = null)
        {
            var propInfo = ConventionHelper.GetPrimaryKeyPropertyInfo(typeof(TEntity));
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var lambda = Expression.Lambda<Func<TEntity, bool>>(
                Expression.Equal(
                    Expression.PropertyOrField(parameter, propInfo.Name),
                    Expression.Constant(key)
                ),
                parameter
            );

            var spec = new Specification<TEntity>(lambda);

            if (fetchStrategy != null)
            {
                spec.FetchStrategy = fetchStrategy;
            }

            return spec;
        }

        /// <summary>
        /// Gets the primary key property information for the specified type.
        /// </summary>
        /// <returns>The primary key property info.</returns>
        protected virtual PropertyInfo GetPrimaryKeyPropertyInfo()
        {
            return ConventionHelper.GetPrimaryKeyPropertyInfo(typeof(TEntity));
        }

        /// <summary>
        /// Gets the value of the specified object primary key property.
        /// </summary>
        /// <param name="entity">The entity containing the property.</param>
        /// <returns>The property value.</returns>
        protected virtual TKey GetPrimaryKey(TEntity entity)
        {
            return (TKey)Convert.ChangeType(ConventionHelper.GetPrimaryKeyPropertyValue(entity), typeof(TKey));
        }

        /// <summary>
        /// Sets a value for the specified object primary key property.
        /// </summary>
        /// <param name="entity">The entity containing the property.</param>
        /// <param name="key">The value to set for the primary key property.</param>
        protected virtual void SetPrimaryKey(TEntity entity, TKey key)
        {
            ConventionHelper.SetPrimaryKeyPropertyValue(entity, key);
        }

        /// <summary>
        /// Generates a new primary id for the entity.
        /// </summary>
        /// <returns>The new generated primary id.</returns>
        protected virtual TKey GeneratePrimaryKey()
        {
            var propertyInfo = GetPrimaryKeyPropertyInfo();
            var propertyType = propertyInfo.PropertyType;

            if (propertyType == typeof(Guid))
                return (TKey)Convert.ChangeType(Guid.NewGuid(), typeof(TKey));

            if (propertyType == typeof(string))
                return (TKey)Convert.ChangeType(Guid.NewGuid().ToString("N"), typeof(TKey));

            if (propertyType == typeof(int))
            {
                var key = GetQuery()
                    .OrderByDescending(x => GetPrimaryKey(x))
                    .Select(x => GetPrimaryKey(x))
                    .FirstOrDefault();

                return (TKey)Convert.ChangeType(Convert.ToInt32(key) + 1, typeof(TKey));
            }

            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyValueTypeInvalid, typeof(TEntity), propertyType));
        }

        /// <summary>
        /// Throws if the entity key value type does not match the type of the property defined.
        /// </summary>
        protected virtual void ThrowIfEntityKeyValueTypeMismatch()
        {
            var propertyInfo = GetPrimaryKeyPropertyInfo();
            if (propertyInfo.PropertyType != typeof(TKey))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyValueTypeMismatch, typeof(TKey), propertyInfo.PropertyType));
        }

        #endregion

        #region Implementation of IRepositoryQueryable<out TEntity>

        /// <summary>
        /// Returns the entity <see cref="System.Linq.IQueryable{TEntity}" />.
        /// </summary>
        public virtual IQueryable<TEntity> AsQueryable()
        {
            return GetQuery();
        }

        #endregion

        #region Implementation of ICanAggregate<TEntity>

        /// <summary>
        /// Returns the number of entities contained in the repository.
        /// </summary>
        /// <returns>The number of entities contained in the repository.</returns>
        public int Count()
        {
            return Count((ISpecification<TEntity>)null);
        }

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="criteria" /> in the repository.
        /// </summary>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="criteria" /> in the repository.</returns>
        public int Count(ISpecification<TEntity> criteria)
        {
            return GetCount(criteria);
        }

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return Count(predicate == null ? null : new Specification<TEntity>(predicate));
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Dictionary<TDictionaryKey, TEntity> ToDictionary<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector, IQueryOptions<TEntity> options = null)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            return ToDictionary((ISpecification<TEntity>)null, keySelector, options);
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TElemen}" /> according to the specified <paramref name="keySelector" />, a comparer, and an element selector function..
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Dictionary<TDictionaryKey, TElement> ToDictionary<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, IQueryOptions<TEntity> options = null)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            return ToDictionary((ISpecification<TEntity>)null, keySelector, elementSelector, options);
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Dictionary<TDictionaryKey, TEntity> ToDictionary<TDictionaryKey>(ISpecification<TEntity> criteria, Expression<Func<TEntity, TDictionaryKey>> keySelector, IQueryOptions<TEntity> options = null)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            return ToDictionary(criteria, keySelector, IdentityExpression<TEntity>.Instance, options);
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TElemen}" /> according to the specified <paramref name="keySelector" />, a comparer, and an element selector function..
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Dictionary<TDictionaryKey, TElement> ToDictionary<TDictionaryKey, TElement>(ISpecification<TEntity> criteria, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, IQueryOptions<TEntity> options = null)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            return GetDictionary(criteria, keySelector, elementSelector, options);
        }

        /// <summary>
        /// Returns a new <see cref="IGrouping{TGroupKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>A new <see cref="IGrouping{TGroupKey, TEntity}" /> that contains keys and values.</returns>
        public IEnumerable<IGrouping<TGroupKey, TEntity>> GroupBy<TGroupKey>(Expression<Func<TEntity, TGroupKey>> keySelector, IQueryOptions<TEntity> options = null)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            return GroupBy((ISpecification<TEntity>)null, keySelector, options);
        }

        /// <summary>
        /// Returns a new <see cref="IGrouping{TGroupKey, TElemen}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>A new <see cref="IGrouping{TGroupKey, TEntity}" /> that contains keys and values.</returns>
        public IEnumerable<IGrouping<TGroupKey, TElement>> GroupBy<TGroupKey, TElement>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, IQueryOptions<TEntity> options = null)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            return GroupBy((ISpecification<TEntity>)null, keySelector, elementSelector, options);
        }

        /// <summary>
        /// Returns a new <see cref="IGrouping{TGroupKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>A new <see cref="IGrouping{TGroupKey, TEntity}" /> that contains keys and values.</returns>
        public IEnumerable<IGrouping<TGroupKey, TEntity>> GroupBy<TGroupKey>(ISpecification<TEntity> criteria, Expression<Func<TEntity, TGroupKey>> keySelector, IQueryOptions<TEntity> options = null)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            return GroupBy(criteria, keySelector, IdentityExpression<TEntity>.Instance, options);
        }

        /// <summary>
        /// Returns a new <see cref="IGrouping{TGroupKey, TElemen}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>A new <see cref="IGrouping{TGroupKey, TEntity}" /> that contains keys and values.</returns>
        public IEnumerable<IGrouping<TGroupKey, TElement>> GroupBy<TGroupKey, TElement>(ISpecification<TEntity> criteria, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, IQueryOptions<TEntity> options = null)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            return GetGroupBy(criteria, keySelector, elementSelector, options);
        }

        #endregion

        #region Implementation of ICanAdd<in TEntity>

        /// <summary>
        /// Adds the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public void Add(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Logger?.Write($"Adding {typeof(TEntity).Name} entity", entity);

            AddItem(entity);
            SaveChanges();

            Logger?.Write($"Added {typeof(TEntity).Name} entity", entity);
        }

        /// <summary>
        /// Adds the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        public void Add(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            Logger?.Write($"Adding {typeof(TEntity).Name} entities", entities);

            foreach (var entity in entities)
            {
                AddItem(entity);
            }

            SaveChanges();

            Logger?.Write($"Added {typeof(TEntity).Name} entities", entities);
        }

        #endregion

        #region Implementation of ICanUpdate<in TEntity>

        /// <summary>
        /// Updates the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public void Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Logger?.Write($"Updating {typeof(TEntity).Name} entity", entity);

            UpdateItem(entity);
            SaveChanges();

            Logger?.Write($"Updated {typeof(TEntity).Name} entity", entity);
        }

        /// <summary>
        /// Updates the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        public void Update(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            Logger?.Write($"Updating {typeof(TEntity).Name} entities", entities);

            foreach (var entity in entities)
            {
                UpdateItem(entity);
            }

            SaveChanges();

            Logger?.Write($"Updated {typeof(TEntity).Name} entities", entities);
        }

        #endregion

        #region Implementation of ICanDelete<in TEntity,in TKey>

        /// <summary>
        /// Deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        public void Delete(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var entity = Get(key);
            if (entity == null)
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyNotFound, key));

            Delete(entity);
        }

        /// <summary>
        /// Deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public void Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Logger?.Write($"Deleting {typeof(TEntity).Name} entity", entity);

            DeleteItem(entity);
            SaveChanges();

            Logger?.Write($"Deleted {typeof(TEntity).Name} entity", entity);
        }

        /// <summary>
        /// Deletes all entities in the repository that satisfied the criteria specified by the <paramref name="criteria" />.
        /// </summary>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        public void Delete(ISpecification<TEntity> criteria)
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            Delete(FindAll(criteria));
        }

        /// <summary>
        /// Deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        public void Delete(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            Logger?.Write($"Deleting {typeof(TEntity).Name} entities", entities);

            foreach (var entity in entities)
            {
                DeleteItem(entity);
            }

            SaveChanges();

            Logger?.Write($"Deleted {typeof(TEntity).Name} entities", entities);
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
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return GetEntity(key);
        }

        /// <summary>
        /// Gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <return>The entity found.</return>
        public TEntity Get(TKey key, IFetchStrategy<TEntity> fetchStrategy)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return GetEntity(key, fetchStrategy);
        }

        /// <summary>
        /// Gets a specific projected entity result with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public TResult Get<TResult>(TKey key, Expression<Func<TEntity, TResult>> selector)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return Get(key, selector, (IFetchStrategy<TEntity>)null);
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
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var result = GetEntity(key, fetchStrategy);
            var selectFunc = selector.Compile();
            var selectedResult = result == null
                ? default(TResult)
                : new[] { result }.AsEnumerable().Select(selectFunc).First();

            return selectedResult;
        }

        /// <summary>
        /// Determines whether the repository contains an entity with the given primary key value
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public bool Exists(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return Get(key) != null;
        }

        #endregion

        #region Implementation of ICanFind<TEntity>

        /// <summary>
        /// Finds the first entity in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The entity that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public TEntity Find(Expression<Func<TEntity, bool>> predicate, IQueryOptions<TEntity> options = null)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return Find(new Specification<TEntity>(predicate), options);
        }

        /// <summary>
        /// Finds the first entity in the repository that satisfies the criteria specified by the <paramref name="criteria" /> in the repository.
        /// </summary>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The entity that satisfied the criteria specified by the <paramref name="criteria" /> in the repository.</returns>
        public TEntity Find(ISpecification<TEntity> criteria, IQueryOptions<TEntity> options = null)
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            return GetEntity(criteria, options);
        }

        /// <summary>
        /// Finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public TResult Find<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, IQueryOptions<TEntity> options = null)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return Find(new Specification<TEntity>(predicate), selector, options);
        }

        /// <summary>
        /// Finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="criteria" /> in the repository.
        /// </summary>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public TResult Find<TResult>(ISpecification<TEntity> criteria, Expression<Func<TEntity, TResult>> selector, IQueryOptions<TEntity> options = null)
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return GetEntity(criteria, options, selector);
        }

        /// <summary>
        /// Finds the collection of entities in the repository.
        /// </summary>
        /// <returns>The collection of entities in the repository.</returns>
        public IEnumerable<TEntity> FindAll()
        {
            return GetEntities(null, null);
        }

        /// <summary>
        /// Finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate, IQueryOptions<TEntity> options = null)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return FindAll(new Specification<TEntity>(predicate), options);
        }

        /// <summary>
        /// Finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="criteria" />.
        /// </summary>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The collection of entities in the repository that satisfied the criteria specified by the <paramref name="criteria" />.</returns>
        public IEnumerable<TEntity> FindAll(ISpecification<TEntity> criteria, IQueryOptions<TEntity> options = null)
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            return GetEntities(criteria, options);
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public IEnumerable<TResult> FindAll<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, IQueryOptions<TEntity> options = null)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return FindAll(new Specification<TEntity>(predicate), selector, options);
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="criteria" />.
        /// </summary>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="criteria" />.</returns>
        public IEnumerable<TResult> FindAll<TResult>(ISpecification<TEntity> criteria, Expression<Func<TEntity, TResult>> selector, IQueryOptions<TEntity> options = null)
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return GetEntities(criteria, options, selector);
        }

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">The predicate used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified predicate; otherwise, <c>false</c>.</returns>
        public bool Exists(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return Exists(new Specification<TEntity>(predicate));
        }

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="criteria" />.
        /// </summary>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        public bool Exists(ISpecification<TEntity> criteria)
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            return GetExist(criteria);
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
    }
}