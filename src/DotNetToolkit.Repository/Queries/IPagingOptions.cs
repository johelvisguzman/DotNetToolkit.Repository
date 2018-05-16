namespace DotNetToolkit.Repository.Queries
{
    /// <summary>
    /// Represents a paging option.
    /// </summary>
    /// <typeparam name="T">The entity type of the repository.</typeparam>
    public interface IPagingOptions<T> : ISortingOptions<T>
    {
        /// <summary>
        /// Gets or sets the number of rows of the data page to retrieve.
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the zero-based index of the data page to retrieve.
        /// </summary>
        int PageIndex { get; set; }
    }
}
