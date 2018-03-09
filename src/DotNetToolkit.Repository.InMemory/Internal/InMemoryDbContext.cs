namespace DotNetToolkit.Repository.InMemory.Internal
{
    using Helpers;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents an internal in-memory database context which houses all of the entities in the in-memory store by their type and primary key value.
    /// </summary>
    internal class InMemoryDbContext
    {
        #region Fields

        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<object, EntitySet>> _context;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryDbContext"/> class.
        /// </summary>
        public InMemoryDbContext()
        {
            _context = new ConcurrentDictionary<Type, ConcurrentDictionary<object, EntitySet>>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether the database context an contains an entity of the specified type with the given primary key value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="key">The primary key value.</param>
        /// <returns>
        ///   <c>true</c> if database context contains an entity of the specified type with the given primary key value; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey<TEntity>(object key) where TEntity : class
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var context = GetContext(typeof(TEntity));

            return context.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether the database context an contains an entity of the specified type.
        /// </summary>
        /// <param name="entitySet">The entity set.</param>
        /// <returns>
        ///   <c>true</c> if database context contains an entity of the specified type; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(EntitySet entitySet)
        {
            if (entitySet == null)
                throw new ArgumentNullException(nameof(entitySet));

            var key = ConventionHelper.GetPrimaryKeyPropertyValue(entitySet.Entity);
            var context = GetContext(entitySet.Entity.GetType());

            return context.ContainsKey(key);
        }

        /// <summary>
        /// Returns an entity of the specified type with the given primary key value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="key">The primary key value.</param>
        /// <returns>The entity with the given primary key value</returns>
        public EntitySet GetEntitySet<TEntity>(object key) where TEntity : class
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var context = GetContext(typeof(TEntity));

            context.TryGetValue(key, out EntitySet entitySet);

            return entitySet;
        }

        /// <summary>
        /// Returns the collection of entity sets of the specified type in the database context.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>The collection of entity set of the specified type in the database context.</returns>
        public IEnumerable<EntitySet> GetEntitySets<TEntity>() where TEntity : class
        {
            var context = GetContext(typeof(TEntity));
            var query = from item in context
                        orderby item.Value.Order
                        select item.Value;

            return query;
        }

        /// <summary>
        /// Returns the collection of all the entity sets in the database context.
        /// </summary>
        /// <returns>The collection of all the entity sets in the database context.</returns>
        public IEnumerable<EntitySet> GetEntitySets()
        {
            var query = from c in _context
                        from item in c.Value
                        orderby item.Value.Order
                        select item.Value;

            return query;
        }

        /// <summary>
        /// Returns the collection of primary key values of the specified entity type in the database context.
        /// </summary>
        /// <returns>The collection of primary key values of the specified entity type in the database context.</returns>
        public IEnumerable<object> GetKeys(Type entityType)
        {
            var context = GetContext(entityType);
            var query = from item in context
                        orderby item.Value.Order
                        select item.Key;

            return query;
        }

        /// <summary>
        /// Removes the specified entity type with the given primary key value from the database context.
        /// </summary>
        /// <param name="entitySet">The entity set.</param>
        /// <returns><c>true</c> if entity is successfully removed; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="entitySet"/> is <c>null</c>.</exception>
        public bool Remove(EntitySet entitySet)
        {
            if (entitySet == null)
                throw new ArgumentNullException(nameof(entitySet));

            var key = ConventionHelper.GetPrimaryKeyPropertyValue(entitySet.Entity);
            var context = GetContext(entitySet.Entity.GetType());

            return context.TryRemove(key, out EntitySet set);
        }

        /// <summary>
        /// Adds an entity to the database context.
        /// </summary>
        /// <param name="entitySet">The entity set.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="entitySet"/> is <c>null</c>.</exception>
        public void Add(EntitySet entitySet)
        {
            if (entitySet == null)
                throw new ArgumentNullException(nameof(entitySet));

            var context = GetContext(entitySet.Entity.GetType());
            var key = context
                .Where(x => x.Value.Entity.Equals(entitySet.Entity))
                .Select(x => x.Key)
                .SingleOrDefault();

            if (key == null)
                key = ConventionHelper.GetPrimaryKeyPropertyValueOrDefault(entitySet.Entity) ?? Guid.NewGuid();

            var order = context.Values
                .OrderByDescending(x => x.Order)
                .Select(x => x.Order)
                .FirstOrDefault();

            entitySet.Order = order + 1;

            context[key] = entitySet;
        }

        /// <summary>
        /// Clears the entire context.
        /// </summary>
        public void Clear()
        {
            _context.Clear();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the in-memory store which contains all the entities for the specified type.
        /// </summary>
        private ConcurrentDictionary<object, EntitySet> GetContext(Type entityType)
        {
            return _context.GetOrAdd(entityType, new ConcurrentDictionary<object, EntitySet>());
        }

        #endregion
    }
}