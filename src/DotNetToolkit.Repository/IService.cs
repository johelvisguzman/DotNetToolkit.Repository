namespace DotNetToolkit.Repository
{
    using Queries;
    using Queries.Strategies;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a service that provides data access and which follows the unit of work and repository pattern with a composite primary key.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    /// <typeparam name="TKey3">The type of the third part of the composite primary key.</typeparam>
    /// <seealso cref="System.IDisposable" />
    public interface IService<TEntity, in TKey1, in TKey2, in TKey3> where TEntity : class
    {
        /// <summary>
        /// Adds the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        void Create(TEntity entity);

        /// <summary>
        /// Adds the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        void Create(IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <returns><c>true</c> is able to successfully delete an entity with the given composite primary key values; otherwise, <c>false</c>.</returns>
        bool TryDelete(TKey1 key1, TKey2 key2, TKey3 key3);

        /// <summary>
        /// Deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        void Delete(TKey1 key1, TKey2 key2, TKey3 key3);

        /// <summary>
        /// Deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Deletes all the entities in the repository that satisfies the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        void Delete(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        void Delete(IQueryOptions<TEntity> options);

        /// <summary>
        /// Deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        void Delete(IEnumerable<TEntity> entities);

        /// <summary>
        /// Updates the specified <paramref name="entity" /> in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Updates the specified <paramref name="entities" /> collection in the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        void Update(IEnumerable<TEntity> entities);

        /// <summary>
        /// Gets an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <return>The entity found.</return>
        TEntity Get(TKey1 key1, TKey2 key2, TKey3 key3);

        /// <summary>
        /// Gets an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <return>The entity found.</return>
        TEntity Get(TKey1 key1, TKey2 key2, TKey3 key3, IFetchQueryStrategy<TEntity> fetchStrategy);

        /// <summary>
        /// Gets the first entity in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The entity that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        TEntity Get(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Gets the first entity in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The entity that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        TEntity Get(IQueryOptions<TEntity> options);

        /// <summary>
        /// Gets the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        TResult Get<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector);

        /// <summary>
        /// Gets the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        TResult Get<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector);

        /// <summary>
        /// Gets the collection of entities in the repository.
        /// </summary>
        /// <returns>The collection of entities in the repository.</returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Gets the collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Gets the collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        IQueryResult<IEnumerable<TEntity>> GetAll(IQueryOptions<TEntity> options);

        /// <summary>
        /// Gets the collection of projected entity results in the repository.
        /// </summary>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository.</returns>
        IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector);

        /// <summary>
        /// Gets the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector);

        /// <summary>
        /// Gets the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        IQueryResult<IEnumerable<TResult>> GetAll<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector);

        /// <summary>
        /// Determines whether the repository contains an entity with the given composite primary key values.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        bool GetExists(TKey1 key1, TKey2 key2, TKey3 key3);

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">The predicate used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified predicate; otherwise, <c>false</c>.</returns>
        bool GetExists(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        bool GetExists(IQueryOptions<TEntity> options);

        /// <summary>
        /// Returns the number of entities contained in the repository.
        /// </summary>
        /// <returns>The number of entities contained in the repository.</returns>
        int GetCount();

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        int GetCount(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        int GetCount(IQueryOptions<TEntity> options);

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        Dictionary<TDictionaryKey, TEntity> GetDictionary<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector);

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        IQueryResult<Dictionary<TDictionaryKey, TEntity>> GetDictionary<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector);

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        Dictionary<TDictionaryKey, TElement> GetDictionary<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector);

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, and an element selector function with entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        IQueryResult<Dictionary<TDictionaryKey, TElement>> GetDictionary<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector);

        /// <summary>
        /// Returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <returns>A new <see cref="IGrouping{TKey,TElement}" /> that contains keys and values.</returns>
        IEnumerable<TResult> GetGroupBy<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector);

        /// <summary>
        /// Returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <returns>A new <see cref="IEnumerable{TResult}" /> that contains the grouped result that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        IQueryResult<IEnumerable<TResult>> GetGroupBy<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector);

        /// <summary>
        /// Asynchronously adds the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task CreateAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously adds the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task CreateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> is able to successfully delete an entity with the given composite primary key values; otherwise, <c>false</c>.</returns>
        Task<bool> TryDeleteAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously deletes all the entities in the repository that satisfies the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously updates the specified <paramref name="entity" /> in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously updates the specified <paramref name="entities" /> collection in the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        Task<TEntity> GetAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        Task<TEntity> GetAsync(TKey1 key1, TKey2 key2, TKey3 key3, IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the first entity in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  entity that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the first entity in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  entity that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        Task<TEntity> GetAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        Task<TResult> GetAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        Task<TResult> GetAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of entities in the repository.</returns>
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        Task<IQueryResult<IEnumerable<TEntity>>> GetAllAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository.
        /// </summary>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of projected entity results in the repository.</returns>
        Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        Task<IQueryResult<IEnumerable<TResult>>> GetAllAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity with the given composite primary key values.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        Task<bool> GetExistsAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">The predicate used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the conditions defined by the specified predicate; otherwise, <c>false</c>.</returns>
        Task<bool> GetExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        Task<bool> GetExistsAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously returns the number of entities contained in the repository.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  number of entities contained in the repository.</returns>
        Task<int> GetCountAsync(CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously returns the number of entities that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  number of entities that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        Task<int> GetCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        Task<int> GetCountAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        Task<Dictionary<TDictionaryKey, TEntity>> GetDictionaryAsync<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        Task<IQueryResult<Dictionary<TDictionaryKey, TEntity>>> GetDictionaryAsync<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        Task<Dictionary<TDictionaryKey, TElement>> GetDictionaryAsync<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken());

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
        Task<IQueryResult<Dictionary<TDictionaryKey, TElement>>> GetDictionaryAsync<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="IGrouping{TKey,TElement}" /> that contains keys and values.</returns>
        Task<IEnumerable<TResult>> GetGroupByAsync<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken());

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
        Task<IQueryResult<IEnumerable<TResult>>> GetGroupByAsync<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken());
    }

    /// <summary>
    /// Represents a service that provides data access and which follows the unit of work and repository pattern with a composite primary key.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    /// <seealso cref="System.IDisposable" />
    public interface IService<TEntity, in TKey1, in TKey2> where TEntity : class
    {
        /// <summary>
        /// Adds the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        void Create(TEntity entity);

        /// <summary>
        /// Adds the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        void Create(IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <returns><c>true</c> is able to successfully delete an entity with the given composite primary key values; otherwise, <c>false</c>.</returns>
        bool TryDelete(TKey1 key1, TKey2 key2);

        /// <summary>
        /// Deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        void Delete(TKey1 key1, TKey2 key2);

        /// <summary>
        /// Deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Deletes all the entities in the repository that satisfies the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        void Delete(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        void Delete(IQueryOptions<TEntity> options);

        /// <summary>
        /// Deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        void Delete(IEnumerable<TEntity> entities);

        /// <summary>
        /// Updates the specified <paramref name="entity" /> in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Updates the specified <paramref name="entities" /> collection in the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        void Update(IEnumerable<TEntity> entities);

        /// <summary>
        /// Gets an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <return>The entity found.</return>
        TEntity Get(TKey1 key1, TKey2 key2);

        /// <summary>
        /// Gets an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <return>The entity found.</return>
        TEntity Get(TKey1 key1, TKey2 key2, IFetchQueryStrategy<TEntity> fetchStrategy);

        /// <summary>
        /// Gets the first entity in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The entity that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        TEntity Get(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Gets the first entity in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The entity that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        TEntity Get(IQueryOptions<TEntity> options);

        /// <summary>
        /// Gets the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        TResult Get<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector);

        /// <summary>
        /// Gets the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        TResult Get<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector);

        /// <summary>
        /// Gets the collection of entities in the repository.
        /// </summary>
        /// <returns>The collection of entities in the repository.</returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Gets the collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Gets the collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        IQueryResult<IEnumerable<TEntity>> GetAll(IQueryOptions<TEntity> options);

        /// <summary>
        /// Gets the collection of projected entity results in the repository.
        /// </summary>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository.</returns>
        IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector);

        /// <summary>
        /// Gets the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector);

        /// <summary>
        /// Gets the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        IQueryResult<IEnumerable<TResult>> GetAll<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector);

        /// <summary>
        /// Determines whether the repository contains an entity with the given composite primary key values.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        bool GetExists(TKey1 key1, TKey2 key2);

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">The predicate used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified predicate; otherwise, <c>false</c>.</returns>
        bool GetExists(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        bool GetExists(IQueryOptions<TEntity> options);

        /// <summary>
        /// Returns the number of entities contained in the repository.
        /// </summary>
        /// <returns>The number of entities contained in the repository.</returns>
        int GetCount();

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        int GetCount(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        int GetCount(IQueryOptions<TEntity> options);

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        Dictionary<TDictionaryKey, TEntity> GetDictionary<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector);

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        IQueryResult<Dictionary<TDictionaryKey, TEntity>> GetDictionary<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector);

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        Dictionary<TDictionaryKey, TElement> GetDictionary<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector);

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, and an element selector function with entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        IQueryResult<Dictionary<TDictionaryKey, TElement>> GetDictionary<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector);

        /// <summary>
        /// Returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <returns>A new <see cref="IGrouping{TKey,TElement}" /> that contains keys and values.</returns>
        IEnumerable<TResult> GetGroupBy<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector);

        /// <summary>
        /// Returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <returns>A new <see cref="IEnumerable{TResult}" /> that contains the grouped result that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        IQueryResult<IEnumerable<TResult>> GetGroupBy<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector);

        /// <summary>
        /// Asynchronously adds the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task CreateAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously adds the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task CreateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> is able to successfully delete an entity with the given composite primary key values; otherwise, <c>false</c>.</returns>
        Task<bool> TryDeleteAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously deletes all the entities in the repository that satisfies the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously updates the specified <paramref name="entity" /> in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously updates the specified <paramref name="entities" /> collection in the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        Task<TEntity> GetAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        Task<TEntity> GetAsync(TKey1 key1, TKey2 key2, IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the first entity in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  entity that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the first entity in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  entity that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        Task<TEntity> GetAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        Task<TResult> GetAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        Task<TResult> GetAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of entities in the repository.</returns>
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        Task<IQueryResult<IEnumerable<TEntity>>> GetAllAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository.
        /// </summary>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of projected entity results in the repository.</returns>
        Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        Task<IQueryResult<IEnumerable<TResult>>> GetAllAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity with the given composite primary key values.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        Task<bool> GetExistsAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">The predicate used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the conditions defined by the specified predicate; otherwise, <c>false</c>.</returns>
        Task<bool> GetExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        Task<bool> GetExistsAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously returns the number of entities contained in the repository.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  number of entities contained in the repository.</returns>
        Task<int> GetCountAsync(CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously returns the number of entities that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  number of entities that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        Task<int> GetCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        Task<int> GetCountAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        Task<Dictionary<TDictionaryKey, TEntity>> GetDictionaryAsync<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        Task<IQueryResult<Dictionary<TDictionaryKey, TEntity>>> GetDictionaryAsync<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        Task<Dictionary<TDictionaryKey, TElement>> GetDictionaryAsync<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken());

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
        Task<IQueryResult<Dictionary<TDictionaryKey, TElement>>> GetDictionaryAsync<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="IGrouping{TKey,TElement}" /> that contains keys and values.</returns>
        Task<IEnumerable<TResult>> GetGroupByAsync<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken());

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
        Task<IQueryResult<IEnumerable<TResult>>> GetGroupByAsync<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken());
    }

    /// <summary>
    /// Represents a service that provides data access and which follows the unit of work and repository pattern with a primary key.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the primary key.</typeparam>
    /// <seealso cref="System.IDisposable" />
    public interface IService<TEntity, in TKey> where TEntity : class
    {
        /// <summary>
        /// Adds the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        void Create(TEntity entity);

        /// <summary>
        /// Adds the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        void Create(IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key"></param>
        /// <returns><c>true</c> is able to successfully delete an entity with the given primary key; otherwise, <c>false</c>.</returns>
        bool TryDelete(TKey key);

        /// <summary>
        /// Deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        void Delete(TKey key);

        /// <summary>
        /// Deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Deletes all the entities in the repository that satisfies the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        void Delete(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        void Delete(IQueryOptions<TEntity> options);

        /// <summary>
        /// Deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        void Delete(IEnumerable<TEntity> entities);

        /// <summary>
        /// Updates the specified <paramref name="entity" /> in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Updates the specified <paramref name="entities" /> collection in the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        void Update(IEnumerable<TEntity> entities);

        /// <summary>
        /// Gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <return>The entity found.</return>
        TEntity Get(TKey key);

        /// <summary>
        /// Gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <return>The entity found.</return>
        TEntity Get(TKey key, IFetchQueryStrategy<TEntity> fetchStrategy);

        /// <summary>
        /// Gets the first entity in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The entity that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        TEntity Get(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Gets the first entity in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The entity that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        TEntity Get(IQueryOptions<TEntity> options);

        /// <summary>
        /// Gets the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        TResult Get<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector);

        /// <summary>
        /// Gets the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        TResult Get<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector);

        /// <summary>
        /// Gets the collection of entities in the repository.
        /// </summary>
        /// <returns>The collection of entities in the repository.</returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Gets the collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Gets the collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        IQueryResult<IEnumerable<TEntity>> GetAll(IQueryOptions<TEntity> options);

        /// <summary>
        /// Gets the collection of projected entity results in the repository.
        /// </summary>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository.</returns>
        IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector);

        /// <summary>
        /// Gets the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector);

        /// <summary>
        /// Gets the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        IQueryResult<IEnumerable<TResult>> GetAll<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector);

        /// <summary>
        /// Determines whether the repository contains an entity with the given primary key value.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        bool GetExists(TKey key);

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">The predicate used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified predicate; otherwise, <c>false</c>.</returns>
        bool GetExists(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        bool GetExists(IQueryOptions<TEntity> options);

        /// <summary>
        /// Returns the number of entities contained in the repository.
        /// </summary>
        /// <returns>The number of entities contained in the repository.</returns>
        int GetCount();

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        int GetCount(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        int GetCount(IQueryOptions<TEntity> options);

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        Dictionary<TDictionaryKey, TEntity> GetDictionary<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector);

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        IQueryResult<Dictionary<TDictionaryKey, TEntity>> GetDictionary<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector);

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        Dictionary<TDictionaryKey, TElement> GetDictionary<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector);

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, and an element selector function with entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        IQueryResult<Dictionary<TDictionaryKey, TElement>> GetDictionary<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector);

        /// <summary>
        /// Returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <returns>A new <see cref="IGrouping{TKey,TElement}" /> that contains keys and values.</returns>
        IEnumerable<TResult> GetGroupBy<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector);

        /// <summary>
        /// Returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <returns>A new <see cref="IEnumerable{TResult}" /> that contains the grouped result that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        IQueryResult<IEnumerable<TResult>> GetGroupBy<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector);

        /// <summary>
        /// Asynchronously adds the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task CreateAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously adds the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task CreateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        ///  Asynchronously deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> is able to successfully delete an entity with the given primary key; otherwise, <c>false</c>.</returns>
        Task<bool> TryDeleteAsync(TKey key, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(TKey key, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously deletes all the entities in the repository that satisfies the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously updates the specified <paramref name="entity" /> in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously updates the specified <paramref name="entities" /> collection in the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        Task<TEntity> GetAsync(TKey key, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        Task<TEntity> GetAsync(TKey key, IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the first entity in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  entity that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the first entity in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  entity that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        Task<TEntity> GetAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        Task<TResult> GetAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        Task<TResult> GetAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of entities in the repository.</returns>
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        Task<IQueryResult<IEnumerable<TEntity>>> GetAllAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository.
        /// </summary>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of projected entity results in the repository.</returns>
        Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        Task<IQueryResult<IEnumerable<TResult>>> GetAllAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity with the given primary key value.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        Task<bool> GetExistsAsync(TKey key, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">The predicate used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the conditions defined by the specified predicate; otherwise, <c>false</c>.</returns>
        Task<bool> GetExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        Task<bool> GetExistsAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously returns the number of entities contained in the repository.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  number of entities contained in the repository.</returns>
        Task<int> GetCountAsync(CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously returns the number of entities that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  number of entities that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        Task<int> GetCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        Task<int> GetCountAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        Task<Dictionary<TDictionaryKey, TEntity>> GetDictionaryAsync<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        Task<IQueryResult<Dictionary<TDictionaryKey, TEntity>>> GetDictionaryAsync<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        Task<Dictionary<TDictionaryKey, TElement>> GetDictionaryAsync<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken());

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
        Task<IQueryResult<Dictionary<TDictionaryKey, TElement>>> GetDictionaryAsync<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="IGrouping{TKey,TElement}" /> that contains keys and values.</returns>
        Task<IEnumerable<TResult>> GetGroupByAsync<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken());

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
        Task<IQueryResult<IEnumerable<TResult>>> GetGroupByAsync<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken());
    }

    /// <summary>
    /// Represents a service that provides data access and which follows the unit of work and repository pattern with a default primary key value of type integer.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IService<TEntity> : IService<TEntity, int> where TEntity : class
    {
    }
}
