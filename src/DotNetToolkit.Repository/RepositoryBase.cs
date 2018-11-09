namespace DotNetToolkit.Repository
{
    using Configuration.Logging;
    using Configuration.Options;
    using Extensions;
    using Properties;
    using Queries.Strategies;
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// An implementation of <see cref="IRepository{TEntity, TKey1, TKey2, TKey3}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    /// <typeparam name="TKey3">The type of the third part of the composite primary key.</typeparam>
    public abstract class RepositoryBase<TEntity, TKey1, TKey2, TKey3> : Internal.RepositoryBase<TEntity>, IRepository<TEntity, TKey1, TKey2, TKey3> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity, TKey1, TKey2, TKey3}"/> class.
        /// </summary>
        /// <param name="options">The repository options.</param>
        protected RepositoryBase(RepositoryOptions options) : base(options) { }

        #endregion

        #region Implementation of IRepository<TEntity, TKey1, TKey2, TKey3>

        /// <summary>
        /// Deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        public void Delete(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            if (!TryDelete(key1, key2, key3))
            {
                var ex = new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyNotFound, key1 + ", " + key2 + ", " + key3));

                Logger.Error(ex);

                throw ex;
            }
        }

        /// <summary>
        /// Deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <returns><c>true</c> is able to successfully delete an entity with the given composite primary key values; otherwise, <c>false</c>.</returns>
        public bool TryDelete(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            var entity = Find(key1, key2, key3);

            if (entity == null)
                return false;

            Delete(entity);

            return true;
        }

        /// <summary>
        /// Finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <return>The entity found.</return>
        public TEntity Find(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            return Find(key1, key2, key3, (IFetchQueryStrategy<TEntity>)null);
        }

        /// <summary>
        /// Finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <return>The entity found.</return>
        public TEntity Find(TKey1 key1, TKey2 key2, TKey3 key3, IFetchQueryStrategy<TEntity> fetchStrategy)
        {
            return InterceptQueryResult<TEntity>(() => Context.Find<TEntity>(fetchStrategy, key1, key2, key3));
        }

        /// <summary>
        /// Determines whether the repository contains an entity with the given composite primary key values.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public bool Exists(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            return Find(key1, key2, key3) != null;
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity with the given composite primary key values.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public async Task<bool> ExistsAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken())
        {
            return await FindAsync(key1, key2, key3, cancellationToken) != null;
        }

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        public Task<TEntity> FindAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken())
        {
            return FindAsync(key1, key2, key3, (IFetchQueryStrategy<TEntity>)null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        public Task<TEntity> FindAsync(TKey1 key1, TKey2 key2, TKey3 key3, IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            return InterceptQueryResultAsync<TEntity>(() => Context.AsAsync().FindAsync<TEntity>(cancellationToken, fetchStrategy, key1, key2, key3));
        }

        /// <summary>
        /// Asynchronously deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken())
        {
            if (!await TryDeleteAsync(key1, key2, key3, cancellationToken))
            {
                var ex = new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyNotFound, key1 + ", " + key2 + ", " + key3));

                Logger.Error(ex);

                throw ex;
            }
        }

        /// <summary>
        /// Asynchronously deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> is able to successfully delete an entity with the given composite primary key values; otherwise, <c>false</c>.</returns>
        public async Task<bool> TryDeleteAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken())
        {
            InterceptError(cancellationToken.ThrowIfCancellationRequested);

            var entity = await FindAsync(key1, key2, key3, cancellationToken);

            if (entity == null)
                return false;

            await DeleteAsync(entity, cancellationToken);

            return true;
        }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IRepository{TEntity, TKey1, TKey2}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    public abstract class RepositoryBase<TEntity, TKey1, TKey2> : Internal.RepositoryBase<TEntity>, IRepository<TEntity, TKey1, TKey2> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity, TKey1, TKey2}"/> class.
        /// </summary>
        /// <param name="options">The repository options.</param>
        protected RepositoryBase(RepositoryOptions options) : base(options) { }

        #endregion

        #region Implementation of IRepository<TEntity, TKey1, TKey2>

        /// <summary>
        /// Deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        public void Delete(TKey1 key1, TKey2 key2)
        {
            if (!TryDelete(key1, key2))
            {
                var ex = new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyNotFound, key1 + ", " + key2));

                Logger.Error(ex);

                throw ex;
            }
        }

        /// <summary>
        /// Deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <returns><c>true</c> is able to successfully delete an entity with the given composite primary key values; otherwise, <c>false</c>.</returns>
        public bool TryDelete(TKey1 key1, TKey2 key2)
        {
            var entity = Find(key1, key2);

            if (entity == null)
                return false;

            Delete(entity);

            return true;
        }

        /// <summary>
        /// Finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <return>The entity found.</return>
        public TEntity Find(TKey1 key1, TKey2 key2)
        {
            return Find(key1, key2, (IFetchQueryStrategy<TEntity>)null);
        }

        /// <summary>
        /// Finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <return>The entity found.</return>
        public TEntity Find(TKey1 key1, TKey2 key2, IFetchQueryStrategy<TEntity> fetchStrategy)
        {
            return InterceptQueryResult<TEntity>(() => Context.Find<TEntity>(fetchStrategy, key1, key2));
        }

        /// <summary>
        /// Determines whether the repository contains an entity with the given composite primary key values.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public bool Exists(TKey1 key1, TKey2 key2)
        {
            return Find(key1, key2) != null;
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity with the given composite primary key values.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public async Task<bool> ExistsAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken())
        {
            return await FindAsync(key1, key2, cancellationToken) != null;
        }

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        public Task<TEntity> FindAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken())
        {
            return FindAsync(key1, key2, (IFetchQueryStrategy<TEntity>)null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        public Task<TEntity> FindAsync(TKey1 key1, TKey2 key2, IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            return InterceptQueryResultAsync<TEntity>(() => Context.AsAsync().FindAsync<TEntity>(cancellationToken, fetchStrategy, key1, key2));
        }

        /// <summary>
        /// Asynchronously deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken())
        {
            if (!await TryDeleteAsync(key1, key2, cancellationToken))
            {
                var ex = new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyNotFound, key1 + ", " + key2));

                Logger.Error(ex);

                throw ex;
            }
        }

        /// <summary>
        /// Asynchronously deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> is able to successfully delete an entity with the given composite primary key values; otherwise, <c>false</c>.</returns>
        public async Task<bool> TryDeleteAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken())
        {
            InterceptError(cancellationToken.ThrowIfCancellationRequested);

            var entity = await FindAsync(key1, key2, cancellationToken);

            if (entity == null)
                return false;

            await DeleteAsync(entity, cancellationToken);

            return true;
        }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IRepository{TEntity, TKey}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public abstract class RepositoryBase<TEntity, TKey> : Internal.RepositoryBase<TEntity>, IRepository<TEntity, TKey> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="options">The repository options.</param>
        protected RepositoryBase(RepositoryOptions options) : base(options) { }

        #endregion

        #region Implementation of IRepository<TEntity, TKey>

        /// <summary>
        /// Deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        public void Delete(TKey key)
        {
            if (!TryDelete(key))
            {
                var ex = new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyNotFound, key));

                Logger.Error(ex);

                throw ex;
            }
        }

        /// <summary>
        /// Deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key"></param>
        /// <returns><c>true</c> is able to successfully delete an entity with the given primary key; otherwise, <c>false</c>.</returns>
        public bool TryDelete(TKey key)
        {
            var entity = Find(key);

            if (entity == null)
                return false;

            Delete(entity);

            return true;
        }

        /// <summary>
        /// Finds an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <return>The entity found.</return>
        public TEntity Find(TKey key)
        {
            return Find(key, (IFetchQueryStrategy<TEntity>)null);
        }

        /// <summary>
        /// Finds an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <return>The entity found.</return>
        public TEntity Find(TKey key, IFetchQueryStrategy<TEntity> fetchStrategy)
        {
            return InterceptQueryResult<TEntity>(() => Context.Find<TEntity>(fetchStrategy, key));
        }

        /// <summary>
        /// Determines whether the repository contains an entity with the given primary key value.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public bool Exists(TKey key)
        {
            return Find(key) != null;
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity with the given primary key value.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public async Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            return await FindAsync(key, cancellationToken) != null;
        }

        /// <summary>
        /// Asynchronously finds an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        public Task<TEntity> FindAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            return FindAsync(key, (IFetchQueryStrategy<TEntity>)null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        public Task<TEntity> FindAsync(TKey key, IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            return InterceptQueryResultAsync<TEntity>(() => Context.AsAsync().FindAsync<TEntity>(cancellationToken, fetchStrategy, key));
        }

        /// <summary>
        /// Asynchronously deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            if (!await TryDeleteAsync(key, cancellationToken))
            {
                var ex = new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyNotFound, key));

                Logger.Error(ex);

                throw ex;
            }
        }

        /// <summary>
        ///  Asynchronously deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> is able to successfully delete an entity with the given primary key; otherwise, <c>false</c>.</returns>
        public async Task<bool> TryDeleteAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            InterceptError(cancellationToken.ThrowIfCancellationRequested);

            var entity = await FindAsync(key, cancellationToken);

            if (entity == null)
                return false;

            await DeleteAsync(entity, cancellationToken);

            return true;
        }

        #endregion
    }
}