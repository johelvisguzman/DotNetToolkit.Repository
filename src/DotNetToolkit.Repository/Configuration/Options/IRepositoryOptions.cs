namespace DotNetToolkit.Repository.Configuration.Options
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a repository options.
    /// </summary>
    public interface IRepositoryOptions
    {
        /// <summary>
        /// Gets the repository extensions that store the configured options.
        /// </summary>
        IEnumerable<IRepositoryOptionsExtensions> Extensions { get; }

        /// <summary>
        /// Gets the extension of the specified type. Returns null if no extension of the specified type is configured.
        /// </summary>
        /// <typeparam name="TExtension">The type of the extension to get.</typeparam>
        /// <returns>The extension, or null if none was found.</returns>
        TExtension FindExtension<TExtension>() where TExtension : class, IRepositoryOptionsExtensions;
    }
}
