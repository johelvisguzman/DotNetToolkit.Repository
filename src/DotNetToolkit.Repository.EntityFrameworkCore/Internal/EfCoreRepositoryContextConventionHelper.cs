namespace DotNetToolkit.Repository.EntityFrameworkCore.Internal
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Reflection;

    internal static class EfCoreRepositoryContextConventionHelper
    {
        public static PropertyInfo[] GetPrimaryKeyPropertyInfos(DbContext context, Type entityType)
        {
            return context.Model
                .FindEntityType(entityType)
                .FindPrimaryKey()
                .Properties
                .Select(x => x.PropertyInfo)
                .ToArray();
        }
    }
}
