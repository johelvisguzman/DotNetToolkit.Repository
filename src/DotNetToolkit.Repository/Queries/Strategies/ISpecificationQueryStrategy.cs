namespace DotNetToolkit.Repository.Queries.Strategies
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Encapsulate predicates and business rules that can be chainged together using boolean logic.
    /// <see cref="https://en.wikipedia.org/wiki/Specification_pattern" />
    /// </summary>
    /// <typeparam name="T">The type pf the entity.</typeparam>
    public interface ISpecificationQueryStrategy<T>
    {
        /// <summary>
        /// Gets the function to test the entity and determine if it satisfies the specified criteria.
        /// </summary>
        Expression<Func<T, bool>> Predicate { get; }
        
        /// <summary>
        /// Returns a collection of entities that satisfied the criteria specified by the <see cref="Predicate" /> from the query.
        /// </summary>
        /// <param name="query">The entity query.</param>
        /// <returns>The collection of entities that satisfied the criteria specified by the <see cref="Predicate" /> from the query.</returns>
        IQueryable<T> SatisfyingEntitiesFrom(IQueryable<T> query);

        /// <summary>
        /// Determines wheter the entity that satisfied the criteria specified by the <see cref="Predicate" />.
        /// </summary>
        /// <param name="entity">The entity to test.</param>
        /// <returns><c>true</c> if the entity satisfied the criteria specified by the <see cref="Predicate" />; otherwise, <c>false</c>.</returns>
        bool IsSatisfiedBy(T entity);
    }
}
