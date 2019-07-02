namespace DotNetToolkit.Repository.AdoNet
{
    using Configuration;

    /// <summary>
    /// Represents an ado.net repository context.
    /// </summary>
    /// <seealso cref="IRepositoryContextAsync" />
    public interface IAdoNetRepositoryContext : IRepositoryContextAsync
    {
        /// <summary>
        /// Gets the database helper which contains various methods for retrieving and manipulating data in a database.
        /// </summary>
        DbHelper DbHelper { get; }
    }
}
