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
        public static PropertyInfo[] GetPrimaryKeyPropertyInfos([NotNull] IRepositoryConventions conventions, [NotNull] Type entityType)
        {
            Guard.NotNull(conventions);
            Guard.NotNull(entityType);
            
            // Gets by checking the annotations
            var propertyInfos = entityType
                .GetRuntimeProperties()
                .Where(x => conventions.IsColumnMapped(x) && x.GetCustomAttribute<KeyAttribute>() != null)
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
                    var propertyInfo = entityType.GetTypeInfo().GetDeclaredProperty(propertyName);

                    if (propertyInfo != null && conventions.IsColumnMapped(propertyInfo))
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
            Guard.NotNull(entityType);

            const string suffix = "Id";

            return new[] { suffix, entityType.Name + suffix };
        }
    }
}
