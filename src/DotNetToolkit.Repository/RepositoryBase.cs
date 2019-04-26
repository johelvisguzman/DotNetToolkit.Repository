namespace DotNetToolkit.Repository
{
    using Configuration;
    using Configuration.Caching;
    using Configuration.Conventions;
    using Configuration.Interceptors;
    using Configuration.Logging;
    using Configuration.Mapper;
    using Configuration.Options;
    using Extensions;
    using Factories;
    using Internal;
    using Properties;
    using Queries;
    using Queries.Strategies;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="IRepository{TEntity, TKey1, TKey2, TKey3}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    /// <typeparam name="TKey3">The type of the third part of the composite primary key.</typeparam>
    public abstract class RepositoryBase<TEntity, TKey1, TKey2, TKey3> : InternalRepositoryBase<TEntity>, IRepository<TEntity, TKey1, TKey2, TKey3> where TEntity : class
    {
        #region Fields

        private IReadOnlyRepository<TEntity, TKey1, TKey2, TKey3> _wrapper;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity, TKey1, TKey2, TKey3}"/> class.
        /// </summary>
        /// <param name="options">The repository options.</param>
        protected RepositoryBase(IRepositoryOptions options) : base(options)
        {
            PrimaryKeyConventionHelper.ThrowsIfInvalidPrimaryKeyDefinition<TEntity>(typeof(TKey1), typeof(TKey2), typeof(TKey3));
        }

        #endregion

        #region Implementation of IRepository<TEntity, TKey1, TKey2, TKey3>

        /// <summary>
        /// Returns a read-only <see cref="IReadOnlyRepository{TEntity, TKey1, TKey2, TKey3}" /> wrapper for the current repository.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper around the current repository.</returns>
        public IReadOnlyRepository<TEntity, TKey1, TKey2, TKey3> AsReadOnly()
        {
            return _wrapper ?? (_wrapper = new ReadOnlyRepositoryWrapper<TEntity, TKey1, TKey2, TKey3>(this));
        }

        /// <summary>
        /// Deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        public void Delete(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            LogExecutingMethod(false);

            if (!TryDelete(key1, key2, key3))
            {
                var ex = new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyNotFound, key1 + ", " + key2 + ", " + key3));

                Logger.Error(ex);

                throw ex;
            }

            LogExecutedMethod(false);
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
            LogExecutingMethod(false);

            var entity = Find(key1, key2, key3);

            if (entity == null)
                return false;

            Delete(entity);

            LogExecutedMethod(false);

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
            LogExecutingMethod();

            IQueryResult<TEntity> Getter() =>
                UseContext<IQueryResult<TEntity>>(
                    context => context.Find<TEntity>(fetchStrategy, key1, key2, key3));

            var queryResult = CacheEnabled
                ? CacheProvider.GetOrSetFind<TEntity>(new object[] { key1, key2, key3 }, fetchStrategy, Getter, Logger)
                : Getter();

            CacheUsed = queryResult.CacheUsed;

            LogExecutedMethod();

            return queryResult.Result;
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
            LogExecutingMethod();

            var result = Find(key1, key2, key3) != null;

            LogExecutedMethod();

            return result;
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
            LogExecutingMethod();

            var result = await FindAsync(key1, key2, key3, cancellationToken) != null;

            LogExecutedMethod();

            return result;
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
        public async Task<TEntity> FindAsync(TKey1 key1, TKey2 key2, TKey3 key3, IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            Task<IQueryResult<TEntity>> Getter() =>
                UseContextAsync<IQueryResult<TEntity>>(
                    context => context.FindAsync<TEntity>(cancellationToken, fetchStrategy, key1, key2, key3));

            var queryResult = CacheEnabled
                ? await CacheProvider.GetOrSetFindAsync<TEntity>(new object[] { key1, key2, key3 }, fetchStrategy, Getter, Logger)
                : await Getter();

            CacheUsed = queryResult.CacheUsed;

            LogExecutedMethod();

            return queryResult.Result;
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
            LogExecutingMethod(false);

            if (!await TryDeleteAsync(key1, key2, key3, cancellationToken))
            {
                InterceptError(() => throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyNotFound, key1 + ", " + key2 + ", " + key3)));
            }

            LogExecutedMethod(false);
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
            LogExecutingMethod(false);

            InterceptError(cancellationToken.ThrowIfCancellationRequested);

            var entity = await FindAsync(key1, key2, key3, cancellationToken);

            if (entity == null)
                return false;

            await DeleteAsync(entity, cancellationToken);

            LogExecutedMethod(false);

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
    public abstract class RepositoryBase<TEntity, TKey1, TKey2> : InternalRepositoryBase<TEntity>, IRepository<TEntity, TKey1, TKey2> where TEntity : class
    {
        #region Fields

        private IReadOnlyRepository<TEntity, TKey1, TKey2> _wrapper;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity, TKey1, TKey2}"/> class.
        /// </summary>
        /// <param name="options">The repository options.</param>
        protected RepositoryBase(IRepositoryOptions options) : base(options)
        {
            PrimaryKeyConventionHelper.ThrowsIfInvalidPrimaryKeyDefinition<TEntity>(typeof(TKey1), typeof(TKey2));
        }

        #endregion

        #region Implementation of IRepository<TEntity, TKey1, TKey2>

        /// <summary>
        /// Returns a read-only <see cref="IReadOnlyRepository{TEntity, TKey1, TKey2}" /> wrapper for the current repository.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper around the current repository.</returns>
        public IReadOnlyRepository<TEntity, TKey1, TKey2> AsReadOnly()
        {
            return _wrapper ?? (_wrapper = new ReadOnlyRepositoryWrapper<TEntity, TKey1, TKey2>(this));
        }

        /// <summary>
        /// Deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        public void Delete(TKey1 key1, TKey2 key2)
        {
            LogExecutingMethod(false);

            if (!TryDelete(key1, key2))
            {
                InterceptError(() => throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyNotFound, key1 + ", " + key2)));
            }

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <returns><c>true</c> is able to successfully delete an entity with the given composite primary key values; otherwise, <c>false</c>.</returns>
        public bool TryDelete(TKey1 key1, TKey2 key2)
        {
            LogExecutingMethod(false);

            var entity = Find(key1, key2);

            if (entity == null)
                return false;

            Delete(entity);

            LogExecutedMethod(false);

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
            LogExecutingMethod();

            IQueryResult<TEntity> Getter() =>
                UseContext<IQueryResult<TEntity>>(
                    context => context.Find<TEntity>(fetchStrategy, key1, key2));

            var queryResult = CacheEnabled
                ? CacheProvider.GetOrSetFind<TEntity>(new object[] { key1, key2 }, fetchStrategy, Getter, Logger)
                : Getter();

            CacheUsed = queryResult.CacheUsed;

            LogExecutedMethod();

            return queryResult.Result;
        }

        /// <summary>
        /// Determines whether the repository contains an entity with the given composite primary key values.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public bool Exists(TKey1 key1, TKey2 key2)
        {
            LogExecutingMethod();

            var result = Find(key1, key2) != null;

            LogExecutedMethod();

            return result;
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
            LogExecutingMethod();

            var result = await FindAsync(key1, key2, cancellationToken) != null;

            LogExecutedMethod();

            return result;
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
        public async Task<TEntity> FindAsync(TKey1 key1, TKey2 key2, IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            Task<IQueryResult<TEntity>> Getter() =>
                UseContextAsync<IQueryResult<TEntity>>(
                    context => context.FindAsync<TEntity>(cancellationToken, fetchStrategy, key1, key2));

            var queryResult = CacheEnabled
                ? await CacheProvider.GetOrSetFindAsync<TEntity>(new object[] { key1, key2 }, fetchStrategy, Getter, Logger)
                : await Getter();

            CacheUsed = queryResult.CacheUsed;

            LogExecutedMethod();

            return queryResult.Result;
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
            LogExecutingMethod(false);

            if (!await TryDeleteAsync(key1, key2, cancellationToken))
            {
                InterceptError(() => throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyNotFound, key1 + ", " + key2)));
            }

            LogExecutedMethod(false);
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
            LogExecutingMethod(false);

            InterceptError(cancellationToken.ThrowIfCancellationRequested);

            var entity = await FindAsync(key1, key2, cancellationToken);

            if (entity == null)
                return false;

            await DeleteAsync(entity, cancellationToken);

            LogExecutedMethod(false);

            return true;
        }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IRepository{TEntity, TKey}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public abstract class RepositoryBase<TEntity, TKey> : InternalRepositoryBase<TEntity>, IRepository<TEntity, TKey> where TEntity : class
    {
        #region Fields

        private IReadOnlyRepository<TEntity, TKey> _wrapper;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="options">The repository options.</param>
        protected RepositoryBase(IRepositoryOptions options) : base(options)
        {
            PrimaryKeyConventionHelper.ThrowsIfInvalidPrimaryKeyDefinition<TEntity>(typeof(TKey));
        }

        #endregion

        #region Implementation of IRepository<TEntity, TKey>

        /// <summary>
        /// Returns a read-only <see cref="IReadOnlyRepository{TEntity, TKey}" /> wrapper for the current repository.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper around the current repository.</returns>
        public IReadOnlyRepository<TEntity, TKey> AsReadOnly()
        {
            return _wrapper ?? (_wrapper = new ReadOnlyRepositoryWrapper<TEntity, TKey>(this));
        }

        /// <summary>
        /// Deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        public void Delete(TKey key)
        {
            LogExecutingMethod(false);

            if (!TryDelete(key))
            {
                InterceptError(() => throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyNotFound, key)));
            }

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key"></param>
        /// <returns><c>true</c> is able to successfully delete an entity with the given primary key; otherwise, <c>false</c>.</returns>
        public bool TryDelete(TKey key)
        {
            LogExecutingMethod(false);

            var entity = Find(key);

            if (entity == null)
                return false;

            Delete(entity);

            LogExecutedMethod(false);

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
            LogExecutingMethod();

            IQueryResult<TEntity> Getter() =>
                UseContext<IQueryResult<TEntity>>(
                    context => context.Find<TEntity>(fetchStrategy, key));

            var queryResult = CacheEnabled
                ? CacheProvider.GetOrSetFind<TEntity>(new object[] { key }, fetchStrategy, Getter, Logger)
                : Getter();

            CacheUsed = queryResult.CacheUsed;

            LogExecutedMethod();

            return queryResult.Result;
        }

        /// <summary>
        /// Determines whether the repository contains an entity with the given primary key value.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public bool Exists(TKey key)
        {
            LogExecutingMethod();

            var result = Find(key) != null;

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity with the given primary key value.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public async Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            var result = await FindAsync(key, cancellationToken) != null;

            LogExecutedMethod();

            return result;
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
        public async Task<TEntity> FindAsync(TKey key, IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            Task<IQueryResult<TEntity>> Getter() =>
                UseContextAsync<IQueryResult<TEntity>>(
                    context => context.FindAsync<TEntity>(cancellationToken, fetchStrategy, key));

            var queryResult = CacheEnabled
                ? await CacheProvider.GetOrSetFindAsync<TEntity>(new object[] { key }, fetchStrategy, Getter, Logger)
                : await Getter();

            CacheUsed = queryResult.CacheUsed;

            LogExecutedMethod();

            return queryResult.Result;
        }

        /// <summary>
        /// Asynchronously deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod(false);

            if (!await TryDeleteAsync(key, cancellationToken))
            {
                InterceptError(() => throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyNotFound, key)));
            }

            LogExecutedMethod(false);
        }

        /// <summary>
        ///  Asynchronously deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> is able to successfully delete an entity with the given primary key; otherwise, <c>false</c>.</returns>
        public async Task<bool> TryDeleteAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod(false);

            InterceptError(cancellationToken.ThrowIfCancellationRequested);

            var entity = await FindAsync(key, cancellationToken);

            if (entity == null)
                return false;

            await DeleteAsync(entity, cancellationToken);

            LogExecutedMethod(false);

            return true;
        }

        #endregion
    }

    /// <summary>
    /// Represents an internal repository base class with common functionality to be used.
    /// </summary>
    [ComVisible(false)]
