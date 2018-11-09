namespace DotNetToolkit.Repository.Factories
{
    using Configuration.Options;
    using System;

    /// <summary>
    /// An implementation of <see cref="IRepositoryFactory" />.
    /// </summary>
    /// <seealso cref="DotNetToolkit.Repository.Factories.IRepositoryFactory" />
    public class RepositoryFactory : IRepositoryFactory
    {
        #region Fields

        private readonly RepositoryOptions _options;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryFactory" /> class.
        /// </summary>
        /// <param name="optionsAction">A builder action used to create or modify options for this repository factory.</param>
        public RepositoryFactory(Action<RepositoryOptionsBuilder> optionsAction)
        {
            if (optionsAction == null)
                throw new ArgumentNullException(nameof(optionsAction));

            var optionsBuilder = new RepositoryOptionsBuilder();

            optionsAction(optionsBuilder);

            _options = optionsBuilder.Options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryFactory" /> class.
        /// </summary>
        /// <param name="options">The repository options.</param>
        public RepositoryFactory(RepositoryOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _options = options;
        }

        #endregion

        #region Implementation of IRepositoryFactory

        /// <inheritdoc />
        public IRepository<TEntity> Create<TEntity>() where TEntity : class
        {
            return CreateInstance<Repository<TEntity>>();
        }

        /// <inheritdoc />
        public IRepository<TEntity, TKey> Create<TEntity, TKey>() where TEntity : class
        {
            return CreateInstance<Repository<TEntity, TKey>>();
        }

        /// <inheritdoc />
        public IRepository<TEntity, TKey1, TKey2> Create<TEntity, TKey1, TKey2>() where TEntity : class
        {
            return CreateInstance<Repository<TEntity, TKey1, TKey2>>();
        }

        /// <inheritdoc />
        public IRepository<TEntity, TKey1, TKey2, TKey3> Create<TEntity, TKey1, TKey2, TKey3>() where TEntity : class
        {
            return CreateInstance<Repository<TEntity, TKey1, TKey2, TKey3>>();
        }

        /// <inheritdoc />
        public T CreateInstance<T>() where T : class
        {
            return (T)Activator.CreateInstance(typeof(T), new object[] { _options });
        }

        #endregion
    }
}
