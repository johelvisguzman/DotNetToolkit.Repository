namespace DotNetToolkit.Repository.EntityFrameworkCore.Internal
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;
    using Utility;

    internal class EfCoreRepositoryConventionHelper
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _primaryKeyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

        private readonly DbContext _context;

        public EfCoreRepositoryConventionHelper(DbContext context)
        {
            _context = Guard.NotNull(context, nameof(context));
        }

        public PropertyInfo[] GetPrimaryKeyPropertyInfos(Type entityType)
        {
            Guard.NotNull(entityType, nameof(entityType));
            
            if (!_primaryKeyCache.TryGetValue(entityType, out PropertyInfo[] result))
            {
                result = GetPrimaryKeyPropertyInfosCore(entityType);
                _primaryKeyCache.TryAdd(entityType, result);
            }
            
            return result;
        }

        private PropertyInfo[] GetPrimaryKeyPropertyInfosCore(Type entityType)
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
