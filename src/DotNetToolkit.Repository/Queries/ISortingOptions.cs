namespace DotNetToolkit.Repository.Queries
{
    /// <summary>
    /// Represents a sorting option.
    /// </summary>
    /// <typeparam name="T">The entity type of the repository.</typeparam>
    public interface ISortingOptions<T> : IQueryOptions<T>
    {
        /// <summary>
        /// Gets or sets a value indicating whether this sorting option is descending.
        /// </summary>
        /// <value>
        /// <c>true</c> if this sorting option is descending; otherwise, <c>false</c>.
        /// </value>
        bool IsDescending { get; set; }

        /// <summary>
        /// Gets the sorting property path.
        /// </summary>
        /// <value>
        /// The sorting property path.
        /// </value>
        string SortingPropertyPath { get; }
    }
}
