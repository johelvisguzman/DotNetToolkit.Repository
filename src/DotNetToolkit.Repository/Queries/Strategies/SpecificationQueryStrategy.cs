namespace DotNetToolkit.Repository.Queries.Strategies
{
    using Helpers;
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// An implementation of <see cref="ISpecificationQueryStrategy{T}" />.
    /// </summary>
    public class SpecificationQueryStrategy<T> : ISpecificationQueryStrategy<T>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ISpecificationQueryStrategyQueryStrategy{T}" /> class.
        /// </summary>
        /// <param name="predicate">A function to test the entity and determine if it satisfies the specified criteria.</param>
        public SpecificationQueryStrategy(Expression<Func<T, bool>> predicate)
        {
            Predicate = predicate;
        }

        #endregion

        #region Public Methods

        /// <summary>    
        /// Returns a new specificationStrategy which has been combined with the current and the specified specificationStrategy using the logical "and".
        /// </summary>    
        public ISpecificationQueryStrategy<T> And(SpecificationQueryStrategy<T> specificationStrategy)
        {
            return new SpecificationQueryStrategy<T>(Predicate.And(specificationStrategy.Predicate));
        }

        /// <summary>    
        /// Returns a new specificationStrategy which has been combined with the current specified predicate using the logical "and".
        /// </summary>    
        public ISpecificationQueryStrategy<T> And(Expression<Func<T, bool>> predicate)
        {
            return new SpecificationQueryStrategy<T>(Predicate.And(predicate));
        }

        /// <summary>    
        /// Returns a new specificationStrategy which has been combined with the current and the specified specificationStrategy using the logical "or".
        /// </summary>    
        public ISpecificationQueryStrategy<T> Or(SpecificationQueryStrategy<T> specificationStrategy)
        {
            return new SpecificationQueryStrategy<T>(Predicate.Or(specificationStrategy.Predicate));
        }

        /// <summary>    
        /// Returns a new specificationStrategy which has been combined with the current specified predicate using the logical "or".
        /// </summary>    
        public ISpecificationQueryStrategy<T> Or(Expression<Func<T, bool>> predicate)
        {
            return new SpecificationQueryStrategy<T>(Predicate.Or(predicate));
        }

        /// <summary>    
        /// Returns a new specificationStrategy which negates the current.
        /// </summary>    
        public ISpecificationQueryStrategy<T> Not()
        {
            return new SpecificationQueryStrategy<T>(Predicate.Not());
        }

        #endregion

        #region Implementation of ISpecificationQueryStrategy<T>

        /// <summary>
        /// Gets the function to test the entity and determine if it satisfies the specified criteria.
        /// </summary>
        public Expression<Func<T, bool>> Predicate { get; }

        /// <summary>
        /// Returns a collection of entities that satisfied the criteria specified by the <see cref="ISpecificationQueryStrategyQueryStrategy{T}.Predicate" /> from the query.
        /// </summary>
        /// <param name="query">The entity query.</param>
        /// <returns>The collection of entities that satisfied the criteria specified by the <see cref="ISpecificationQueryStrategyQueryStrategy{T}.Predicate" /> from the query.</returns>
        public virtual IQueryable<T> SatisfyingEntitiesFrom(IQueryable<T> query)
        {
            return Predicate == null ? query : query.Where(Predicate);
        }

        /// <summary>
        /// Determines wheter the entity that satisfied the criteria specified by the <see cref="ISpecificationQueryStrategyQueryStrategy{T}.Predicate" />.
        /// </summary>
        /// <param name="entity">The entity to test.</param>
        /// <returns><c>true</c> if the entity satisfied the criteria specified by the <see cref="ISpecificationQueryStrategyQueryStrategy{T}.Predicate" />; otherwise, <c>false</c>.</returns>
        public bool IsSatisfiedBy(T entity)
        {
            return Predicate == null || new[] { entity }.AsQueryable().Any(Predicate);
        }

        #endregion

        #region Overrides of Object

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"SpecificationQueryStrategy<{typeof(T).Name}>: [ Predicate = {ExpressionHelper.TranslateToString(Predicate)} ]";
        }

        #endregion
    }
}
