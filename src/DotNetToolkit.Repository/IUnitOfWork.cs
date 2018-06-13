namespace DotNetToolkit.Repository
{
    using System;

    /// <summary>
    /// Maintains business objects in-memory which have been changed (inserted, updated, or deleted) in a single transaction.
    /// Once the transaction is completed, the changes will be sent to the database in one single unit of work.
    /// </summary>
    /// <see cref="System.IDisposable"/>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets a new repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>A new repository for the specified entity type.</returns>
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        /// <summary>
        /// Gets a new repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <returns>A new repository for the specified entity and primary key type.</returns>
        IRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : class;

        /// <summary>
        /// Commits all changes made in this unit of work.
        /// </summary>
        void Commit();
    }
}
