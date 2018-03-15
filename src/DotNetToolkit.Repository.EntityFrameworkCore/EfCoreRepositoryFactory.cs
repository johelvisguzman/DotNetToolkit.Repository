﻿namespace DotNetToolkit.Repository.EntityFrameworkCore
{
    using Microsoft.EntityFrameworkCore;
    using System;

    /// <summary>
    /// An implementation of <see cref="IRepositoryFactory" />.
    /// </summary>
    public class EfCoreRepositoryFactory : IRepositoryFactory
    {
        #region Private Methods

        private static DbContext GetDbContext(IRepositoryOptions options)
        {
            if (options.DbContextType == null)
                throw new InvalidOperationException($"The repository options must provide a {nameof(options.DbContextType)}.");

            DbContext context;

            if (options.DbContextArgs == null)
                context = (DbContext)Activator.CreateInstance(options.DbContextType);
            else
                context = (DbContext)Activator.CreateInstance(options.DbContextType, options.DbContextArgs);

            return context;
        }

        #endregion

        #region Implementation of IRepositoryFactory

        /// <summary>
        /// Creates a new repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="options">The options.</param>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity> Create<TEntity>(IRepositoryOptions options) where TEntity : class
        {
            return new EfCoreRepository<TEntity>(GetDbContext(options));
        }

        /// <summary>
        /// Creates a new repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <param name="options">The options.</param>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity, TKey> Create<TEntity, TKey>(IRepositoryOptions options) where TEntity : class
        {
            return new EfCoreRepository<TEntity, TKey>(GetDbContext(options));
        }

        #endregion
    }
}