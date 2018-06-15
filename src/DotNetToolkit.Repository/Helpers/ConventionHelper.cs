namespace DotNetToolkit.Repository.Helpers
{
    using Properties;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents a convention helper for the repositories.
    /// </summary>
    public static class ConventionHelper
    {
        /// <summary>
        /// Gets the value of the specified object primary key property.
        /// </summary>
        /// <param name="obj">The object containing the property.</param>
        /// <returns>The property value.</returns>
        public static T GetPrimaryKeyPropertyValue<T>(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var propertyInfo = GetPrimaryKeyPropertyInfo(obj.GetType());
            var value = propertyInfo.GetValue(obj, null);

            return (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// Sets a value for the specified object primary key property.
        /// </summary>
        /// <param name="obj">The object containing the property.</param>
        /// <param name="value">The value to set for the property.</param>
        public static void SetPrimaryKeyPropertyValue(object obj, object value)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var propertyInfo = GetPrimaryKeyPropertyInfo(obj.GetType());
            var convertedValue = Convert.ChangeType(value, propertyInfo.PropertyType);

            propertyInfo.SetValue(obj, convertedValue, null);
        }

        /// <summary>
        /// Gets the primary key property information for the specified type.
        /// </summary>
        /// <param name="entityType">The entity type to get the primary key from.</param>
        /// <returns>The primary key property info.</returns>
        public static PropertyInfo GetPrimaryKeyPropertyInfo(Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));

            if (InMemoryCache.Instance.PrimaryKeyMapping.ContainsKey(entityType))
                return InMemoryCache.Instance.PrimaryKeyMapping[entityType];

            var propertyInfo = entityType
                .GetRuntimeProperties()
                .Where(x => IsMapped(x))
                .SingleOrDefault(x => IsMapped(x) && x.GetCustomAttribute<KeyAttribute>() != null);

            if (propertyInfo != null)
                return propertyInfo;

            foreach (var propertyName in GetDefaultPrimaryKeyNameChecks(entityType))
            {
                propertyInfo = entityType.GetTypeInfo().GetDeclaredProperty(propertyName);

                if (propertyInfo != null && IsMapped(propertyInfo))
                {
                    InMemoryCache.Instance.PrimaryKeyMapping[entityType] = propertyInfo;

                    return propertyInfo;
                }
            }

            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityRequiresPrimaryKey, entityType));
        }

        /// <summary>
        /// Gets the primary key property information for the specified type.
        /// </summary>
        /// <returns>The primary key property info.</returns>
        public static PropertyInfo GetPrimaryKeyPropertyInfo<T>()
        {
            return GetPrimaryKeyPropertyInfo(typeof(T));
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>The name of the table.</returns>
        public static string GetTableName(Type entityType)
        {
            if (InMemoryCache.Instance.TableNameMapping.ContainsKey(entityType))
                return InMemoryCache.Instance.TableNameMapping[entityType];

            var tableName = entityType.GetTypeInfo().GetCustomAttribute<TableAttribute>()?.Name;

            if (string.IsNullOrEmpty(tableName))
                tableName = PluralizationHelper.Pluralize(entityType.Name);

            InMemoryCache.Instance.TableNameMapping[entityType] = tableName;

            return tableName;
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <returns>The name of the table.</returns>
        public static string GetTableName<T>()
        {
            return GetTableName(typeof(T));
        }

        /// <summary>
        /// Determines whether the specified property is mapped (does not have a <see cref="NotMappedAttribute"/> defined).
        /// </summary>
        /// <param name="pi">The property info.</param>
        /// <returns>
        ///   <c>true</c> if the property is mapped; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMapped(PropertyInfo pi)
        {
            if (pi == null)
                throw new ArgumentNullException(nameof(pi));

            return pi.GetCustomAttribute<NotMappedAttribute>() == null;
        }

        /// <summary>
        /// Gets the column order for the specified property.
        /// </summary>
        /// <param name="pi">The property info.</param>
        /// <returns>The column order.</returns>
        public static int GetColumnOrder(PropertyInfo pi)
        {
            if (pi == null)
                throw new ArgumentNullException(nameof(pi));

            var columnAttribute = pi.GetCustomAttribute<ColumnAttribute>();
            if (columnAttribute == null)
                return -1;

            return columnAttribute.Order;
        }

        /// <summary>
        /// Gets the name of the foreign key that matches the specified foreign type.
        /// </summary>
        /// <param name="sourceType">The source type.</param>
        /// <param name="foreignType">The foreign type to match.</param>
        /// <returns>The name of the foreign key.</returns>
        public static PropertyInfo GetForeignKeyPropertyInfo(Type sourceType, Type foreignType)
        {
            var tupleKey = Tuple.Create(sourceType, foreignType);

            if (InMemoryCache.Instance.ForeignKeyMapping.ContainsKey(tupleKey))
                return InMemoryCache.Instance.ForeignKeyMapping[tupleKey];

            var properties = sourceType.GetRuntimeProperties().Where(IsMapped);
            var foreignPropertyInfo = properties.SingleOrDefault(x => x.PropertyType == foreignType);

            PropertyInfo propertyInfo = null;

            if (foreignPropertyInfo != null)
            {
                var propertyInfosWithForeignKeys = properties.Where(x => x.GetCustomAttribute<ForeignKeyAttribute>() != null);
                if (propertyInfosWithForeignKeys.Any())
                {
                    // Try to find by checking on the foreign key property
                    propertyInfo = propertyInfosWithForeignKeys
                        .Where(IsPrimitive)
                        .SingleOrDefault(x => x.GetCustomAttribute<ForeignKeyAttribute>().Name.Equals(foreignPropertyInfo.Name));

                    // Try to find by checking on the navigation property
                    if (propertyInfo == null)
                    {
                        propertyInfo = properties
                            .Where(IsPrimitive)
                            .SingleOrDefault(x => foreignPropertyInfo.GetCustomAttribute<ForeignKeyAttribute>().Name.Equals(GetColumnName(x)));
                    }
                }

                // Try to find by naming convention
                if (propertyInfo == null)
                {
                    var foreignPrimaryKeyName = GetColumnName(GetPrimaryKeyPropertyInfo(foreignType));

                    propertyInfo = properties
                        .SingleOrDefault(x => x.Name == $"{foreignPropertyInfo.Name}{foreignPrimaryKeyName}");
                }
            }

            InMemoryCache.Instance.ForeignKeyMapping[tupleKey] = propertyInfo;

            return propertyInfo;
        }

        /// <summary>
        /// Determines if the specified property is a complex type.
        /// </summary>
        /// <param name="pi">The property info.</param>
        /// <returns><c>true</c> if the specified type is a complex type; otherwise, <c>false</c>.</returns>
        public static bool IsComplex(PropertyInfo pi)
        {
            return pi.PropertyType.Namespace != "System";
        }

        /// <summary>
        /// Determines if the specified property is a primitive type.
        /// </summary>
        /// <param name="pi">The property info.</param>
        /// <returns><c>true</c> if the specified type is a primitive type; otherwise, <c>false</c>.</returns>
        public static bool IsPrimitive(PropertyInfo pi)
        {
            return !IsComplex(pi);
        }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <param name="pi">The property info.</param>
        /// <returns>The name of the column.</returns>
        public static string GetColumnName(PropertyInfo pi)
        {
            if (pi == null)
                throw new ArgumentNullException(nameof(pi));

            // If this is a complex object then don't worry about finding a column attribute for it
            if (IsComplex(pi))
                return pi.Name;

            var columnName = pi.GetCustomAttribute<ColumnAttribute>()?.Name;

            if (string.IsNullOrEmpty(columnName))
                columnName = pi.Name;

            return columnName;
        }

        /// <summary>
        /// Gets the primary key name checks.
        /// </summary>
        /// <param name="entityType">The entity type to get the primary key from.</param>
        /// <remarks>Assumes the entity has either an 'Id' property or 'EntityName' + 'Id'.</remarks>
        /// <returns>The list of primary key names to check.</returns>
        private static IEnumerable<string> GetDefaultPrimaryKeyNameChecks(Type entityType)
        {
            const string suffix = "Id";

            return new[] { suffix, entityType.Name + suffix };
        }
    }
}