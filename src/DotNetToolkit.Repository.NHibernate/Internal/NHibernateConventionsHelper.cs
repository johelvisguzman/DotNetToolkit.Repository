namespace DotNetToolkit.Repository.NHibernate.Internal
{
    using global::NHibernate;
    using global::NHibernate.Persister.Entity;
    using System;
    using System.Linq;
    using System.Reflection;
    using Utility;

    internal class NHibernateConventionsHelper
    {
        private readonly ISessionFactory _sessionFactory;

        public NHibernateConventionsHelper(ISessionFactory sessionFactory)
        {
            _sessionFactory = Guard.NotNull(sessionFactory, nameof(sessionFactory));
        }

        public PropertyInfo[] GetPrimaryKeyPropertyInfos(Type entityType)
            => GetAbstractEntityPersister(entityType)?
                .KeyColumnNames
                .Select(entityType.GetProperty)
                .ToArray();

        public string GetColumnName(PropertyInfo pi)
            => GetAbstractEntityPersister(Guard.NotNull(pi, nameof(pi)).DeclaringType)
                .GetPropertyColumnNames(pi.Name)
                .FirstOrDefault();

        public int? GetColumnOrder(PropertyInfo pi)
        {
            var propertyNames = GetPropertyNames(pi);

            for (var i = 0; i < propertyNames.Length; i++)
            {
                if (propertyNames[i].Equals(pi.Name))
                    return i + 1;
            }

            return null;
        }

        public bool IsColumnMapped(PropertyInfo pi)
            => GetPropertyNames(pi).Contains(pi.Name);

        private string[] GetPropertyNames(PropertyInfo pi)
        {
            var persister = GetAbstractEntityPersister(Guard.NotNull(pi, nameof(pi)).DeclaringType);

            return persister.KeyColumnNames.Concat(persister.PropertyNames).ToArray();
        }

        private AbstractEntityPersister GetAbstractEntityPersister(Type entityType)
            => (AbstractEntityPersister)_sessionFactory.GetClassMetadata(Guard.NotNull(entityType, nameof(entityType)));
    }
}
