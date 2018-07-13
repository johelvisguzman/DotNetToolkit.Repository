namespace DotNetToolkit.Repository
{
    /// <summary>
    /// Represents an internal configurable repository context.
    /// </summary>
    internal interface IHaveRepositoryContextConfiguration
    {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        void Initialize<TEntity>() where TEntity : class;
    }
}
