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

        public static PropertyInfo[] GetForeignKeyPropertyInfos([NotNull] PropertyInfo pi)
        {
            Guard.NotNull(pi, nameof(pi));
            
            if (!_foreignKeyCache.TryGetValue(pi, out PropertyInfo[] result))
            {
                result = GetForeignKeyPropertyInfosCore(pi);
                _foreignKeyCache.TryAdd(pi, result);
            }
            
            return result;
        }

        private static PropertyInfo[] GetForeignKeyPropertyInfosCore(PropertyInfo pi)
        {
            var foreignType = pi.PropertyType.GetGenericTypeOrDefault();
            var declaringType = pi.DeclaringType;

            if (foreignType.IsEnumerable() || declaringType.IsEnumerable())
                return new PropertyInfo[0];

            PropertyInfo[] propertyInfos = new PropertyInfo[0];

            if (TryGetForeignKeyPropertyInfos(foreignType, declaringType, out var foreignKeyPropertyInfosFromTarget, out _))
            {
                propertyInfos = foreignKeyPropertyInfosFromTarget;
            }
            else if (TryGetForeignKeyPropertyInfos(declaringType, foreignType, out _, out var primaryKeyPropertyInfosFromSource))
            {
                propertyInfos = primaryKeyPropertyInfosFromSource;
            }

            return propertyInfos;
        }

        private static bool TryGetForeignKeyPropertyInfos(Type source, Type target, out PropertyInfo[] foreignKeyPropertyInfosFromTarget, out PropertyInfo[] primaryKeyPropertyInfosFromSource)
        {
            var properties = source.GetRuntimeProperties().Where(ModelConventionHelper.IsColumnMapped).ToList();
            var foreignNavigationPropertyInfo = properties.SingleOrDefault(x => x.PropertyType == target);
            var propertyInfos = new List<PropertyInfo>();

            foreignKeyPropertyInfosFromTarget = null;
            primaryKeyPropertyInfosFromSource = null;

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
                                    source.FullName,
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
                var primaryKeyPropertyInfos = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(target);

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

                if (propertyInfos.Any())
                {
                    foreignKeyPropertyInfosFromTarget = propertyInfos.ToArray();
                    primaryKeyPropertyInfosFromSource = primaryKeyPropertyInfos;

                    return true;
                }
            }

            return false;
        }
    }
}
