namespace DotNetToolkit.Repository.Configuration.Interceptors
{
    /// <summary>
    /// An implementation of <see cref="IRepositoryInterceptor" />.
    /// </summary>
    /// <seealso cref="IRepositoryInterceptor" />
    public abstract class RepositoryInterceptorBase : IRepositoryInterceptor
    {
        #region Implementation of IRepositoryInterceptor

        /// <inheritdoc />
        public virtual void AddExecuting<TEntity>(TEntity entity) { }

        /// <inheritdoc />
        public virtual void AddExecuted<TEntity>(TEntity entity) { }

        /// <inheritdoc />
        public virtual void DeleteExecuting<TEntity>(TEntity entity) { }

        /// <inheritdoc />
        public virtual void DeleteExecuted<TEntity>(TEntity entity) { }

        /// <inheritdoc />
        public virtual void UpdateExecuting<TEntity>(TEntity entity) { }

        /// <inheritdoc />
        public virtual void UpdateExecuted<TEntity>(TEntity entity) { }

        #endregion
    }
}
