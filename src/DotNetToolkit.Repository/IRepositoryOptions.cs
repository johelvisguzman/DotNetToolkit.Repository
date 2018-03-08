namespace DotNetToolkit.Repository
{
    using System;

    /// <summary>
    /// Represents a repository option for creating new repositories.
    /// </summary>
    public interface IRepositoryOptions
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the type of the database context.
        /// </summary>
        Type DbContextType { get; set; }
    }
}
