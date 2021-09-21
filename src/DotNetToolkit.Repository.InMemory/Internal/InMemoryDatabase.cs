namespace DotNetToolkit.Repository.InMemory.Internal
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Utility;

    internal class InMemoryDatabase
    {
        #region Fields

        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<object, object>> _store;

        #endregion

        #region Contructors

        private InMemoryDatabase()
        {
            _store = new ConcurrentDictionary<Type, ConcurrentDictionary<object, object>>();
        }

        #endregion

        #region Public Methods

        public static InMemoryDatabase Empty()
        {
            return new InMemoryDatabase();
        }

        public void Clear()
        {
            _store.Clear();
        }

        public void Clear(Type entityType)
        {
            Guard.NotNull(entityType, nameof(entityType));

            var context = GetContext(entityType);
            context.Clear();
        }

        public bool Contains(Type entityType, object key)
        {
            Guard.NotNull(entityType, nameof(entityType));
            Guard.NotNull(key, nameof(key));

            return GetContext(entityType).ContainsKey(key);
        }

        public bool Contains(Type entityType)
        {
            Guard.NotNull(entityType, nameof(entityType));

            return GetContext(entityType).Count > 0;
        }

        public void AddOrUpdate(Type entityType, object entity, object key)
        {
            Guard.NotNull(entityType, nameof(entityType));
            Guard.NotNull(entity, nameof(entity));

            var context = GetContext(entityType);

            context[key] = Clone(entity);
        }

        public bool Remove(Type entityType, object key)
        {
            Guard.NotNull(entityType, nameof(entityType));
            Guard.NotNull(key, nameof(key));

            var context = GetContext(entityType);

            return context.TryRemove(key, out _);
        }

        public object GetLastKey(Type entityType)
        {
            Guard.NotNull(entityType, nameof(entityType));

            var context = GetContext(entityType);

            return context.Keys.LastOrDefault();
        }

        public bool TryFind(Type entityType, object key, out object entity)
        {
            Guard.NotNull(entityType, nameof(entityType));
            Guard.NotNull(key, nameof(key));

            var context = GetContext(entityType);

            if (context.TryGetValue(key, out object obj))
            {
                entity = Clone(obj);
                return true;
            }

            entity = null;
            return false;
        }

        public IEnumerable<object> FindAll(Type entityType)
        {
            Guard.NotNull(entityType, nameof(entityType));

            var context = GetContext(entityType);

            foreach (var item in context)
            {
                yield return Clone(item.Value);
            }
        }

        #endregion

        #region Private Methods

        private ConcurrentDictionary<object, object> GetContext(Type entityType)
        {
            return _store.GetOrAdd(entityType, new ConcurrentDictionary<object, object>());
        }

        private static object Clone(object entity)
        {
            return CloneableHelper.DeepCopy(entity);
        }

        #endregion
    }
}