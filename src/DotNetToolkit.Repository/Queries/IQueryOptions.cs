namespace DotNetToolkit.Repository.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// Represents a query options which can be used for sorting or paging on queries.
    /// </summary>
    /// <typeparam name="T">The entity type of the repository.</typeparam>
    public interface IQueryOptions<T> where T : class
    {
        /// <summary>
        /// Gets the number of rows of the data page to retrieve.
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Gets the zero-based index of the data page to retrieve.
        /// </summary>
        int PageIndex { get; }

        /// <summary>
        /// Gets a collection of sorting property paths.
        /// </summary>
        IDictionary<string, bool> SortingPropertiesMapping { get; }

        /// <summary>
        /// Applies a primary ascending sort order according to the specified property name.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The current instance.</returns>
        IQueryOptions<T> SortBy(string propertyName);

        /// <summary>
        /// Applies a secondary ascending sort order according to the specified property name.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The current instance.</returns>
        IQueryOptions<T> ThenSortBy(string propertyName);

        /// <summary>
        /// Applies a primary descending sort order according to the specified property name.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The current instance.</returns>
        IQueryOptions<T> SortByDescending(string propertyName);

        /// <summary>
        /// Applies a secondary descending sort order according to the specified property name.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The current instance.</returns>
        IQueryOptions<T> ThenSortByDescending(string propertyName);

        /// <summary>
        /// Applies a primary ascending sort order according to the specified property name.
        /// </summary>
        /// <param name="property">The sorting property expression.</param>
        /// <returns>The current instance.</returns>
        IQueryOptions<T> SortBy(Expression<Func<T, object>> property);

        /// <summary>
        /// Applies a primary descending sort order according to the specified property name.
        /// </summary>
        /// <param name="property">The sorting property expression.</param>
        /// <returns>The current instance.</returns>
        IQueryOptions<T> ThenSortBy(Expression<Func<T, object>> property);

        /// <summary>
        /// Applies a primary descending sort order according to the specified property name.
        /// </summary>
        /// <param name="property">The sorting property expression.</param>
        /// <returns>The current instance.</returns>
        IQueryOptions<T> SortByDescending(Expression<Func<T, object>> property);

        /// <summary>
        /// Applies a secondary descending sort order according to the specified property name.
        /// </summary>
        /// <param name="property">The sorting property expression.</param>
        /// <returns>The current instance.</returns>
        IQueryOptions<T> ThenSortByDescending(Expression<Func<T, object>> property);

        /// <summary>
        /// Applies paging according to the specified index and page size.
        /// </summary>
        /// <param name="pageIndex">The zero-based index of the data page to retrieve.</param>
        /// <param name="pageSize">The number of rows of the data page to retrieve.</param>
        /// <returns>The current instance.</returns>
        IQueryOptions<T> Page(int pageIndex, int pageSize);

        /// <summary>
        /// Applies paging according to the specified index and a default page size of 100.
        /// </summary>
        /// <param name="pageIndex">The zero-based index of the data page to retrieve.</param>
        /// <returns>The current instance.</returns>
        IQueryOptions<T> Page(int pageIndex);
    }
}
