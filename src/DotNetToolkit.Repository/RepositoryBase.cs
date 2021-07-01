namespace DotNetToolkit.Repository
{
    using Configuration;
    using Configuration.Caching;
    using Configuration.Caching.Internal;
    using Configuration.Conventions;
    using Configuration.Interceptors;
    using Configuration.Logging;
    using Configuration.Options;
    using Extensions;
    using Extensions.Internal;
    using Internal;
    using JetBrains.Annotations;
    using Properties;
    using Query;
    using Query.Strategies;
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
        protected RepositoryBase([NotNull] IRepositoryOptions options) : base(options) { }

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
        public void Delete([NotNull] TKey1 key1, [NotNull] TKey2 key2, [NotNull] TKey3 key3)
        {
            LogExecutingMethod(false);

            if (!TryDelete(key1, key2, key3))
            {
                InterceptError(() => new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyNotFound, key1 + ", " + key2 + ", " + key3)));
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
        public bool TryDelete([NotNull] TKey1 key1, [NotNull] TKey2 key2, [NotNull] TKey3 key3)
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
        public TEntity Find([NotNull] TKey1 key1, [NotNull] TKey2 key2, [NotNull] TKey3 key3)
        {
            return Find(key1, key2, key3, (IFetchQueryStrategy<TEntity>)null);
        }

        /// <summary>
        /// Finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="paths">The dot-separated list of related objects to return in the query results.</param>
        /// <return>The entity found.</return>
        public TEntity Find([NotNull] TKey1 key1, [NotNull] TKey2 key2, [NotNull] TKey3 key3, [NotNull] params string[] paths)
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotNull(key1, nameof(key1));
                Guard.NotNull(key2, nameof(key2));
                Guard.NotNull(key3, nameof(key3));
                Guard.NotNull(paths, nameof(paths));
            });

            var result = Find(key1, key2, key3, paths.ToFetchQueryStrategy<TEntity>());

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="paths">A collection of lambda expressions representing the paths to include.</param>
        /// <return>The entity found.</return>
        public TEntity Find([NotNull] TKey1 key1, [NotNull] TKey2 key2, [NotNull] TKey3 key3, [NotNull] params Expression<Func<TEntity, object>>[] paths)
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotNull(key1, nameof(key1));
                Guard.NotNull(key2, nameof(key2));
                Guard.NotNull(key3, nameof(key3));
                Guard.NotNull(paths, nameof(paths));
            });

            var result = Find(key1, key2, key3, paths.ToFetchQueryStrategy<TEntity>());

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity.</param>
        /// <return>The entity found.</return>
        public TEntity Find([NotNull] TKey1 key1, [NotNull] TKey2 key2, [NotNull] TKey3 key3, [CanBeNull] IFetchQueryStrategy<TEntity> fetchStrategy)
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotNull(key1, nameof(key1));
                Guard.NotNull(key2, nameof(key2));
                Guard.NotNull(key3, nameof(key3));
            });

            TEntity Getter() =>
                UseContext<TEntity>(
                    context => context.Find<TEntity>(fetchStrategy, key1, key2, key3));

            TEntity result;

            if (CacheEnabled)
            {
                var cacheResult = InterceptError<ICacheQueryResult<TEntity>>(
                    () => CacheProvider.GetOrSetFind<TEntity>(new object[] { key1, key2, key3 }, fetchStrategy, Getter, Logger));

                result = cacheResult.Result;
                CacheUsed = cacheResult.CacheUsed;
            }
            else
            {
                result = Getter();
                CacheUsed = false;
            }

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Determines whether the repository contains an entity with the given composite primary key values.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public bool Exists([NotNull] TKey1 key1, [NotNull] TKey2 key2, [NotNull] TKey3 key3)
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
        public async Task<bool> ExistsAsync([NotNull] TKey1 key1, [NotNull] TKey2 key2, [NotNull] TKey3 key3, CancellationToken cancellationToken = new CancellationToken())
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
        public Task<TEntity> FindAsync([NotNull] TKey1 key1, [NotNull] TKey2 key2, [NotNull] TKey3 key3, CancellationToken cancellationToken = new CancellationToken())
        {
            return FindAsync(key1, key2, key3, (IFetchQueryStrategy<TEntity>)null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="paths">The dot-separated list of related objects to return in the query results.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        public Task<TEntity> FindAsync([NotNull] TKey1 key1, [NotNull] TKey2 key2, [NotNull] TKey3 key3, [NotNull] params string[] paths)
        {
            return FindAsync(key1, key2, key3, paths, default(CancellationToken));
        }

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="paths">The dot-separated list of related objects to return in the query results.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        public async Task<TEntity> FindAsync([NotNull] TKey1 key1, [NotNull] TKey2 key2, [NotNull] TKey3 key3, [NotNull] string[] paths, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotNull(key1, nameof(key1));
                Guard.NotNull(key2, nameof(key2));
                Guard.NotNull(key3, nameof(key3));
                Guard.NotNull(paths, nameof(paths));
            });

            var result = await FindAsync(key1, key2, key3, paths.ToFetchQueryStrategy<TEntity>(), cancellationToken);

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="paths">A collection of lambda expressions representing the paths to include.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        public Task<TEntity> FindAsync([NotNull] TKey1 key1, [NotNull] TKey2 key2, [NotNull] TKey3 key3, [NotNull] params Expression<Func<TEntity, object>>[] paths)
        {
            return FindAsync(key1, key2, key3, paths, default(CancellationToken));
        }

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="paths">A collection of lambda expressions representing the paths to include.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        public async Task<TEntity> FindAsync([NotNull] TKey1 key1, [NotNull] TKey2 key2, [NotNull] TKey3 key3, [NotNull] Expression<Func<TEntity, object>>[] paths, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotNull(key1, nameof(key1));
                Guard.NotNull(key2, nameof(key2));
                Guard.NotNull(key3, nameof(key3));
                Guard.NotNull(paths, nameof(paths));
            });

            var result = await FindAsync(key1, key2, key3, paths.ToFetchQueryStrategy<TEntity>(), cancellationToken);

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        public async Task<TEntity> FindAsync([NotNull] TKey1 key1, [NotNull] TKey2 key2, [NotNull] TKey3 key3, [CanBeNull] IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotNull(key1, nameof(key1));
                Guard.NotNull(key2, nameof(key2));
                Guard.NotNull(key3, nameof(key3));
            });

            Task<TEntity> Getter() =>
                UseContextAsync<TEntity>(
                    context => context.FindAsync<TEntity>(cancellationToken, fetchStrategy, key1, key2, key3));

            TEntity result;

            if (CacheEnabled)
            {
                var cacheResult = await InterceptErrorAsync<ICacheQueryResult<TEntity>>(
                    () => CacheProvider.GetOrSetFindAsync<TEntity>(new object[] { key1, key2, key3 }, fetchStrategy, Getter, Logger));

                result = cacheResult.Result;
                CacheUsed = cacheResult.CacheUsed;
            }
            else
            {
                result = await Getter();
                CacheUsed = false;
            }

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Asynchronously deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task DeleteAsync([NotNull] TKey1 key1, [NotNull] TKey2 key2, [NotNull] TKey3 key3, CancellationToken cancellationToken = new CancellationToken())
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
        public async Task<bool> TryDeleteAsync([NotNull] TKey1 key1, [NotNull] TKey2 key2, [NotNull] TKey3 key3, CancellationToken cancellationToken = new CancellationToken())
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
        protected RepositoryBase([NotNull] IRepositoryOptions options) : base(options) { }

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
        public void Delete([NotNull] TKey1 key1, [NotNull] TKey2 key2)
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
        public bool TryDelete([NotNull] TKey1 key1, [NotNull] TKey2 key2)
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
        public TEntity Find([NotNull] TKey1 key1, [NotNull] TKey2 key2)
        {
            return Find(key1, key2, (IFetchQueryStrategy<TEntity>)null);
        }

        /// <summary>
        /// Finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="paths">The dot-separated list of related objects to return in the query results.</param>
        /// <return>The entity found.</return>
        public TEntity Find([NotNull] TKey1 key1, [NotNull] TKey2 key2, [NotNull] params string[] paths)
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotNull(key1, nameof(key1));
                Guard.NotNull(key2, nameof(key2));
                Guard.NotNull(paths, nameof(paths));
            });

            var result = Find(key1, key2, paths.ToFetchQueryStrategy<TEntity>());

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="paths">A collection of lambda expressions representing the paths to include.</param>
        /// <return>The entity found.</return>
        public TEntity Find([NotNull] TKey1 key1, [NotNull] TKey2 key2, [NotNull] params Expression<Func<TEntity, object>>[] paths)
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotNull(key1, nameof(key1));
                Guard.NotNull(key2, nameof(key2));
                Guard.NotNull(paths, nameof(paths));
            });

            var result = Find(key1, key2, paths.ToFetchQueryStrategy<TEntity>());

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity.</param>
        /// <return>The entity found.</return>
        public TEntity Find([NotNull] TKey1 key1, [NotNull] TKey2 key2, [CanBeNull] IFetchQueryStrategy<TEntity> fetchStrategy)
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotNull(key1, nameof(key1));
                Guard.NotNull(key2, nameof(key2));
            });

            TEntity Getter() =>
                UseContext<TEntity>(
                    context => context.Find<TEntity>(fetchStrategy, key1, key2));

            TEntity result;

            if (CacheEnabled)
            {
                var cacheResult = InterceptError<ICacheQueryResult<TEntity>>(
                    () => CacheProvider.GetOrSetFind<TEntity>(new object[] { key1, key2 }, fetchStrategy, Getter, Logger));

                result = cacheResult.Result;
                CacheUsed = cacheResult.CacheUsed;
            }
            else
            {
                result = Getter();
                CacheUsed = false;
            }

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Determines whether the repository contains an entity with the given composite primary key values.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public bool Exists([NotNull] TKey1 key1, [NotNull] TKey2 key2)
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
        public async Task<bool> ExistsAsync([NotNull] TKey1 key1, [NotNull] TKey2 key2, CancellationToken cancellationToken = new CancellationToken())
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
        public Task<TEntity> FindAsync([NotNull] TKey1 key1, [NotNull] TKey2 key2, CancellationToken cancellationToken = new CancellationToken())
        {
            return FindAsync(key1, key2, (IFetchQueryStrategy<TEntity>)null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="paths">The dot-separated list of related objects to return in the query results.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        public Task<TEntity> FindAsync([NotNull] TKey1 key1, [NotNull] TKey2 key2, [NotNull] params string[] paths)
        {
            return FindAsync(key1, key2, paths, default(CancellationToken));
        }

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="paths">The dot-separated list of related objects to return in the query results.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        public async Task<TEntity> FindAsync([NotNull] TKey1 key1, [NotNull] TKey2 key2, [NotNull] string[] paths, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotNull(key1, nameof(key1));
                Guard.NotNull(key2, nameof(key2));
                Guard.NotNull(paths, nameof(paths));
            });

            var result = await FindAsync(key1, key2, paths.ToFetchQueryStrategy<TEntity>(), cancellationToken);

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="paths">A collection of lambda expressions representing the paths to include.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        public Task<TEntity> FindAsync([NotNull] TKey1 key1, [NotNull] TKey2 key2, [NotNull] params Expression<Func<TEntity, object>>[] paths)
        {
            return FindAsync(key1, key2, paths, default(CancellationToken));
        }

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="paths">A collection of lambda expressions representing the paths to include.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        public async Task<TEntity> FindAsync([NotNull] TKey1 key1, [NotNull] TKey2 key2, [NotNull] Expression<Func<TEntity, object>>[] paths, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotNull(key1, nameof(key1));
                Guard.NotNull(key2, nameof(key2));
                Guard.NotNull(paths, nameof(paths));
            });

            var result = await FindAsync(key1, key2, paths.ToFetchQueryStrategy<TEntity>(), cancellationToken);

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        public async Task<TEntity> FindAsync([NotNull] TKey1 key1, [NotNull] TKey2 key2, [CanBeNull] IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotNull(key1, nameof(key1));
                Guard.NotNull(key2, nameof(key2));
            });

            Task<TEntity> Getter() =>
                UseContextAsync<TEntity>(
                    context => context.FindAsync<TEntity>(cancellationToken, fetchStrategy, key1, key2));

            TEntity result;

            if (CacheEnabled)
            {
                var cacheResult = await InterceptErrorAsync<ICacheQueryResult<TEntity>>(
                    () => CacheProvider.GetOrSetFindAsync<TEntity>(new object[] { key1, key2 }, fetchStrategy, Getter, Logger));

                result = cacheResult.Result;
                CacheUsed = cacheResult.CacheUsed;
            }
            else
            {
                result = await Getter();
                CacheUsed = false;
            }

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Asynchronously deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task DeleteAsync([NotNull] TKey1 key1, [NotNull] TKey2 key2, CancellationToken cancellationToken = new CancellationToken())
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
        public async Task<bool> TryDeleteAsync([NotNull] TKey1 key1, [NotNull] TKey2 key2, CancellationToken cancellationToken = new CancellationToken())
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
        protected RepositoryBase([NotNull] IRepositoryOptions options) : base(options) { }

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
        public void Delete([NotNull] TKey key)
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
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <returns><c>true</c> is able to successfully delete an entity with the given primary key; otherwise, <c>false</c>.</returns>
        public bool TryDelete([NotNull] TKey key)
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
        public TEntity Find([NotNull] TKey key)
        {
            return Find(key, (IFetchQueryStrategy<TEntity>)null);
        }

        /// <summary>
        /// Finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="paths">The dot-separated list of related objects to return in the query results.</param>
        /// <return>The entity found.</return>
        public TEntity Find([NotNull] TKey key, [NotNull] params string[] paths)
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotNull(key, nameof(key));
                Guard.NotNull(paths, nameof(paths));
            });

            var result = Find(key, paths.ToFetchQueryStrategy<TEntity>());

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="paths">A collection of lambda expressions representing the paths to include.</param>
        /// <return>The entity found.</return>
        public TEntity Find([NotNull] TKey key, [NotNull] params Expression<Func<TEntity, object>>[] paths)
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotNull(key, nameof(key));
                Guard.NotNull(paths, nameof(paths));
            });

            var result = Find(key, paths.ToFetchQueryStrategy<TEntity>());

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Finds an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity.</param>
        /// <return>The entity found.</return>
        public TEntity Find([NotNull] TKey key, [CanBeNull] IFetchQueryStrategy<TEntity> fetchStrategy)
        {
            LogExecutingMethod();

            InterceptError(() => Guard.NotNull(key, nameof(key)));

            TEntity Getter() =>
                UseContext<TEntity>(
                    context => context.Find<TEntity>(fetchStrategy, key));

            TEntity result;

            if (CacheEnabled)
            {
                var cacheResult = InterceptError<ICacheQueryResult<TEntity>>(
                    () => CacheProvider.GetOrSetFind<TEntity>(new object[] { key }, fetchStrategy, Getter, Logger));

                result = cacheResult.Result;
                CacheUsed = cacheResult.CacheUsed;
            }
            else
            {
                result = Getter();
                CacheUsed = false;
            }

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Determines whether the repository contains an entity with the given primary key value.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public bool Exists([NotNull] TKey key)
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
        public async Task<bool> ExistsAsync([NotNull] TKey key, CancellationToken cancellationToken = new CancellationToken())
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
        public Task<TEntity> FindAsync([NotNull] TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            return FindAsync(key, (IFetchQueryStrategy<TEntity>)null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="paths">The dot-separated list of related objects to return in the query results.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        public Task<TEntity> FindAsync([NotNull] TKey key, [NotNull] params string[] paths)
        {
            return FindAsync(key, paths, default(CancellationToken));
        }

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="paths">The dot-separated list of related objects to return in the query results.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        public async Task<TEntity> FindAsync([NotNull] TKey key, [NotNull] string[] paths, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotNull(key, nameof(key));
                Guard.NotNull(paths, nameof(paths));
            });

            var result = await FindAsync(key, paths.ToFetchQueryStrategy<TEntity>(), cancellationToken);

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="paths">A collection of lambda expressions representing the paths to include.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        public Task<TEntity> FindAsync([NotNull] TKey key, [NotNull] params Expression<Func<TEntity, object>>[] paths)
        {
            return FindAsync(key, paths, default(CancellationToken));
        }

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="paths">A collection of lambda expressions representing the paths to include.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        public async Task<TEntity> FindAsync([NotNull] TKey key, [NotNull] Expression<Func<TEntity, object>>[] paths, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotNull(key, nameof(key));
                Guard.NotNull(paths, nameof(paths));
            });

            var result = await FindAsync(key, paths.ToFetchQueryStrategy<TEntity>(), cancellationToken);

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Asynchronously finds an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        public async Task<TEntity> FindAsync([NotNull] TKey key, [CanBeNull] IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            InterceptError(() => Guard.NotNull(key, nameof(key)));

            Task<TEntity> Getter() =>
                UseContextAsync<TEntity>(
                    context => context.FindAsync<TEntity>(cancellationToken, fetchStrategy, key));

            TEntity result;

            if (CacheEnabled)
            {
                var cacheResult = await InterceptErrorAsync<ICacheQueryResult<TEntity>>(
                    () => CacheProvider.GetOrSetFindAsync<TEntity>(new object[] { key }, fetchStrategy, Getter, Logger));

                result = cacheResult.Result;
                CacheUsed = cacheResult.CacheUsed;
            }
            else
            {
                result = await Getter();
                CacheUsed = false;
            }

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Asynchronously deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task DeleteAsync([NotNull] TKey key, CancellationToken cancellationToken = new CancellationToken())
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
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> is able to successfully delete an entity with the given primary key; otherwise, <c>false</c>.</returns>
        public async Task<bool> TryDeleteAsync([NotNull] TKey key, CancellationToken cancellationToken = new CancellationToken())
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
        private readonly ILoggerProvider _loggerProvider;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the value indicating whether all the repository interceptors should be enabled or not.
        /// </summary>
        public bool InterceptorsEnabled { get; set; }

        /// <summary>
        /// Gets a dictionary for indicating whether the repository interceptor of the specified type should be disabled.
        /// </summary>
        public Dictionary<Type, bool> InterceptorTypesDisabled { get; }

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
        /// Gets the caching provider.
        /// </summary>
        protected internal ICacheProvider CacheProvider { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalRepositoryBase{TEntity}"/> class.
        /// </summary>
        /// <param name="options">The repository options.</param>
        internal InternalRepositoryBase([NotNull] IRepositoryOptions options)
        {
            Guard.NotNull(options, nameof(options));

            var optionsBuilder = new RepositoryOptionsBuilder(options);

            OnConfiguring(optionsBuilder);

            _options = optionsBuilder.Options;

            _contextFactory = Guard.EnsureNotNull(_options.ContextFactory, "No context provider has been configured for this repository.");

            // Sets the default logger provider (prints all messages levels)
            _loggerProvider = _options.LoggerProvider ?? new ConsoleLoggerProvider(LogLevel.Debug);

            Logger = _loggerProvider.Create($"DotNetToolkit.Repository<{typeof(TEntity).Name}>");

            CacheProvider = _options.CachingProvider ?? NullCacheProvider.Instance;

            if (CacheProvider.GetType() != typeof(NullCacheProvider))
                CacheEnabled = true;

            InterceptorsEnabled = true;
            InterceptorTypesDisabled = new Dictionary<Type, bool>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void ClearCache()
        {
            if (CacheEnabled)
                InterceptError(() => CacheProvider.IncrementCounter<TEntity>());
        }

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database and returns a collection of entities.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public IEnumerable<TEntity> ExecuteSqlQuery([NotNull] string sql, CommandType cmdType, [CanBeNull] object[] parameters, [NotNull] Func<IDataReader, TEntity> projector)
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotEmpty(sql, nameof(sql));
                Guard.NotNull(projector, nameof(projector));
            });

            var parametersDict = ConvertToParametersDictionary(parameters);

            IEnumerable<TEntity> Getter() =>
                UseContext<IEnumerable<TEntity>>(
                    context => context.ExecuteSqlQuery(sql, cmdType, parametersDict, projector));

            IEnumerable<TEntity> result;

            if (CacheEnabled)
            {
                var cacheResult = InterceptError<ICacheQueryResult<IEnumerable<TEntity>>>(
                    () => CacheProvider.GetOrSetExecuteSqlQuery<TEntity>(sql, cmdType, parametersDict, projector, Getter, Logger));

                result = cacheResult.Result;
                CacheUsed = cacheResult.CacheUsed;
            }
            else
            {
                result = Getter();
                CacheUsed = false;
            }

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database and returns a collection of entities.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public IEnumerable<TEntity> ExecuteSqlQuery([NotNull] string sql, [CanBeNull] object[] parameters, [NotNull] Func<IDataReader, TEntity> projector)
        {
            return ExecuteSqlQuery(sql, CommandType.Text, parameters, projector);
        }

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database and returns a collection of entities.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public IEnumerable<TEntity> ExecuteSqlQuery([NotNull] string sql, [NotNull] Func<IDataReader, TEntity> projector)
        {
            return ExecuteSqlQuery(sql, (object[])null, projector);
        }

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <returns>The number of rows affected.</returns>
        public int ExecuteSqlCommand([NotNull] string sql, CommandType cmdType, [CanBeNull] object[] parameters)
        {
            LogExecutingMethod();

            InterceptError(() => Guard.NotEmpty(sql, nameof(sql)));

            var parametersDict = ConvertToParametersDictionary(parameters);

            int Getter() =>
                UseContext<int>(
                    context => context.ExecuteSqlCommand(sql, cmdType, parametersDict));

            int result;

            if (CacheEnabled)
            {
                var cacheResult = InterceptError<ICacheQueryResult<int>>(
                    () => CacheProvider.GetOrSetExecuteSqlCommand<TEntity>(sql, cmdType, parametersDict, Getter, Logger));

                result = cacheResult.Result;
                CacheUsed = cacheResult.CacheUsed;
            }
            else
            {
                result = Getter();
                CacheUsed = false;
            }

            ClearCache(sql);

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <returns>The number of rows affected.</returns>
        public int ExecuteSqlCommand([NotNull] string sql, [CanBeNull] object[] parameters)
        {
            return ExecuteSqlCommand(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <returns>The number of rows affected.</returns>
        public int ExecuteSqlCommand([NotNull] string sql)
        {
            return ExecuteSqlCommand(sql, (object[])null);
        }

        /// <summary>
        /// Adds the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public void Add([NotNull] TEntity entity)
        {
            LogExecutingMethod(false);

            InterceptError(() => Guard.NotNull(entity, nameof(entity)));

            UseContext(context =>
            {
                Intercept(x => x.AddExecuting(
                    new RepositoryInterceptionContext<TEntity>(entity, context)));

                context.Add(entity);
                context.SaveChanges();
            });

            ClearCache();

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Adds the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        public void Add([NotNull] IEnumerable<TEntity> entities)
        {
            LogExecutingMethod(false);

            InterceptError(() => Guard.NotNull(entities, nameof(entities)));

            UseContext(context =>
            {
                foreach (var entity in entities)
                {
                    Intercept(x => x.AddExecuting(
                        new RepositoryInterceptionContext<TEntity>(entity, context)));

                    context.Add(entity);
                }

                context.SaveChanges();
            });

            ClearCache();

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Updates the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public void Update([NotNull] TEntity entity)
        {
            LogExecutingMethod(false);

            InterceptError(() => Guard.NotNull(entity, nameof(entity)));

            UseContext(context =>
            {
                Intercept(x => x.UpdateExecuting(
                    new RepositoryInterceptionContext<TEntity>(entity, context)));

                context.Update(entity);
                context.SaveChanges();
            });

            ClearCache();

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Updates the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        public void Update([NotNull] IEnumerable<TEntity> entities)
        {
            LogExecutingMethod(false);

            InterceptError(() => Guard.NotNull(entities, nameof(entities)));

            UseContext(context =>
            {
                foreach (var entity in entities)
                {
                    Intercept(x => x.UpdateExecuting(
                        new RepositoryInterceptionContext<TEntity>(entity, context)));

                    context.Update(entity);
                }

                context.SaveChanges();
            });

            ClearCache();

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public void Delete([NotNull] TEntity entity)
        {
            LogExecutingMethod(false);

            InterceptError(() => Guard.NotNull(entity, nameof(entity)));

            UseContext(context =>
            {
                Intercept(x => x.DeleteExecuting(
                    new RepositoryInterceptionContext<TEntity>(entity, context)));

                context.Remove(entity);
                context.SaveChanges();
            });

            ClearCache();

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Deletes all the entities in the repository that satisfies the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        public void Delete([NotNull] Expression<Func<TEntity, bool>> predicate)
        {
            LogExecutingMethod();

            InterceptError(() => Guard.NotNull(predicate, nameof(predicate)));

            Delete(predicate.ToQueryOptions<TEntity>());

            LogExecutedMethod();
        }

        /// <summary>
        /// Deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        public void Delete([NotNull] IQueryOptions<TEntity> options)
        {
            LogExecutingMethod(false);

            InterceptError(() =>
            {
                Guard.NotNull(options, nameof(options));
                Guard.EnsureNotNull(options.SpecificationStrategy, Resources.SpecificationMissingFromQueryOptions);
            });

            Delete(FindAll(options).Result);

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        public void Delete([NotNull] IEnumerable<TEntity> entities)
        {
            LogExecutingMethod(false);

            InterceptError(() => Guard.NotNull(entities, nameof(entities)));

            UseContext(context =>
            {
                foreach (var entity in entities)
                {
                    Intercept(x => x.DeleteExecuting(
                        new RepositoryInterceptionContext<TEntity>(entity, context)));

                    context.Remove(entity);
                }

                context.SaveChanges();
            });

            ClearCache();

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Finds the first entity in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The entity that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public TEntity Find([NotNull] Expression<Func<TEntity, bool>> predicate)
        {
            return Find<TEntity>(predicate, IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Finds the first entity in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The entity that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public TEntity Find([CanBeNull] IQueryOptions<TEntity> options)
        {
            return Find<TEntity>(options, IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public TResult Find<TResult>([NotNull] Expression<Func<TEntity, bool>> predicate, [NotNull] Expression<Func<TEntity, TResult>> selector)
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotNull(predicate, nameof(predicate));
                Guard.NotNull(selector, nameof(selector));
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
        public TResult Find<TResult>(IQueryOptions<TEntity> options, [NotNull] Expression<Func<TEntity, TResult>> selector)
        {
            LogExecutingMethod();

            InterceptError(() => Guard.NotNull(selector, nameof(selector)));

            TResult Getter() =>
                UseContext<TResult>(
                    context => context.Find<TEntity, TResult>(options, selector));

            TResult result;

            if (CacheEnabled)
            {
                var cacheResult = InterceptError<ICacheQueryResult<TResult>>(
                    () => CacheProvider.GetOrSetFind<TEntity, TResult>(options, selector, Getter, Logger));

                result = cacheResult.Result;
                CacheUsed = cacheResult.CacheUsed;
            }
            else
            {
                result = Getter();
                CacheUsed = false;
            }

            LogExecutedMethod();

            return result;
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
        public IEnumerable<TEntity> FindAll([NotNull] Expression<Func<TEntity, bool>> predicate)
        {
            return FindAll<TEntity>(predicate, IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public PagedQueryResult<IEnumerable<TEntity>> FindAll([CanBeNull] IQueryOptions<TEntity> options)
        {
            return FindAll<TEntity>(options, IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository.
        /// </summary>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository.</returns>
        public IEnumerable<TResult> FindAll<TResult>([NotNull] Expression<Func<TEntity, TResult>> selector)
        {
            return FindAll<TResult>((IQueryOptions<TEntity>)null, selector).Result;
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public IEnumerable<TResult> FindAll<TResult>([NotNull] Expression<Func<TEntity, bool>> predicate, [NotNull] Expression<Func<TEntity, TResult>> selector)
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotNull(predicate, nameof(predicate));
                Guard.NotNull(selector, nameof(selector));
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
        public PagedQueryResult<IEnumerable<TResult>> FindAll<TResult>(IQueryOptions<TEntity> options, [NotNull] Expression<Func<TEntity, TResult>> selector)
        {
            LogExecutingMethod();

            InterceptError(() => Guard.NotNull(selector, nameof(selector)));

            PagedQueryResult<IEnumerable<TResult>> Getter() =>
                UseContext<PagedQueryResult<IEnumerable<TResult>>>(
                    context => context.FindAll<TEntity, TResult>(options, selector));

            PagedQueryResult<IEnumerable<TResult>> result;

            if (CacheEnabled)
            {
                var cacheResult = InterceptError<ICachePagedQueryResult<IEnumerable<TResult>>>(
                    () => CacheProvider.GetOrSetFindAll<TEntity, TResult>(options, selector, Getter, Logger));

                result = cacheResult.Result;
                CacheUsed = cacheResult.CacheUsed;
            }
            else
            {
                result = Getter();
                CacheUsed = false;
            }

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">The predicate used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified predicate; otherwise, <c>false</c>.</returns>
        public bool Exists([NotNull] Expression<Func<TEntity, bool>> predicate)
        {
            LogExecutingMethod();

            InterceptError(() => Guard.NotNull(predicate, nameof(predicate)));

            var result = Exists(predicate.ToQueryOptions<TEntity>());

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        public bool Exists([NotNull] IQueryOptions<TEntity> options)
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotNull(options, nameof(options));
                Guard.EnsureNotNull(options.SpecificationStrategy, Resources.SpecificationMissingFromQueryOptions);
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
        public int Count([NotNull] Expression<Func<TEntity, bool>> predicate)
        {
            LogExecutingMethod();

            InterceptError(() => Guard.NotNull(predicate, nameof(predicate)));

            var result = Count(predicate.ToQueryOptions<TEntity>());

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public int Count([CanBeNull] IQueryOptions<TEntity> options)
        {
            LogExecutingMethod();

            int Getter() =>
                UseContext<int>(
                    context => context.Count<TEntity>(options));

            int result;

            if (CacheEnabled)
            {
                var cacheResult = InterceptError<ICacheQueryResult<int>>(
                    () => CacheProvider.GetOrSetCount<TEntity>(options, Getter, Logger));

                result = cacheResult.Result;
                CacheUsed = cacheResult.CacheUsed;
            }
            else
            {
                result = Getter();
                CacheUsed = false;
            }

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Dictionary<TDictionaryKey, TEntity> ToDictionary<TDictionaryKey>([NotNull] Expression<Func<TEntity, TDictionaryKey>> keySelector)
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
        public PagedQueryResult<Dictionary<TDictionaryKey, TEntity>> ToDictionary<TDictionaryKey>([CanBeNull] IQueryOptions<TEntity> options, [NotNull] Expression<Func<TEntity, TDictionaryKey>> keySelector)
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
        public Dictionary<TDictionaryKey, TElement> ToDictionary<TDictionaryKey, TElement>([NotNull] Expression<Func<TEntity, TDictionaryKey>> keySelector, [NotNull] Expression<Func<TEntity, TElement>> elementSelector)
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
        public PagedQueryResult<Dictionary<TDictionaryKey, TElement>> ToDictionary<TDictionaryKey, TElement>([CanBeNull] IQueryOptions<TEntity> options, [NotNull] Expression<Func<TEntity, TDictionaryKey>> keySelector, [NotNull] Expression<Func<TEntity, TElement>> elementSelector)
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotNull(keySelector, nameof(keySelector));
                Guard.NotNull(elementSelector, nameof(elementSelector));
            });

            PagedQueryResult<Dictionary<TDictionaryKey, TElement>> Getter() =>
                UseContext<PagedQueryResult<Dictionary<TDictionaryKey, TElement>>>(
                    context => context.ToDictionary(options, keySelector, elementSelector));

            PagedQueryResult<Dictionary<TDictionaryKey, TElement>> result;

            if (CacheEnabled)
            {
                var cacheResult = InterceptError<ICachePagedQueryResult<Dictionary<TDictionaryKey, TElement>>>(
                    () => CacheProvider.GetOrSetDictionary<TEntity, TDictionaryKey, TElement>(options, keySelector, elementSelector, Getter, Logger));

                result = cacheResult.Result;
                CacheUsed = cacheResult.CacheUsed;
            }
            else
            {
                result = Getter();
                CacheUsed = false;
            }

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A transform function to produce a result value from each element.</param>
        /// <returns>A new <see cref="IEnumerable{TResult}" /> that contains the grouped result.</returns>
        public IEnumerable<TResult> GroupBy<TGroupKey, TResult>([NotNull] Expression<Func<TEntity, TGroupKey>> keySelector, [NotNull] Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
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
        public PagedQueryResult<IEnumerable<TResult>> GroupBy<TGroupKey, TResult>([CanBeNull] IQueryOptions<TEntity> options, [NotNull] Expression<Func<TEntity, TGroupKey>> keySelector, [NotNull] Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotNull(keySelector, nameof(keySelector));
                Guard.NotNull(resultSelector, nameof(resultSelector));
            });

            PagedQueryResult<IEnumerable<TResult>> Getter() =>
                UseContext<PagedQueryResult<IEnumerable<TResult>>>(
                    context => context.GroupBy(options, keySelector, resultSelector));

            PagedQueryResult<IEnumerable<TResult>> result;

            if (CacheEnabled)
            {
                var cacheResult = InterceptError<ICachePagedQueryResult<IEnumerable<TResult>>>(
                    () => CacheProvider.GetOrSetGroup<TEntity, TGroupKey, TResult>(options, keySelector, resultSelector, Getter, Logger));

                result = cacheResult.Result;
                CacheUsed = cacheResult.CacheUsed;
            }
            else
            {
                result = Getter();
                CacheUsed = false;
            }

            LogExecutedMethod();

            return result;
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
        public async Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync([NotNull] string sql, CommandType cmdType, [CanBeNull] object[] parameters, [NotNull] Func<IDataReader, TEntity> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotEmpty(sql, nameof(sql));
                Guard.NotNull(projector, nameof(projector));
            });

            var parametersDict = ConvertToParametersDictionary(parameters);

            Task<IEnumerable<TEntity>> Getter() =>
                UseContextAsync<IEnumerable<TEntity>>(
                    context => context.ExecuteSqlQueryAsync(sql, cmdType, parametersDict, projector, cancellationToken));

            IEnumerable<TEntity> result;

            if (CacheEnabled)
            {
                var cacheResult = await InterceptErrorAsync<ICacheQueryResult<IEnumerable<TEntity>>>(
                    () => CacheProvider.GetOrSetExecuteSqlQueryAsync<TEntity>(sql, cmdType, parametersDict, projector, Getter, Logger));

                result = cacheResult.Result;
                CacheUsed = cacheResult.CacheUsed;
            }
            else
            {
                result = await Getter();
                CacheUsed = false;
            }

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Asynchronously creates a raw SQL query that is executed directly in the database and returns a collection of entities.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns> 
        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync([NotNull] string sql, [CanBeNull] object[] parameters, [NotNull] Func<IDataReader, TEntity> projector, CancellationToken cancellationToken = new CancellationToken())
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
        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync([NotNull] string sql, [NotNull] Func<IDataReader, TEntity> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteSqlQueryAsync(sql, (object[])null, projector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates raw SQL query that is executed directly in the database.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of rows affected.</returns>
        public async Task<int> ExecuteSqlCommandAsync([NotNull] string sql, CommandType cmdType, [CanBeNull] object[] parameters, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            InterceptError(() => Guard.NotEmpty(sql, nameof(sql)));

            var parametersDict = ConvertToParametersDictionary(parameters);

            Task<int> Getter() =>
                UseContextAsync<int>(
                    context => context.ExecuteSqlCommandAsync(sql, cmdType, parametersDict, cancellationToken));

            int result;

            if (CacheEnabled)
            {
                var cacheResult = await InterceptErrorAsync<ICacheQueryResult<int>>(
                    () => CacheProvider.GetOrSetExecuteSqlCommandAsync<TEntity>(sql, cmdType, parametersDict, Getter, Logger));

                result = cacheResult.Result;
                CacheUsed = cacheResult.CacheUsed;
            }
            else
            {
                result = await Getter();
                CacheUsed = false;
            }

            ClearCache(sql);

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Asynchronously creates raw SQL query that is executed directly in the database.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of rows affected.</returns>
        public Task<int> ExecuteSqlCommandAsync([NotNull] string sql, [CanBeNull] object[] parameters, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteSqlCommandAsync(sql, CommandType.Text, parameters, cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates raw SQL query that is executed directly in the database.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of rows affected.</returns>
        public Task<int> ExecuteSqlCommandAsync([NotNull] string sql, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteSqlCommandAsync(sql, (object[])null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously adds the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task AddAsync([NotNull] TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod(false);

            InterceptError(() =>
            {
                Guard.NotNull(entity, nameof(entity));

                cancellationToken.ThrowIfCancellationRequested();
            });

            await UseContextAsync(async context =>
            {
                await InterceptAsync(x => x.AddExecutingAsync(
                    new RepositoryInterceptionContext<TEntity>(entity, context),
                    cancellationToken));

                context.Add(entity);
                await context.SaveChangesAsync(cancellationToken);
            });

            ClearCache();

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Asynchronously adds the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task AddAsync([NotNull] IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod(false);

            InterceptError(() =>
            {
                Guard.NotNull(entities, nameof(entities));

                cancellationToken.ThrowIfCancellationRequested();
            });

            await UseContextAsync(async context =>
            {
                foreach (var entity in entities)
                {
                    await InterceptAsync(x => x.AddExecutingAsync(
                        new RepositoryInterceptionContext<TEntity>(entity, context),
                        cancellationToken));

                    context.Add(entity);
                }

                await context.SaveChangesAsync(cancellationToken);
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
        public async Task UpdateAsync([NotNull] TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod(false);

            InterceptError(() =>
            {
                Guard.NotNull(entity, nameof(entity));

                cancellationToken.ThrowIfCancellationRequested();
            });

            await UseContextAsync(async context =>
            {
                await InterceptAsync(x => x.UpdateExecutingAsync(
                    new RepositoryInterceptionContext<TEntity>(entity, context),
                    cancellationToken));

                context.Update(entity);
                await context.SaveChangesAsync(cancellationToken);
            });

            ClearCache();

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Asynchronously updates the specified <paramref name="entities" /> collection in the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task UpdateAsync([NotNull] IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod(false);

            InterceptError(() =>
            {
                Guard.NotNull(entities, nameof(entities));

                cancellationToken.ThrowIfCancellationRequested();
            });

            await UseContextAsync(async context =>
            {
                foreach (var entity in entities)
                {
                    await InterceptAsync(x => x.UpdateExecutingAsync(
                        new RepositoryInterceptionContext<TEntity>(entity, context),
                        cancellationToken));

                    context.Update(entity);
                }

                await context.SaveChangesAsync(cancellationToken);
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
        public async Task DeleteAsync([NotNull] TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod(false);

            InterceptError(() =>
            {
                Guard.NotNull(entity, nameof(entity));

                cancellationToken.ThrowIfCancellationRequested();
            });

            await UseContextAsync(async context =>
            {
                await InterceptAsync(x => x.DeleteExecutingAsync(
                    new RepositoryInterceptionContext<TEntity>(entity, context),
                    cancellationToken));

                context.Remove(entity);
                await context.SaveChangesAsync(cancellationToken);
            });

            ClearCache();

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Asynchronously deletes all the entities in the repository that satisfies the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task DeleteAsync([NotNull] Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod(false);

            InterceptError(() => Guard.NotNull(predicate, nameof(predicate)));

            await DeleteAsync(predicate.ToQueryOptions<TEntity>(), cancellationToken);

            LogExecutedMethod(false);
        }

        /// <summary>
        /// Asynchronously deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task DeleteAsync([NotNull] IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod(false);

            InterceptError(() =>
            {
                Guard.NotNull(options, nameof(options));
                Guard.EnsureNotNull(options.SpecificationStrategy, Resources.SpecificationMissingFromQueryOptions);
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
        public async Task DeleteAsync([NotNull] IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod(false);

            InterceptError(() =>
            {
                Guard.NotNull(entities, nameof(entities));

                cancellationToken.ThrowIfCancellationRequested();
            });

            await UseContextAsync(async context =>
            {
                foreach (var entity in entities)
                {
                    await InterceptAsync(x => x.DeleteExecutingAsync(
                        new RepositoryInterceptionContext<TEntity>(entity, context),
                        cancellationToken));

                    context.Remove(entity);
                }

                await context.SaveChangesAsync(cancellationToken);
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
        public Task<TEntity> FindAsync([NotNull] Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return FindAsync<TEntity>(predicate, IdentityExpression<TEntity>.Instance, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds the first entity in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public Task<TEntity> FindAsync([CanBeNull] IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
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
        public Task<TResult> FindAsync<TResult>([NotNull] Expression<Func<TEntity, bool>> predicate, [NotNull] Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            InterceptError(() => Guard.NotNull(predicate, nameof(predicate)));

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
        public async Task<TResult> FindAsync<TResult>([CanBeNull] IQueryOptions<TEntity> options, [NotNull] Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            InterceptError(() => Guard.NotNull(selector, nameof(selector)));

            Task<TResult> Getter() =>
                UseContextAsync<TResult>(
                    context => context.FindAsync<TEntity, TResult>(options, selector, cancellationToken));

            TResult result;

            if (CacheEnabled)
            {
                var cacheResult = await InterceptErrorAsync<ICacheQueryResult<TResult>>(
                    () => CacheProvider.GetOrSetFindAsync<TEntity, TResult>(options, selector, Getter, Logger));

                result = cacheResult.Result;
                CacheUsed = cacheResult.CacheUsed;
            }
            else
            {
                result = await Getter();
                CacheUsed = false;
            }

            LogExecutedMethod();

            return result;
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
        public Task<IEnumerable<TEntity>> FindAllAsync([NotNull] Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return FindAllAsync<TEntity>(predicate, IdentityExpression<TEntity>.Instance, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public Task<PagedQueryResult<IEnumerable<TEntity>>> FindAllAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return FindAllAsync<TEntity>(options, IdentityExpression<TEntity>.Instance, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository.
        /// </summary>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the collection of projected entity results in the repository.</returns>
        public async Task<IEnumerable<TResult>> FindAllAsync<TResult>([NotNull] Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
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
        public async Task<IEnumerable<TResult>> FindAllAsync<TResult>([NotNull] Expression<Func<TEntity, bool>> predicate, [NotNull] Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            InterceptError(() => Guard.NotNull(predicate, nameof(predicate)));

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
        public async Task<PagedQueryResult<IEnumerable<TResult>>> FindAllAsync<TResult>([CanBeNull] IQueryOptions<TEntity> options, [NotNull] Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            InterceptError(() => Guard.NotNull(selector, nameof(selector)));

            Task<PagedQueryResult<IEnumerable<TResult>>> Getter() =>
                UseContextAsync<PagedQueryResult<IEnumerable<TResult>>>(
                    context => context.FindAllAsync<TEntity, TResult>(options, selector, cancellationToken));

            PagedQueryResult<IEnumerable<TResult>> result;

            if (CacheEnabled)
            {
                var cacheResult = await InterceptErrorAsync<ICachePagedQueryResult<IEnumerable<TResult>>>(
                    () => CacheProvider.GetOrSetFindAllAsync<TEntity, TResult>(options, selector, Getter, Logger));

                result = cacheResult.Result;
                CacheUsed = cacheResult.CacheUsed;
            }
            else
            {
                result = await Getter();
                CacheUsed = false;
            }

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">The predicate used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the conditions defined by the specified predicate; otherwise, <c>false</c>.</returns>
        public Task<bool> ExistsAsync([NotNull] Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            InterceptError(() => Guard.NotNull(predicate, nameof(predicate)));

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
        public async Task<bool> ExistsAsync([NotNull] IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotNull(options, nameof(options));
                Guard.EnsureNotNull(options.SpecificationStrategy, Resources.SpecificationMissingFromQueryOptions);
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
        public Task<int> CountAsync([NotNull] Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            InterceptError(() => Guard.NotNull(predicate, nameof(predicate)));

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
        public async Task<int> CountAsync([CanBeNull] IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            Task<int> Getter() =>
                UseContextAsync<int>(
                    context => context.CountAsync<TEntity>(options, cancellationToken));

            int result;

            if (CacheEnabled)
            {
                var cacheResult = await InterceptErrorAsync<ICacheQueryResult<int>>(
                    () => CacheProvider.GetOrSetCountAsync<TEntity>(options, Getter, Logger));

                result = cacheResult.Result;
                CacheUsed = cacheResult.CacheUsed;
            }
            else
            {
                result = await Getter();
                CacheUsed = false;
            }

            LogExecutedMethod();

            return result;
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public async Task<Dictionary<TDictionaryKey, TEntity>> ToDictionaryAsync<TDictionaryKey>([NotNull] Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
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
        public Task<PagedQueryResult<Dictionary<TDictionaryKey, TEntity>>> ToDictionaryAsync<TDictionaryKey>(IQueryOptions<TEntity> options, [NotNull] Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
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
        public async Task<Dictionary<TDictionaryKey, TElement>> ToDictionaryAsync<TDictionaryKey, TElement>([NotNull] Expression<Func<TEntity, TDictionaryKey>> keySelector, [NotNull] Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
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
        public async Task<PagedQueryResult<Dictionary<TDictionaryKey, TElement>>> ToDictionaryAsync<TDictionaryKey, TElement>([CanBeNull] IQueryOptions<TEntity> options, [NotNull] Expression<Func<TEntity, TDictionaryKey>> keySelector, [NotNull] Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotNull(keySelector, nameof(keySelector));
                Guard.NotNull(elementSelector, nameof(elementSelector));
            });

            Task<PagedQueryResult<Dictionary<TDictionaryKey, TElement>>> Getter() =>
                UseContextAsync<PagedQueryResult<Dictionary<TDictionaryKey, TElement>>>(
                    context => context.ToDictionaryAsync(options, keySelector, elementSelector, cancellationToken));

            PagedQueryResult<Dictionary<TDictionaryKey, TElement>> result;

            if (CacheEnabled)
            {
                var cacheResult = await InterceptErrorAsync<ICachePagedQueryResult<Dictionary<TDictionaryKey, TElement>>>(
                    () => CacheProvider.GetOrSetDictionaryAsync<TEntity, TDictionaryKey, TElement>(options, keySelector, elementSelector, Getter, Logger));

                result = cacheResult.Result;
                CacheUsed = cacheResult.CacheUsed;
            }
            else
            {
                result = await Getter();
                CacheUsed = false;
            }

            LogExecutedMethod();

            return result;

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
        public async Task<IEnumerable<TResult>> GroupByAsync<TGroupKey, TResult>([NotNull] Expression<Func<TEntity, TGroupKey>> keySelector, [NotNull] Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
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
        public async Task<PagedQueryResult<IEnumerable<TResult>>> GroupByAsync<TGroupKey, TResult>([CanBeNull] IQueryOptions<TEntity> options, [NotNull] Expression<Func<TEntity, TGroupKey>> keySelector, [NotNull] Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            LogExecutingMethod();

            InterceptError(() =>
            {
                Guard.NotNull(keySelector, nameof(keySelector));
                Guard.NotNull(resultSelector, nameof(resultSelector));
            });

            Task<PagedQueryResult<IEnumerable<TResult>>> Getter() =>
                UseContextAsync<PagedQueryResult<IEnumerable<TResult>>>(
                    context => context.GroupByAsync(options, keySelector, resultSelector, cancellationToken));

            PagedQueryResult<IEnumerable<TResult>> result;

            if (CacheEnabled)
            {
                var cacheResult = await InterceptErrorAsync<ICachePagedQueryResult<IEnumerable<TResult>>>(
                    () => CacheProvider.GetOrSetGroupAsync<TEntity, TGroupKey, TResult>(options, keySelector, resultSelector, Getter, Logger));

                result = cacheResult.Result;
                CacheUsed = cacheResult.CacheUsed;
            }
            else
            {
                result = await Getter();
                CacheUsed = false;
            }

            LogExecutedMethod();

            return result;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the repository context.
        /// </summary>
        /// <returns>The repository context.</returns>
        protected IRepositoryContext GetContext()
        {
            var context = _contextFactory.Create();

            try
            {
                ConfigureContext(context);
            }
            catch (Exception)
            {
                DisposeContext(context);

                throw;
            }

            return context;
        }

        /// <summary>
        /// Override this method to configure the repository.
        /// </summary>
        /// <param name="optionsBuilder">A builder used to create or modify options for this repository.</param>
        protected virtual void OnConfiguring(RepositoryOptionsBuilder optionsBuilder) { }

        /// <summary>
        /// A method for using a disposable repository context.
        /// </summary>
        protected void UseContext([NotNull] Action<IRepositoryContext> action)
        {
            InterceptError(() =>
            {
                Guard.NotNull(action, nameof(action));

                var context = GetContext();

                try
                {
                    action(context);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    DisposeContext(context);
                }
            });
        }

        /// <summary>
        /// A method for using a disposable repository context.
        /// </summary>
        protected T UseContext<T>([NotNull] Func<IRepositoryContext, T> action)
        {
            return InterceptError<T>(() =>
            {
                Guard.NotNull(action, nameof(action));

                var context = GetContext();

                try
                {
                    return action(context);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    DisposeContext(context);
                }
            });
        }

        /// <summary>
        /// A method for using a disposable repository context asynchronously.
        /// </summary>
        protected Task UseContextAsync([NotNull] Func<IRepositoryContextAsync, Task> action)
        {
            return InterceptErrorAsync(async () =>
            {
                Guard.NotNull(action, nameof(action));

                var context = GetContext().AsAsync();

                try
                {
                    await action(context);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    DisposeContext(context);
                }
            });
        }

        /// <summary>
        /// A method for using a disposable repository context asynchronously.
        /// </summary>
        protected Task<T> UseContextAsync<T>([NotNull] Func<IRepositoryContextAsync, Task<T>> action)
        {
            return InterceptErrorAsync<T>(async () =>
            {
                Guard.NotNull(action, nameof(action));

                var context = GetContext().AsAsync();

                try
                {
                    return await action(context);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    DisposeContext(context);
                }
            });
        }

        /// <summary>
        /// Intercepts an error while executing the specified <paramref name="action"/>.
        /// </summary>
        /// <param name="action">The action to intercept.</param>
        protected void InterceptError([NotNull] Action action)
        {
            try
            {
                Guard.NotNull(action, nameof(action))();
            }
            catch (Exception ex)
            {
                Logger.Error(new RepositoryException(ex));

                throw;
            }
        }

        /// <summary>
        /// Intercepts an error while executing the specified <paramref name="action"/>.
        /// </summary>
        /// <typeparam name="T">The type of the result returned by the specified <paramref name="action"/>.</typeparam>
        /// <param name="action">The action to intercept.</param>
        /// <returns>The result returned by the specified <paramref name="action"/>.</returns>
        protected T InterceptError<T>([NotNull] Func<T> action)
        {
            try
            {
                return Guard.NotNull(action, nameof(action))();
            }
            catch (Exception ex)
            {
                Logger.Error(new RepositoryException(ex));

                throw;
            }
        }

        /// <summary>
        /// Asynchronously intercepts an error while executing the specified <paramref name="action"/>.
        /// </summary>
        /// <param name="action">The action to intercept.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the result returned by the specified <paramref name="action"/>.</returns>
        protected async Task<T> InterceptErrorAsync<T>([NotNull] Func<Task<T>> action)
        {
            try
            {
                return await Guard.NotNull(action, nameof(action))();
            }
            catch (Exception ex)
            {
                Logger.Error(new RepositoryException(ex));

                throw;
            }
        }

        /// <summary>
        /// Asynchronously intercepts an error while executing the specified <paramref name="action"/>.
        /// </summary>
        /// <param name="action">The action to intercept.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        protected async Task InterceptErrorAsync([NotNull] Func<Task> action)
        {
            try
            {
                await Guard.NotNull(action, nameof(action))();
            }
            catch (Exception ex)
            {
                Logger.Error(new RepositoryException(ex));

                throw;
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

        internal void Intercept([NotNull] Action<IRepositoryInterceptor> action)
        {
            Guard.NotNull(action, nameof(action));

            if (!InterceptorsEnabled)
                return;

            foreach (var interceptor in GetInterceptors())
            {
                var interceptorType = interceptor.GetType();
                var isEnabled = !InterceptorTypesDisabled.ContainsKey(interceptorType) ||
                    !InterceptorTypesDisabled[interceptorType];

                if (isEnabled) action(interceptor);
            }
        }

        internal async Task InterceptAsync([NotNull] Func<IRepositoryInterceptor, Task> action)
        {
            Guard.NotNull(action, nameof(action));

            if (!InterceptorsEnabled)
                return;

            foreach (var interceptor in GetInterceptors())
            {
                var interceptorType = interceptor.GetType();
                var isEnabled = !InterceptorTypesDisabled.ContainsKey(interceptorType) ||
                    !InterceptorTypesDisabled[interceptorType];

                if (isEnabled) await action(interceptor);
            }
        }

        #endregion

        #region Private Methods

        private IEnumerable<IRepositoryInterceptor> GetInterceptors()
        {
            return _interceptors ?? (_interceptors = _options.Interceptors.Values.Select(lazyInterceptor => lazyInterceptor.Value));
        }

        private static void DisposeContext([CanBeNull] IRepositoryContext context)
        {
            if (context != null && context.CurrentTransaction == null)
                context.Dispose();
        }

        private void ConfigureContext(IRepositoryContext context)
        {
            Guard.EnsureNotNull(context.Conventions, "No conventions have been configured for this context.");

            if (_options.Conventions != null)
                RepositoryConventions.Combine(_options.Conventions, context.Conventions);

            if (_loggerProvider != null)
                context.LoggerProvider = _loggerProvider;

            context.Conventions.ThrowsIfInvalidPrimaryKeyDefinition<TEntity>();
        }

        private void ClearCache([NotNull] string sql)
        {
            var s = sql.ToUpperInvariant();

            var canClearCache = s.Contains("UPDATE") || s.Contains("DELETE FROM") || s.Contains("INSERT INTO");

            if (canClearCache)
                ClearCache();
        }

        private static Dictionary<string, object> ConvertToParametersDictionary([CanBeNull] object[] parameters)
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
