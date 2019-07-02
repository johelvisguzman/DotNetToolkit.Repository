namespace DotNetToolkit.Repository.InMemory
{
    using Configuration;

    /// <summary>
    /// Represents an internal repository context for in-memory operations (for testing purposes).
    /// </summary>
    /// <seealso cref="IRepositoryContext" />
    public interface IInMemoryRepositoryContext : IRepositoryContext
    {
        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        string DatabaseName { get; set; }

        /// <summary>
        /// Ensures the in-memory store is completely deleted.
        /// </summary>
        void EnsureDeleted();
    }
}
