namespace DotNetToolkit.Repository.Configuration
{
    /// <summary>
    /// Represents an internal trait for initializing a repository context.
    /// </summary>
    internal interface IHaveRepositoryContextInitializer
    {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        void Initialize<TEntity>() where TEntity : class;
    }
}