namespace DotNetToolkit.Repository.Traits
{
    using Queries;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a trait for deleting items from a repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the primary key.</typeparam>
    public interface ICanDelete<TEntity, in TKey> where TEntity : class
    {
        /// <summary>
        /// Deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        void Delete(TKey key);

        /// <summary>
        /// Deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        void Delete(IQueryOptions<TEntity> options);

        /// <summary>
        /// Deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        void Delete(IEnumerable<TEntity> entities);
    }

    /// <summary>
    /// Represents a trait for deleting items from a repository.
    /// </summary>
    public interface ICanDelete
    {
        /// <summary>
        /// Deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the primary key.</typeparam>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        void Delete<TEntity, TKey>(TKey key) where TEntity : class;

        /// <summary>
        /// Deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity to delete.</param>
        void Delete<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        void Delete<TEntity>(IQueryOptions<TEntity> options) where TEntity : class;

        /// <summary>
        /// Deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entities">The collection of entities to delete.</param>
        void Delete<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
    }
}