#if !NETSTANDARD1_3
    [Browsable(false)]
#endif
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class InternalRepositoryBase<TEntity> where TEntity : class
    {
        #region Fields

        private readonly IRepositoryOptions _options;
        private readonly IRepositoryContextFactory _contextFactory;
        private IEnumerable<IRepositoryInterceptor> _interceptors;
        private string _currentExecutingLoggingMethod;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the value indicating whether caching is enabled or not.
        /// </summary>
        public bool CacheEnabled { get; set; }

        /// <summary>
        /// Gets a value indicating whether the cached was used or not on the previous executed query.
        /// </summary>
        public bool CacheUsed { get; internal set; }

        /// <summary>
        /// Gets the repository logger.
        /// </summary>
        protected internal ILogger Logger { get; }

        /// <summary>
        /// Gets the repository logger provider.
        /// </summary>
        protected internal ILoggerProvider LoggerProvider { get; }

        /// <summary>
        /// Gets the caching provider.
        /// </summary>
        protected internal ICacheProvider CacheProvider { get; private set; }

        /// <summary>
        /// Gets the mapping provider.
        /// </summary>
        protected internal IMapperProvider MapperProvider { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalRepositoryBase{TEntity}"/> class.
        /// </summary>
        /// <param name="options">The repository options.</param>
        internal InternalRepositoryBase(IRepositoryOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var optionsBuilder = new RepositoryOptionsBuilder(options);

            OnConfiguring(optionsBuilder);

            // Sets the default logger provider (prints all messages levels)
            LoggerProvider = optionsBuilder.Options.LoggerProvider ?? new ConsoleLoggerProvider(LogLevel.Debug);

            Logger = LoggerProvider.Create($"DotNetToolkit.Repository<{typeof(TEntity).Name}>");

            var contextFactory = optionsBuilder.Options.ContextFactory;
            if (contextFactory == null)
                throw new InvalidOperationException("No context provider has been configured for this repository.");

            _contextFactory = contextFactory;

            var cachingProvider = options.CachingProvider ?? NullCacheProvider.Instance;

            if (cachingProvider.GetType() != typeof(NullCacheProvider))
                CacheEnabled = true;

            CacheProvider = cachingProvider;

            MapperProvider = optionsBuilder.Options.MapperProvider ?? Configuration.Mapper.MapperProvider.Instance;

            _options = optionsBuilder.Options;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void ClearCache()
        {
            if (CacheEnabled)
                CacheProvider.IncrementCounter<TEntity>();
        }

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database and returns a collection of entities.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, CommandType cmdType, object[] parameters, Func<IDataReader, TEntity> projector)
        {
            LogExecutingMethod();

            var parametersDict = ConvertToParametersDictionary(parameters);

            IQueryResult<IEnumerable<TEntity>> Getter() =>
                UseContext<IQueryResult<IEnumerable<TEntity>>>(
                    context => context.ExecuteSqlQuery(sql, cmdType, parametersDict, projector));

            var queryResult = CacheEnabled
                ? CacheProvider.GetOrSetExecuteSqlQuery<TEntity>(sql, cmdType, parametersDict, projector, Getter, Logger)
                : Getter();

            ClearCache(sql);

            CacheUsed = queryResult.CacheUsed;

            LogExecutedMethod();

            return queryResult.Result;
        }

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database and returns a collection of entities.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, object[] parameters, Func<IDataReader, TEntity> projector)
        {
            return ExecuteSqlQuery(sql, CommandType.Text, parameters, projector);
        }

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database and returns a collection of entities.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, Func<IDataReader, TEntity> projector)
        {
            return ExecuteSqlQuery(sql, (object[])null, projector);
        }

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database and returns a collection of entities.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <returns>A list which each entity has been projected into a new form using a default mapping provider.</returns>
        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, CommandType cmdType, object[] parameters)
        {
            var mapper = MapperProvider.Create<TEntity>();

            return ExecuteSqlQuery(sql, CommandType.Text, parameters, r => mapper.Map(r));
        }

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database and returns a collection of entities.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <returns>A list which each entity has been projected into a new form using a default mapping provider.</returns>
        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, object[] parameters)
        {
            return ExecuteSqlQuery(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database and returns a collection of entities.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <returns>A list which each entity has been projected into a new form using a default mapping provider.</returns>
        public IEnumerable<TEntity> ExecuteSqlQuery(string sql)
        {
            return ExecuteSqlQuery(sql, (object[])null);
        }

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <returns>The number of rows affected.</returns>
        public int ExecuteSqlCommand(string sql, CommandType cmdType, object[] parameters)
        {
            LogExecutingMethod();

            var parametersDict = ConvertToParametersDictionary(parameters);

            IQueryResult<int> Getter() =>
                UseContext<IQueryResult<int>>(
                    context => context.ExecuteSqlCommand(sql, cmdType, parametersDict));

            var queryResult = CacheEnabled
                ? CacheProvider.GetOrSetExecuteSqlCommand<TEntity>(sql, cmdType, parametersDict, Getter, Logger)
                : Getter();

            ClearCache(sql);

            CacheUsed = queryResult.CacheUsed;

            LogExecutedMethod();

            return queryResult.Result;
        }

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <returns>The number of rows affected.</returns>
        public int ExecuteSqlCommand(string sql, object[] parameters)
        {
            return ExecuteSqlCommand(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <returns>The number of rows affected.</returns>
        public int ExecuteSqlCommand(string sql)
        {
            return ExecuteSqlCommand(sql, (object[])null);
        }

        /// <summary>
        /// Asynchronously creates raw SQL query that is executed directly in the database and returns a collection of entities.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns> 
        public async Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, CommandType cmdType, object[] parameters, Func<IDataReader, TEntity> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            var parametersDict = ConvertToParametersDictionary(parameters);

            Task<IQueryResult<IEnumerable<TEntity>>> Getter() =>
                UseContextAsync<IQueryResult<IEnumerable<TEntity>>>(
                    context => context.ExecuteSqlQueryAsync(sql, cmdType, parametersDict, projector, cancellationToken));

            var queryResult = CacheEnabled
                ? await CacheProvider.GetOrSetExecuteSqlQueryAsync<TEntity>(sql, cmdType, parametersDict, projector, Getter, Logger)
                : await Getter();

            ClearCache(sql);

            CacheUsed = queryResult.CacheUsed;

            LogExecutedMethod();

            return queryResult.Result;
        }

        /// <summary>
        /// Asynchronously creates a raw SQL query that is executed directly in the database and returns a collection of entities.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns> 
        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, object[] parameters, Func<IDataReader, TEntity> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteSqlQueryAsync(sql, CommandType.Text, parameters, projector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates raw SQL query that is executed directly in the database and returns a collection of entities.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns> 
        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, Func<IDataReader, TEntity> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteSqlQueryAsync(sql, (object[])null, projector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates raw SQL query that is executed directly in the database and returns a collection of entities.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing A list which each entity has been projected into a new form using a default mapping provider.</returns> 
        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, CommandType cmdType, object[] parameters, CancellationToken cancellationToken = new CancellationToken())
        {
            var mapper = MapperProvider.Create<TEntity>();

            return ExecuteSqlQueryAsync(sql, CommandType.Text, parameters, r => mapper.Map(r), cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates a raw SQL query that is executed directly in the database and returns a collection of entities.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing A list which each entity has been projected into a new form using a default mapping provider.</returns> 
        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, object[] parameters, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteSqlQueryAsync(sql, CommandType.Text, parameters, cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates raw SQL query that is executed directly in the database and returns a collection of entities.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing A list which each entity has been projected into a new form using a default mapping provider.</returns> 
        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteSqlQueryAsync(sql, (object[])null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates raw SQL query that is executed directly in the database.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of rows affected.</returns>
        public async Task<int> ExecuteSqlCommandAsync(string sql, CommandType cmdType, object[] parameters, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            var parametersDict = ConvertToParametersDictionary(parameters);

            Task<IQueryResult<int>> Getter() =>
                UseContextAsync<IQueryResult<int>>(
                    context => context.ExecuteSqlCommandAsync(sql, cmdType, parametersDict, cancellationToken));

            var queryResult = CacheEnabled
                ? await CacheProvider.GetOrSetExecuteSqlCommandAsync<TEntity>(sql, cmdType, parametersDict, Getter, Logger)
                : await Getter();

            ClearCache(sql);

            CacheUsed = queryResult.CacheUsed;

            LogExecutedMethod();

            return queryResult.Result;
        }

        /// <summary>
        /// Asynchronously creates raw SQL query that is executed directly in the database.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of rows affected.</returns>
        public Task<int> ExecuteSqlCommandAsync(string sql, object[] parameters, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteSqlCommandAsync(sql, CommandType.Text, parameters, cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates raw SQL query that is executed directly in the database.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of rows affected.</returns>
        public Task<int> ExecuteSqlCommandAsync(string sql, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteSqlCommandAsync(sql, (object[])null, cancellationToken);
        }

        /// <summary>
        /// Adds the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public void Add(TEntity entity)
        {
            LogExecutingMethod(false);

            InterceptError(() =>
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));
            });

            Intercept(x => x.AddExecuting(entity));

            UseContext(context =>
            {
                context.Add(entity);
                context.SaveChanges();
            });

            Intercept(x => x.AddExecuted(entity));

            ClearCache();

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Adds the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        public void Add(IEnumerable<TEntity> entities)
        {
            LogExecutingMethod(false);

            InterceptError(() =>
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));
            });

            UseContext(context =>
            {
                foreach (var entity in entities)
                {
                    Intercept(x => x.AddExecuting(entity));

                    context.Add(entity);
                }

                context.SaveChanges();

                foreach (var entity in entities)
                {
                    Intercept(x => x.AddExecuted(entity));
                }
            });

            ClearCache();

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Updates the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public void Update(TEntity entity)
        {
            LogExecutingMethod(false);

            InterceptError(() =>
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));
            });

            Intercept(x => x.UpdateExecuting(entity));

            UseContext(context =>
            {
                context.Update(entity);
                context.SaveChanges();
            });

            Intercept(x => x.UpdateExecuted(entity));

            ClearCache();

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Updates the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        public void Update(IEnumerable<TEntity> entities)
        {
            LogExecutingMethod(false);

            InterceptError(() =>
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));
            });

            UseContext(context =>
            {
                foreach (var entity in entities)
                {
                    Intercept(x => x.UpdateExecuting(entity));

                    context.Update(entity);
                }

                context.SaveChanges();

                foreach (var entity in entities)
                {
                    Intercept(x => x.UpdateExecuted(entity));
                }
            });

            ClearCache();

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public void Delete(TEntity entity)
        {
            LogExecutingMethod(false);

            InterceptError(() =>
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));
            });

            Intercept(x => x.DeleteExecuting(entity));

            UseContext(context =>
            {
                context.Remove(entity);
                context.SaveChanges();
            });

            Intercept(x => x.DeleteExecuted(entity));

            ClearCache();

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Deletes all the entities in the repository that satisfies the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                if (predicate == null)
                    throw new ArgumentNullException(nameof(predicate));
            });

            Delete(predicate.ToQueryOptions<TEntity>());

            LogExecutedMethod();
        }

        /// <summary>
        /// Deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        public void Delete(IQueryOptions<TEntity> options)
        {
            LogExecutingMethod(false);

            InterceptError(() =>
            {
                if (options == null)
                    throw new ArgumentNullException(nameof(options));

                if (options.SpecificationStrategy == null)
                    throw new InvalidOperationException("The specified query options is missing a specification predicate.");
            });

            Delete(FindAll(options).Result);

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        public void Delete(IEnumerable<TEntity> entities)
        {
            LogExecutingMethod(false);

            InterceptError(() =>
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));
            });

            UseContext(context =>
            {
                foreach (var entity in entities)
                {
                    Intercept(x => x.DeleteExecuting(entity));

                    context.Remove(entity);
                }

                context.SaveChanges();

                foreach (var entity in entities)
                {
                    Intercept(x => x.DeleteExecuted(entity));
                }
            });

            ClearCache();

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Finds the first entity in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The entity that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public TEntity Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Find<TEntity>(predicate, IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Finds the first entity in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The entity that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public TEntity Find(IQueryOptions<TEntity> options)
        {
            return Find<TEntity>(options, IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public TResult Find<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                if (predicate == null)
                    throw new ArgumentNullException(nameof(predicate));
            });

            var result = Find<TResult>(predicate.ToQueryOptions<TEntity>(), selector);

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public TResult Find<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            LogExecutingMethod();

            IQueryResult<TResult> Getter() =>
                UseContext<IQueryResult<TResult>>(
                    context => context.Find<TEntity, TResult>(options, selector));

            var queryResult = CacheEnabled
                ? CacheProvider.GetOrSetFind<TEntity, TResult>(options, selector, Getter, Logger)
                : Getter();

            CacheUsed = queryResult.CacheUsed;

            LogExecutedMethod();

            return queryResult.Result;
        }

        /// <summary>
        /// Finds the collection of entities in the repository.
        /// </summary>
        /// <returns>The collection of entities in the repository.</returns>
        public IEnumerable<TEntity> FindAll()
        {
            return FindAll<TEntity>(IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate)
        {
            return FindAll<TEntity>(predicate, IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public IPagedQueryResult<IEnumerable<TEntity>> FindAll(IQueryOptions<TEntity> options)
        {
            return FindAll<TEntity>(options, IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository.
        /// </summary>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository.</returns>
        public IEnumerable<TResult> FindAll<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return FindAll<TResult>((IQueryOptions<TEntity>)null, selector).Result;
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public IEnumerable<TResult> FindAll<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                if (predicate == null)
                    throw new ArgumentNullException(nameof(predicate));
            });

            var result = FindAll<TResult>(predicate.ToQueryOptions<TEntity>(), selector).Result;

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public IPagedQueryResult<IEnumerable<TResult>> FindAll<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            LogExecutingMethod();

            IPagedQueryResult<IEnumerable<TResult>> Getter() =>
                UseContext<IPagedQueryResult<IEnumerable<TResult>>>(
                    context => context.FindAll<TEntity, TResult>(options, selector));

            var queryResult = CacheEnabled
                ? CacheProvider.GetOrSetFindAll<TEntity, TResult>(options, selector, Getter, Logger)
                : Getter();

            CacheUsed = queryResult.CacheUsed;

            LogExecutedMethod();

            return queryResult;
        }

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">The predicate used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified predicate; otherwise, <c>false</c>.</returns>
        public bool Exists(Expression<Func<TEntity, bool>> predicate)
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                if (predicate == null)
                    throw new ArgumentNullException(nameof(predicate));
            });

            var result = Exists(predicate.ToQueryOptions<TEntity>());

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        public bool Exists(IQueryOptions<TEntity> options)
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                if (options == null)
                    throw new ArgumentNullException(nameof(options));

                if (options.SpecificationStrategy == null)
                    throw new InvalidOperationException("The specified query options is missing a specification predicate.");
            });

            var result = Find(options) != null;

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Returns the number of entities contained in the repository.
        /// </summary>
        /// <returns>The number of entities contained in the repository.</returns>
        public int Count()
        {
            return Count((IQueryOptions<TEntity>)null);
        }

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                if (predicate == null)
                    throw new ArgumentNullException(nameof(predicate));
            });

            var result = Count(predicate.ToQueryOptions<TEntity>());

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public int Count(IQueryOptions<TEntity> options)
        {
            LogExecutingMethod();

            IQueryResult<int> Getter() =>
                UseContext<IQueryResult<int>>(
                    context => context.Count<TEntity>(options));

            var queryResult = CacheEnabled
                ? CacheProvider.GetOrSetCount<TEntity>(options, Getter, Logger)
                : Getter();

            CacheUsed = queryResult.CacheUsed;

            LogExecutedMethod();

            return queryResult.Result;
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Dictionary<TDictionaryKey, TEntity> ToDictionary<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            return ToDictionary<TDictionaryKey>((IQueryOptions<TEntity>)null, keySelector).Result;
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public IPagedQueryResult<Dictionary<TDictionaryKey, TEntity>> ToDictionary<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            return ToDictionary<TDictionaryKey, TEntity>(options, keySelector, IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Dictionary<TDictionaryKey, TElement> ToDictionary<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            return ToDictionary<TDictionaryKey, TElement>((IQueryOptions<TEntity>)null, keySelector, elementSelector).Result;
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, and an element selector function with entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public IPagedQueryResult<Dictionary<TDictionaryKey, TElement>> ToDictionary<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            LogExecutingMethod();

            IPagedQueryResult<Dictionary<TDictionaryKey, TElement>> Getter() =>
                UseContext<IPagedQueryResult<Dictionary<TDictionaryKey, TElement>>>(
                    context => context.ToDictionary(options, keySelector, elementSelector));

            var queryResult = CacheEnabled
                ? CacheProvider.GetOrSetDictionary<TEntity, TDictionaryKey, TElement>(options, keySelector, elementSelector, Getter, Logger)
                : Getter();

            CacheUsed = queryResult.CacheUsed;

            LogExecutedMethod();

            return queryResult;
        }

        /// <summary>
        /// Returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A transform function to produce a result value from each element.</param>
        /// <returns>A new <see cref="IEnumerable{TResult}" /> that contains the grouped result.</returns>
        public IEnumerable<TResult> GroupBy<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            return GroupBy<TGroupKey, TResult>((IQueryOptions<TEntity>)null, keySelector, resultSelector).Result;
        }

        /// <summary>
        /// Returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A transform function to produce a result value from each element.</param>
        /// <returns>A new <see cref="IEnumerable{TResult}" /> that contains the grouped result that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public IPagedQueryResult<IEnumerable<TResult>> GroupBy<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            LogExecutingMethod();

            IPagedQueryResult<IEnumerable<TResult>> Getter() =>
                UseContext<IPagedQueryResult<IEnumerable<TResult>>>(
                    context => context.GroupBy(options, keySelector, resultSelector));

            var queryResult = CacheEnabled
                ? CacheProvider.GetOrSetGroup<TEntity, TGroupKey, TResult>(options, keySelector, resultSelector, Getter, Logger)
                : Getter();

            CacheUsed = queryResult.CacheUsed;

            LogExecutedMethod();

            return queryResult;
        }

        /// <summary>
        /// Asynchronously adds the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod(false);

            InterceptError(() =>
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                cancellationToken.ThrowIfCancellationRequested();
            });

            Intercept(x => x.AddExecuting(entity));

            await UseContextAsync(async context =>
            {
                context.Add(entity);
                await context.SaveChangesAsync(cancellationToken);
            });

            Intercept(x => x.AddExecuted(entity));

            ClearCache();

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Asynchronously adds the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod(false);

            InterceptError(() =>
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                cancellationToken.ThrowIfCancellationRequested();
            });

            await UseContextAsync(async context =>
            {
                foreach (var entity in entities)
                {
                    Intercept(x => x.AddExecuting(entity));

                    context.Add(entity);
                }

                await context.SaveChangesAsync(cancellationToken);

                foreach (var entity in entities)
                {
                    Intercept(x => x.AddExecuted(entity));
                }
            });

            ClearCache();

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Asynchronously updates the specified <paramref name="entity" /> in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod(false);

            InterceptError(() =>
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                cancellationToken.ThrowIfCancellationRequested();
            });

            Intercept(x => x.UpdateExecuting(entity));

            await UseContextAsync(async context =>
            {
                context.Update(entity);
                await context.SaveChangesAsync(cancellationToken);
            });

            Intercept(x => x.UpdateExecuted(entity));

            ClearCache();

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Asynchronously updates the specified <paramref name="entities" /> collection in the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod(false);

            InterceptError(() =>
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                cancellationToken.ThrowIfCancellationRequested();
            });

            await UseContextAsync(async context =>
            {
                foreach (var entity in entities)
                {
                    Intercept(x => x.UpdateExecuting(entity));

                    context.Update(entity);
                }

                await context.SaveChangesAsync(cancellationToken);

                foreach (var entity in entities)
                {
                    Intercept(x => x.UpdateExecuted(entity));
                }
            });

            ClearCache();

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Asynchronously deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod(false);

            InterceptError(() =>
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                cancellationToken.ThrowIfCancellationRequested();
            });

            Intercept(x => x.DeleteExecuting(entity));

            await UseContextAsync(async context =>
            {
                context.Remove(entity);
                await context.SaveChangesAsync(cancellationToken);
            });

            Intercept(x => x.DeleteExecuted(entity));

            ClearCache();

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Asynchronously deletes all the entities in the repository that satisfies the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod(false);

            InterceptError(() =>
            {
                if (predicate == null)
                    throw new ArgumentNullException(nameof(predicate));
            });

            await DeleteAsync(predicate.ToQueryOptions<TEntity>(), cancellationToken);

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Asynchronously deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod(false);

            InterceptError(() =>
            {
                if (options == null)
                    throw new ArgumentNullException(nameof(options));

                if (options.SpecificationStrategy == null)
                    throw new InvalidOperationException("The specified query options is missing a specification predicate.");
            });

            var entitiesInDb = (await FindAllAsync(options, cancellationToken)).Result;

            await DeleteAsync(entitiesInDb, cancellationToken);

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Asynchronously deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod(false);

            InterceptError(() =>
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                cancellationToken.ThrowIfCancellationRequested();
            });

            await UseContextAsync(async context =>
            {
                foreach (var entity in entities)
                {
                    Intercept(x => x.DeleteExecuting(entity));

                    context.Remove(entity);
                }

                await context.SaveChangesAsync(cancellationToken);

                foreach (var entity in entities)
                {
                    Intercept(x => x.DeleteExecuted(entity));
                }
            });

            ClearCache();

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Asynchronously finds the first entity in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return FindAsync<TEntity>(predicate, IdentityExpression<TEntity>.Instance, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds the first entity in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public Task<TEntity> FindAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return FindAsync<TEntity>(options, IdentityExpression<TEntity>.Instance, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public Task<TResult> FindAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                if (predicate == null)
                    throw new ArgumentNullException(nameof(predicate));
            });

            var result = FindAsync<TResult>(predicate.ToQueryOptions<TEntity>(), selector, cancellationToken);

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Asynchronously finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public async Task<TResult> FindAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            Task<IQueryResult<TResult>> Getter() =>
                UseContextAsync<IQueryResult<TResult>>(
                    context => context.FindAsync<TEntity, TResult>(options, selector, cancellationToken));

            var queryResult = CacheEnabled
                ? await CacheProvider.GetOrSetFindAsync<TEntity, TResult>(options, selector, Getter, Logger)
                : await Getter();

            CacheUsed = queryResult.CacheUsed;

            LogExecutedMethod();

            return queryResult.Result;
        }

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the collection of entities in the repository.</returns>
        public Task<IEnumerable<TEntity>> FindAllAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return FindAllAsync<TEntity>(IdentityExpression<TEntity>.Instance, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return FindAllAsync<TEntity>(predicate, IdentityExpression<TEntity>.Instance, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public Task<IPagedQueryResult<IEnumerable<TEntity>>> FindAllAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return FindAllAsync<TEntity>(options, IdentityExpression<TEntity>.Instance, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository.
        /// </summary>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the collection of projected entity results in the repository.</returns>
        public async Task<IEnumerable<TResult>> FindAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return (await FindAllAsync<TResult>((IQueryOptions<TEntity>)null, selector, cancellationToken)).Result;
        }

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public async Task<IEnumerable<TResult>> FindAllAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                if (predicate == null)
                    throw new ArgumentNullException(nameof(predicate));
            });

            var result = (await FindAllAsync<TResult>(predicate.ToQueryOptions<TEntity>(), selector, cancellationToken)).Result;

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public async Task<IPagedQueryResult<IEnumerable<TResult>>> FindAllAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            Task<IPagedQueryResult<IEnumerable<TResult>>> Getter() =>
                UseContextAsync<IPagedQueryResult<IEnumerable<TResult>>>(
                    context => context.FindAllAsync<TEntity, TResult>(options, selector, cancellationToken));

            var queryResult = CacheEnabled
                ? await CacheProvider.GetOrSetFindAllAsync<TEntity, TResult>(options, selector, Getter, Logger)
                : await Getter();

            CacheUsed = queryResult.CacheUsed;

            LogExecutedMethod();

            return queryResult;
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">The predicate used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the conditions defined by the specified predicate; otherwise, <c>false</c>.</returns>
        public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                if (predicate == null)
                    throw new ArgumentNullException(nameof(predicate));
            });

            var result = ExistsAsync(predicate.ToQueryOptions<TEntity>(), cancellationToken);

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        public async Task<bool> ExistsAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                if (options == null)
                    throw new ArgumentNullException(nameof(options));

                if (options.SpecificationStrategy == null)
                    throw new InvalidOperationException("The specified query options is missing a specification predicate.");
            });

            var result = await FindAsync(options, cancellationToken) != null;

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Asynchronously returns the number of entities contained in the repository.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The number of entities contained in the repository.</returns>
        public Task<int> CountAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return CountAsync((IQueryOptions<TEntity>)null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously returns the number of entities that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of entities that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                if (predicate == null)
                    throw new ArgumentNullException(nameof(predicate));
            });

            var result = CountAsync(predicate.ToQueryOptions<TEntity>(), cancellationToken);

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Asynchronously returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public async Task<int> CountAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            Task<IQueryResult<int>> Getter() =>
                UseContextAsync<IQueryResult<int>>(
                    context => context.CountAsync<TEntity>(options, cancellationToken));

            var queryResult = CacheEnabled
                ? await CacheProvider.GetOrSetCountAsync<TEntity>(options, Getter, Logger)
                : await Getter();

            CacheUsed = queryResult.CacheUsed;

            LogExecutedMethod();

            return queryResult.Result;
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public async Task<Dictionary<TDictionaryKey, TEntity>> ToDictionaryAsync<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return (await ToDictionaryAsync<TDictionaryKey>((IQueryOptions<TEntity>)null, keySelector, cancellationToken)).Result;
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public Task<IPagedQueryResult<Dictionary<TDictionaryKey, TEntity>>> ToDictionaryAsync<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ToDictionaryAsync<TDictionaryKey, TEntity>(options, keySelector, IdentityExpression<TEntity>.Instance, cancellationToken);
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public async Task<Dictionary<TDictionaryKey, TElement>> ToDictionaryAsync<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return (await ToDictionaryAsync<TDictionaryKey, TElement>((IQueryOptions<TEntity>)null, keySelector, elementSelector, cancellationToken)).Result;
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, and an element selector function with entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public async Task<IPagedQueryResult<Dictionary<TDictionaryKey, TElement>>> ToDictionaryAsync<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            Task<IPagedQueryResult<Dictionary<TDictionaryKey, TElement>>> Getter() =>
                UseContextAsync<IPagedQueryResult<Dictionary<TDictionaryKey, TElement>>>(
                    context => context.ToDictionaryAsync(options, keySelector, elementSelector, cancellationToken));

            var queryResult = CacheEnabled
                ? await CacheProvider.GetOrSetDictionaryAsync<TEntity, TDictionaryKey, TElement>(options, keySelector, elementSelector, Getter, Logger)
                : await Getter();

            CacheUsed = queryResult.CacheUsed;

            LogExecutedMethod();

            return queryResult;

        }

        /// <summary>
        /// Asynchronously returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A transform function to produce a result value from each element.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="IEnumerable{TResult}" /> that contains the grouped result.</returns>
        public async Task<IEnumerable<TResult>> GroupByAsync<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return (await GroupByAsync<TGroupKey, TResult>((IQueryOptions<TEntity>)null, keySelector, resultSelector, cancellationToken)).Result;
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A transform function to produce a result value from each element.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="IEnumerable{TResult}" /> that contains the grouped result that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public async Task<IPagedQueryResult<IEnumerable<TResult>>> GroupByAsync<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            Task<IPagedQueryResult<IEnumerable<TResult>>> Getter() =>
                UseContextAsync<IPagedQueryResult<IEnumerable<TResult>>>(
                    context => context.GroupByAsync(options, keySelector, resultSelector, cancellationToken));

            var queryResult = CacheEnabled
                ? await CacheProvider.GetOrSetGroupAsync<TEntity, TGroupKey, TResult>(options, keySelector, resultSelector, Getter, Logger)
                : await Getter();

            CacheUsed = queryResult.CacheUsed;

            LogExecutedMethod();

            return queryResult;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Override this method to configure the repository.
        /// </summary>
        /// <param name="optionsBuilder">A builder used to create or modify options for this repository.</param>
        protected virtual void OnConfiguring(RepositoryOptionsBuilder optionsBuilder) { }

        /// <summary>
        /// A method for using a disposable repository context.
        /// </summary>
        protected void UseContext(Action<IRepositoryContext> action)
        {
            var context = GetContext();

            try
            {
                InterceptError(() => action(context));
            }
            finally
            {
                DisposeContext(context);
            }
        }

        /// <summary>
        /// A method for using a disposable repository context.
        /// </summary>
        protected T UseContext<T>(Func<IRepositoryContext, T> action)
        {
            var context = GetContext();

            try
            {
                return InterceptError<T>(() => action(context));
            }
            finally
            {
                DisposeContext(context);
            }
        }

        /// <summary>
        /// A method for using a disposable repository context asynchronously.
        /// </summary>
        protected async Task UseContextAsync(Func<IRepositoryContextAsync, Task> action)
        {
            var context = GetContext().AsAsync();

            try
            {
                await InterceptErrorAsync(() => action(context));
            }
            finally
            {
                DisposeContext(context);
            }
        }

        /// <summary>
        /// A method for using a disposable repository context asynchronously.
        /// </summary>
        protected async Task<T> UseContextAsync<T>(Func<IRepositoryContextAsync, Task<T>> action)
        {
            var context = GetContext().AsAsync();

            try
            {
                return await InterceptErrorAsync<T>(() => action(context));
            }
            finally
            {
                DisposeContext(context);
            }
        }

        /// <summary>
        /// Intercepts an error while executing the specified <paramref name="action"/>.
        /// </summary>
        /// <param name="action">The action to intercept.</param>
        protected void InterceptError(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                throw;
            }
        }

        /// <summary>
        /// Intercepts an error while executing the specified <paramref name="action"/>.
        /// </summary>
        /// <typeparam name="T">The type of the result returned by the specified <paramref name="action"/>.</typeparam>
        /// <param name="action">The action to intercept.</param>
        /// <returns>The result returned by the specified <paramref name="action"/>.</returns>
        protected T InterceptError<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                throw;
            }
        }

        /// <summary>
        /// Asynchronously intercepts an error while executing the specified <paramref name="action"/>.
        /// </summary>
        /// <param name="action">The action to intercept.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the result returned by the specified <paramref name="action"/>.</returns>
        protected async Task<T> InterceptErrorAsync<T>(Func<Task<T>> action)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                throw;
            }
        }

        /// <summary>
        /// Asynchronously intercepts an error while executing the specified <paramref name="action"/>.
        /// </summary>
        /// <param name="action">The action to intercept.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        protected async Task InterceptErrorAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                throw;
            }
        }

        /// <summary>
        /// Intercepts the specified action.
        /// </summary>
        /// <param name="action">The action to intercept.</param>
        protected void Intercept(Action<IRepositoryInterceptor> action)
        {
            foreach (var interceptor in GetInterceptors())
            {
                action(interceptor);
            }
        }

        #endregion

        #region Internal Methods

        internal void LogExecutingMethod(bool appendCachingDetails = true, [CallerMemberName] string method = null)
        {
            if (!string.IsNullOrEmpty(_currentExecutingLoggingMethod))
                return;

            _currentExecutingLoggingMethod = method;

            Logger.Debug(appendCachingDetails
                ? $"Executing [ Method = {method}, CacheEnabled = {CacheEnabled} ]"
                : $"Executing [ Method = {method} ]");
        }

        internal void LogExecutedMethod(bool appendCachingDetails = true, [CallerMemberName] string method = null)
        {
            if (string.IsNullOrEmpty(_currentExecutingLoggingMethod) || !_currentExecutingLoggingMethod.Equals(method))
                return;

            _currentExecutingLoggingMethod = null;

            Logger.Debug(appendCachingDetails
                ? $"Executed [ Method = {method}, CacheUsed = {CacheUsed} ]"
                : $"Executed [ Method = {method} ]");
        }

        #endregion

        #region Private Methods

        private IEnumerable<IRepositoryInterceptor> GetInterceptors()
        {
            if (_interceptors == null)
            {
                _interceptors = _options.Interceptors.Any()
                    ? _options.Interceptors.Values.Select(lazyInterceptor => lazyInterceptor.Value)
                    : Enumerable.Empty<IRepositoryInterceptor>();
            }

            return _interceptors;
        }

        private IRepositoryContext GetContext()
        {
            var context = _contextFactory.Create();

            if (context.Logger == null || context.Logger is NullLogger)
                context.Logger = LoggerProvider.Create(context.GetType().FullName);

            return context;
        }

        private static void DisposeContext(IRepositoryContext context)
        {
            if (context != null && context.CurrentTransaction == null)
                context.Dispose();
        }

        private void ClearCache(string sql)
        {
            if (sql == null)
                throw new ArgumentNullException(nameof(sql));

            var s = sql.ToUpperInvariant();

            var canClearCache = s.Contains("UPDATE") || s.Contains("DELETE FROM") || s.Contains("INSERT INTO");

            if (canClearCache)
                ClearCache();
        }

        private static Dictionary<string, object> ConvertToParametersDictionary(object[] parameters)
        {
            var parametersDict = new Dictionary<string, object>();

            if (parameters != null && parameters.Any())
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    parametersDict.Add($"@p{i}", parameters[i]);
                }
            }

            return parametersDict;
        }

        #endregion
    }
}
