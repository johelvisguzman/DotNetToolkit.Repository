namespace DotNetToolkit.Repository
{
    using Factories;
    using Queries.Strategies;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// An implementation of <see cref="IService{TEntity, TKey1, TKey2, TKey3}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    /// <typeparam name="TKey3">The type of the third part of the composite primary key.</typeparam>
    public abstract class ServiceBase<TEntity, TKey1, TKey2, TKey3> : Internal.ServiceBase<TEntity>, IService<TEntity, TKey1, TKey2, TKey3> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBase{TEntity, TKey1, TKey2, TKey3}"/> class.
        /// </summary>
        /// <param name="unitOfWorkFactory">The unit of work factory.</param>
        protected ServiceBase(IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory) { }

        #endregion

        #region Implementation of IService<TEntity, TKey1, TKey2, TKey3>

        /// <summary>
        /// Determines whether the repository contains an entity with the given composite primary key values.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public virtual bool GetExists(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().Exists(key1, key2, key3);
            }
        }

        /// <summary>
        /// Finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <return>The entity found.</return>
        public virtual TEntity Get(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().Find(key1, key2, key3);
            }
        }

        /// <summary>
        /// Finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <return>The entity found.</return>
        public virtual TEntity Get(TKey1 key1, TKey2 key2, TKey3 key3, IFetchQueryStrategy<TEntity> fetchStrategy)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().Find(key1, key2, key3, fetchStrategy);
            }
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity with the given composite primary key values.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public virtual async Task<bool> GetExistsAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().ExistsAsync(key1, key2, key3, cancellationToken);
            }
        }

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        public virtual async Task<TEntity> GetAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAsync(key1, key2, key3, cancellationToken);
            }
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
        public virtual async Task<TEntity> GetAsync(TKey1 key1, TKey2 key2, TKey3 key3, IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAsync(key1, key2, key3, fetchStrategy, cancellationToken);
            }
        }

        /// <summary>
        /// Deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        public virtual void Delete(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2, TKey3>().Delete(key1, key2, key3);
                uow.Commit();
            }
        }

        /// <summary>
        /// Deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <returns><c>true</c> is able to successfully delete an entity with the given composite primary key values; otherwise, <c>false</c>.</returns>
        public virtual bool TryDelete(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().TryDelete(key1, key2, key3);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task DeleteAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2, TKey3>().DeleteAsync(key1, key2, key3, cancellationToken);
                uow.Commit();
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
        public virtual async Task<bool> TryDeleteAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().TryDeleteAsync(key1, key2, key3, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IService{TEntity, TKey1, TKey2}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    public abstract class ServiceBase<TEntity, TKey1, TKey2> : Internal.ServiceBase<TEntity>, IService<TEntity, TKey1, TKey2> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBase{TEntity, TKey1, TKey2}"/> class.
        /// </summary>
        /// <param name="unitOfWorkFactory">The unit of work factory.</param>
        protected ServiceBase(IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory) { }

        #endregion

        #region Implementation of IService<TEntity, TKey1, TKey2>

        /// <summary>
        /// Determines whether the repository contains an entity with the given composite primary key values.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public virtual bool GetExists(TKey1 key1, TKey2 key2)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().Exists(key1, key2);
            }
        }

        /// <summary>
        /// Finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <return>The entity found.</return>
        public virtual TEntity Get(TKey1 key1, TKey2 key2)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().Find(key1, key2);
            }
        }

        /// <summary>
        /// Finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <return>The entity found.</return>
        public virtual TEntity Get(TKey1 key1, TKey2 key2, IFetchQueryStrategy<TEntity> fetchStrategy)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().Find(key1, key2, fetchStrategy);
            }
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity with the given composite primary key values.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public virtual async Task<bool> GetExistsAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().ExistsAsync(key1, key2, cancellationToken);
            }
        }

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        public virtual async Task<TEntity> GetAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().FindAsync(key1, key2, cancellationToken);
            }
        }

        /// <summary>
        /// Asynchronously finds an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        public virtual async Task<TEntity> GetAsync(TKey1 key1, TKey2 key2, IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().FindAsync(key1, key2, fetchStrategy, cancellationToken);
            }
        }

        /// <summary>
        /// Deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        public virtual void Delete(TKey1 key1, TKey2 key2)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2>().Delete(key1, key2);
                uow.Commit();
            }
        }

        /// <summary>
        /// Deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <returns><c>true</c> is able to successfully delete an entity with the given composite primary key values; otherwise, <c>false</c>.</returns>
        public virtual bool TryDelete(TKey1 key1, TKey2 key2)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().TryDelete(key1, key2);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task DeleteAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2>().DeleteAsync(key1, key2, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        /// Asynchronously deletes an entity with the given composite primary key values in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> is able to successfully delete an entity with the given composite primary key values; otherwise, <c>false</c>.</returns>
        public virtual async Task<bool> TryDeleteAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().TryDeleteAsync(key1, key2, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IService{TEntity, TKey}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public abstract class ServiceBase<TEntity, TKey> : Internal.ServiceBase<TEntity>, IService<TEntity, TKey> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="unitOfWorkFactory">The unit of work factory.</param>
        protected ServiceBase(IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory) { }

        #endregion

        #region Implementation of IService<TEntity, TKey>

        /// <summary>
        /// Determines whether the repository contains an entity with the given primary key value.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public virtual bool GetExists(TKey key)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().Exists(key);
            }
        }

        /// <summary>
        /// Finds an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <return>The entity found.</return>
        public virtual TEntity Get(TKey key)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().Find(key);
            }
        }

        /// <summary>
        /// Finds an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <return>The entity found.</return>
        public virtual TEntity Get(TKey key, IFetchQueryStrategy<TEntity> fetchStrategy)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().Find(key, fetchStrategy);
            }
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity with the given primary key value.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public virtual async Task<bool> GetExistsAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().ExistsAsync(key, cancellationToken);
            }
        }

        /// <summary>
        /// Asynchronously finds an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        public virtual async Task<TEntity> GetAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().FindAsync(key, cancellationToken);
            }
        }

        /// <summary>
        /// Asynchronously finds an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        public virtual async Task<TEntity> GetAsync(TKey key, IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().FindAsync(key, fetchStrategy, cancellationToken);
            }
        }

        /// <summary>
        /// Deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        public virtual void Delete(TKey key)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey>().Delete(key);
                uow.Commit();
            }
        }

        /// <summary>
        /// Deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key"></param>
        /// <returns><c>true</c> is able to successfully delete an entity with the given primary key; otherwise, <c>false</c>.</returns>
        public virtual bool TryDelete(TKey key)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().TryDelete(key);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task DeleteAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey>().DeleteAsync(key, cancellationToken);
                uow.Commit();
            }
        }

        /// <summary>
        ///  Asynchronously deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> is able to successfully delete an entity with the given primary key; otherwise, <c>false</c>.</returns>
        public virtual async Task<bool> TryDeleteAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().TryDeleteAsync(key, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        #endregion
    }
}
