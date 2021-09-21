namespace DotNetToolkit.Repository.InMemory.Internal
{
    using Configuration.Conventions;
    using Configuration.Conventions.Internal;
    using Extensions;
    using Properties;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Utility;

    internal class InMemoryUnderlyingDbContext : IDisposable
    {
        #region Fields

        private readonly IRepositoryConventions _conventions;
        private readonly BlockingCollection<EntitySet> _items = new BlockingCollection<EntitySet>();
        private readonly InMemoryDatabase _db;

        #endregion

        #region Constructors

        public InMemoryUnderlyingDbContext(string databaseName, IRepositoryConventions conventions)
        {
            _conventions = Guard.NotNull(conventions, nameof(conventions));
            _db = InMemoryDatabaseStoreCache.Instance.GetDatabase(databaseName);
        }

        #endregion

        #region Public Methods

        public void ClearDatabase()
        {
            ClearPendingItems();
            _db.Clear();
        }

        public void Add(object entity)
        {
            _items.Add(new EntitySet(Guard.NotNull(entity, nameof(entity)), EntityState.Added));
        }

        public void Remove(object entity)
        {
            _items.Add(new EntitySet(Guard.NotNull(entity, nameof(entity)), EntityState.Removed));
        }

        public void Update(object entity)
        {
            _items.Add(new EntitySet(Guard.NotNull(entity, nameof(entity)), EntityState.Modified));
        }

        public T Find<T>(params object[] keyValues)
        {
            var entityType = typeof(T);

            if (!_db.Contains(entityType))
                return default(T);

            var key = Combine(keyValues);

            if (_db.TryFind(entityType, key, out object entity))
            {
                var result = (T)Convert.ChangeType(entity, entityType);

                return result;
            }

            return default(T);
        }

        public IEnumerable<object> FindAll(Type entityType)
        {
            return _db.FindAll(entityType);
        }

        public IEnumerable<T> FindAll<T>()
        {
            return FindAll(typeof(T)).Cast<T>();
        }

        public int SaveChanges()
        {
            var count = 0;

            try
            {
                while (_items.TryTake(out var entitySet))
                {
                    var entity = entitySet.Entity;
                    var entityType = entity.GetType();
                    var key = GetPrimaryKeyValue(entity);

                    if (entitySet.State == EntityState.Added)
                    {
                        if (_db.Contains(entityType, key))
                        {
                            throw new InvalidOperationException(
                                string.Format(
                                    CultureInfo.CurrentCulture,
                                    Resources.EntityAlreadyBeingTrackedInStore,
                                    entityType));
                        }

                        var primaryKeyPropertyInfo = _conventions.GetPrimaryKeyPropertyInfos(entityType).First();

                        if (ModelConventionHelper.IsColumnIdentity(_conventions, primaryKeyPropertyInfo))
                        {
                            key = GeneratePrimaryKey(primaryKeyPropertyInfo, entityType);

                            primaryKeyPropertyInfo.SetValue(entity, key);
                        }
                    }
                    else if (!_db.Contains(entityType, key))
                    {
                        throw new InvalidOperationException(Resources.EntityNotFoundInStore);
                    }

                    if (entitySet.State == EntityState.Removed)
                    {
                        _db.Remove(entityType, key);
                    }
                    else
                    {
                        _db.AddOrUpdate(entityType, entity, key);
                    }

                    count++;
                }
            }
            finally
            {
                ClearPendingItems();
            }

            return count;
        }

        #endregion

        #region Private Methods

        private void ClearPendingItems()
        {
            // Clears the collection
            while (_items.Count > 0)
            {
                _items.TryTake(out _);
            }
        }

        private object GeneratePrimaryKey(PropertyInfo propertyInfo, Type entityType)
        {
            var propertyType = propertyInfo.PropertyType;

            if (propertyType == typeof(Guid))
                return Guid.NewGuid();

            if (propertyType == typeof(string))
                return Guid.NewGuid().ToString("N");

            if (propertyType == typeof(int))
            {
                if (!_db.Contains(entityType))
                    return 1;

                var key = _db.GetLastKey(entityType);

                return Convert.ToInt32(key) + 1;
            }

            throw new InvalidOperationException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.EntityKeyValueTypeInvalid,
                    entityType.FullName,
                    propertyType));
        }

        private object GetPrimaryKeyValue(object entity)
        {
            return Combine(_conventions.GetPrimaryKeyValues(entity));
        }

        private static object Combine(object[] keyValues)
        {
            return keyValues.Length == 1 ? keyValues[0] : string.Join(":", keyValues);
        }

        #endregion

        #region Nested Type: EntitySet

        class EntitySet
        {
            public EntitySet(object entity, EntityState state)
            {
                Entity = entity;
                State = state;
            }

            public object Entity { get; }

            public EntityState State { get; }
        }

        #endregion

        #region Nested Type: EntityState

        enum EntityState
        {
            Added,
            Removed,
            Modified
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            ClearPendingItems();
        }

        #endregion
    }
}