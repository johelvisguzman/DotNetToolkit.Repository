namespace DotNetToolkit.Repository.Configuration.Conventions
{
    using Internal;
    using System;
    using System.Reflection;

    /// <summary>
    /// An implementation of <see cref="IRepositoryConventions" />.
    /// </summary>
    public class RepositoryConventions : IRepositoryConventions
    {
        /// <summary>
        /// Gets or sets a callback function for getting a collection of primary keys for the specified type.
        /// </summary>
        public Func<Type, PropertyInfo[]> PrimaryKeysCallback { get; set; }

        /// <summary>
        /// Gets or sets a callback function for getting a collection of foreign key properties that matches the specified foreign type.
        /// </summary>
        public Func<Type, Type, PropertyInfo[]> ForeignKeysCallback { get; set; }

        /// <summary>
        /// Gets or sets a callback function for getting a table name for the specified type.
        /// </summary>
        public Func<Type, string> TableNameCallback { get; set; }

        /// <summary>
        /// Gets or sets a callback function for getting a column name for the specified property.
        /// </summary>
        public Func<PropertyInfo, string> ColumnNameCallback { get; set; }

        /// <summary>
        /// Gets or sets a callback function for getting a column order for the specified property.
        /// </summary>
        public Func<PropertyInfo, int?> ColumnOrderCallback { get; set; }

        /// <summary>
        /// Gets or sets a callback function for determining whether the specified property is defined as identity.
        /// </summary>
        public Func<PropertyInfo, bool> IsColumnIdentityCallback { get; set; }

        /// <summary>
        /// Gets or sets a callback function for determining whether the specified property is mapped.
        /// </summary>
        public Func<PropertyInfo, bool> IsColumnMappedCallback { get; set; }

        /// <summary>
        /// Gets the default conventions.
        /// </summary>
        public static IRepositoryConventions Default
        {
            get
            {
                var conventions = new RepositoryConventions();

                conventions.PrimaryKeysCallback = pi => PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(conventions, pi);
                conventions.ForeignKeysCallback = (sourceType, foreignType) => ForeignKeyConventionHelper.GetForeignKeyPropertyInfos(conventions, sourceType, foreignType);
                conventions.TableNameCallback = type => ModelConventionHelper.GetTableName(type);
                conventions.ColumnNameCallback = pi => ModelConventionHelper.GetColumnName(pi);
                conventions.ColumnOrderCallback = pi => ModelConventionHelper.GetColumnOrder(conventions, pi);
                conventions.IsColumnIdentityCallback = pi => ModelConventionHelper.IsColumnIdentity(conventions, pi);
                conventions.IsColumnMappedCallback = pi => ModelConventionHelper.IsColumnMapped(pi);

                return conventions;
            }
        }
    }
}
