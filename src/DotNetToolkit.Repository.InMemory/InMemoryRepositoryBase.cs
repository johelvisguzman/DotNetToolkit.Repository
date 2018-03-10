namespace DotNetToolkit.Repository.InMemory
{
    using FetchStrategies;
    using Internal;
    using System;
    using System.Linq;

    /// <summary>
    /// Represents a repository for in-memory operations (for testing purposes).
    /// </summary>
    public abstract class InMemoryRepositoryBase<TEntity, TKey> : RepositoryBase<TEntity, TKey> where TEntity : class
    {
        #region Fields

        private readonly InMemoryStore _store;
        private bool _disposed;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryBase{TEntity,TKey}"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        protected InMemoryRepositoryBase(string databaseName = null)
        {
            _store = new InMemoryStore(databaseName);
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
                if (_store != null)
                {
                    _store.Dispose();
                }
            }

            _disposed = true;
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

            _store.Add(entity);
        }

        /// <summary>
        /// A protected overridable method for deleting the specified <paramref name="entity" /> from the repository.
        /// </summary>
        protected override void DeleteItem(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _store.Remove(entity);
        }

        /// <summary>
        /// A protected overridable method for updating the specified <paramref name="entity" /> in the repository.
        /// </summary>
        protected override void UpdateItem(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _store.Update(entity);
        }

        /// <summary>
        /// A protected overridable method for saving changes made in the current unit of work in the repository.
        /// </summary>
        protected override void SaveChanges()
        {
            _store.SaveChanges();
        }

        /// <summary>
        /// A protected overridable method for getting an entity query that supplies the specified fetching strategy from the repository.
        /// </summary>
        protected override IQueryable<TEntity> GetQuery(IFetchStrategy<TEntity> fetchStrategy = null)
        {
            return _store.Query<TEntity>();
        }

        #endregion
    }
}
