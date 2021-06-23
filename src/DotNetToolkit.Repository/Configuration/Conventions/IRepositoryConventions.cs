namespace DotNetToolkit.Repository.Configuration.Conventions
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Represents configurable conventions for getting primary/foreign keys for a repository.
    /// </summary>
    public interface IRepositoryConventions
    {
        /// <summary>
        /// Gets or sets a callback function for getting a collection of primary keys for the specified type.
        /// </summary>
        Func<Type, PropertyInfo[]> PrimaryKeysCallback { get; set; }
    }
}
