namespace DotNetToolkit.Repository.Queries
{
    using Helpers;
    using System;
    using System.Collections.Generic;
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

        #endregion
    }
}
