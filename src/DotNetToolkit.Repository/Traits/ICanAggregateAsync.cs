namespace DotNetToolkit.Repository.Traits
{
    using Queries;
    using Specifications;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an asynchronous trait for aggregating items from a repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface ICanAggregateAsync<TEntity> : ICanAggregate<TEntity> where TEntity : class
    {
        /// <summary>
        /// Asynchronously returns the number of entities contained in the repository.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of entities contained in the repository.</returns>
        Task<int> CountAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Asynchronously returns the number of entities that satisfies the criteria specified by the <paramref name="criteria" /> in the repository.
        /// </summary>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of entities that satisfied the criteria specified by the <paramref name="criteria" /> in the repository.</returns>
        Task<int> CountAsync(ISpecification<TEntity> criteria, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Asynchronously returns the number of entities that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of entities that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Asynchronously returns new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        Task<Dictionary<TDictionaryKey, TEntity>> ToDictionaryAsync<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector, IQueryOptions<TEntity> options = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Asynchronously returns new <see cref="Dictionary{TDictionaryKey, TElemen}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        Task<Dictionary<TDictionaryKey, TElement>> ToDictionaryAsync<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, IQueryOptions<TEntity> options = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Asynchronously returns new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        Task<Dictionary<TDictionaryKey, TEntity>> ToDictionaryAsync<TDictionaryKey>(ISpecification<TEntity> criteria, Expression<Func<TEntity, TDictionaryKey>> keySelector, IQueryOptions<TEntity> options = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Asynchronously returns new <see cref="Dictionary{TDictionaryKey, TElemen}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        Task<Dictionary<TDictionaryKey, TElement>> ToDictionaryAsync<TDictionaryKey, TElement>(ISpecification<TEntity> criteria, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, IQueryOptions<TEntity> options = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Asynchronously returns a new <see cref="IGrouping{TKey,TElement}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="IGrouping{TGroupKey, TEntity}" /> that contains keys and values.</returns>
        Task<IEnumerable<IGrouping<TGroupKey, TEntity>>> GroupByAsync<TGroupKey>(Expression<Func<TEntity, TGroupKey>> keySelector, IQueryOptions<TEntity> options = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Asynchronously returns a new <see cref="IGrouping{TGroupKey, TElemen}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="IGrouping{TGroupKey, TEntity}" /> that contains keys and values.</returns>
        Task<IEnumerable<IGrouping<TGroupKey, TElement>>> GroupByAsync<TGroupKey, TElement>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, IQueryOptions<TEntity> options = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Asynchronously returns a new <see cref="IGrouping{TGroupKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="IGrouping{TGroupKey, TEntity}" /> that contains keys and values.</returns>
        Task<IEnumerable<IGrouping<TGroupKey, TEntity>>> GroupByAsync<TGroupKey>(ISpecification<TEntity> criteria, Expression<Func<TEntity, TGroupKey>> keySelector, IQueryOptions<TEntity> options = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Asynchronously returns a new <see cref="IGrouping{TGroupKey, TElemen}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="IGrouping{TGroupKey, TEntity}" /> that contains keys and values.</returns>
        Task<IEnumerable<IGrouping<TGroupKey, TElement>>> GroupByAsync<TGroupKey, TElement>(ISpecification<TEntity> criteria, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, IQueryOptions<TEntity> options = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}
