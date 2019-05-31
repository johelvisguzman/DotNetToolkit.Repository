namespace DotNetToolkit.Repository.Configuration
{
    /// <summary>
    /// Represents a repository context configuration.
    /// </summary>
    public interface IRepositoryContextFactory
    {
        /// <summary>
        /// Create a new repository context.
        /// </summary>
        /// <returns>The new repository context.</returns>
        IRepositoryContext Create();
    }
}