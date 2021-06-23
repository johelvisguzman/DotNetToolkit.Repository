namespace DotNetToolkit.Repository.Configuration.Conventions.Internal
{
    using Extensions;
    using JetBrains.Annotations;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Reflection;
    using Utility;

    internal class PrimaryKeyConventionHelper
    {
        public static PropertyInfo[] GetPrimaryKeyPropertyInfos([NotNull] Type entityType)
        {
            Guard.NotNull(entityType, nameof(entityType));
            
            // Gets by checking the annotations
            var propertyInfos = entityType
                .GetRuntimeProperties()
                .Where(x => ModelConventionHelper.IsColumnMapped(x) && x.GetCustomAttribute<KeyAttribute>() != null)
                .OrderBy(x =>
                {
                    var columnAttribute = x.GetCustomAttribute<ColumnAttribute>();
                    if (columnAttribute != null && columnAttribute.Order > 0)
                        return columnAttribute.Order;

                    return int.MaxValue;
                })
                .ToList();

            // Gets by naming convention
            if (!propertyInfos.Any())
            {
                foreach (var propertyName in GetDefaultPrimaryKeyNameChecks(entityType))
                {
                    var propertyInfo = entityType.GetProperty(propertyName);

                    if (propertyInfo != null && ModelConventionHelper.IsColumnMapped(propertyInfo))
                    {
                        propertyInfos.Add(propertyInfo);

                        break;
                    }
                }
            }

            return propertyInfos.ToArray();
        }

        private static IEnumerable<string> GetDefaultPrimaryKeyNameChecks([NotNull] Type entityType)
        {
            Guard.NotNull(entityType, nameof(entityType));

            const string suffix = "Id";

            return new[] { suffix, entityType.Name + suffix };
        }
    }
}
