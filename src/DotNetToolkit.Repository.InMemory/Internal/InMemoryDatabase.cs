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

        #region Private Methods

        private static object CombinePrimaryKeyValues(object[] keyValues)
        {
            return keyValues.Length == 1 ? keyValues[0] : string.Join(":", keyValues);
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

        public void AddOrUpdate<T>(T entity, object[] keyValues)
        {
            Guard.NotNull(entity, nameof(entity));
            Guard.NotEmpty(keyValues, nameof(keyValues));

            var entityType = typeof(T);
            var key = CombinePrimaryKeyValues(keyValues);
            var context = GetContext(entityType);

            context[key] = Clone(entity);
        }

        public bool Remove<T>(object[] keyValues)
        {
            Guard.NotEmpty(keyValues, nameof(keyValues));

            var entityType = typeof(T); 
            var key = CombinePrimaryKeyValues(keyValues);
            var context = GetContext(entityType);

            return context.TryRemove(key, out _);
        }

        public bool TryFind<T>(object[] keyValues, out object entity)
        {
            Guard.NotEmpty(keyValues, nameof(keyValues));

            var entityType = typeof(T);
            var key = CombinePrimaryKeyValues(keyValues);
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

        public IEnumerable<T> FindAll<T>()
        {
            return FindAll(typeof(T)).Cast<T>();
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