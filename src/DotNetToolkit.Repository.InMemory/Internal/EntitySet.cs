namespace DotNetToolkit.Repository.InMemory.Internal
{
    /// <summary>
    /// Represents an internal entity set in the in-memory store, which holds the entity and it's state representing the operation that was performed at the time.
    /// </summary>
    internal class EntitySet
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySet" /> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="state">The state.</param>
        public EntitySet(object entity, EntityState state)
        {
            Entity = entity;
            State = state;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the entity.
        /// </summary>
        public object Entity { get; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        public EntityState State { get; }

        #endregion
    }
}
