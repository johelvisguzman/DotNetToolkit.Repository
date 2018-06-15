﻿namespace DotNetToolkit.Repository.AdoNet
{
    using Factories;
    using Interceptors;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;

    /// <summary>
    /// An implementation of <see cref="IRepositoryFactoryAsync" />.
    /// </summary>
    public class AdoNetRepositoryFactory : IRepositoryFactoryAsync
    {
        #region Fields

        private readonly DbTransaction _transaction;
        private readonly string _connectionString;
        private readonly string _providerName;
        private readonly IEnumerable<IRepositoryInterceptor> _interceptors;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryFactory"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetRepositoryFactory(string connectionString, IEnumerable<IRepositoryInterceptor> interceptors = null)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            _connectionString = connectionString;
            _interceptors = interceptors ?? Enumerable.Empty<IRepositoryInterceptor>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryFactory"/> class.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetRepositoryFactory(string providerName, string connectionString, IEnumerable<IRepositoryInterceptor> interceptors = null)
        {
            if (providerName == null)
                throw new ArgumentNullException(nameof(providerName));

            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            _connectionString = connectionString;
            _providerName = providerName;
            _interceptors = interceptors ?? Enumerable.Empty<IRepositoryInterceptor>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryFactory"/> class.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetRepositoryFactory(DbTransaction transaction, IEnumerable<IRepositoryInterceptor> interceptors = null)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            _transaction = transaction;
            _connectionString = _transaction.Connection.ConnectionString;
            _interceptors = interceptors ?? Enumerable.Empty<IRepositoryInterceptor>();
        }

        #endregion

        #region Implementation of IRepositoryFactory

        /// <summary>
        /// Creates a new repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity> Create<TEntity>() where TEntity : class
        {
            return CreateAsync<TEntity>();
        }

        /// <summary>
        /// Creates a new repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity, TKey> Create<TEntity, TKey>() where TEntity : class
        {
            return CreateAsync<TEntity, TKey>();
        }

        /// <summary>
        /// Creates a new repository for the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The new repository.</returns>
        public T CreateInstance<T>() where T : class
        {
            var args = new List<object>();

            if (_transaction != null)
            {
                args.Add(_transaction);
            }
            else
            {
                if (!string.IsNullOrEmpty(_providerName))
                    args.Add(_providerName);

                args.Add(_connectionString);
            }

            if (_interceptors.Any())
                args.Add(_interceptors);

            return (T)Activator.CreateInstance(typeof(T), args.ToArray());
        }

        #endregion

        #region Implementation of IRepositoryFactoryAsync

        /// <summary>
        /// Creates a new asynchronous repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>The new asynchronous repository.</returns>
        public IRepositoryAsync<TEntity> CreateAsync<TEntity>() where TEntity : class
        {
            return CreateInstance<AdoNetRepository<TEntity>>();
        }

        /// <summary>
        /// Creates a new asynchronous repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <returns>The new asynchronous repository.</returns>
        public IRepositoryAsync<TEntity, TKey> CreateAsync<TEntity, TKey>() where TEntity : class
        {
            return CreateInstance<AdoNetRepository<TEntity, TKey>>();
        }

        #endregion
    }
}