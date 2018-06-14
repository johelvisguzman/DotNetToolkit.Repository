namespace DotNetToolkit.Repository.Traits
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a trait for updating items in a repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface ICanUpdate<in TEntity> where TEntity : class
    {
        /// <summary>
        /// Updates the specified <paramref name="entity" /> in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Updates the specified <paramref name="entities" /> collection in the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        void Update(IEnumerable<TEntity> entities);
    }
}

namespace DotNetToolkit.Repository.Transactions.Traits
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a trait for updating items in a unit of work repository.
    /// </summary>
    public interface ICanUpdate
    {
        /// <summary>
        /// Updates the specified <paramref name="entity" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity to update.</param>
        void Update<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Updates the specified <paramref name="entities" /> collection in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entities">The collection of entities to update.</param>
        void Update<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
    }
}
