namespace DotNetToolkit.Repository.Traits
{
    using Queries;
    using Specifications;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Represents a trait for aggregating items from a repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface ICanAggregate<TEntity> where TEntity : class
    {
        /// <summary>
        /// Returns the number of entities contained in the repository.
        /// </summary>
        /// <returns>The number of entities contained in the repository.</returns>
        int Count();

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="criteria" /> in the repository.
        /// </summary>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="criteria" /> in the repository.</returns>
        int Count(ISpecification<TEntity> criteria);

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        int Count(Expression<Func<TEntity, bool>> predicate);
        
        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>A new  <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        Dictionary<TDictionaryKey, TEntity> ToDictionary<TDictionaryKey>(Func<TEntity, TDictionaryKey> keySelector, IQueryOptions<TEntity> options = null);

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TElemen}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>A new  <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        Dictionary<TDictionaryKey, TElement> ToDictionary<TDictionaryKey, TElement>(Func<TEntity, TDictionaryKey> keySelector, Func<TEntity, TElement> elementSelector, IQueryOptions<TEntity> options = null);

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>A new  <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        Dictionary<TDictionaryKey, TEntity> ToDictionary<TDictionaryKey>(ISpecification<TEntity> criteria, Func<TEntity, TDictionaryKey> keySelector, IQueryOptions<TEntity> options = null);
        
        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TElemen}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>A new  <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        Dictionary<TDictionaryKey, TElement> ToDictionary<TDictionaryKey, TElement>(ISpecification<TEntity> criteria, Func<TEntity, TDictionaryKey> keySelector, Func<TEntity, TElement> elementSelector, IQueryOptions<TEntity> options = null);

        /// <summary>
        /// Returns a new <see cref="IGrouping{TGroupKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>A new  <see cref="IGrouping{TGroupKey, TEntity}" /> that contains keys and values.</returns>
        IEnumerable<IGrouping<TGroupKey, TEntity>> GroupBy<TGroupKey>(Func<TEntity, TGroupKey> keySelector, IQueryOptions<TEntity> options = null);
        
        /// <summary>
        /// Returns a new <see cref="IGrouping{TGroupKey, TElemen}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>A new  <see cref="IGrouping{TGroupKey, TEntity}" /> that contains keys and values.</returns>
        IEnumerable<IGrouping<TGroupKey, TElement>> GroupBy<TGroupKey, TElement>(Func<TEntity, TGroupKey> keySelector, Func<TEntity, TElement> elementSelector, IQueryOptions<TEntity> options = null);

        /// <summary>
        /// Returns a new <see cref="IGrouping{TGroupKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>A new  <see cref="IGrouping{TGroupKey, TEntity}" /> that contains keys and values.</returns>
        IEnumerable<IGrouping<TGroupKey, TEntity>> GroupBy<TGroupKey>(ISpecification<TEntity> criteria, Func<TEntity, TGroupKey> keySelector, IQueryOptions<TEntity> options = null);

        /// <summary>
        /// Returns a new <see cref="IGrouping{TGroupKey, TElemen}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>A new  <see cref="IGrouping{TGroupKey, TEntity}" /> that contains keys and values.</returns>
        IEnumerable<IGrouping<TGroupKey, TElement>> GroupBy<TGroupKey, TElement>(ISpecification<TEntity> criteria, Func<TEntity, TGroupKey> keySelector, Func<TEntity, TElement> elementSelector, IQueryOptions<TEntity> options = null);
    }
}
