namespace DotNetToolkit.Repository.Configuration.Conventions.Internal
{
    using Extensions;
    using Extensions.Internal;
    using JetBrains.Annotations;
    using Properties;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Reflection;
    using Utility;

    internal class ForeignKeyConventionHelper
    {
        private static readonly ConcurrentDictionary<PropertyInfo, PropertyInfo[]> _foreignKeyCache = new ConcurrentDictionary<PropertyInfo, PropertyInfo[]>();

        public static PropertyInfo[] GetForeignKeyPropertyInfos([NotNull] IRepositoryConventions conventions, [NotNull] PropertyInfo pi)
        {
            Guard.NotNull(conventions, nameof(conventions));
            Guard.NotNull(pi, nameof(pi));
            
            if (!_foreignKeyCache.TryGetValue(pi, out PropertyInfo[] result))
            {
                result = GetForeignKeyPropertyInfosCore(conventions, pi);
                _foreignKeyCache.TryAdd(pi, result);
            }
            
            return result;
        }

        private static PropertyInfo[] GetForeignKeyPropertyInfosCore([NotNull] IRepositoryConventions conventions, PropertyInfo pi)
        {
            var foreignType = pi.PropertyType.GetGenericTypeOrDefault();
            var declaringType = pi.DeclaringType;

            if (foreignType.IsEnumerable() || declaringType.IsEnumerable())
                return new PropertyInfo[0];

            var properties = foreignType.GetRuntimeProperties().Where(ModelConventionHelper.IsColumnMapped).ToList();
            var foreignNavigationPropertyInfo = properties.SingleOrDefault(x => x.PropertyType == declaringType);
            var propertyInfos = new List<PropertyInfo>();

            if (foreignNavigationPropertyInfo != null)
            {
                // Gets by checking the annotations
                var propertyInfosWithForeignKeys = properties.Where(x => x.GetCustomAttribute<ForeignKeyAttribute>() != null).ToList();
                if (propertyInfosWithForeignKeys.Any())
                {
                    // Ensure that the foreign key names are valid
                    foreach (var propertyInfosWithForeignKey in propertyInfosWithForeignKeys)
                    {
                        var foreignKeyAttributeName = propertyInfosWithForeignKey.GetCustomAttribute<ForeignKeyAttribute>().Name;
                        if (!properties.Any(x => foreignKeyAttributeName.Equals(ModelConventionHelper.GetColumnName(x))))
                        {
                            throw new InvalidOperationException(
                                string.Format(
                                    Resources.ForeignKeyAttributeOnPropertyNotFoundOnDependentType,
                                    propertyInfosWithForeignKey.Name,
                                    foreignType.FullName,
                                    foreignKeyAttributeName));
                        }
                    }

                    // Try to find by checking on the foreign key property
                    propertyInfos = propertyInfosWithForeignKeys
                        .Where(DotNetToolkit.Repository.Extensions.Internal.PropertyInfoExtensions.IsPrimitive)
                        .Where(x => x.GetCustomAttribute<ForeignKeyAttribute>().Name.Equals(foreignNavigationPropertyInfo.Name))
                        .ToList();

                    // Try to find by checking on the navigation property
                    if (!propertyInfos.Any())
                    {
                        propertyInfos = properties
                            .Where(DotNetToolkit.Repository.Extensions.Internal.PropertyInfoExtensions.IsPrimitive)
                            .Where(x => foreignNavigationPropertyInfo.GetCustomAttribute<ForeignKeyAttribute>().Name.Equals(ModelConventionHelper.GetColumnName(x)))
                            .ToList();
                    }
                }

                // Try to find by naming convention
                var primaryKeyPropertyInfos = conventions.GetPrimaryKeyPropertyInfos(declaringType);

                if (!propertyInfos.Any() && primaryKeyPropertyInfos.Any())
                {
                    foreach (var primaryKeyPropertyInfo in primaryKeyPropertyInfos)
                    {
                        var foreignPrimaryKeyName = ModelConventionHelper.GetColumnName(primaryKeyPropertyInfo);
                        var propertyInfo = properties.FirstOrDefault(x => x.Name == $"{foreignNavigationPropertyInfo.Name}{foreignPrimaryKeyName}");

                        if (propertyInfo != null)
                        {
                            propertyInfos.Add(propertyInfo);
                        }
                    }
                }
            }

            return propertyInfos.ToArray();
        }
    }
}
