namespace DotNetToolkit.Repository.AdoNet
{
    using Factories;
    using Interceptors;
    using Properties;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Common;

    /// <summary>
    /// Represents a unit of work for entity framework.
    /// </summary>
    /// <seealso cref="DotNetToolkit.Repository.IUnitOfWorkAsync" />
    public class AdoNetUnitOfWork : IUnitOfWorkAsync
    {
        #region Fields

        private readonly Dictionary<Type, object> _repositories;
        private readonly IRepositoryFactoryAsync _repositoryFactory;
        private bool _disposed;
        private DbTransaction _transaction;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetUnitOfWork"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetUnitOfWork(string connectionString, IEnumerable<IRepositoryInterceptor> interceptors = null)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            var ccs = ConfigurationManager.ConnectionStrings[connectionString];
            if (ccs == null)
                throw new ArgumentException(Resources.ConnectionStringDoestNotExistInConfigFile);

            BeginTransaction(ccs.ProviderName, connectionString);

            _repositoryFactory = new AdoNetRepositoryFactory(_transaction, interceptors);
            _repositories = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetUnitOfWork"/> class.
        /// </summary>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetUnitOfWork(string providerName, string connectionString, IEnumerable<IRepositoryInterceptor> interceptors = null)
        {
            if (providerName == null)
                throw new ArgumentNullException(nameof(providerName));

            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            BeginTransaction(providerName, connectionString);

            _repositoryFactory = new AdoNetRepositoryFactory(_transaction, interceptors);
            _repositories = new Dictionary<Type, object>();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_transaction != null)
                {
                    _transaction.Rollback();
                    _transaction.Dispose();
                    _transaction = null;
                }
            }

            _disposed = true;
        }

        #endregion

        #region Private Methods

        private void BeginTransaction(string providerName, string connectionString)
        {
            if (providerName == null)
                throw new ArgumentNullException(nameof(providerName));

            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            var dbProviderFactory = Internal.DbProviderFactories.GetFactory(providerName);
            var connection = dbProviderFactory.CreateConnection();

            connection.ConnectionString = connectionString;
            connection.Open();

            _transaction = connection.BeginTransaction();
        }

        #endregion

        #region Implementation of IUnitOfWork

        /// <summary>
        /// Gets a new repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>A new repository for the specified entity type.</returns>
        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return GetRepositoryAsync<TEntity>();
        }

        /// <summary>
        /// Gets a new repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <returns>A new repository for the specified entity and primary key type.</returns>
        public IRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : class
        {
            return GetRepositoryAsync<TEntity, TKey>();
        }

        /// <summary>
        /// Commits all changes made in this unit of work.
        /// </summary>
        public void Commit()
        {
            if (_transaction == null)
                throw new InvalidOperationException("The transaction has already been committed or disposed.");

            _transaction.Commit();
            _transaction = null;
        }

        #endregion

        #region Implementation of IUnitOfWorkAsync

        /// <summary>
        /// Gets a new asynchronous repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>A new asynchronous repository for the specified entity type.</returns>
        public IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>() where TEntity : class
        {
            if (_repositories.ContainsKey(typeof(TEntity)))
                return _repositories[typeof(TEntity)] as IRepositoryAsync<TEntity>;

            var repo = _repositoryFactory.CreateAsync<TEntity>();

            _repositories.Add(typeof(TEntity), repo);

            return repo;
        }

        /// <summary>
        /// Gets a new asynchronous repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <returns>A new asynchronous repository for the specified entity and primary key type.</returns>
        public IRepositoryAsync<TEntity, TKey> GetRepositoryAsync<TEntity, TKey>() where TEntity : class
        {
            if (_repositories.ContainsKey(typeof(TEntity)))
                return _repositories[typeof(TEntity)] as IRepositoryAsync<TEntity, TKey>;

            var repo = _repositoryFactory.CreateAsync<TEntity, TKey>();

            _repositories.Add(typeof(TEntity), repo);

            return repo;
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
