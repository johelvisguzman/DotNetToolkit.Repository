namespace DotNetToolkit.Repository.Specifications
{
    using Helpers;
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// An implementation of <see cref="ISpecification{T}" />.
    /// </summary>
    public class Specification<T> : ISpecification<T>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ISpecification{T}" /> class.
        /// </summary>
        /// <param name="predicate">A function to test the entity and determine if it satisfies the specified criteria.</param>
        public Specification(Expression<Func<T, bool>> predicate)
        {
            Predicate = predicate;
        }

        #endregion

        #region Public Methods

        /// <summary>    
        /// Returns a new specification which has been combined with the current and the specified specification using the logical "and".
        /// </summary>    
        public ISpecification<T> And(Specification<T> specification)
        {
            return new Specification<T>(Predicate.And(specification.Predicate));
        }

        /// <summary>    
        /// Returns a new specification which has been combined with the current specified predicate using the logical "and".
        /// </summary>    
        public ISpecification<T> And(Expression<Func<T, bool>> predicate)
        {
            return new Specification<T>(Predicate.And(predicate));
        }

        /// <summary>    
        /// Returns a new specification which has been combined with the current and the specified specification using the logical "or".
        /// </summary>    
        public ISpecification<T> Or(Specification<T> specification)
        {
            return new Specification<T>(Predicate.Or(specification.Predicate));
        }

        /// <summary>    
        /// Returns a new specification which has been combined with the current specified predicate using the logical "or".
        /// </summary>    
        public ISpecification<T> Or(Expression<Func<T, bool>> predicate)
        {
            return new Specification<T>(Predicate.Or(predicate));
        }

        /// <summary>    
        /// Returns a new specification which negates the current.
        /// </summary>    
        public ISpecification<T> Not()
        {
            return new Specification<T>(Predicate.Not());
        }

        #endregion

        #region Implementation of ISpecification<T>

        /// <summary>
        /// Gets the function to test the entity and determine if it satisfies the specified criteria.
        /// </summary>
        public Expression<Func<T, bool>> Predicate { get; }

        /// <summary>
        /// Returns a collection of entities that satisfied the criteria specified by the <see cref="ISpecification{T}.Predicate"/> from the query.
        /// </summary>
        /// <param name="query">The entity query.</param>
        /// <returns>The collection of entities that satisfied the criteria specified by the <see cref="ISpecification{T}.Predicate"/> from the query.</returns>
        public virtual IQueryable<T> SatisfyingEntitiesFrom(IQueryable<T> query)
        {
            return Predicate == null ? query : query.Where(Predicate);
        }

        /// <summary>
        /// Determines wheter the entity that satisfied the criteria specified by the <see cref="ISpecification{T}.Predicate"/>.
        /// </summary>
        /// <param name="entity">The entity to test.</param>
        /// <returns><c>true</c> if the entity satisfied the criteria specified by the <see cref="ISpecification{T}.Predicate"/>; otherwise, <c>false</c>.</returns>
        public bool IsSatisfiedBy(T entity)
        {
            return Predicate == null || new[] { entity }.AsQueryable().Any(Predicate);
        }

        #endregion
    }
}
