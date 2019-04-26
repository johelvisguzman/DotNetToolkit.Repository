namespace DotNetToolkit.Repository.Configuration.Conventions
{
    using Extensions;
    using Properties;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Reflection;
    using Utility;

    internal class ForeignKeyConventionHelper
    {
        /// <summary>
        /// Gets the collection of foreign key properties that matches the specified foreign type.
        /// </summary>
        /// <param name="sourceType">The source type.</param>
        /// <param name="foreignType">The foreign type to match.</param>
        /// <returns>The collection of foreign key properties.</returns>
        public static IEnumerable<PropertyInfo> GetForeignKeyPropertyInfos(Type sourceType, Type foreignType)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            if (foreignType == null)
                throw new ArgumentNullException(nameof(foreignType));

            if (sourceType.IsEnumerable() || foreignType.IsEnumerable())
                return Enumerable.Empty<PropertyInfo>();

            var tupleKey = Tuple.Create(sourceType, foreignType);

            if (InMemoryCache.Instance.ForeignKeyMapping.ContainsKey(tupleKey))
                return InMemoryCache.Instance.ForeignKeyMapping[tupleKey];

            var properties = sourceType.GetRuntimeProperties().Where(x => x.IsColumnMapped());
            var foreignNavigationPropertyInfo = properties.SingleOrDefault(x => x.PropertyType == foreignType);
            var propertyInfos = new List<PropertyInfo>();

            if (foreignNavigationPropertyInfo != null)
            {
                var propertyInfosWithForeignKeys = properties.Where(x => x.GetCustomAttribute<ForeignKeyAttribute>() != null);
                if (propertyInfosWithForeignKeys.Any())
                {
                    // Ensure that the foreign key names are valid
                    foreach (var propertyInfosWithForeignKey in propertyInfosWithForeignKeys)
                    {
                        var foreignKeyAttributeName = propertyInfosWithForeignKey.GetCustomAttribute<ForeignKeyAttribute>().Name;
                        if (!properties.Any(x => foreignKeyAttributeName.Equals(x.GetColumnName())))
                        {
                            throw new InvalidOperationException(
                                string.Format(
                                    Resources.ForeignKeyAttributeOnPropertyNotFoundOnDependentType,
                                    propertyInfosWithForeignKey.Name,
                                    sourceType.FullName,
                                    foreignKeyAttributeName));
                        }
                    }

                    // Try to find by checking on the foreign key property
                    propertyInfos = propertyInfosWithForeignKeys
                        .Where(Extensions.PropertyInfoExtensions.IsPrimitive)
                        .Where(x => x.GetCustomAttribute<ForeignKeyAttribute>().Name.Equals(foreignNavigationPropertyInfo.Name))
                        .ToList();

                    // Try to find by checking on the navigation property
                    if (!propertyInfos.Any())
                    {
                        propertyInfos = properties
                            .Where(Extensions.PropertyInfoExtensions.IsPrimitive)
                            .Where(x => foreignNavigationPropertyInfo.GetCustomAttribute<ForeignKeyAttribute>().Name.Equals(x.GetColumnName()))
                            .ToList();
                    }
                }

                // Try to find by naming convention
                if (!propertyInfos.Any() && !PrimaryKeyConventionHelper.HasCompositePrimaryKey(foreignType))
                {
                    var primaryKeyPropertyInfo = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(foreignType).FirstOrDefault();
                    if (primaryKeyPropertyInfo != null)
                    {
                        var foreignPrimaryKeyName = primaryKeyPropertyInfo.GetColumnName();
                        var propertyInfo = properties.SingleOrDefault(x => x.Name == $"{foreignNavigationPropertyInfo.Name}{foreignPrimaryKeyName}");

                        if (propertyInfo != null)
                        {
                            propertyInfos.Add(propertyInfo);
                        }
                    }
                }
            }

            InMemoryCache.Instance.ForeignKeyMapping[tupleKey] = propertyInfos;

            return propertyInfos;
        }
    }
}
