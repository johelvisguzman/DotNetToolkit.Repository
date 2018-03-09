namespace DotNetToolkit.Repository.Specifications
{
    using FetchStrategies;
    using System;
    using System.Collections.Generic;
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
            FetchStrategy = new FetchStrategy<T>();
        }

        #endregion

        #region Public Methods

        /// <summary>    
        /// Returns a new specification which has been combined with the current and the specified specification using the logical "and".
        /// </summary>    
        public ISpecification<T> And(Specification<T> specification)
        {
            return GetSpecification(Predicate.And(specification.Predicate), GetFetchStrategy(specification.FetchStrategy));
        }

        /// <summary>    
        /// Returns a new specification which has been combined with the current and the specified predicate using the logical "and".
        /// </summary>    
        public ISpecification<T> And(Expression<Func<T, bool>> predicate)
        {
            return GetSpecification(Predicate.And(predicate), FetchStrategy);
        }

        /// <summary>    
        /// Returns a new specification which has been combined with the current and the specified specification using the logical "or".
        /// </summary>    
        public ISpecification<T> Or(Specification<T> specification)
        {
            return GetSpecification(Predicate.Or(specification.Predicate), GetFetchStrategy(specification.FetchStrategy));
        }

        /// <summary>    
        /// Returns a new specification which has been combined with the current and the specified predicate using the logical "or".
        /// </summary>    
        public ISpecification<T> Or(Expression<Func<T, bool>> predicate)
        {
            return GetSpecification(Predicate.Or(predicate), FetchStrategy);
        }

        /// <summary>    
        /// Returns a new specification which negates the current.
        /// </summary>    
        public ISpecification<T> Not()
        {
            return GetSpecification(Predicate.Not());
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets a new specification using the specificied predicate.
        /// </summary>
        protected virtual Specification<T> GetSpecification(Expression<Func<T, bool>> predicate, IFetchStrategy<T> strategy = null)
        {
            var specification = new Specification<T>(predicate);
            if (strategy != null)
                specification.FetchStrategy = strategy;

            return specification;
        }

        /// <summary>
        /// Gets a new fetch strategy using the specificied strategy.
        /// </summary>
        protected virtual IFetchStrategy<T> GetFetchStrategy(IFetchStrategy<T> strategy)
        {
            var thisPaths = FetchStrategy != null ? FetchStrategy.IncludePaths : new List<string>();
            var paramPaths = strategy != null ? strategy.IncludePaths : new List<string>();
            var includePaths = thisPaths.Union(paramPaths);

            var newStrategy = new FetchStrategy<T>();
            foreach (var includePath in includePaths)
            {
                newStrategy.Include(includePath);
            }

            return newStrategy;
        }


        #endregion

        #region Implementation of ISpecification<T>

        /// <summary>
        /// Gets the function to test the entity and determine if it satisfies the specified criteria.
        /// </summary>
        public Expression<Func<T, bool>> Predicate { get; }

        /// <summary>
        /// Gets or Sets the fetch strategy which defines the child objects that should be retrieved when loading the entity.
        /// </summary>
        public IFetchStrategy<T> FetchStrategy { get; set; }

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
