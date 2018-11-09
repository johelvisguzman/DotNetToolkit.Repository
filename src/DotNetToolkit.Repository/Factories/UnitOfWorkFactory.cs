namespace DotNetToolkit.Repository.Factories
{
    using Configuration.Options;
    using System;
    using Transactions;

    /// <summary>
    /// An implementation of <see cref="IUnitOfWorkFactory" />.
    /// </summary>
    /// <seealso cref="DotNetToolkit.Repository.Factories.IUnitOfWorkFactory" />
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        #region Fields

        private readonly RepositoryOptions _options;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkFactory" /> class.
        /// </summary>
        /// <param name="optionsAction">A builder action used to create or modify options for this unit of work factory.</param>
        public UnitOfWorkFactory(Action<RepositoryOptionsBuilder> optionsAction)
        {
            if (optionsAction == null)
                throw new ArgumentNullException(nameof(optionsAction));

            var optionsBuilder = new RepositoryOptionsBuilder();

            optionsAction(optionsBuilder);

            _options = optionsBuilder.Options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkFactory" /> class.
        /// </summary>
        /// <param name="options">The repository options.</param>
        public UnitOfWorkFactory(RepositoryOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _options = options;
        }

        #endregion

        #region Implementation of IUnitOfWorkFactory

        /// <inheritdoc />
        public IUnitOfWork Create()
        {
            return CreateInstance<UnitOfWork>();
        }

        /// <inheritdoc />
        public T CreateInstance<T>() where T : class
        {
            return (T)Activator.CreateInstance(typeof(T), new object[] { _options });
        }

        #endregion
    }
}
