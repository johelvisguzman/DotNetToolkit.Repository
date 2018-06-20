namespace DotNetToolkit.Repository.Transactions
{
    using Factories;
    using FetchStrategies;
    using Helpers;
    using Properties;
    using Queries;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// An implementation of <see cref="IUnitOfWork" />.
    /// </summary>
    public abstract class UnitOfWorkBase : IUnitOfWork
    {
        #region Fields

        internal readonly Dictionary<Type, object> _repositories;
        private bool _disposed;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the repository factory.
        /// </summary>
        protected IRepositoryFactory Factory { get; set; }

        /// <summary>
        /// Gets the transaction manager.
        /// </summary>
        protected ITransactionManager TransactionManager { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkBase"/> class.
        /// </summary>
        /// <param name="transactionManager">The transaction.</param>
        /// <exception cref="ArgumentNullException">transaction</exception>
        protected UnitOfWorkBase(ITransactionManager transactionManager)
        {
            if (transactionManager == null)
                throw new ArgumentNullException(nameof(transactionManager));

            TransactionManager = transactionManager;

            _repositories = new Dictionary<Type, object>();
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
                if (TransactionManager != null)
                {
                    TransactionManager.Rollback();
                    TransactionManager.Dispose();
                    TransactionManager = null;
                }
            }

            _disposed = true;
        }

        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>The new repository.</returns>
        protected virtual IRepository<TEntity, object> GetRepository<TEntity>() where TEntity : class
        {
            if (Factory == null)
                throw new InvalidOperationException("A repository factory has not been specified.");

            if (_repositories.ContainsKey(typeof(TEntity)))
                return _repositories[typeof(TEntity)] as IRepository<TEntity, object>;

            var repo = Factory.Create<TEntity, object>();

            _repositories.Add(typeof(TEntity), repo);

            return repo;
        }

        /// <summary>
        /// Throws if the entity key value type does not match the type of the property defined.
        /// </summary>
        protected virtual void ThrowIfEntityKeyValueTypeMismatch<TEntity>(Type keyType) where TEntity : class
        {
            var propertyInfo = ConventionHelper.GetPrimaryKeyPropertyInfo<TEntity>();
            if (propertyInfo.PropertyType != keyType)
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyValueTypeMismatch, keyType, propertyInfo.PropertyType));
        }

        #endregion

        #region Implementation of IUnitOfWork

        /// <summary>
        /// Commits all changes made in this unit of work.
        /// </summary>
        public virtual void Commit()
        {
            if (TransactionManager == null)
                throw new InvalidOperationException("The transaction has already been committed or disposed.");

            TransactionManager.Commit();
            TransactionManager = null;
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Implementation of ICanQuery

        /// <summary>
        /// Returns the entity <see cref="System.Linq.IQueryable{TEntity}" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        public IQueryable<TEntity> AsQueryable<TEntity>() where TEntity : class
        {
            return GetRepository<TEntity>().AsQueryable();
        }

        #endregion

        #region Implementation of ICanAggregate

        /// <summary>
        /// Returns the number of entities contained in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>The number of entities contained in the repository.</returns>
        public int Count<TEntity>() where TEntity : class
        {
            return GetRepository<TEntity>().Count();
        }

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public int Count<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return GetRepository<TEntity>().Count(predicate);
        }

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public int Count<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            return GetRepository<TEntity>().Count(options);
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Dictionary<TDictionaryKey, TEntity> ToDictionary<TEntity, TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector) where TEntity : class
        {
            return GetRepository<TEntity>().ToDictionary<TDictionaryKey>(keySelector);
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public Dictionary<TDictionaryKey, TEntity> ToDictionary<TEntity, TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector) where TEntity : class
        {
            return GetRepository<TEntity>().ToDictionary<TDictionaryKey>(options, keySelector);
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Dictionary<TDictionaryKey, TElement> ToDictionary<TEntity, TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector) where TEntity : class
        {
            return GetRepository<TEntity>().ToDictionary<TDictionaryKey, TElement>(keySelector, elementSelector);
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, and an element selector function with entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public Dictionary<TDictionaryKey, TElement> ToDictionary<TEntity, TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector) where TEntity : class
        {
            return GetRepository<TEntity>().ToDictionary<TDictionaryKey, TElement>(options, keySelector, elementSelector);
        }

        /// <summary>
        /// Returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <returns>A new <see cref="IGrouping{TGroupKey, TEntity}" /> that contains keys and values.</returns>
        public IEnumerable<TResult> GroupBy<TEntity, TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<IGrouping<TGroupKey, TEntity>, TResult>> resultSelector) where TEntity : class
        {
            return GetRepository<TEntity>().GroupBy<TGroupKey, TResult>(keySelector, resultSelector);
        }

        /// <summary>
        /// Returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <returns>A new <see cref="IGrouping{TGroupKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public IEnumerable<TResult> GroupBy<TEntity, TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<IGrouping<TGroupKey, TEntity>, TResult>> resultSelector) where TEntity : class
        {
            return GetRepository<TEntity>().GroupBy<TGroupKey, TResult>(options, keySelector, resultSelector);
        }

        #endregion

        #region Implementation of ICanAdd

        /// <summary>
        /// Adds the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity to add.</param>
        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            GetRepository<TEntity>().Add(entity);
        }

        /// <summary>
        /// Adds the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entities">The collection of entities to add.</param>
        public void Add<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            GetRepository<TEntity>().Add(entities);
        }

        #endregion

        #region Implementation of ICanUpdate

        /// <summary>
        /// Updates the specified <paramref name="entity" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity to update.</param>
        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            GetRepository<TEntity>().Update(entity);
        }

        /// <summary>
        /// Updates the specified <paramref name="entities" /> collection in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entities">The collection of entities to update.</param>
        public void Update<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            GetRepository<TEntity>().Update(entities);
        }

        #endregion

        #region Implementation of ICanDelete

        /// <summary>
        /// Deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the primary key.</typeparam>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        public void Delete<TEntity, TKey>(TKey key) where TEntity : class
        {
            GetRepository<TEntity>().Delete(key);
        }

        /// <summary>
        /// Deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity to delete.</param>
        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            GetRepository<TEntity>().Delete(entity);
        }

        /// <summary>
        /// Deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        public void Delete<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            GetRepository<TEntity>().Delete(options);
        }

        /// <summary>
        /// Deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entities">The collection of entities to delete.</param>
        public void Delete<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            GetRepository<TEntity>().Delete(entities);
        }

        #endregion

        #region Implementation of ICanGet

        /// <summary>
        /// Gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <return>The entity found.</return>
        public TEntity Get<TEntity>(object key) where TEntity : class
        {
            ThrowIfEntityKeyValueTypeMismatch<TEntity>(key.GetType());

            return GetRepository<TEntity>().Get(key);
        }

        /// <summary>
        /// Gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <return>The entity found.</return>
        public TEntity Get<TEntity>(object key, IFetchStrategy<TEntity> fetchStrategy) where TEntity : class
        {
            ThrowIfEntityKeyValueTypeMismatch<TEntity>(key.GetType());

            return GetRepository<TEntity>().Get(key, fetchStrategy);
        }

        /// <summary>
        /// Gets a specific projected entity result with the given primary key value in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public TResult Get<TEntity, TResult>(object key, Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            ThrowIfEntityKeyValueTypeMismatch<TEntity>(key.GetType());

            return GetRepository<TEntity>().Get(key, selector);
        }

        /// <summary>
        /// Gets a specific projected entity result with the given primary key value in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public TResult Get<TEntity, TResult>(object key, Expression<Func<TEntity, TResult>> selector, IFetchStrategy<TEntity> fetchStrategy) where TEntity : class
        {
            ThrowIfEntityKeyValueTypeMismatch<TEntity>(key.GetType());

            return GetRepository<TEntity>().Get(key, selector, fetchStrategy);
        }

        #endregion

        #region Implementation of ICanFind

        /// <summary>
        /// Finds the first entity in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The entity that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public TEntity Find<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return GetRepository<TEntity>().Find(predicate);
        }

        /// <summary>
        /// Finds the first entity in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The entity that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public TEntity Find<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            return GetRepository<TEntity>().Find(options);
        }

        /// <summary>
        /// Finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public TResult Find<TEntity, TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            return GetRepository<TEntity>().Find(predicate, selector);
        }

        /// <summary>
        /// Finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public TResult Find<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            return GetRepository<TEntity>().Find(options, selector);
        }

        /// <summary>
        /// Finds the collection of entities in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>The collection of entities in the repository.</returns>
        public IEnumerable<TEntity> FindAll<TEntity>() where TEntity : class
        {
            return GetRepository<TEntity>().FindAll();
        }

        /// <summary>
        /// Finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public IEnumerable<TEntity> FindAll<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return GetRepository<TEntity>().FindAll(predicate);
        }

        /// <summary>
        /// Finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public IEnumerable<TEntity> FindAll<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            return GetRepository<TEntity>().FindAll(options);
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository.</returns>
        public IEnumerable<TResult> FindAll<TEntity, TResult>(Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            return GetRepository<TEntity>().FindAll<TResult>(selector);
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public IEnumerable<TResult> FindAll<TEntity, TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            return GetRepository<TEntity>().FindAll<TResult>(predicate, selector);
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public IEnumerable<TResult> FindAll<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            return GetRepository<TEntity>().FindAll<TResult>(options, selector);
        }

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="predicate" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="predicate">The predicate used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified predicate; otherwise, <c>false</c>.</returns>
        public bool Exists<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return GetRepository<TEntity>().Exists(predicate);
        }

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        public bool Exists<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            return GetRepository<TEntity>().Exists(options);
        }

        #endregion
    }
}
