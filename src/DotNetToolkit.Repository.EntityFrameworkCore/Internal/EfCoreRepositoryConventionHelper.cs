namespace DotNetToolkit.Repository.EntityFrameworkCore.Internal
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Reflection;
    using Utility;

    internal class EfCoreRepositoryConventionHelper
    {
        private readonly DbContext _context;

        public EfCoreRepositoryConventionHelper(DbContext context)
        {
            _context = Guard.NotNull(context, nameof(context));
        }

        public PropertyInfo[] GetPrimaryKeyPropertyInfos(Type entityType)
        {
            return _context.Model
                .FindEntityType(entityType)
                ?.FindPrimaryKey()
                ?.Properties
                .Select(x => x.PropertyInfo)
                .ToArray();
        }
    }
}
