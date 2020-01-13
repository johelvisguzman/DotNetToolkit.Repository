namespace DotNetToolkit.Repository.Query
{
    using Strategies;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a query options which can be used for sorting or paging on queries.
    /// </summary>
    /// <typeparam name="T">The entity type of the repository.</typeparam>
    public interface IQueryOptions<T>
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
        IReadOnlyDictionary<string, SortOrder> SortingProperties { get; }

        /// <summary>
        /// Gets the fetch strategy which defines the child objects that should be retrieved when loading the entity.
        /// </summary>
        IFetchQueryStrategy<T> FetchStrategy { get; }

        /// <summary>
        /// Gets the specification.
        /// </summary>
        ISpecificationQueryStrategy<T> SpecificationStrategy { get; }
    }
}
