namespace DotNetToolkit.Repository
{
    using Logging;
    using System;

    /// <summary>
    /// Represents a repository option for creating new repositories.
    /// </summary>
    public interface IRepositoryOptions
    {
        /// <summary>
        /// Gets or sets the database context arguments.
        /// </summary>
        object[] DbContextArgs { get; set; }

        /// <summary>
        /// Gets or sets the type of the database context.
        /// </summary>
        Type DbContextType { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        ILogger Logger { get; set; }
    }
}
