namespace DotNetToolkit.Repository.NHibernate.Internal
{
    using global::NHibernate;
    using global::NHibernate.Persister.Entity;
    using System;
    using System.Linq;
    using System.Reflection;
    using Utility;

    internal class NHibernateConventionHelper
    {
        private readonly ISessionFactory _sessionFactory;

        public NHibernateConventionHelper(ISessionFactory sessionFactory)
        {
            _sessionFactory = Guard.NotNull(sessionFactory, nameof(sessionFactory));
        }

        public PropertyInfo[] GetPrimaryKeyPropertyInfos(Type entityType)
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
