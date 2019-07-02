namespace DotNetToolkit.Repository.EntityFrameworkCore
{
    using Configuration;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Represents a entity framework repository context.
    /// </summary>
    /// <seealso cref="IRepositoryContextAsync" />
    public interface IEfCoreRepositoryContext : IRepositoryContextAsync
    {
        /// <summary>
        /// Gets the underlying context.
        /// </summary>
        DbContext UnderlyingContext { get; }
    }
}
