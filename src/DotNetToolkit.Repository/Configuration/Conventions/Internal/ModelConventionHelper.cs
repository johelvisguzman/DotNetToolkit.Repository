namespace DotNetToolkit.Repository.Configuration.Conventions.Internal
{
    using Extensions;
    using Extensions.Internal;
    using JetBrains.Annotations;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Reflection;
    using Utility;

    internal static class ModelConventionHelper
    {
        public static string GetTableName([NotNull] Type type)
        {
            Guard.NotNull(type, nameof(type));

            var tableName = type.GetTypeInfo().GetCustomAttribute<TableAttribute>()?.Name;

            if (string.IsNullOrEmpty(tableName))
                tableName = PluralizationService.Pluralize(type.Name);

            return tableName;
        }

        public static bool IsColumnMapped([NotNull] PropertyInfo pi)
        {
            Guard.NotNull(pi, nameof(pi));

            if (pi.GetCustomAttribute<NotMappedAttribute>() != null)
                return false;

            // Ensures the property has public setter
            return pi.CanWrite && pi.GetSetMethod(nonPublic: true).IsPublic;
        }

        public static string GetColumnName([NotNull] PropertyInfo pi)
        {
            Guard.NotNull(pi, nameof(pi));

            // If  is a complex object then don't worry about finding a column attribute for it
            if (pi.IsComplex())
                return pi.Name;

            var columnName = pi.GetCustomAttribute<ColumnAttribute>()?.Name;

            if (string.IsNullOrEmpty(columnName))
                columnName = pi.Name;

            return columnName;
        }

        public static int? GetColumnOrder([NotNull] IRepositoryConventions conventions, [NotNull] PropertyInfo pi)
        {
            Guard.NotNull(conventions, nameof(conventions));
            Guard.NotNull(pi, nameof(pi));

            var columnAttribute = pi.GetCustomAttribute<ColumnAttribute>();
            if (columnAttribute == null)
            {
                // Checks to see if the property is a primary key, and if so, try to give it the lowest ordering number
                var primerKeyPropertyInfos = conventions.GetPrimaryKeyPropertyInfos(pi.DeclaringType);

                if ((primerKeyPropertyInfos.Length == 1) && primerKeyPropertyInfos.First().Name.Equals(pi.Name))
                    return -1;
            }
            else if (columnAttribute.Order > 0)
            {
                return columnAttribute.Order;
            }

            return null;
        }

        public static bool IsColumnIdentity([NotNull] IRepositoryConventions conventions, [NotNull] PropertyInfo pi)
        {
            Guard.NotNull(pi, nameof(pi));

            var databaseGeneratedAttribute = pi.GetCustomAttribute<DatabaseGeneratedAttribute>();
            if (databaseGeneratedAttribute == null)
            {
                var primaryKeyPropertyInfos = conventions.GetPrimaryKeyPropertyInfos(pi.DeclaringType);

                return (primaryKeyPropertyInfos.Length == 1) && primaryKeyPropertyInfos.First().Name.Equals(pi.Name);
            }

            return databaseGeneratedAttribute.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity;
        }
    }
}
