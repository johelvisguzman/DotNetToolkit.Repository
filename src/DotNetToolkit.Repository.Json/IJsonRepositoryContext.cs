namespace DotNetToolkit.Repository.Json
{
    using Configuration;
    using System;

    /// <summary>
    /// Represents an internal JSON repository context.
    /// </summary>
    /// <seealso cref="IRepositoryContext" />
    public interface IJsonRepositoryContext : IRepositoryContext
    {
        /// <summary>
        /// Gets the file name for the specified type.
        /// </summary>
        /// <param name="type">The type to get the file name from.</param>
        /// <returns>The file name for the specified type.</returns>
        string GetFileName(Type type);
    }
}
