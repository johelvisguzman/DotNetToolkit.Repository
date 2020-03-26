namespace DotNetToolkit.Repository.Configuration.Interceptors
{
    using JetBrains.Annotations;
    using Utility;

    /// <summary>
    /// Represents contextual information associated with calls into <see cref="IRepositoryInterceptor" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity associated to the current operation.</typeparam>
    public class RepositoryInterceptionContext<TEntity> where TEntity : class
    {
        /// <summary>
        /// Gets the entity associated with the current operation.
        /// </summary>
        public TEntity Entity { get; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        public IRepositoryContext Context { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryInterceptionContext{TEntity}"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="context">The context.</param>
        public RepositoryInterceptionContext([NotNull] TEntity entity, [NotNull] IRepositoryContext context)
        {
            Entity = Guard.NotNull(entity, nameof(entity));
            Context = Guard.NotNull(context, nameof(context));
        }
    }
}
