namespace DotNetToolkit.Repository.Transactions
{
    using Configuration.Options;
    using JetBrains.Annotations;
    using System;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="IUnitOfWorkFactory" />.
    /// </summary>
    /// <seealso cref="DotNetToolkit.Repository.Transactions.IUnitOfWorkFactory" />
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        #region Fields

        private readonly IRepositoryOptions _options;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkFactory" /> class.
        /// </summary>
        /// <param name="optionsAction">A builder action used to create or modify options for this unit of work factory.</param>
        public UnitOfWorkFactory([NotNull] Action<RepositoryOptionsBuilder> optionsAction)
        {
            Guard.NotNull(optionsAction, nameof(optionsAction));

            var optionsBuilder = new RepositoryOptionsBuilder();

            optionsAction(optionsBuilder);

            _options = optionsBuilder.Options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkFactory" /> class.
        /// </summary>
        /// <param name="options">The repository options.</param>
        public UnitOfWorkFactory([NotNull] IRepositoryOptions options)
        {
            _options = Guard.NotNull(options, nameof(options));
        }

        #endregion

        #region Implementation of IUnitOfWorkFactory

        /// <summary>
        /// Creates a new unit of work.
        /// </summary>
        /// <returns>The new unit of work.</returns>
        public IUnitOfWork Create()
        {
            return new UnitOfWork(_options);
        }

        /// <summary>
        /// Creates a new repository for the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The new repository.</returns>
        public T CreateInstance<T>() where T : class
        {
            return (T)Activator.CreateInstance(typeof(T), new object[] { _options });
        }

        #endregion
    }
}
