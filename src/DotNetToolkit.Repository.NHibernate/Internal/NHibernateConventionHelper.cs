namespace DotNetToolkit.Repository.NHibernate.Internal
{
    using global::NHibernate;
    using global::NHibernate.Persister.Entity;
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;
    using Utility;

    internal class NHibernateConventionHelper
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _primaryKeyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

        private readonly ISessionFactory _sessionFactory;

        public NHibernateConventionHelper(ISessionFactory sessionFactory)
        {
            _sessionFactory = Guard.NotNull(sessionFactory, nameof(sessionFactory));
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
            return GetAbstractEntityPersister(entityType)
                ?.KeyColumnNames
                .Select(entityType.GetProperty)
                .ToArray();
        }

        private AbstractEntityPersister GetAbstractEntityPersister(Type entityType)
        {
            return (AbstractEntityPersister)_sessionFactory.GetClassMetadata(entityType);
        }
    }
}
