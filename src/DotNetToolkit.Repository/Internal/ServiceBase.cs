﻿namespace DotNetToolkit.Repository.Internal
{
    using Factories;
    using Queries;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an internal service base class with common functionality to be used.
    /// </summary>
    [ComVisible(false)]
#if !NETSTANDARD1_3
    [Browsable(false)]
#endif
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class ServiceBase<TEntity> where TEntity : class
    {
        #region Properties

        /// <summary>
        /// Gets the unit of work factory.
        /// </summary>
        protected IUnitOfWorkFactory UnitOfWorkFactory { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBase{TEntity}"/> class.
        /// </summary>
        /// <param name="unitOfWorkFactory">The unit of work factory.</param>
        internal ServiceBase(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));

            UnitOfWorkFactory = unitOfWorkFactory;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public virtual void Create(TEntity entity)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity>().Add(entity);
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
                uow.Create<TEntity>().Add(entities);
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
                uow.Create<TEntity>().Delete(entity);
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
                uow.Create<TEntity>().Delete(predicate);
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
                uow.Create<TEntity>().Delete(options);
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
                uow.Create<TEntity>().Delete(entities);
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
                uow.Create<TEntity>().Update(entity);
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
                uow.Create<TEntity>().Update(entities);
                uow.Commit();
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
                await uow.Create<TEntity>().AddAsync(entity, cancellationToken);
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
                await uow.Create<TEntity>().AddAsync(entities, cancellationToken);
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
                await uow.Create<TEntity>().DeleteAsync(entity, cancellationToken);
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
                await uow.Create<TEntity>().DeleteAsync(predicate, cancellationToken);
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
                await uow.Create<TEntity>().DeleteAsync(options, cancellationToken);
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
                await uow.Create<TEntity>().DeleteAsync(entities, cancellationToken);
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
                await uow.Create<TEntity>().UpdateAsync(entity, cancellationToken);
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
                await uow.Create<TEntity>().UpdateAsync(entities, cancellationToken);
                uow.Commit();
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
                return uow.Create<TEntity>().Find(predicate);
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
                return uow.Create<TEntity>().Find(options);
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
                return uow.Create<TEntity>().Find<TResult>(predicate, selector);
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
                return uow.Create<TEntity>().Find<TResult>(options, selector);
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
                return uow.Create<TEntity>().FindAll();
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
                return uow.Create<TEntity>().FindAll(predicate);
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
                return uow.Create<TEntity>().FindAll(options);
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
                return uow.Create<TEntity>().FindAll<TResult>(selector);
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
                return uow.Create<TEntity>().FindAll<TResult>(predicate, selector);
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
                return uow.Create<TEntity>().FindAll<TResult>(options, selector);
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
                return uow.Create<TEntity>().Exists(predicate);
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
                return uow.Create<TEntity>().Exists(options);
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
                return uow.Create<TEntity>().Count();
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
                return uow.Create<TEntity>().Count(predicate);
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
                return uow.Create<TEntity>().Count(options);
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
                return uow.Create<TEntity>().ToDictionary<TDictionaryKey>(keySelector);
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
                return uow.Create<TEntity>().ToDictionary<TDictionaryKey>(options, keySelector);
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
                return uow.Create<TEntity>().ToDictionary<TDictionaryKey, TElement>(keySelector, elementSelector);
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
                return uow.Create<TEntity>().ToDictionary<TDictionaryKey, TElement>(options, keySelector, elementSelector);
            }
        }

        /// <summary>
        /// Returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <returns>A new <see cref="IGrouping{TKey,TElement}" /> that contains keys and values.</returns>
        public virtual IEnumerable<TResult> GetGroupBy<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity>().GroupBy<TGroupKey, TResult>(keySelector, resultSelector);
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
                return uow.Create<TEntity>().GroupBy<TGroupKey, TResult>(options, keySelector, resultSelector);
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
                return await uow.Create<TEntity>().FindAsync(predicate, cancellationToken);
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
                return await uow.Create<TEntity>().FindAsync(options, cancellationToken);
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
                return await uow.Create<TEntity>().FindAsync<TResult>(predicate, selector, cancellationToken);
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
                return await uow.Create<TEntity>().FindAsync<TResult>(options, selector, cancellationToken);
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
                return await uow.Create<TEntity>().FindAllAsync(cancellationToken);
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
                return await uow.Create<TEntity>().FindAllAsync(cancellationToken);
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
                return await uow.Create<TEntity>().FindAllAsync(options, cancellationToken);
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
                return await uow.Create<TEntity>().FindAllAsync<TResult>(selector, cancellationToken);
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
                return await uow.Create<TEntity>().FindAllAsync<TResult>(predicate, selector, cancellationToken);
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
                return await uow.Create<TEntity>().FindAllAsync<TResult>(options, selector, cancellationToken);
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
                return await uow.Create<TEntity>().ExistsAsync(predicate, cancellationToken);
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
                return await uow.Create<TEntity>().ExistsAsync(options, cancellationToken);
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
                return await uow.Create<TEntity>().CountAsync(cancellationToken);
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
                return await uow.Create<TEntity>().CountAsync(predicate, cancellationToken);
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
                return await uow.Create<TEntity>().CountAsync(options, cancellationToken);
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
                return await uow.Create<TEntity>().ToDictionaryAsync<TDictionaryKey>(keySelector, cancellationToken);
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
                return await uow.Create<TEntity>().ToDictionaryAsync<TDictionaryKey>(options, keySelector, cancellationToken);
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
                return await uow.Create<TEntity>().ToDictionaryAsync<TDictionaryKey, TElement>(keySelector, elementSelector, cancellationToken);
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
                return await uow.Create<TEntity>().ToDictionaryAsync<TDictionaryKey, TElement>(options, keySelector, elementSelector, cancellationToken);
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
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="IGrouping{TKey,TElement}" /> that contains keys and values.</returns>
        public virtual async Task<IEnumerable<TResult>> GetGroupByAsync<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity>().GroupByAsync<TGroupKey, TResult>(keySelector, resultSelector, cancellationToken);
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
                return await uow.Create<TEntity>().GroupByAsync<TGroupKey, TResult>(options, keySelector, resultSelector, cancellationToken);
            }
        }

        #endregion
    }
}