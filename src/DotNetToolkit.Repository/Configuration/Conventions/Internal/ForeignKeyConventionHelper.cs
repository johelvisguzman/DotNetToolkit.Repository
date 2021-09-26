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
        private static readonly ConcurrentDictionary<PropertyInfo, Result> _foreignKeyCache = new ConcurrentDictionary<PropertyInfo, Result>();

        public static Result GetForeignKeyPropertyInfos([NotNull] PropertyInfo pi)
        {
            Guard.NotNull(pi, nameof(pi));

            if (!_foreignKeyCache.TryGetValue(pi, out var result))
            {
                result = GetForeignKeyPropertyInfosCore(pi);
                _foreignKeyCache.TryAdd(pi, result);
            }

            return result;
        }

        private static Result GetForeignKeyPropertyInfosCore(PropertyInfo pi)
        {
            var foreignType = pi.PropertyType.GetGenericTypeOrDefault();
            var declaringType = pi.DeclaringType;

            if (foreignType.IsEnumerable() || declaringType.IsEnumerable())
                return null;

            bool foundInSource;

            if (TryGetForeignKeyPropertyInfos(foreignType, declaringType,
                out var foreignKeyPropertyInfos,
                out var foreignNavPropertyInfo,
                out var adjacentNavPropertyInfo))
            {
                foundInSource = false;
                adjacentNavPropertyInfo = pi;
            }
            else if (TryGetForeignKeyPropertyInfos(declaringType, foreignType,
                out foreignKeyPropertyInfos,
                out foreignNavPropertyInfo,
                out adjacentNavPropertyInfo))
            {
                foundInSource = true;
            }
            else
            {
                return null;
            }

            var rightNavPi = pi;
            var rightPiType = rightNavPi.PropertyType.GetGenericTypeOrDefault();
            var rightKeysToJoinOn = foundInSource
                ? PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(rightPiType)
                : foreignKeyPropertyInfos;

            var leftNavPi = foreignNavPropertyInfo;
            var leftPiType = leftNavPi.PropertyType.GetGenericTypeOrDefault();
            var leftKeysToJoinOn = foundInSource
                ? foreignKeyPropertyInfos
                : PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(leftPiType);

            var newLeftNavPi = foundInSource ? adjacentNavPropertyInfo : leftNavPi;

            return new Result(newLeftNavPi, leftKeysToJoinOn, rightNavPi, rightKeysToJoinOn);
        }

        private static bool TryGetForeignKeyPropertyInfos(Type source, Type target,
            out PropertyInfo[] foreignKeyPropertyInfos,
            out PropertyInfo foreignNavPropertyInfo,
            out PropertyInfo adjacentNavPropertyInfo)
        {
            var propsFromSource = source.GetRuntimeProperties().Where(ModelConventionHelper.IsColumnMapped).ToList();
            var propsFromTarget = target.GetRuntimeProperties().Where(ModelConventionHelper.IsColumnMapped).ToList();

            var foreignNavPi = propsFromSource.FirstOrDefault(x => x.PropertyType == target);
            var adjacentNavPi = propsFromTarget.FirstOrDefault(x => x.PropertyType == source);

            var foreignKeyPiList = new List<PropertyInfo>();

            adjacentNavPropertyInfo = null;
            foreignKeyPropertyInfos = null;
            foreignNavPropertyInfo = null;

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
                                    source.FullName,
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
                var primaryKeyPropertyInfos = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(target);

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
