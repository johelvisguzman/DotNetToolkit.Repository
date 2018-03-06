namespace DotNetToolkit.Repository.EntityFramework
{
    using FetchStrategies;
    using System;
    using System.Data.Entity;
    using System.Linq;

    /// <summary>
    /// Represents a repository for entity framework.
    /// </summary>
    public abstract class EfRepositoryBase<TEntity, TKey> : RepositoryBase<TEntity, TKey> where TEntity : class
    {
        #region Fields

        private bool _disposed;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the entity set.
        /// </summary>
        protected IDbSet<TEntity> DbSet { get; private set; }

        /// <summary>
        /// Gets the database context.
        /// </summary>
        protected DbContext Context { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfRepositoryBase{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        protected EfRepositoryBase(DbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            Context = context;
            DbSet = Context.Set<TEntity>();
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
                if (Context != null)
                {
                    Context.Dispose();
                }
            }

            _disposed = true;
        }

        #endregion

        #region Overrides of RepositoryBase<TEntity,TKey>

        /// <summary>
        /// A protected overridable method for adding the specified <paramref name="entity" /> into the repository.
        /// </summary>
        protected override void AddItem(TEntity entity)
        {
            DbSet.Add(entity);
        }

        /// <summary>
        /// A protected overridable method for deleting the specified <paramref name="entity" /> from the repository.
        /// </summary>
        protected override void DeleteItem(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        /// <summary>
        /// A protected overridable method for updating the specified <paramref name="entity" /> in the repository.
        /// </summary>
        protected override void UpdateItem(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// A protected overridable method for saving changes made in the current unit of work in the repository.
        /// </summary>
        protected override void SaveChanges()
        {
            Context.SaveChanges();
        }

        /// <summary>
        /// A protected overridable method for getting an entity query that supplies the specified fetching strategy from the repository.
        /// </summary>
        protected override IQueryable<TEntity> GetQuery(IFetchStrategy<TEntity> fetchStrategy = null)
        {
            var query = DbSet.AsQueryable();
            return fetchStrategy == null ? query : fetchStrategy.IncludePaths.Aggregate(query, (current, path) => current.Include(path));
        }

        /// <summary>
        /// Gets an entity query with the given primary key value from the repository.
        /// </summary>
        protected override TEntity GetQuery(TKey key, IFetchStrategy<TEntity> fetchStrategy)
        {
            return fetchStrategy == null ? DbSet.Find(key) : base.GetQuery(key, fetchStrategy);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
