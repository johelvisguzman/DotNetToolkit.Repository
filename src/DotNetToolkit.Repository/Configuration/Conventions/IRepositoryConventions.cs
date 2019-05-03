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

        /// <summary>
        /// Gets or sets a callback function for getting a collection of foreign key properties that matches the specified foreign type.
        /// </summary>
        Func<Type, Type, PropertyInfo[]> ForeignKeysCallback { get; set; }

        /// <summary>
        /// Gets or sets a callback function for getting a table name for the specified type.
        /// </summary>
        Func<Type, string> TableNameCallback { get; set; }

        /// <summary>
        /// Gets or sets a callback function for getting a column name for the specified property.
        /// </summary>
        Func<PropertyInfo, string> ColumnNameCallback { get; set; }

        /// <summary>
        /// Gets or sets a callback function for getting a column order for the specified property.
        /// </summary>
        Func<PropertyInfo, int?> ColumnOrderCallback { get; set; }

        /// <summary>
        /// Gets or sets a callback function for determining whether the specified property is defined as identity.
        /// </summary>
        Func<PropertyInfo, bool> IsColumnIdentityCallback { get; set; }

        /// <summary>
        /// Gets or sets a callback function for determining whether the specified property is mapped.
        /// </summary>
        Func<PropertyInfo, bool> IsColumnMappedCallback { get; set; }
    }
}
