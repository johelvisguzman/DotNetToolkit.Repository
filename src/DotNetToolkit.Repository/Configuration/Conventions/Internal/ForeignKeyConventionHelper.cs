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
        private static readonly ConcurrentDictionary<Tuple<Type, Type>, PropertyInfo[]> _foreignKeyCache = new ConcurrentDictionary<Tuple<Type, Type>, PropertyInfo[]>();

        public static PropertyInfo[] GetForeignKeyPropertyInfos([NotNull] IRepositoryConventions conventions, [NotNull] Type sourceType, [NotNull] Type foreignType)
        {
            Guard.NotNull(sourceType, nameof(sourceType));
            Guard.NotNull(foreignType, nameof(foreignType));
            
            var key = Tuple.Create(sourceType, foreignType);
            if (!_foreignKeyCache.TryGetValue(key, out PropertyInfo[] result))
            {
                result = GetForeignKeyPropertyInfosCore(conventions, sourceType, foreignType);
                _foreignKeyCache.TryAdd(key, result);
            }
            
            return result;
        }

        private static PropertyInfo[] GetForeignKeyPropertyInfosCore(IRepositoryConventions conventions, Type sourceType, Type foreignType)
        {
            if (sourceType.IsEnumerable() || foreignType.IsEnumerable())
                return new PropertyInfo[0];

            var properties = sourceType.GetRuntimeProperties().Where(ModelConventionHelper.IsColumnMapped).ToList();
            var foreignNavigationPropertyInfo = properties.SingleOrDefault(x => x.PropertyType == foreignType);
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
                                    sourceType.FullName,
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
                var primaryKeyPropertyInfos = conventions.GetPrimaryKeyPropertyInfos(foreignType);

                if (!propertyInfos.Any() && primaryKeyPropertyInfos.Length == 1)
                {
                    var primaryKeyPropertyInfo = primaryKeyPropertyInfos.First();
                    var foreignPrimaryKeyName = ModelConventionHelper.GetColumnName(primaryKeyPropertyInfo);
                    var propertyInfo = properties.SingleOrDefault(x => x.Name == $"{foreignNavigationPropertyInfo.Name}{foreignPrimaryKeyName}");

                    if (propertyInfo != null)
                    {
                        propertyInfos.Add(propertyInfo);
                    }
                }
            }

            return propertyInfos.ToArray();
        }
    }
}
