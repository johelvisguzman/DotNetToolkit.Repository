namespace DotNetToolkit.Repository
{
    using Factories;
    using JetBrains.Annotations;

    /// <summary>
    /// An implementation of <see cref="IService{TEntity, TKey1, TKey2, TKey3}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    /// <typeparam name="TKey3">The type of the third part of the composite primary key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IService{TEntity, TKey1, TKey2, TKey3}" />
    public sealed class Service<TEntity, TKey1, TKey2, TKey3> : ServiceBase<TEntity, TKey1, TKey2, TKey3> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Service{TEntity, TKey1, TKey2, TKey3}"/> class.
        /// </summary>
        /// <param name="unitOfWorkFactory">The unit of work factory.</param>
        public Service([NotNull] IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory) { }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IService{TEntity, TKey1, TKey2}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IService{TEntity, TKey1, TKey2}" />
    public sealed class Service<TEntity, TKey1, TKey2> : ServiceBase<TEntity, TKey1, TKey2> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Service{TEntity, TKey1, TKey2}"/> class.
        /// </summary>
        /// <param name="unitOfWorkFactory">The unit of work factory.</param>
        public Service([NotNull] IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory) { }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IService{TEntity, TKey}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IService{TEntity, TKey}" />
    public sealed class Service<TEntity, TKey> : ServiceBase<TEntity, TKey> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Service{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="unitOfWorkFactory">The unit of work factory.</param>
        public Service([NotNull] IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory) { }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IService{TEntity}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IService{TEntity}" />
    public sealed class Service<TEntity> : ServiceBase<TEntity, int>, IService<TEntity> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Service{TEntity}"/> class.
        /// </summary>
        /// <param name="unitOfWorkFactory">The unit of work factory.</param>
        public Service([NotNull] IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory) { }

        #endregion
    }
}
