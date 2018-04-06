﻿namespace DotNetToolkit.Repository.AdoNet
{
    using System;

    /// <summary>
    /// An implementation of <see cref="IRepositoryFactory" />.
    /// </summary>
    public class AdoNetRepositoryFactory : IRepositoryFactory
    {
        #region Private Methods

        private Tuple<string, string> GetProviderAndConnectionString(IRepositoryOptions options)
        {
            var arg1 = options.DbContextArgs[0];
            var provider = arg1 as string;

            if (arg1 != null && provider == null)
                throw new ArgumentException($"The provided {nameof(options.DbContextArgs)} must be a valid string argument.");

            var arg2 = options.DbContextArgs[1];
            var connectionString = arg1 as string;

            if (arg2 != null && connectionString == null)
                throw new ArgumentException($"The connection string {nameof(options.DbContextArgs)} must be a valid string argument.");

            return Tuple.Create<string, string>(provider, connectionString);
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
            var t = GetProviderAndConnectionString(options);
            return new AdoNetRepository<TEntity>(t.Item1, t.Item2, options.Logger);
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
            var t = GetProviderAndConnectionString(options);
            return new AdoNetRepository<TEntity, TKey>(t.Item1, t.Item2, options.Logger);
        }

        #endregion
    }
}