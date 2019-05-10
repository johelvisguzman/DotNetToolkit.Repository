namespace DotNetToolkit.Repository
{
    using Configuration.Options;
    using JetBrains.Annotations;

    /// <summary>
    /// An implementation of <see cref="IRepository{TEntity, TKey1, TKey2, TKey3}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    /// <typeparam name="TKey3">The type of the third part of the composite primary key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IRepository{TEntity, TKey1, TKey2, TKey3}" />
    public sealed class Repository<TEntity, TKey1, TKey2, TKey3> : RepositoryBase<TEntity, TKey1, TKey2, TKey3> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey1, TKey2, TKey3}"/> class.
        /// </summary>
        /// <param name="options">The repository options.</param>
        public Repository([NotNull] IRepositoryOptions options) : base(options) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey1, TKey2, TKey3}"/> class with the <see cref="RepositoryDependencyResolver"/> using an IOC container to resolve the <see cref="IRepositoryOptions"/>
        /// </summary>
        public Repository() : this(RepositoryDependencyResolver.Current.Resolve<IRepositoryOptions>()) { }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IRepository{TEntity, TKey1, TKey2}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IRepository{TEntity, TKey1, TKey2}" />
    public sealed class Repository<TEntity, TKey1, TKey2> : RepositoryBase<TEntity, TKey1, TKey2> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey1, TKey2}"/> class.
        /// </summary>
        /// <param name="options">The repository options.</param>
        public Repository([NotNull] IRepositoryOptions options) : base(options) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey1, TKey2}"/> class with the <see cref="RepositoryDependencyResolver"/> using an IOC container to resolve the <see cref="IRepositoryOptions"/>.
        /// </summary>
        public Repository() : this(RepositoryDependencyResolver.Current.Resolve<IRepositoryOptions>()) { }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IRepository{TEntity, TKey}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IRepository{TEntity, TKey}" />
    public sealed class Repository<TEntity, TKey> : RepositoryBase<TEntity, TKey> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="options">The repository options.</param>
        public Repository([NotNull] IRepositoryOptions options) : base(options) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey}"/> class with the <see cref="RepositoryDependencyResolver"/> using an IOC container to resolve the <see cref="IRepositoryOptions"/>.
        /// </summary>
        public Repository() : this(RepositoryDependencyResolver.Current.Resolve<IRepositoryOptions>()) { }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IRepository{TEntity}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IRepository{TEntity}" />
    public sealed class Repository<TEntity> : RepositoryBase<TEntity, int>, IRepository<TEntity> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity}"/> class.
        /// </summary>
        /// <param name="options">The repository options.</param>
        public Repository([NotNull] IRepositoryOptions options) : base(options) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity}"/> class with the <see cref="RepositoryDependencyResolver"/> using an IOC container to resolve the <see cref="IRepositoryOptions"/>.
        /// </summary>
        public Repository() : this(RepositoryDependencyResolver.Current.Resolve<IRepositoryOptions>()) { }

        #endregion
    }
}