namespace DotNetToolkit.Repository.EntityFramework
{
    using Factories;
    using Interceptors;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using Transactions;

    /// <summary>
    /// An implementation of <see cref="IUnitOfWorkFactoryAsync" />.
    /// </summary>
    /// <seealso cref="DotNetToolkit.Repository.Factories.IUnitOfWorkFactoryAsync" />
    public class EfUnitOfWorkFactory : IUnitOfWorkFactoryAsync
    {
        #region Fields

        private readonly Func<DbContext> _dbContextFactory;
        private readonly IEnumerable<IRepositoryInterceptor> _interceptors;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfUnitOfWorkFactory"/> class.
        /// </summary>
        /// <param name="dbContextFactory">The database context factory.</param>
        public EfUnitOfWorkFactory(Func<DbContext> dbContextFactory) : this(dbContextFactory, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfUnitOfWorkFactory"/> class.
        /// </summary>
        /// <param name="dbContextFactory">The database context factory.</param>
        /// <param name="interceptor">The interceptor.</param>
        public EfUnitOfWorkFactory(Func<DbContext> dbContextFactory, IRepositoryInterceptor interceptor) : this(dbContextFactory, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfUnitOfWorkFactory"/> class.
        /// </summary>
        /// <param name="dbContextFactory">The database context factory.</param>
        /// <param name="interceptors">The interceptors.</param>
        public EfUnitOfWorkFactory(Func<DbContext> dbContextFactory, IEnumerable<IRepositoryInterceptor> interceptors)
        {
            if (dbContextFactory == null)
                throw new ArgumentNullException(nameof(dbContextFactory));

            _dbContextFactory = dbContextFactory;
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
            var args = new List<object> { _dbContextFactory() };

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
            return CreateInstance<EfUnitOfWork>();
        }

        #endregion
    }
}
