namespace DotNetToolkit.Repository.Xml
{
    using Configuration;
    using System;

    /// <summary>
    /// Represents an internal XML repository context.
    /// </summary>
    /// <seealso cref="IRepositoryContext" />
    public interface IXmlRepositoryContext : IRepositoryContext
    {
        /// <summary>
        /// Gets the file name for the specified type.
        /// </summary>
        /// <param name="type">The type to get the file name from.</param>
        /// <returns>The file name for the specified type.</returns>
        string GetFileName(Type type);
    }
}
