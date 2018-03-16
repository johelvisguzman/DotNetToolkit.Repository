namespace DotNetToolkit.Repository.Helpers
{
    using Properties;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;

    internal static class ConventionHelper
    {
        /// <summary>
        /// Gets the value of the specified object primary key property.
        /// </summary>
        /// <param name="obj">The object containing the property.</param>
        /// <returns>The property value.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="obj" /> is <c>null</c>.</exception>
        /// <exception cref="System.InvalidOperationException">Primary key could not be found for the entity type.</exception>
        public static object GetPrimaryKeyPropertyValue(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var propertyInfo = GetPrimaryKeyPropertyInfo(obj.GetType());
            return propertyInfo.GetValue(obj, null);
        }

        /// <summary>
        /// Sets a value for the specified object primary key property.
        /// </summary>
        /// <param name="obj">The object containing the property.</param>
        /// <param name="value">The value to set for the property.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="obj" /> is <c>null</c>.</exception>
        /// <exception cref="System.InvalidOperationException">The instance of entity type requires a primary key to be defined.</exception>
        public static void SetPrimaryKeyPropertyValue(object obj, object value)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var propertyInfo = GetPrimaryKeyPropertyInfo(obj.GetType());
            propertyInfo.SetValue(obj, value, null);
        }

        /// <summary>
        /// Gets the primary key property information for the specified type.
        /// </summary>
        /// <param name="entityType">The entity type to get the primary key from.</param>
        /// <returns>The primary key property info.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="entityType" /> is <c>null</c>.</exception>
        /// <exception cref="System.InvalidOperationException">The instance of entity type requires a primary key to be defined.</exception>
        public static PropertyInfo GetPrimaryKeyPropertyInfo(Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));

            foreach (var propertyName in GetPrimaryKeyNameChecks(entityType))
            {
                var propInfo = entityType.GetTypeInfo().GetDeclaredProperty(propertyName);

                if (propInfo != null)
                {
                    return propInfo;
                }
            }

            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityRequiresPrimaryKey, entityType));
        }

        /// <summary>
        /// Gets the primary key name checks.
        /// </summary>
        /// <param name="entityType">The entity type to get the primary key from.</param>
        /// <remarks>Assumes the entity has either an 'Id' property or 'EntityName' + 'Id'.</remarks>
        /// <returns>The list of primary key names to check.</returns>
        private static IEnumerable<string> GetPrimaryKeyNameChecks(Type entityType)
        {
            const string suffix = "Id";
            return new[] { suffix, entityType.Name + suffix };
        }
    }
}
