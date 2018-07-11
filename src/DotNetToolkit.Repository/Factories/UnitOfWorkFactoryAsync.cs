namespace DotNetToolkit.Repository.Factories
{
    using Interceptors;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Transactions;

    /// <summary>
    /// An implementation of <see cref="IUnitOfWorkFactoryAsync" />.
    /// </summary>
    /// <seealso cref="DotNetToolkit.Repository.Factories.IUnitOfWorkFactoryAsync" />
    public class UnitOfWorkFactoryAsync : IUnitOfWorkFactoryAsync
    {
        #region Fields

        private readonly Func<IRepositoryContextAsync> _contextFactory;
        private readonly IEnumerable<IRepositoryInterceptor> _interceptors;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkFactoryAsync" /> class.
        /// </summary>
        /// <param name="contextFactory">The repository context factory.</param>
        public UnitOfWorkFactoryAsync(Func<IRepositoryContextAsync> contextFactory) : this(contextFactory, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkFactoryAsync" /> class.
        /// </summary>
        /// <param name="contextFactory">The repository context factory.</param>
        /// <param name="interceptor">The interceptor.</param>
        public UnitOfWorkFactoryAsync(Func<IRepositoryContextAsync> contextFactory, IRepositoryInterceptor interceptor) : this(contextFactory, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkFactoryAsync" /> class.
        /// </summary>
        /// <param name="contextFactory">The repository context factory.</param>
        /// <param name="interceptors">The interceptors.</param>
        public UnitOfWorkFactoryAsync(Func<IRepositoryContextAsync> contextFactory, IEnumerable<IRepositoryInterceptor> interceptors)
        {
            if (contextFactory == null)
                throw new ArgumentNullException(nameof(contextFactory));

            _contextFactory = contextFactory;
            _interceptors = interceptors ?? Enumerable.Empty<IRepositoryInterceptor>();
        }

        #endregion

        #region Implementation of IUnitOfWorkFactory

        /// <summary>
        /// Creates a new unit of work.
        /// </summary>
        /// <returns>The new unit of work.</returns>
        public IUnitOfWork Create()
        {
            return CreateAsync();
        }

        /// <summary>
        /// Creates a new repository for the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The new repository.</returns>
        public T CreateInstance<T>() where T : class
        {
            var args = new List<object> { _contextFactory() };

            if (_interceptors.Any())
                args.Add(_interceptors);

            return (T)Activator.CreateInstance(typeof(T), args.ToArray());
        }

        #endregion

        #region Implementation of IUnitOfWorkFactoryAsync

        /// <summary>
        /// Creates a new asynchronous unit of work.
        /// </summary>
        /// <returns>The new asynchronous unit of work.</returns>
        public IUnitOfWorkAsync CreateAsync()
        {
            return CreateInstance<UnitOfWorkAsync>();
        }

        #endregion
    }
}
