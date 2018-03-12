namespace DotNetToolkit.Repository.InMemory
{
    using FetchStrategies;
    using Internal;
    using Properties;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents a repository for in-memory operations (for testing purposes).
    /// </summary>
    public abstract class InMemoryRepositoryBase<TEntity, TKey> : RepositoryBase<TEntity, TKey> where TEntity : class
    {
        #region Fields

        private const string DefaultDatabaseName = "DotNetToolkit.Repository.InMemory";

        private readonly string _name;
        private InMemoryDbContext _context;
        private bool _disposed;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryBase{TEntity,TKey}"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        protected InMemoryRepositoryBase(string databaseName = null)
        {
            _name = string.IsNullOrEmpty(databaseName) ? DefaultDatabaseName : databaseName;
            _context = new InMemoryDbContext();
        }

        #endregion

        #region Protected Methods

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

        #region	Private Methods

        /// <summary>
        /// Returns a deep copy of the specified object. This method does not require the object to be marked as serializable.
        /// </summary>
        /// <param name="obj">The object to be copy.</param>
        /// <returns>The deep copy of the specified object.</returns>
        private static object DeepCopy(object obj)
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

        #region Overrides of RepositoryBase<TEntity,TKey>

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// A protected overridable method for adding the specified <paramref name="entity" /> into the repository.
        /// </summary>
        protected override void AddItem(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Add(new EntitySet(entity, GetPrimaryKeyPropertyValue(entity), EntityState.Added));
        }

        /// <summary>
        /// A protected overridable method for deleting the specified <paramref name="entity" /> from the repository.
        /// </summary>
        protected override void DeleteItem(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Add(new EntitySet(entity, GetPrimaryKeyPropertyValue(entity), EntityState.Removed));
        }

        /// <summary>
        /// A protected overridable method for updating the specified <paramref name="entity" /> in the repository.
        /// </summary>
        protected override void UpdateItem(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Add(new EntitySet(entity, GetPrimaryKeyPropertyValue(entity), EntityState.Modified));
        }

        /// <summary>
        /// A protected overridable method for saving changes made in the current unit of work in the repository.
        /// </summary>
        protected override void SaveChanges()
        {
            var context = InMemoryDbStorage.Instance.GetScopedContext(_name);

            foreach (var entitySet in _context.GetEntitySets())
            {
                var key = GetPrimaryKeyPropertyValue(entitySet.Entity);

                if (entitySet.State == EntityState.Added)
                {
                    if (key == null)
                    {
                        key = GeneratePrimaryKey(entitySet.Entity.GetType());
                        SetPrimaryKeyPropertyValue(entitySet.Entity, key);
                    }
                    else if (context.Contains(entitySet))
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityAlreadyBeingTrackedInStore, entitySet.Entity.GetType()));
                    }
                }
                else if (!context.Contains(entitySet))
                {
                    throw new InvalidOperationException(Resources.EntityNotFoundInStore);
                }

                if (entitySet.State == EntityState.Removed)
                {
                    context.Remove(entitySet);
                }
                else
                {
                    context.Add(new EntitySet(DeepCopy(entitySet.Entity), key, EntityState.Unchanged));
                }
            }

            _context.Clear();
        }

        /// <summary>
        /// A protected overridable method for getting an entity query that supplies the specified fetching strategy from the repository.
        /// </summary>
        protected override IQueryable<TEntity> GetQuery(IFetchStrategy<TEntity> fetchStrategy = null)
        {
            var context = InMemoryDbStorage.Instance.GetScopedContext(_name);

            return context.GetEntitySets<TEntity>().AsQueryable().Select(x => (TEntity)x.Entity);
        }

        /// <summary>
        /// Gets an entity query with the given primary key value from the repository.
        /// </summary>
        protected override TEntity GetEntity(TKey key, IFetchStrategy<TEntity> fetchStrategy)
        {
            if (fetchStrategy == null)
            {
                var propertyInfo = GetPrimaryKeyPropertyInfo(typeof(TEntity));

                if (propertyInfo.PropertyType != key.GetType())
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyValueTypeMismatch, key.GetType(), propertyInfo.PropertyType));

                var context = InMemoryDbStorage.Instance.GetScopedContext(_name);
                var entitySet = context.GetEntitySet<TEntity>(key);

                return (TEntity)entitySet?.Entity;
            }

            return base.GetEntity(key, fetchStrategy);
        }

        #endregion
    }
}