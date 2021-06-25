namespace DotNetToolkit.Repository.Configuration.Conventions.Internal
{
    using Extensions;
    using Extensions.Internal;
    using JetBrains.Annotations;
    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Reflection;
    using Utility;

    internal static class ModelConventionHelper
    {
        private static readonly ConcurrentDictionary<Type, string> _tableNameCache = new ConcurrentDictionary<Type, string>();
        private static readonly ConcurrentDictionary<PropertyInfo, bool> _isColumnMappedCache = new ConcurrentDictionary<PropertyInfo, bool>();
        private static readonly ConcurrentDictionary<PropertyInfo, bool> _isColumnIdentityCache = new ConcurrentDictionary<PropertyInfo, bool>();
        private static readonly ConcurrentDictionary<PropertyInfo, int?> _columnOrderCache = new ConcurrentDictionary<PropertyInfo, int?>();
        private static readonly ConcurrentDictionary<PropertyInfo, string> _columnNameCache = new ConcurrentDictionary<PropertyInfo, string>();

        public static string GetTableName([NotNull] Type entityType)
        {
            Guard.NotNull(entityType, nameof(entityType));

            if (!_tableNameCache.TryGetValue(entityType, out string result))
            {
                result = GetTableNameCore(entityType);
                _tableNameCache.TryAdd(entityType, result);
            }

            return result;
        }

        private static string GetTableNameCore(Type type)
        {
            var tableName = type.GetTypeInfo().GetCustomAttribute<TableAttribute>()?.Name;

            if (string.IsNullOrEmpty(tableName))
                tableName = PluralizationService.Pluralize(type.Name);

            return tableName;
        }

        public static bool IsColumnMapped([NotNull] PropertyInfo pi)
        {
            Guard.NotNull(pi, nameof(pi));

            if (!_isColumnMappedCache.TryGetValue(pi, out bool result))
            {
                result = IsColumnMappedCore(pi);
                _isColumnMappedCache.TryAdd(pi, result);
            }

            return result;
        }

        private static bool IsColumnMappedCore(PropertyInfo pi)
        {
            if (pi.GetCustomAttribute<NotMappedAttribute>() != null)
                return false;

            // Ensures the property has public setter
            return pi.CanWrite && pi.GetSetMethod(nonPublic: true).IsPublic;
        }

        public static string GetColumnName([NotNull] PropertyInfo pi)
        {
            Guard.NotNull(pi, nameof(pi));

            if (!_columnNameCache.TryGetValue(pi, out string result))
            {
                result = GetColumnNameCore(pi);
                _columnNameCache.TryAdd(pi, result);
            }

            return result;
        }

        private static string GetColumnNameCore(PropertyInfo pi)
        {
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

            if (!_columnOrderCache.TryGetValue(pi, out int? result))
            {
                result = GetColumnOrderCore(conventions, pi);
                _columnOrderCache.TryAdd(pi, result);
            }

            return result;
        }

        private static int? GetColumnOrderCore(IRepositoryConventions conventions, PropertyInfo pi)
        {
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

        public static int GetColumnOrderOrDefault([NotNull] IRepositoryConventions conventions, [NotNull] PropertyInfo pi)
        {
            return GetColumnOrder(conventions, pi) ?? Int32.MaxValue;
        }

        public static bool IsColumnIdentity([NotNull] IRepositoryConventions conventions, [NotNull] PropertyInfo pi)
        {
            Guard.NotNull(pi, nameof(pi));

            if (!_isColumnIdentityCache.TryGetValue(pi, out bool result))
            {
                result = IsColumnIdentityCore(conventions, pi);
                _isColumnIdentityCache.TryAdd(pi, result);
            }

            return result;
        }

        private static bool IsColumnIdentityCore(IRepositoryConventions conventions, PropertyInfo pi)
        {
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
