namespace DotNetToolkit.Repository.Configuration.Conventions.Internal
{
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
        private static readonly ConcurrentDictionary<Tuple<Type, Type, string>, Result> _foreignKeyCache = new ConcurrentDictionary<Tuple<Type, Type, string>, Result>();

        public static Result GetForeignKeyPropertyInfos([NotNull] PropertyInfo pi)
        {
            Guard.NotNull(pi, nameof(pi));

            var sourceType = pi.DeclaringType;
            var targetType = pi.PropertyType.GetGenericTypeOrDefault();

            return GetForeignKeyPropertyInfos(sourceType, targetType, pi.Name);
        }

        public static Result GetForeignKeyPropertyInfos([NotNull] Type sourceType, [NotNull] Type targetType, [NotNull] string navPiName)
        {
            Guard.NotNull(sourceType, nameof(sourceType));
            Guard.NotNull(targetType, nameof(targetType));

            var key = Tuple.Create(sourceType, targetType, navPiName);

            if (!_foreignKeyCache.TryGetValue(key, out var result))
            {
                result = GetForeignKeyPropertyInfosCore(sourceType, targetType, navPiName);
                _foreignKeyCache.TryAdd(key, result);
            }

            return result;
        }

        private static Result GetForeignKeyPropertyInfosCore(Type sourceType, Type targetType, string navPiName)
        {
            if (sourceType.IsEnumerable() || targetType.IsEnumerable())
                return null;

            bool foundInSource;

            if (TryGetForeignKeyPropertyInfos(
                sourceType, targetType, navPiName,
                searchNavPiInSource: true,
                out var foreignKeyPropertyInfos,
                out var foreignNavPropertyInfo,
                out var adjacentNavPropertyInfo))
            {
                foundInSource = true;
            }
            else if (TryGetForeignKeyPropertyInfos(
                targetType, sourceType, navPiName,
                searchNavPiInSource: false,
                out foreignKeyPropertyInfos,
                out foreignNavPropertyInfo,
                out adjacentNavPropertyInfo))
            {
                foundInSource = false;
            }
            else
            {
                return null;
            }

            var leftNavPi = foundInSource ? adjacentNavPropertyInfo : foreignNavPropertyInfo;
            var leftKeysToJoinOn = foundInSource
                ? foreignKeyPropertyInfos
                : PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(
                    leftNavPi.PropertyType.GetGenericTypeOrDefault());

            var rightNavPi = foundInSource ? foreignNavPropertyInfo : adjacentNavPropertyInfo;
            var rightKeysToJoinOn = foundInSource
                ? PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(
                    rightNavPi.PropertyType.GetGenericTypeOrDefault())
                : foreignKeyPropertyInfos;

            return new Result(leftNavPi, leftKeysToJoinOn, rightNavPi, rightKeysToJoinOn);
        }

        private static bool TryGetForeignKeyPropertyInfos(Type sourceType, Type targetType, string navPiName, bool searchNavPiInSource,
            out PropertyInfo[] foreignKeyPropertyInfos,
            out PropertyInfo foreignNavPropertyInfo,
            out PropertyInfo adjacentNavPropertyInfo)
        {
            adjacentNavPropertyInfo = null;
            foreignKeyPropertyInfos = null;
            foreignNavPropertyInfo = null;

            var propsFromSource = sourceType.GetRuntimeProperties().Where(ModelConventionHelper.IsColumnMapped).ToList();
            var propsFromTarget = targetType.GetRuntimeProperties().Where(ModelConventionHelper.IsColumnMapped).ToList();

            PropertyInfo foreignNavPi = null;
            PropertyInfo adjacentNavPi = null;

            if (!string.IsNullOrEmpty(navPiName))
            {
                if (searchNavPiInSource)
                {
                    foreignNavPi = propsFromSource.FirstOrDefault(x => x.Name == navPiName);
                    adjacentNavPi = propsFromTarget.FirstOrDefault(x => x.PropertyType == sourceType);
                }
                else
                {
                    adjacentNavPi = propsFromTarget.FirstOrDefault(x => x.Name == navPiName);
                    foreignNavPi = propsFromSource.FirstOrDefault(x => x.PropertyType == targetType);
                }
            }

            var foreignKeyPiList = new List<PropertyInfo>();

            if (foreignNavPi != null)
            {
                // Gets by checking the annotations in source
                var propertyInfosWithForeignKeys = propsFromSource.Where(x => x.GetCustomAttribute<ForeignKeyAttribute>() != null).ToList();
                if (propertyInfosWithForeignKeys.Any())
                {
                    // Ensure that the foreign key names are valid
                    foreach (var propertyInfosWithForeignKey in propertyInfosWithForeignKeys)
                    {
                        var foreignKeyAttributeName = propertyInfosWithForeignKey.GetCustomAttribute<ForeignKeyAttribute>().Name;
                        if (!propsFromSource.Any(x => foreignKeyAttributeName.Equals(ModelConventionHelper.GetColumnName(x))))
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
                    foreignKeyPiList = propertyInfosWithForeignKeys
                        .Where(DotNetToolkit.Repository.Extensions.Internal.PropertyInfoExtensions.IsPrimitive)
                        .Where(x => x.GetCustomAttribute<ForeignKeyAttribute>().Name.Equals(foreignNavPi.Name))
                        .ToList();

                    // Try to find by checking on the navigation property
                    if (!foreignKeyPiList.Any())
                    {
                        foreignKeyPiList = propsFromSource
                            .Where(DotNetToolkit.Repository.Extensions.Internal.PropertyInfoExtensions.IsPrimitive)
                            .Where(x => foreignNavPi.GetCustomAttribute<ForeignKeyAttribute>().Name.Equals(ModelConventionHelper.GetColumnName(x)))
                            .ToList();
                    }
                }

                // Try to find by naming convention
                var primaryKeyPropertyInfos = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(targetType);

                if (!foreignKeyPiList.Any() && primaryKeyPropertyInfos.Any())
                {
                    foreach (var primaryKeyPropertyInfo in primaryKeyPropertyInfos)
                    {
                        var foreignPrimaryKeyName = ModelConventionHelper.GetColumnName(primaryKeyPropertyInfo);
                        var propertyName = $"{foreignNavPi.Name}{foreignPrimaryKeyName}";
                        var propertyInfo = propsFromSource.FirstOrDefault(x => x.Name == propertyName);

                        if (propertyInfo != null)
                        {
                            foreignKeyPiList.Add(propertyInfo);
                        }
                    }
                }

                if (foreignKeyPiList.Any())
                {
                    foreignKeyPropertyInfos = foreignKeyPiList.ToArray();
                    foreignNavPropertyInfo = foreignNavPi;
                    adjacentNavPropertyInfo = adjacentNavPi;

                    return true;
                }
            }

            return false;
        }

        public class Result
        {
            public PropertyInfo LeftNavPi { get; }
            public PropertyInfo[] LeftKeysToJoinOn { get; }
            public PropertyInfo RightNavPi { get; }
            public PropertyInfo[] RightKeysToJoinOn { get; }

            public Result(PropertyInfo leftNavPi, PropertyInfo[] leftKeysToJoinOn, PropertyInfo rightNavPi, PropertyInfo[] rightKeysToJoinOn)
            {
                LeftNavPi = leftNavPi;
                LeftKeysToJoinOn = leftKeysToJoinOn;
                RightNavPi = rightNavPi;
                RightKeysToJoinOn = rightKeysToJoinOn;
            }
        }
    }
}
