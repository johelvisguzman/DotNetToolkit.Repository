namespace DotNetToolkit.Repository.Queries.Strategies
{
    using JetBrains.Annotations;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="ISpecificationQueryStrategy{T}" />.
    /// </summary>
    public class SpecificationQueryStrategy<T> : ISpecificationQueryStrategy<T>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecificationQueryStrategy{T}" /> class.
        /// </summary>
        /// <param name="predicate">A function to test the entity and determine if it satisfies the specified criteria.</param>
        public SpecificationQueryStrategy([NotNull] Expression<Func<T, bool>> predicate)
        {
            Predicate = Guard.NotNull(predicate, nameof(predicate));
        }

        #endregion

        #region Public Methods

        /// <summary>    
        /// Returns a new specificationStrategy which has been combined with the current and the specified specificationStrategy using the logical "and".
        /// </summary>    
        public ISpecificationQueryStrategy<T> And([NotNull] SpecificationQueryStrategy<T> specificationStrategy)
        {
            return new SpecificationQueryStrategy<T>(Predicate.And(Guard.NotNull(specificationStrategy, nameof(specificationStrategy)).Predicate));
        }

        /// <summary>    
        /// Returns a new specificationStrategy which has been combined with the current specified predicate using the logical "and".
        /// </summary>    
        public ISpecificationQueryStrategy<T> And([NotNull] Expression<Func<T, bool>> predicate)
        {
            return new SpecificationQueryStrategy<T>(Predicate.And(Guard.NotNull(predicate, nameof(predicate))));
        }

        /// <summary>    
        /// Returns a new specificationStrategy which has been combined with the current and the specified specificationStrategy using the logical "or".
        /// </summary>    
        public ISpecificationQueryStrategy<T> Or([NotNull] SpecificationQueryStrategy<T> specificationStrategy)
        {
            return new SpecificationQueryStrategy<T>(Predicate.Or(Guard.NotNull(specificationStrategy, nameof(specificationStrategy)).Predicate));
        }

        /// <summary>    
        /// Returns a new specificationStrategy which has been combined with the current specified predicate using the logical "or".
        /// </summary>    
        public ISpecificationQueryStrategy<T> Or([NotNull] Expression<Func<T, bool>> predicate)
        {
            return new SpecificationQueryStrategy<T>(Predicate.Or(Guard.NotNull(predicate, nameof(predicate))));
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
        /// Returns a collection of entities that satisfied the criteria specified by the <see cref="ISpecificationQueryStrategy{T}.Predicate" /> from the query.
        /// </summary>
        /// <param name="query">The entity query.</param>
        /// <returns>The collection of entities that satisfied the criteria specified by the <see cref="ISpecificationQueryStrategy{T}.Predicate" /> from the query.</returns>
        public virtual IQueryable<T> SatisfyingEntitiesFrom([NotNull] IQueryable<T> query)
        {
            return Guard.NotNull(query, nameof(query)).Where(Predicate);
        }

        /// <summary>
        /// Determines wheter the entity that satisfied the criteria specified by the <see cref="ISpecificationQueryStrategy{T}.Predicate" />.
        /// </summary>
        /// <param name="entity">The entity to test.</param>
        /// <returns><c>true</c> if the entity satisfied the criteria specified by the <see cref="ISpecificationQueryStrategy{T}.Predicate" />; otherwise, <c>false</c>.</returns>
        public bool IsSatisfiedBy([NotNull] T entity)
        {
            return new[] { Guard.NotNull(entity, nameof(entity)) }
                .AsQueryable()
                .Any(Predicate);
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
