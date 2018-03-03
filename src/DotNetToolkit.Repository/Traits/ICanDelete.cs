namespace DotNetToolkit.Repository.Traits
{
    using Specifications;
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
        /// Deletes all entities in the repository that satisfied the criteria specified by the <paramref name="criteria" />.
        /// </summary>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        void Delete(ISpecification<TEntity> criteria);

        /// <summary>
        /// Deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        void Delete(IEnumerable<TEntity> entities);
    }
}
