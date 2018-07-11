namespace DotNetToolkit.Repository.Wrappers
{
    using FetchStrategies;
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// An implementation of <see cref="IReadOnlyRepository{TEntity, TKey1, TKey2, TKey3}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    /// <typeparam name="TKey3">The type of the third part of the composite primary key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.Wrappers.ReadOnlyRepositoryBase{TEntity}" />
    /// <seealso cref="DotNetToolkit.Repository.Wrappers.IReadOnlyRepository{TEntity, TKey1, TKey2, TKey3}" />
    [ComVisible(false)]
    internal class ReadOnlyRepository<TEntity, TKey1, TKey2, TKey3> : ReadOnlyRepositoryBase<TEntity>, IReadOnlyRepository<TEntity, TKey1, TKey2, TKey3> where TEntity : class
    {
        #region Fields

        private readonly IRepository<TEntity, TKey1, TKey2, TKey3> _repo;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRepository{TEntity, TKey1, TKey2, TKey3}" /> class.
        /// </summary>
        /// <param name="repo">The underlying repository.</param>
        public ReadOnlyRepository(IRepository<TEntity, TKey1, TKey2, TKey3> repo) : base(repo)
        {
            if (repo == null)
                throw new ArgumentNullException(nameof(repo));

            _repo = repo;
        }

        #endregion

        #region Implementation of IReadOnlyRepository<TEntity, TKey1, TKey2, TKey3>

        /// <summary>
        /// Determines whether the repository contains an entity with the given primary key value
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public bool Exists(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            return _repo.Exists(key1, key2, key3);
        }

        /// <summary>
        /// Gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <return>The entity found.</return>
        public TEntity Get(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            return _repo.Get(key1, key2, key3);
        }

        /// <summary>
        /// Gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <return>The entity found.</return>
        public TEntity Get(TKey1 key1, TKey2 key2, TKey3 key3, IFetchStrategy<TEntity> fetchStrategy)
        {
            return _repo.Get(key1, key2, key3, fetchStrategy);
        }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IReadOnlyRepository{TEntity, TKey1, TKey2}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.Wrappers.ReadOnlyRepositoryBase{TEntity}" />
    /// <seealso cref="DotNetToolkit.Repository.Wrappers.IReadOnlyRepository{TEntity, TKey1, TKey2}" />
    [ComVisible(false)]
    internal class ReadOnlyRepository<TEntity, TKey1, TKey2> : ReadOnlyRepositoryBase<TEntity>, IReadOnlyRepository<TEntity, TKey1, TKey2> where TEntity : class
    {
        #region Fields

        private readonly IRepository<TEntity, TKey1, TKey2> _repo;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRepository{TEntity, TKey1, TKey2}" /> class.
        /// </summary>
        /// <param name="repo">The underlying repository.</param>
        public ReadOnlyRepository(IRepository<TEntity, TKey1, TKey2> repo) : base(repo)
        {
            if (repo == null)
                throw new ArgumentNullException(nameof(repo));

            _repo = repo;
        }

        #endregion

        #region Implementation of IReadOnlyRepository<TEntity, TKey1, TKey2>

        /// <summary>
        /// Determines whether the repository contains an entity with the given primary key value
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public bool Exists(TKey1 key1, TKey2 key2)
        {
            return _repo.Exists(key1, key2);
        }

        /// <summary>
        /// Gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <return>The entity found.</return>
        public TEntity Get(TKey1 key1, TKey2 key2)
        {
            return _repo.Get(key1, key2);
        }

        /// <summary>
        /// Gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <return>The entity found.</return>
        public TEntity Get(TKey1 key1, TKey2 key2, IFetchStrategy<TEntity> fetchStrategy)
        {
            return _repo.Get(key1, key2, fetchStrategy);
        }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IReadOnlyRepository{TEntity, TKey}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.Wrappers.ReadOnlyRepositoryBase{TEntity}" />
    /// <seealso cref="DotNetToolkit.Repository.Wrappers.IReadOnlyRepository{TEntity, TKey}" />
    [ComVisible(false)]
    internal class ReadOnlyRepository<TEntity, TKey> : ReadOnlyRepositoryBase<TEntity>, IReadOnlyRepository<TEntity, TKey> where TEntity : class
    {
        #region Fields

        private readonly IRepository<TEntity, TKey> _repo;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRepository{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="repo">The underlying repository.</param>
        public ReadOnlyRepository(IRepository<TEntity, TKey> repo) : base(repo)
        {
            if (repo == null)
                throw new ArgumentNullException(nameof(repo));

            _repo = repo;
        }

        #endregion

        #region Implementation of IReadOnlyRepository<TEntity, TKey>

        /// <summary>
        /// Determines whether the repository contains an entity with the given primary key value
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public bool Exists(TKey key)
        {
            return _repo.Exists(key);
        }

        /// <summary>
        /// Gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <return>The entity found.</return>
        public TEntity Get(TKey key)
        {
            return _repo.Get(key);
        }

        /// <summary>
        /// Gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <return>The entity found.</return>
        public TEntity Get(TKey key, IFetchStrategy<TEntity> fetchStrategy)
        {
            return _repo.Get(key, fetchStrategy);
        }

        #endregion
    }
}
