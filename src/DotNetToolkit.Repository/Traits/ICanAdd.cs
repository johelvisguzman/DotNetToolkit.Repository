namespace DotNetToolkit.Repository.Traits
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a trait for adding items to a repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface ICanAdd<in TEntity> where TEntity : class
    {
        /// <summary>
        /// Adds the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        void Add(TEntity entity);

        /// <summary>
        /// Adds the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        void Add(IEnumerable<TEntity> entities);
    }
}
