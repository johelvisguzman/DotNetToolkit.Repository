namespace DotNetToolkit.Repository.InMemory.Internal
{
    using Helpers;
    using Properties;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents an in-memory store.
    /// </summary>
    /// <see cref="System.IDisposable" />
    internal class InMemoryStore : IDisposable
    {
        #region	Fields

        private readonly string _name;
        private InMemoryDbContext _context;
        private bool _disposed;

        #endregion

        #region	Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryStore"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        public InMemoryStore(string databaseName = null)
        {
            _name = string.IsNullOrEmpty(databaseName) ? typeof(InMemoryStore).FullName : databaseName;
            _context = new InMemoryDbContext();
        }

        #endregion

        #region Public Methods

        /// <summary>        
        /// Returns the entity query from the store.
        /// </summary>
        /// <returns>A <see cref="System.Linq.IQueryable{TEntity}" /> for the entity.</returns>
        public virtual IQueryable<TEntity> Query<TEntity>() where TEntity : class
        {
            ThrowIfDisposed();

            var context = InMemoryDbStorage.Instance.GetScopedContext(_name);

            return context.GetEntitySets<TEntity>().AsQueryable().Select(x => (TEntity)x.Entity);
        }

        /// <summary>
        /// Finds an entity with the given primary key.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="key">The primary key for the entity to be found.</param>
        /// <returns>The entity with the given primary key.</returns>
        public virtual TEntity Find<TEntity>(object key) where TEntity : class
        {
            ThrowIfDisposed();

            if (key == null) return null;

            var propertyInfo = ConventionHelper.GetPrimaryKeyPropertyInfo(typeof(TEntity));

            if (propertyInfo.PropertyType != key.GetType())
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyValueTypeMismatch, key.GetType(), propertyInfo.PropertyType));

            var context = InMemoryDbStorage.Instance.GetScopedContext(_name);
            var entitySet = context.GetEntitySet<TEntity>(key);

            return (TEntity)entitySet?.Entity;
        }

        /// <summary>
        /// Adds the specified <paramref name="entity" /> into the store.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity to add.</param>
        public virtual void Add<TEntity>(TEntity entity) where TEntity : class
        {
            ThrowIfDisposed();

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Add(new EntitySet(entity, EntityState.Added));
        }

        /// <summary>
        /// Removes the specified <paramref name="entity" /> in the store.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity to remove.</param>
        public virtual void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            ThrowIfDisposed();

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Add(new EntitySet(entity, EntityState.Removed));
        }

        /// <summary>
        /// Updates the specified <paramref name="entity" /> in the store.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity to update.</param>
        public virtual void Update<TEntity>(TEntity entity) where TEntity : class
        {
            ThrowIfDisposed();

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Add(new EntitySet(entity, EntityState.Modified));
        }

        /// <summary>
        /// Saves all pending changes made to the in-memory store.
        /// </summary>
        /// <returns>The number of items that were saved/persisted.</returns>
        public virtual int SaveChanges()
        {
            ThrowIfDisposed();

            var context = InMemoryDbStorage.Instance.GetScopedContext(_name);
            var count = 0;

            foreach (var entitySet in _context.GetEntitySets())
            {
                if (entitySet.State == EntityState.Added)
                {
                    var key = ConventionHelper.GetPrimaryKeyPropertyValueOrDefault(entitySet.Entity);
                    if (key == null)
                    {
                        var newKey = GeneratePrimaryKey(entitySet.Entity.GetType());

                        ConventionHelper.SetPrimaryKeyPropertyValue(entitySet.Entity, newKey);
                    }
                    else
                    {
                        if (context.Contains(entitySet))
                            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityAlreadyBeingTrackedInStore, entitySet.Entity.GetType()));
                    }
                }
                else
                {
                    if (!context.Contains(entitySet))
                        throw new InvalidOperationException(Resources.EntityNotFoundInStore);
                }

                if (entitySet.State == EntityState.Removed)
                {
                    context.Remove(entitySet);
                }
                else
                {
                    var entity = DeepCopy(entitySet.Entity);

                    context.Add(new EntitySet(entity, EntityState.Unchanged));
                }

                count++;
            }

            _context.Clear();

            return count;
        }

        /// <summary>
        /// Ensures the store is completely deleted.
        /// </summary>
        public void EnsureDeleted()
        {
            ThrowIfDisposed();

            _context.Clear();

            InMemoryDbStorage.Instance.GetScopedContext(_name).Clear();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Throws if this class has been disposed.
        /// </summary>
        protected virtual void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _context.Clear();
                _context = null;
            }

            _disposed = true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generates a new primary id for the entity.
        /// </summary>
        /// <returns>The new generated primary id.</returns>
        private object GeneratePrimaryKey(Type entityType)
        {
            var propertyInfo = ConventionHelper.GetPrimaryKeyPropertyInfo(entityType);
            var propertyType = propertyInfo.PropertyType;

            if (propertyType == typeof(Guid))
                return Convert.ChangeType(Guid.NewGuid(), propertyType);

            if (propertyType == typeof(string))
                return Convert.ChangeType(Guid.NewGuid().ToString("N"), propertyType);

            if (propertyType == typeof(int))
            {
                var context = InMemoryDbStorage.Instance.GetScopedContext(_name);

                return Convert.ToInt32(context.GetKeys(entityType).LastOrDefault()) + 1;
            }

            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyValueTypeInvalid, entityType, propertyType));
        }

        /// <summary>
        /// Returns a deep copy of the specified object. This method does not require the object to be marked as serializable.
        /// </summary>
        /// <param name="obj">The object to be copy.</param>
        /// <returns>The deep copy of the specified object.</returns>
        public static object DeepCopy(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var newItem = (object)Activator.CreateInstance(obj.GetType());
            foreach (var propInfo in obj.GetType().GetRuntimeProperties())
            {
                if (propInfo.CanWrite)
                    propInfo.SetValue(newItem, propInfo.GetValue(obj, null), null);
            }

            return newItem;
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}