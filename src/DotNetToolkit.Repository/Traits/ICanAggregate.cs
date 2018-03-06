namespace DotNetToolkit.Repository.Traits
{
    using Specifications;
    using System;
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
    }
}
