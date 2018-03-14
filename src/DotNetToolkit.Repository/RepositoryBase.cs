namespace DotNetToolkit.Repository
{
    using FetchStrategies;
    using Helpers;
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
        /// Returns the entity <see cref="System.Linq.IQueryable{TEntity}" />.
        /// </summary>
        protected virtual IQueryable<TEntity> AsQueryable()
        {
            return GetQuery();
        }

        /// <summary>
        /// Gets an entity query that satisfies the criteria specified by the <paramref name="criteria" /> from the repository.
        /// </summary>
        protected IQueryable<TEntity> GetQuery(ISpecification<TEntity> criteria, IQueryOptions<TEntity> options)
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            var query = GetQuery(criteria.FetchStrategy);

            query = criteria.SatisfyingEntitiesFrom(query);

            if (options != null)
                query = options.Apply(query);

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
        protected virtual TEntity GetEntity(TKey key)
        {
            return Get(key, (IFetchStrategy<TEntity>)null);
        }

        /// <summary>
        /// Gets an entity that satisfies the criteria specified by the <paramref name="criteria" /> from the repository.
        /// </summary>
        protected virtual TEntity GetEntity(ISpecification<TEntity> criteria, IQueryOptions<TEntity> options)
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            return GetQuery(criteria, options).FirstOrDefault();
        }

        /// <summary>
        /// Gets an entity that satisfies the criteria specified by the <paramref name="criteria" /> from the repository.
        /// </summary>
        protected virtual TResult GetEntity<TResult>(ISpecification<TEntity> criteria, IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            return GetQuery(criteria, options).Select(selector).FirstOrDefault();
        }

        /// <summary>
        /// Gets a collection of entities that satisfies the criteria specified by the <paramref name="criteria" /> from the repository.
        /// </summary>
        protected virtual IEnumerable<TEntity> GetEntities(ISpecification<TEntity> criteria, IQueryOptions<TEntity> options)
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            return GetQuery(criteria, options).ToList();
        }

        /// <summary>
        /// Gets a collection of entities that satisfies the criteria specified by the <paramref name="criteria" /> from the repository.
        /// </summary>
        protected virtual IEnumerable<TResult> GetEntities<TResult>(ISpecification<TEntity> criteria, IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
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
        /// <param name="entityType">The entity type to get the primary key from.</param>
        /// <returns>The primary key property info.</returns>
        protected virtual PropertyInfo GetPrimaryKeyPropertyInfo(Type entityType)
        {
            return ConventionHelper.GetPrimaryKeyPropertyInfo(entityType);
        }

        /// <summary>
        /// Gets the value of the specified object primary key property.
        /// </summary>
        /// <param name="obj">The object containing the property.</param>
        /// <returns>The property value.</returns>
        protected virtual object GetPrimaryKeyPropertyValue(object obj)
        {
            return ConventionHelper.GetPrimaryKeyPropertyValueOrDefault(obj);
        }

        /// <summary>
        /// Sets a value for the specified object primary key property.
        /// </summary>
        /// <param name="obj">The object containing the property.</param>
        /// <param name="value">The value to set for the property.</param>
        protected virtual void SetPrimaryKeyPropertyValue(object obj, object value)
        {
            ConventionHelper.SetPrimaryKeyPropertyValue(obj, value);
        }

        /// <summary>
        /// Generates a new primary id for the entity.
        /// </summary>
        /// <returns>The new generated primary id.</returns>
        protected virtual object GeneratePrimaryKey(Type entityType)
        {
            var propertyInfo = GetPrimaryKeyPropertyInfo(entityType);
            var propertyType = propertyInfo.PropertyType;

            if (propertyType == typeof(Guid))
                return Convert.ChangeType(Guid.NewGuid(), propertyType);

            if (propertyType == typeof(string))
                return Convert.ChangeType(Guid.NewGuid().ToString("N"), propertyType);

            if (propertyType == typeof(int))
                return Convert.ToInt32(GetQuery().Select(x => GetPrimaryKeyPropertyValue(x)).LastOrDefault()) + 1;

            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyValueTypeInvalid, entityType, propertyType));
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

            AddItem(entity);
            SaveChanges();
        }

        /// <summary>
        /// Adds the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        public void Add(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
            {
                AddItem(entity);
            }

            SaveChanges();
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

            UpdateItem(entity);
            SaveChanges();
        }

        /// <summary>
        /// Updates the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        public void Update(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
            {
                UpdateItem(entity);
            }

            SaveChanges();
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

            DeleteItem(entity);
            SaveChanges();
        }

        /// <summary>
        /// Deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public void Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            DeleteItem(entity);
            SaveChanges();
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

            foreach (var entity in entities)
            {
                DeleteItem(entity);
            }

            SaveChanges();
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
    }
}