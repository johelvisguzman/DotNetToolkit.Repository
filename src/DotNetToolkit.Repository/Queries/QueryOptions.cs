namespace DotNetToolkit.Repository.Queries
{
    using FetchStrategies;
    using Helpers;
    using Specifications;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// An implementation of <see cref="IQueryOptions{T}" />.
    /// </summary>
    /// <typeparam name="T">The entity type of the repository.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.Queries.IQueryOptions{T}" />
    public class QueryOptions<T> : IQueryOptions<T> where T : class
    {
        #region Fields

        private const int DefaultPageSize = 100;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryOptions{T}"/> class.
        /// </summary>
        public QueryOptions()
        {
            PageSize = -1;
            PageIndex = -1;

            SortingPropertiesMapping = new Dictionary<string, bool>();
        }

        #endregion

        #region Implementation of IQueryOptions<T>

        /// <summary>
        /// Gets the number of rows of the data page to retrieve.
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// Gets the zero-based index of the data page to retrieve.
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// Gets a collection of sorting property paths.
        /// </summary>
        public IDictionary<string, bool> SortingPropertiesMapping { get; }

        /// <summary>
        /// Applies a ascending sort order according to the specified property name.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The current instance.</returns>
        public IQueryOptions<T> SortBy(string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            // Check if a primary sorting has been applied already
            if (SortingPropertiesMapping.Count > 0)
                return this;

            SortingPropertiesMapping.Add(propertyName, false);

            return this;
        }

        /// <summary>
        /// Applies a secondary ascending sort order according to the specified property name.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The current instance.</returns>
        public IQueryOptions<T> ThenSortBy(string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            if (SortingPropertiesMapping.Count == 0)
                throw new InvalidOperationException("Cannot perform sorting action. A primary sorting will need to be applied first.");

            if (!SortingPropertiesMapping.ContainsKey(propertyName))
                SortingPropertiesMapping.Add(propertyName, false);

            return this;
        }

        /// <summary>
        /// Applies a descending sort order according to the specified property name.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The current instance.</returns>
        public IQueryOptions<T> SortByDescending(string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            // Check if a primary sorting has been applied already
            if (SortingPropertiesMapping.Count > 0)
                return this;

            SortingPropertiesMapping.Add(propertyName, true);

            return this;
        }

        /// <summary>
        /// Applies a secondary descending sort order according to the specified property name.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The current instance.</returns>
        public IQueryOptions<T> ThenSortByDescending(string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            if (SortingPropertiesMapping.Count == 0)
                throw new InvalidOperationException("Cannot perform sorting action. A primary sorting will need to be applied first.");

            if (!SortingPropertiesMapping.ContainsKey(propertyName))
                SortingPropertiesMapping.Add(propertyName, true);

            return this;
        }

        /// <summary>
        /// Applies a ascending sort order according to the specified property name.
        /// </summary>
        /// <param name="property">The sorting property expression.</param>
        /// <returns>The current instance.</returns>
        public IQueryOptions<T> SortBy(Expression<Func<T, object>> property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            return SortBy(ExpressionHelper.GetPropertyPath(property));
        }

        /// <summary>
        /// Applies a primary descending sort order according to the specified property name.
        /// </summary>
        /// <param name="property">The sorting property expression.</param>
        /// <returns>The current instance.</returns>
        public IQueryOptions<T> ThenSortBy(Expression<Func<T, object>> property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            return ThenSortBy(ExpressionHelper.GetPropertyPath(property));
        }

        /// <summary>
        /// Applies a descending sort order according to the specified property name.
        /// </summary>
        /// <param name="property">The sorting property expression.</param>
        /// <returns>The current instance.</returns>
        public IQueryOptions<T> SortByDescending(Expression<Func<T, object>> property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            return SortByDescending(ExpressionHelper.GetPropertyPath(property));
        }

        /// <summary>
        /// Applies a secondary descending sort order according to the specified property name.
        /// </summary>
        /// <param name="property">The sorting property expression.</param>
        /// <returns>The current instance.</returns>
        public IQueryOptions<T> ThenSortByDescending(Expression<Func<T, object>> property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            return ThenSortByDescending(ExpressionHelper.GetPropertyPath(property));
        }

        /// <summary>
        /// Applies paging according to the specified index and page size.
        /// </summary>
        /// <param name="pageIndex">The zero-based index of the data page to retrieve.</param>
        /// <param name="pageSize">The number of rows of the data page to retrieve.</param>
        /// <returns>The current instance.</returns>
        public IQueryOptions<T> Page(int pageIndex, int pageSize)
        {
            if (pageIndex < 1)
                throw new ArgumentException("Cannot be lower than 1.", nameof(pageIndex));

            if (pageSize <= 0)
                throw new ArgumentException("Cannot be lower than zero.", nameof(pageSize));

            PageIndex = pageIndex;
            PageSize = pageSize;

            return this;
        }

        /// <summary>
        /// Applies paging according to the specified index and a default page size of 100 items.
        /// </summary>
        /// <param name="pageIndex">The zero-based index of the data page to retrieve.</param>
        /// <returns>The current instance.</returns>
        public IQueryOptions<T> Page(int pageIndex)
        {
            var pageSize = PageSize == -1 ? DefaultPageSize : PageSize;

            return Page(pageIndex, pageSize);
        }

        /// <summary>
        /// Applies a criteria that is used for matching entities against and combine it with the current specified predicate using the logical "and".
        /// </summary>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <returns>The current instance.</returns>
        public IQueryOptions<T> SatisfyBy(ISpecification<T> criteria)
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            var predicate = Specification != null ? Specification.Predicate.And(criteria.Predicate) : criteria.Predicate;

            Specification = new Specification<T>(predicate);

            return this;
        }

        /// <summary>
        /// Applies a criteria that is used for matching entities against and combine it with the current specified predicate using the logical "and".
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The current instance.</returns>
        public IQueryOptions<T> SatisfyBy(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            predicate = Specification != null ? Specification.Predicate.And(predicate) : predicate;

            Specification = new Specification<T>(predicate);

            return this;
        }

        /// <summary>
        /// Applies the fetch strategy which defines the child objects that should be retrieved when loading the entity.
        /// </summary>
        /// <param name="fetchStrategy">The fetch strategy.</param>
        /// <returns>The current instance.</returns>
        public IQueryOptions<T> Fetch(IFetchStrategy<T> fetchStrategy)
        {
            if (fetchStrategy == null)
                throw new ArgumentNullException(nameof(fetchStrategy));

            var paths = FetchStrategy != null ? FetchStrategy.IncludePaths : new List<string>();
            var mergedPaths = paths.Union(fetchStrategy.IncludePaths).ToList();

            FetchStrategy = FetchStrategy ?? new FetchStrategy<T>();

            mergedPaths.ForEach(path => FetchStrategy.Include(path));

            return this;
        }

        /// <summary>
        /// Specifies the related objects to include in the query results.
        /// </summary>
        /// <param name="path">The dot-separated list of related objects to return in the query results.</param>
        /// <returns>The current instance.</returns>
        public IQueryOptions<T> Fetch(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            FetchStrategy = FetchStrategy ?? new FetchStrategy<T>();

            FetchStrategy.Include(path);

            return this;
        }

        /// <summary>
        /// Specifies the related objects to include in the query results.
        /// </summary>
        /// <param name="path">A lambda expression representing the path to include.</param>
        /// <returns>The current instance.</returns>
        public IQueryOptions<T> Fetch(Expression<Func<T, object>> path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return Fetch(path.ToIncludeString());
        }

        /// <summary>
        /// Gets the fetch strategy which defines the child objects that should be retrieved when loading the entity.
        /// </summary>
        public IFetchStrategy<T> FetchStrategy { get; private set; }

        /// <summary>
        /// Gets the specification.
        /// </summary>
        public ISpecification<T> Specification { get; private set; }

        #endregion
    }
}
