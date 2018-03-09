namespace DotNetToolkit.Repository.Specifications
{
    using FetchStrategies;
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Encapsulate predicates and business rules that can be chainged together using boolean logic.
    /// <see cref="https://en.wikipedia.org/wiki/Specification_pattern" />
    /// </summary>
    /// <typeparam name="T">The type pf the entity.</typeparam>
    public interface ISpecification<T>
    {
        /// <summary>
        /// Gets the function to test the entity and determine if it satisfies the specified criteria.
        /// </summary>
        Expression<Func<T, bool>> Predicate { get; }

        /// <summary>
        /// Gets or Sets the fetch strategy which defines the child objects that should be retrieved when loading the entity.
        /// </summary>
        IFetchStrategy<T> FetchStrategy { get; set; }

        /// <summary>
        /// Returns a collection of entities that satisfied the criteria specified by the <see cref="Predicate"/> from the query.
        /// </summary>
        /// <param name="query">The entity query.</param>
        /// <returns>The collection of entities that satisfied the criteria specified by the <see cref="Predicate"/> from the query.</returns>
        IQueryable<T> SatisfyingEntitiesFrom(IQueryable<T> query);

        /// <summary>
        /// Determines wheter the entity that satisfied the criteria specified by the <see cref="Predicate"/>.
        /// </summary>
        /// <param name="entity">The entity to test.</param>
        /// <returns><c>true</c> if the entity satisfied the criteria specified by the <see cref="Predicate"/>; otherwise, <c>false</c>.</returns>
        bool IsSatisfiedBy(T entity);
    }
}
