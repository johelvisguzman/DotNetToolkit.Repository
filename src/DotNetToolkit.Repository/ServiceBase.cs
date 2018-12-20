namespace DotNetToolkit.Repository
{
    using Configuration.Conventions;
    using Factories;
    using Queries;
    using Queries.Strategies;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// An implementation of <see cref="IService{TEntity, TKey1, TKey2, TKey3}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    /// <typeparam name="TKey3">The type of the third part of the composite primary key.</typeparam>
    public abstract class ServiceBase<TEntity, TKey1, TKey2, TKey3> : IService<TEntity, TKey1, TKey2, TKey3> where TEntity : class
    {
        #region Properties

        /// <summary>
        /// Gets the unit of work factory.
        /// </summary>
        protected IUnitOfWorkFactory UnitOfWorkFactory { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBase{TEntity, TKey1, TKey2, TKey3}"/> class.
        /// </summary>
        /// <param name="unitOfWorkFactory">The unit of work factory.</param>
        protected ServiceBase(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));

            UnitOfWorkFactory = unitOfWorkFactory;

            PrimaryKeyConventionHelper.ThrowsIfInvalidPrimaryKeyDefinition<TEntity>(typeof(TKey1), typeof(TKey2), typeof(TKey3));
        }

        #endregion

        #region Implementation of IService<TEntity, TKey1, TKey2, TKey3>

        /// <summary>
        /// Creates the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public virtual void Create(TEntity entity)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2, TKey3>().Add(entity);
                uow.Commit();
            }
        }

        /// <summary>
        /// Creates the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        public virtual void Create(IEnumerable<TEntity> entities)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2, TKey3>().Add(entities);
                uow.Commit();
            }
        }

        /// <summary>
        /// Deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <returns><c>true</c> is able to successfully delete an entity with the given composite primary key values; otherwise, <c>false</c>.</returns>
        public virtual bool TryDelete(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().TryDelete(key1, key2, key3);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        public virtual void Delete(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2, TKey3>().Delete(key1, key2, key3);
                uow.Commit();
            }
        }

        /// <summary>
        /// Deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public virtual void Delete(TEntity entity)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2, TKey3>().Delete(entity);
                uow.Commit();
            }
        }

        /// <summary>
        /// Deletes all the entities in the repository that satisfies the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2, TKey3>().Delete(predicate);
                uow.Commit();
            }
        }

        /// <summary>
        /// Deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        public virtual void Delete(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2, TKey3>().Delete(options);
                uow.Commit();
            }
        }

        /// <summary>
        /// Deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        public virtual void Delete(IEnumerable<TEntity> entities)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2, TKey3>().Delete(entities);
                uow.Commit();
            }
        }

        /// <summary>
        /// Updates the specified <paramref name="entity" /> in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public virtual void Update(TEntity entity)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2, TKey3>().Update(entity);
                uow.Commit();
            }
        }

        /// <summary>
        /// Updates the specified <paramref name="entities" /> collection in the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        public virtual void Update(IEnumerable<TEntity> entities)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2, TKey3>().Update(entities);
                uow.Commit();
            }
        }

        /// <summary>
        /// Gets an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <return>The entity found.</return>
        public virtual TEntity Get(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().Find(key1, key2, key3);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Gets an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <return>The entity found.</return>
        public virtual TEntity Get(TKey1 key1, TKey2 key2, TKey3 key3, IFetchQueryStrategy<TEntity> fetchStrategy)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().Find(key1, key2, key3, fetchStrategy);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the first entity in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The entity that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public virtual TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().Find(predicate);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the first entity in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The entity that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual TEntity Get(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().Find(options);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public virtual TResult Get<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().Find<TResult>(predicate, selector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public virtual TResult Get<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().Find<TResult>(options, selector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the collection of entities in the repository.
        /// </summary>
        /// <returns>The collection of entities in the repository.</returns>
        public virtual IEnumerable<TEntity> GetAll()
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAll();

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public virtual IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAll(predicate);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public virtual IQueryResult<IEnumerable<TEntity>> GetAll(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAll(options);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository.
        /// </summary>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository.</returns>
        public virtual IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAll<TResult>(selector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public virtual IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAll<TResult>(predicate, selector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public virtual IQueryResult<IEnumerable<TResult>> GetAll<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAll<TResult>(options, selector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Determines whether the repository contains an entity with the given composite primary key values.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public virtual bool GetExists(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().Exists(key1, key2, key3);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">The predicate used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified predicate; otherwise, <c>false</c>.</returns>
        public virtual bool GetExists(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().Exists(predicate);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        public virtual bool GetExists(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().Exists(options);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Returns the number of entities contained in the repository.
        /// </summary>
        /// <returns>The number of entities contained in the repository.</returns>
        public virtual int GetCount()
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().Count();

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public virtual int GetCount(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().Count(predicate);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual int GetCount(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().Count(options);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public virtual Dictionary<TDictionaryKey, TEntity> GetDictionary<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().ToDictionary<TDictionaryKey>(keySelector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual IQueryResult<Dictionary<TDictionaryKey, TEntity>> GetDictionary<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().ToDictionary<TDictionaryKey>(options, keySelector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public virtual Dictionary<TDictionaryKey, TElement> GetDictionary<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().ToDictionary<TDictionaryKey, TElement>(keySelector, elementSelector);

                uow.Commit();

                return result;
            }
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
        public virtual IQueryResult<Dictionary<TDictionaryKey, TElement>> GetDictionary<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().ToDictionary<TDictionaryKey, TElement>(options, keySelector, elementSelector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <returns>A new <see cref="IEnumerable{TResult}" /> that contains keys and values.</returns>
        public virtual IEnumerable<TResult> GetGroupBy<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().GroupBy<TGroupKey, TResult>(keySelector, resultSelector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <returns>A new <see cref="IEnumerable{TResult}" /> that contains the grouped result that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual IQueryResult<IEnumerable<TResult>> GetGroupBy<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().GroupBy<TGroupKey, TResult>(options, keySelector, resultSelector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously creates the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task CreateAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2, TKey3>().AddAsync(entity, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously creates the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task CreateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2, TKey3>().AddAsync(entities, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> is able to successfully delete an entity with the given composite primary key values; otherwise, <c>false</c>.</returns>
        public virtual async Task<bool> TryDeleteAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().TryDeleteAsync(key1, key2, key3, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task DeleteAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2, TKey3>().DeleteAsync(key1, key2, key3, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2, TKey3>().DeleteAsync(entity, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously deletes all the entities in the repository that satisfies the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2, TKey3>().DeleteAsync(predicate, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task DeleteAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2, TKey3>().DeleteAsync(options, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2, TKey3>().DeleteAsync(entities, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously updates the specified <paramref name="entity" /> in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2, TKey3>().UpdateAsync(entity, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously updates the specified <paramref name="entities" /> collection in the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2, TKey3>().UpdateAsync(entities, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        public virtual async Task<TEntity> GetAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAsync(key1, key2, key3, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        public virtual async Task<TEntity> GetAsync(TKey1 key1, TKey2 key2, TKey3 key3, IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAsync(key1, key2, key3, fetchStrategy, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the first entity in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  entity that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAsync(predicate, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the first entity in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  entity that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual async Task<TEntity> GetAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAsync(options, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public virtual async Task<TResult> GetAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAsync<TResult>(predicate, selector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public virtual async Task<TResult> GetAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAsync<TResult>(options, selector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of entities in the repository.</returns>
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAllAsync(cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAllAsync(cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public virtual async Task<IQueryResult<IEnumerable<TEntity>>> GetAllAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAllAsync(options, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository.
        /// </summary>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of projected entity results in the repository.</returns>
        public virtual async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAllAsync<TResult>(selector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public virtual async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAllAsync<TResult>(predicate, selector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public virtual async Task<IQueryResult<IEnumerable<TResult>>> GetAllAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAllAsync<TResult>(options, selector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity with the given composite primary key values.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public virtual async Task<bool> GetExistsAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().ExistsAsync(key1, key2, key3, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">The predicate used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the conditions defined by the specified predicate; otherwise, <c>false</c>.</returns>
        public virtual async Task<bool> GetExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().ExistsAsync(predicate, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        public virtual async Task<bool> GetExistsAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().ExistsAsync(options, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously returns the number of entities contained in the repository.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  number of entities contained in the repository.</returns>
        public virtual async Task<int> GetCountAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().CountAsync(cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously returns the number of entities that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  number of entities that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public virtual async Task<int> GetCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().CountAsync(predicate, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual async Task<int> GetCountAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().CountAsync(options, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public virtual async Task<Dictionary<TDictionaryKey, TEntity>> GetDictionaryAsync<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().ToDictionaryAsync<TDictionaryKey>(keySelector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual async Task<IQueryResult<Dictionary<TDictionaryKey, TEntity>>> GetDictionaryAsync<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().ToDictionaryAsync<TDictionaryKey>(options, keySelector, cancellationToken);

                uow.Commit();

                return result;
            }
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
        public virtual async Task<Dictionary<TDictionaryKey, TElement>> GetDictionaryAsync<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().ToDictionaryAsync<TDictionaryKey, TElement>(keySelector, elementSelector, cancellationToken);

                uow.Commit();

                return result;
            }
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
        public virtual async Task<IQueryResult<Dictionary<TDictionaryKey, TElement>>> GetDictionaryAsync<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().ToDictionaryAsync<TDictionaryKey, TElement>(options, keySelector, elementSelector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="IEnumerable{TResult}" /> that contains keys and values.</returns>
        public virtual async Task<IEnumerable<TResult>> GetGroupByAsync<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().GroupByAsync<TGroupKey, TResult>(keySelector, resultSelector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="IEnumerable{TResult}" /> that contains the grouped result that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual async Task<IQueryResult<IEnumerable<TResult>>> GetGroupByAsync<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().GroupByAsync<TGroupKey, TResult>(options, keySelector, resultSelector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IService{TEntity, TKey1, TKey2}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    public abstract class ServiceBase<TEntity, TKey1, TKey2> : IService<TEntity, TKey1, TKey2> where TEntity : class
    {
        #region Properties

        /// <summary>
        /// Gets the unit of work factory.
        /// </summary>
        protected IUnitOfWorkFactory UnitOfWorkFactory { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBase{TEntity, TKey1, TKey2}"/> class.
        /// </summary>
        /// <param name="unitOfWorkFactory">The unit of work factory.</param>
        protected ServiceBase(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));

            UnitOfWorkFactory = unitOfWorkFactory;

            PrimaryKeyConventionHelper.ThrowsIfInvalidPrimaryKeyDefinition<TEntity>(typeof(TKey1), typeof(TKey2));
        }

        #endregion

        #region Implementation of IService<TEntity, TKey1, TKey2>

        /// <summary>
        /// Creates the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public virtual void Create(TEntity entity)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2>().Add(entity);
                uow.Commit();
            }
        }

        /// <summary>
        /// Creates the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        public virtual void Create(IEnumerable<TEntity> entities)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2>().Add(entities);
                uow.Commit();
            }
        }

        /// <summary>
        /// Deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <returns><c>true</c> is able to successfully delete an entity with the given composite primary key values; otherwise, <c>false</c>.</returns>
        public virtual bool TryDelete(TKey1 key1, TKey2 key2)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().TryDelete(key1, key2);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        public virtual void Delete(TKey1 key1, TKey2 key2)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2>().Delete(key1, key2);
                uow.Commit();
            }
        }

        /// <summary>
        /// Deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public virtual void Delete(TEntity entity)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2>().Delete(entity);
                uow.Commit();
            }
        }

        /// <summary>
        /// Deletes all the entities in the repository that satisfies the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2>().Delete(predicate);
                uow.Commit();
            }
        }

        /// <summary>
        /// Deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        public virtual void Delete(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2>().Delete(options);
                uow.Commit();
            }
        }

        /// <summary>
        /// Deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        public virtual void Delete(IEnumerable<TEntity> entities)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2>().Delete(entities);
                uow.Commit();
            }
        }

        /// <summary>
        /// Updates the specified <paramref name="entity" /> in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public virtual void Update(TEntity entity)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2>().Update(entity);
                uow.Commit();
            }
        }

        /// <summary>
        /// Updates the specified <paramref name="entities" /> collection in the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        public virtual void Update(IEnumerable<TEntity> entities)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2>().Update(entities);
                uow.Commit();
            }
        }

        /// <summary>
        /// Gets an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <return>The entity found.</return>
        public virtual TEntity Get(TKey1 key1, TKey2 key2)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().Find(key1, key2);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Gets an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <return>The entity found.</return>
        public virtual TEntity Get(TKey1 key1, TKey2 key2, IFetchQueryStrategy<TEntity> fetchStrategy)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().Find(key1, key2, fetchStrategy);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the first entity in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The entity that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public virtual TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().Find(predicate);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the first entity in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The entity that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual TEntity Get(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().Find(options);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public virtual TResult Get<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().Find<TResult>(predicate, selector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public virtual TResult Get<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().Find<TResult>(options, selector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the collection of entities in the repository.
        /// </summary>
        /// <returns>The collection of entities in the repository.</returns>
        public virtual IEnumerable<TEntity> GetAll()
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().FindAll();

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public virtual IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().FindAll(predicate);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public virtual IQueryResult<IEnumerable<TEntity>> GetAll(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().FindAll(options);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository.
        /// </summary>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository.</returns>
        public virtual IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().FindAll<TResult>(selector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public virtual IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().FindAll<TResult>(predicate, selector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public virtual IQueryResult<IEnumerable<TResult>> GetAll<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().FindAll<TResult>(options, selector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Determines whether the repository contains an entity with the given composite primary key values.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public virtual bool GetExists(TKey1 key1, TKey2 key2)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().Exists(key1, key2);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">The predicate used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified predicate; otherwise, <c>false</c>.</returns>
        public virtual bool GetExists(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().Exists(predicate);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        public virtual bool GetExists(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().Exists(options);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Returns the number of entities contained in the repository.
        /// </summary>
        /// <returns>The number of entities contained in the repository.</returns>
        public virtual int GetCount()
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().Count();

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public virtual int GetCount(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().Count(predicate);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual int GetCount(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().Count(options);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public virtual Dictionary<TDictionaryKey, TEntity> GetDictionary<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().ToDictionary<TDictionaryKey>(keySelector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual IQueryResult<Dictionary<TDictionaryKey, TEntity>> GetDictionary<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().ToDictionary<TDictionaryKey>(options, keySelector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public virtual Dictionary<TDictionaryKey, TElement> GetDictionary<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().ToDictionary<TDictionaryKey, TElement>(keySelector, elementSelector);

                uow.Commit();

                return result;
            }
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
        public virtual IQueryResult<Dictionary<TDictionaryKey, TElement>> GetDictionary<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().ToDictionary<TDictionaryKey, TElement>(options, keySelector, elementSelector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <returns>A new <see cref="IEnumerable{TResult}" /> that contains keys and values.</returns>
        public virtual IEnumerable<TResult> GetGroupBy<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().GroupBy<TGroupKey, TResult>(keySelector, resultSelector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <returns>A new <see cref="IEnumerable{TResult}" /> that contains the grouped result that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual IQueryResult<IEnumerable<TResult>> GetGroupBy<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().GroupBy<TGroupKey, TResult>(options, keySelector, resultSelector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously creates the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task CreateAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2>().AddAsync(entity, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously creates the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task CreateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2>().AddAsync(entities, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> is able to successfully delete an entity with the given composite primary key values; otherwise, <c>false</c>.</returns>
        public virtual async Task<bool> TryDeleteAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().TryDeleteAsync(key1, key2, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task DeleteAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2>().DeleteAsync(key1, key2, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2>().DeleteAsync(entity, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously deletes all the entities in the repository that satisfies the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2>().DeleteAsync(predicate, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task DeleteAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2>().DeleteAsync(options, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2>().DeleteAsync(entities, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously updates the specified <paramref name="entity" /> in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2>().UpdateAsync(entity, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously updates the specified <paramref name="entities" /> collection in the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2>().UpdateAsync(entities, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        public async Task<TEntity> GetAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().FindAsync(key1, key2, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        public virtual async Task<TEntity> GetAsync(TKey1 key1, TKey2 key2, IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().FindAsync(key1, key2, fetchStrategy, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the first entity in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  entity that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().FindAsync(predicate, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the first entity in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  entity that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual async Task<TEntity> GetAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().FindAsync(options, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public virtual async Task<TResult> GetAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().FindAsync<TResult>(predicate, selector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public virtual async Task<TResult> GetAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().FindAsync<TResult>(options, selector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of entities in the repository.</returns>
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().FindAllAsync(cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().FindAllAsync(cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public virtual async Task<IQueryResult<IEnumerable<TEntity>>> GetAllAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().FindAllAsync(options, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository.
        /// </summary>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of projected entity results in the repository.</returns>
        public virtual async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().FindAllAsync<TResult>(selector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public virtual async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().FindAllAsync<TResult>(predicate, selector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public virtual async Task<IQueryResult<IEnumerable<TResult>>> GetAllAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().FindAllAsync<TResult>(options, selector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity with the given composite primary key values.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public virtual async Task<bool> GetExistsAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().ExistsAsync(key1, key2, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">The predicate used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the conditions defined by the specified predicate; otherwise, <c>false</c>.</returns>
        public virtual async Task<bool> GetExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().ExistsAsync(predicate, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        public virtual async Task<bool> GetExistsAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().ExistsAsync(options, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously returns the number of entities contained in the repository.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  number of entities contained in the repository.</returns>
        public virtual async Task<int> GetCountAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().CountAsync(cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously returns the number of entities that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  number of entities that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public virtual async Task<int> GetCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().CountAsync(predicate, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual async Task<int> GetCountAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().CountAsync(options, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public virtual async Task<Dictionary<TDictionaryKey, TEntity>> GetDictionaryAsync<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().ToDictionaryAsync<TDictionaryKey>(keySelector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual async Task<IQueryResult<Dictionary<TDictionaryKey, TEntity>>> GetDictionaryAsync<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().ToDictionaryAsync<TDictionaryKey>(options, keySelector, cancellationToken);

                uow.Commit();

                return result;
            }
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
        public virtual async Task<Dictionary<TDictionaryKey, TElement>> GetDictionaryAsync<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().ToDictionaryAsync<TDictionaryKey, TElement>(keySelector, elementSelector, cancellationToken);

                uow.Commit();

                return result;
            }
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
        public virtual async Task<IQueryResult<Dictionary<TDictionaryKey, TElement>>> GetDictionaryAsync<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().ToDictionaryAsync<TDictionaryKey, TElement>(options, keySelector, elementSelector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="IEnumerable{TResult}" /> that contains keys and values.</returns>
        public virtual async Task<IEnumerable<TResult>> GetGroupByAsync<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().GroupByAsync<TGroupKey, TResult>(keySelector, resultSelector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="IEnumerable{TResult}" /> that contains the grouped result that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual async Task<IQueryResult<IEnumerable<TResult>>> GetGroupByAsync<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().GroupByAsync<TGroupKey, TResult>(options, keySelector, resultSelector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IService{TEntity, TKey}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public abstract class ServiceBase<TEntity, TKey> : IService<TEntity, TKey> where TEntity : class
    {
        #region Properties

        /// <summary>
        /// Gets the unit of work factory.
        /// </summary>
        protected IUnitOfWorkFactory UnitOfWorkFactory { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="unitOfWorkFactory">The unit of work factory.</param>
        protected ServiceBase(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));

            UnitOfWorkFactory = unitOfWorkFactory;

            PrimaryKeyConventionHelper.ThrowsIfInvalidPrimaryKeyDefinition<TEntity>(typeof(TKey));
        }

        #endregion

        #region Implementation of IService<TEntity, TKey>

        /// <summary>
        /// Creates the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public virtual void Create(TEntity entity)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey>().Add(entity);
                uow.Commit();
            }
        }

        /// <summary>
        /// Creates the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        public virtual void Create(IEnumerable<TEntity> entities)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey>().Add(entities);
                uow.Commit();
            }
        }

        /// <summary>
        /// Deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key"></param>
        /// <returns><c>true</c> is able to successfully delete an entity with the given primary key; otherwise, <c>false</c>.</returns>
        public virtual bool TryDelete(TKey key)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().TryDelete(key);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        public virtual void Delete(TKey key)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey>().Delete(key);
                uow.Commit();
            }
        }

        /// <summary>
        /// Deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public virtual void Delete(TEntity entity)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey>().Delete(entity);
                uow.Commit();
            }
        }

        /// <summary>
        /// Deletes all the entities in the repository that satisfies the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey>().Delete(predicate);
                uow.Commit();
            }
        }

        /// <summary>
        /// Deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        public virtual void Delete(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey>().Delete(options);
                uow.Commit();
            }
        }

        /// <summary>
        /// Deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        public virtual void Delete(IEnumerable<TEntity> entities)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey>().Delete(entities);
                uow.Commit();
            }
        }

        /// <summary>
        /// Updates the specified <paramref name="entity" /> in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public virtual void Update(TEntity entity)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey>().Update(entity);
                uow.Commit();
            }
        }

        /// <summary>
        /// Updates the specified <paramref name="entities" /> collection in the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        public virtual void Update(IEnumerable<TEntity> entities)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey>().Update(entities);
                uow.Commit();
            }
        }

        /// <summary>
        /// Finds an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <return>The entity found.</return>
        public virtual TEntity Get(TKey key)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().Find(key);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <return>The entity found.</return>
        public virtual TEntity Get(TKey key, IFetchQueryStrategy<TEntity> fetchStrategy)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().Find(key, fetchStrategy);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the first entity in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The entity that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public virtual TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().Find(predicate);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the first entity in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The entity that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual TEntity Get(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().Find(options);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public virtual TResult Get<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().Find<TResult>(predicate, selector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public virtual TResult Get<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().Find<TResult>(options, selector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the collection of entities in the repository.
        /// </summary>
        /// <returns>The collection of entities in the repository.</returns>
        public virtual IEnumerable<TEntity> GetAll()
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().FindAll();

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public virtual IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().FindAll(predicate);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public virtual IQueryResult<IEnumerable<TEntity>> GetAll(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().FindAll(options);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository.
        /// </summary>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository.</returns>
        public virtual IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().FindAll<TResult>(selector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public virtual IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().FindAll<TResult>(predicate, selector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public virtual IQueryResult<IEnumerable<TResult>> GetAll<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().FindAll<TResult>(options, selector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Determines whether the repository contains an entity with the given primary key value.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public virtual bool GetExists(TKey key)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().Exists(key);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">The predicate used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified predicate; otherwise, <c>false</c>.</returns>
        public virtual bool GetExists(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().Exists(predicate);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        public virtual bool GetExists(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().Exists(options);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Returns the number of entities contained in the repository.
        /// </summary>
        /// <returns>The number of entities contained in the repository.</returns>
        public virtual int GetCount()
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().Count();

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public virtual int GetCount(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().Count(predicate);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual int GetCount(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().Count(options);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public virtual Dictionary<TDictionaryKey, TEntity> GetDictionary<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().ToDictionary<TDictionaryKey>(keySelector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual IQueryResult<Dictionary<TDictionaryKey, TEntity>> GetDictionary<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().ToDictionary<TDictionaryKey>(options, keySelector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public virtual Dictionary<TDictionaryKey, TElement> GetDictionary<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().ToDictionary<TDictionaryKey, TElement>(keySelector, elementSelector);

                uow.Commit();

                return result;
            }
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
        public virtual IQueryResult<Dictionary<TDictionaryKey, TElement>> GetDictionary<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().ToDictionary<TDictionaryKey, TElement>(options, keySelector, elementSelector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <returns>A new <see cref="IEnumerable{TResult}" /> that contains keys and values.</returns>
        public virtual IEnumerable<TResult> GetGroupBy<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().GroupBy<TGroupKey, TResult>(keySelector, resultSelector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <returns>A new <see cref="IEnumerable{TResult}" /> that contains the grouped result that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual IQueryResult<IEnumerable<TResult>> GetGroupBy<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().GroupBy<TGroupKey, TResult>(options, keySelector, resultSelector);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously creates the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task CreateAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey>().AddAsync(entity, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously creates the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task CreateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey>().AddAsync(entities, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey>().DeleteAsync(entity, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        ///  Asynchronously deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> is able to successfully delete an entity with the given primary key; otherwise, <c>false</c>.</returns>
        public virtual async Task<bool> TryDeleteAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().TryDeleteAsync(key, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task DeleteAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey>().DeleteAsync(key, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously deletes all the entities in the repository that satisfies the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey>().DeleteAsync(predicate, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task DeleteAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey>().DeleteAsync(options, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey>().DeleteAsync(entities, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously updates the specified <paramref name="entity" /> in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey>().UpdateAsync(entity, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously updates the specified <paramref name="entities" /> collection in the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey>().UpdateAsync(entities, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously finds an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        public virtual async Task<TEntity> GetAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().FindAsync(key, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        public virtual async Task<TEntity> GetAsync(TKey key, IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().FindAsync(key, fetchStrategy, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the first entity in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  entity that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().FindAsync(predicate, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the first entity in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  entity that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual async Task<TEntity> GetAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().FindAsync(options, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public virtual async Task<TResult> GetAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().FindAsync<TResult>(predicate, selector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public virtual async Task<TResult> GetAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().FindAsync<TResult>(options, selector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of entities in the repository.</returns>
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().FindAllAsync(cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().FindAllAsync(cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public virtual async Task<IQueryResult<IEnumerable<TEntity>>> GetAllAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().FindAllAsync(options, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository.
        /// </summary>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of projected entity results in the repository.</returns>
        public virtual async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().FindAllAsync<TResult>(selector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public virtual async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().FindAllAsync<TResult>(predicate, selector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public virtual async Task<IQueryResult<IEnumerable<TResult>>> GetAllAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().FindAllAsync<TResult>(options, selector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity with the given primary key value.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public virtual async Task<bool> GetExistsAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().ExistsAsync(key, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">The predicate used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the conditions defined by the specified predicate; otherwise, <c>false</c>.</returns>
        public virtual async Task<bool> GetExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().ExistsAsync(predicate, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        public virtual async Task<bool> GetExistsAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().ExistsAsync(options, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously returns the number of entities contained in the repository.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  number of entities contained in the repository.</returns>
        public virtual async Task<int> GetCountAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().CountAsync(cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously returns the number of entities that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  number of entities that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public virtual async Task<int> GetCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().CountAsync(predicate, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual async Task<int> GetCountAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().CountAsync(options, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public virtual async Task<Dictionary<TDictionaryKey, TEntity>> GetDictionaryAsync<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().ToDictionaryAsync<TDictionaryKey>(keySelector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual async Task<IQueryResult<Dictionary<TDictionaryKey, TEntity>>> GetDictionaryAsync<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().ToDictionaryAsync<TDictionaryKey>(options, keySelector, cancellationToken);

                uow.Commit();

                return result;
            }
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
        public virtual async Task<Dictionary<TDictionaryKey, TElement>> GetDictionaryAsync<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().ToDictionaryAsync<TDictionaryKey, TElement>(keySelector, elementSelector, cancellationToken);

                uow.Commit();

                return result;
            }
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
        public virtual async Task<IQueryResult<Dictionary<TDictionaryKey, TElement>>> GetDictionaryAsync<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().ToDictionaryAsync<TDictionaryKey, TElement>(options, keySelector, elementSelector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="IEnumerable{TResult}" /> that contains keys and values.</returns>
        public virtual async Task<IEnumerable<TResult>> GetGroupByAsync<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().GroupByAsync<TGroupKey, TResult>(keySelector, resultSelector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="IEnumerable{TResult}" /> that contains the grouped result that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual async Task<IQueryResult<IEnumerable<TResult>>> GetGroupByAsync<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().GroupByAsync<TGroupKey, TResult>(options, keySelector, resultSelector, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        #endregion
    }
}
