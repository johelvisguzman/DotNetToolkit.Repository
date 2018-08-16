namespace DotNetToolkit.Repository.Configuration.Interceptors
{
    using System;
    using Queries;

    /// <summary>
    /// An implementation of <see cref="IRepositoryInterceptor" />.
    /// </summary>
    /// <seealso cref="IRepositoryInterceptor" />
    public abstract class RepositoryInterceptorBase : IRepositoryInterceptor
    {
        #region Implementation of IRepositoryInterceptor

        /// <summary>
        /// An activity method which is executed when adding an entity to the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public virtual void AddExecuting<TEntity>(TEntity entity) { }

        /// <summary>
        /// An activity method which is executed when an entity has been added to the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public virtual void AddExecuted<TEntity>(TEntity entity) { }

        /// <summary>
        /// An activity method which is executed when deleting an entity from the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public virtual void DeleteExecuting<TEntity>(TEntity entity) { }

        /// <summary>
        /// An activity method which is executed when an entity has been deleted from the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public virtual void DeleteExecuted<TEntity>(TEntity entity) { }

        /// <summary>
        /// An activity method which is executed when updating an entity in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public virtual void UpdateExecuting<TEntity>(TEntity entity) { }

        /// <summary>
        /// An activity method which is executed when an entity has been updated in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public virtual void UpdateExecuted<TEntity>(TEntity entity) { }

        /// <summary>
        /// An activity method which is executed when a query result is executed in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="queryResult">The query result.</param>
        public void QueryExecuted<TEntity, TResult>(QueryResult<TResult> queryResult) { }

        /// <summary>
        /// An activity method which is executed when an error/exception occurs in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="ex">The ex.</param>
        public virtual void Error<TEntity>(Exception ex) { }

        #endregion
    }
}
