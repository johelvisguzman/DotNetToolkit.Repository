namespace DotNetToolkit.Repository.EntityFramework
{
    using Configuration;
    using System.Data.Entity;

    /// <summary>
    /// Represents a entity framework repository context.
    /// </summary>
    /// <seealso cref="IRepositoryContextAsync" />
    public interface IEfRepositoryContext : IRepositoryContextAsync
    {
        /// <summary>
        /// Gets the underlying context.
        /// </summary>
        DbContext UnderlyingContext { get; }
    }
}
