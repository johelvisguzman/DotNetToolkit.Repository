namespace DotNetToolkit.Repository.Queries
{
    using Extensions;
    using Helpers;
    using Strategies;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;

    /// <summary>
    /// An implementation of <see cref="IQueryOptions{T}" />.
    /// </summary>
    /// <typeparam name="T">The entity type of the repository.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.Queries.IQueryOptions{T}" />
    public class QueryOptions<T> : IQueryOptions<T>
    {
        #region Fields

        private int _pageSize;
        private int _pageIndex;
        private IFetchQueryStrategy<T> _fetchStrategy;
        private ISpecificationQueryStrategy<T> _specificationStrategy;
        private Dictionary<string, SortOrder> _sortingPropertiesMapping;

        private const int DefaultPageSize = 100;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryOptions{T}" /> class.
        /// </summary>
        public QueryOptions()
        {
            _pageSize = -1;
            _pageIndex = -1;

            _sortingPropertiesMapping = new Dictionary<string, SortOrder>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clones the current configured options to a new instance.
        /// </summary>
        /// <returns>The new clone instance.</returns>
        public QueryOptions<T> Clone()
        {
            var copy = new QueryOptions<T>();

            if (this._fetchStrategy != null)
                copy.Include(this._fetchStrategy);

            if (this._specificationStrategy != null)
                copy.Include(this._specificationStrategy);

            copy._sortingPropertiesMapping = this._sortingPropertiesMapping.ToDictionary(x => x.Key, x => x.Value);
            copy._pageIndex = this._pageIndex;
            copy._pageSize = this._pageSize;

            return copy;
        }

        /// <summary>
        /// Applies an ascending sort order according to the specified property name.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The current instance.</returns>
        public QueryOptions<T> OrderBy(string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            if (!_sortingPropertiesMapping.ContainsKey(propertyName))
                _sortingPropertiesMapping.Add(propertyName, SortOrder.Ascending);
            else
                _sortingPropertiesMapping[propertyName] = SortOrder.Ascending;

            return this;
        }

        /// <summary>
        /// Applies a descending sort order according to the specified property name.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The current instance.</returns>
        public QueryOptions<T> OrderByDescending(string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            if (!_sortingPropertiesMapping.ContainsKey(propertyName))
                _sortingPropertiesMapping.Add(propertyName, SortOrder.Descending);
            else
                _sortingPropertiesMapping[propertyName] = SortOrder.Descending;

            return this;
        }

        /// <summary>
        /// Applies an ascending sort order according to the specified property name.
        /// </summary>
        /// <param name="property">The sorting property expression.</param>
        /// <returns>The current instance.</returns>
        public QueryOptions<T> OrderBy(Expression<Func<T, object>> property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            return OrderBy(ExpressionHelper.GetPropertyPath(property));
        }

        /// <summary>
        /// Applies a descending sort order according to the specified property name.
        /// </summary>
        /// <param name="property">The sorting property expression.</param>
        /// <returns>The current instance.</returns>
        public QueryOptions<T> OrderByDescending(Expression<Func<T, object>> property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            return OrderByDescending(ExpressionHelper.GetPropertyPath(property));
        }

        /// <summary>
        /// Applies paging according to the specified index and page size.
        /// </summary>
        /// <param name="pageIndex">The zero-based index of the data page to retrieve.</param>
        /// <param name="pageSize">The number of rows of the data page to retrieve.</param>
        /// <returns>The current instance.</returns>
        public QueryOptions<T> Page(int pageIndex, int pageSize)
        {
            if (pageIndex < 1)
                throw new ArgumentException("Cannot be lower than 1.", nameof(pageIndex));

            if (pageSize <= 0)
                throw new ArgumentException("Cannot be lower than zero.", nameof(pageSize));

            _pageIndex = pageIndex;
            _pageSize = pageSize;

            return this;
        }

        /// <summary>
        /// Applies paging according to the specified index and a default page size of 100 items.
        /// </summary>
        /// <param name="pageIndex">The zero-based index of the data page to retrieve.</param>
        /// <returns>The current instance.</returns>
        public QueryOptions<T> Page(int pageIndex)
        {
            var pageSize = _pageSize == -1 ? DefaultPageSize : _pageSize;

            return Page(pageIndex, pageSize);
        }

        /// <summary>
        /// Includes a specification strategy/criteria that is used for matching entities against and combine it with the current specified predicate using the logical "and".
        /// </summary>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <returns>The current instance.</returns>
        public QueryOptions<T> Include(ISpecificationQueryStrategy<T> criteria)
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            var predicate = _specificationStrategy != null ? _specificationStrategy.Predicate.And(criteria.Predicate) : criteria.Predicate;

            _specificationStrategy = new SpecificationQueryStrategy<T>(predicate);

            return this;
        }

        /// <summary>
        /// Applies a criteria that is used for matching entities against and combine it with the current specified predicate using the logical "and".
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The current instance.</returns>
        public QueryOptions<T> SatisfyBy(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            predicate = _specificationStrategy != null ? _specificationStrategy.Predicate.And(predicate) : predicate;

            _specificationStrategy = new SpecificationQueryStrategy<T>(predicate);

            return this;
        }

        /// <summary>
        /// Includes the fetch strategy which defines the child objects that should be retrieved when loading the entity and combines it with the current property pahts collection.
        /// </summary>
        /// <param name="fetchStrategy">The fetch strategy.</param>
        /// <returns>The current instance.</returns>
        public QueryOptions<T> Include(IFetchQueryStrategy<T> fetchStrategy)
        {
            if (fetchStrategy == null)
                throw new ArgumentNullException(nameof(fetchStrategy));

            var paths = _fetchStrategy != null ? ((IFetchQueryStrategy<T>)_fetchStrategy).PropertyPaths : new List<string>();
            var mergedPaths = paths.Union(fetchStrategy.PropertyPaths).ToList();

            _fetchStrategy = _fetchStrategy ?? new FetchQueryStrategy<T>();

            mergedPaths.ForEach(path => _fetchStrategy.Fetch(path));

            return this;
        }

        /// <summary>
        /// Specifies the related objects to include in the query results.
        /// </summary>
        /// <param name="path">The dot-separated list of related objects to return in the query results.</param>
        /// <returns>The current instance.</returns>
        public QueryOptions<T> Fetch(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            _fetchStrategy = _fetchStrategy ?? new FetchQueryStrategy<T>();

            _fetchStrategy.Fetch(path);

            return this;
        }

        /// <summary>
        /// Specifies the related objects to include in the query results.
        /// </summary>
        /// <param name="path">A lambda expression representing the path to include.</param>
        /// <returns>The current instance.</returns>
        public QueryOptions<T> Fetch(Expression<Func<T, object>> path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return Fetch(path.ToIncludeString());
        }

        #endregion

        #region Implementation of IQueryOptions<T>

        /// <summary>
        /// Gets the number of rows of the data page to retrieve.
        /// </summary>
        int IQueryOptions<T>.PageSize { get { return _pageSize; } }

        /// <summary>
        /// Gets the zero-based index of the data page to retrieve.
        /// </summary>
        int IQueryOptions<T>.PageIndex { get { return _pageIndex; } }

        /// <summary>
        /// Gets a collection of sorting property paths.
        /// </summary>
        IReadOnlyDictionary<string, SortOrder> IQueryOptions<T>.SortingPropertiesMapping { get { return _sortingPropertiesMapping; } }

        /// <summary>
        /// Gets the fetch strategy which defines the child objects that should be retrieved when loading the entity.
        /// </summary>
        IFetchQueryStrategy<T> IQueryOptions<T>.FetchStrategy { get { return _fetchStrategy; } }

        /// <summary>
        /// Gets the specification.
        /// </summary>
        ISpecificationQueryStrategy<T> IQueryOptions<T>.SpecificationStrategy { get { return _specificationStrategy; } }

        #endregion

        #region Overrides of Object

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append($"QueryOptions<{typeof(T).Name}>: [ ");

            sb.Append(_specificationStrategy != null
                ? $"\n\t{_specificationStrategy.ToString()},"
                : $"\n\tSpecificationQueryStrategy<{typeof(T).Name}>: [ null ],");

            sb.Append(_fetchStrategy != null
                ? $"\n\t{_fetchStrategy.ToString()},"
                : $"\n\tFetchQueryStrategy<{typeof(T).Name}>: [ null ],");

            if (_sortingPropertiesMapping != null && _sortingPropertiesMapping.Any())
                sb.Append($"\n\tSort: [ {string.Join(", ", _sortingPropertiesMapping.Select(x => x.Key + " = " + x.Value).ToArray())} ],");
            else
                sb.Append("\n\tSort: [ null ],");

            sb.Append($"\n\tPage: [ Index = {_pageIndex}, Size = {_pageSize} ]");

            sb.Append(" ]");

            return sb.ToString();
        }

        #endregion
    }
}
