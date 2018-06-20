namespace DotNetToolkit.Repository.AdoNet
{
    using Factories;
    using Interceptors;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Transactions;

    /// <summary>
    /// An implementation of <see cref="IUnitOfWorkFactoryAsync" />.
    /// </summary>
    /// <seealso cref="DotNetToolkit.Repository.Factories.IUnitOfWorkFactoryAsync" />
    public class AdoNetUnitOfWorkFactory : IUnitOfWorkFactoryAsync
    {
        #region Fields

        private readonly string _connectionString;
        private readonly string _providerName;
        private readonly IEnumerable<IRepositoryInterceptor> _interceptors;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetUnitOfWorkFactory"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public AdoNetUnitOfWorkFactory(string connectionString) : this(connectionString, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetUnitOfWorkFactory"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="interceptor">The interceptor.</param>
        public AdoNetUnitOfWorkFactory(string connectionString, IRepositoryInterceptor interceptor) : this(connectionString, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetUnitOfWorkFactory"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetUnitOfWorkFactory(string connectionString, IEnumerable<IRepositoryInterceptor> interceptors)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            _connectionString = connectionString;
            _interceptors = interceptors ?? Enumerable.Empty<IRepositoryInterceptor>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetUnitOfWorkFactory"/> class.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        public AdoNetUnitOfWorkFactory(string providerName, string connectionString) : this(providerName, connectionString, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetUnitOfWorkFactory"/> class.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="interceptor">The interceptor.</param>
        public AdoNetUnitOfWorkFactory(string providerName, string connectionString, IRepositoryInterceptor interceptor) : this(providerName, connectionString, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetUnitOfWorkFactory"/> class.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetUnitOfWorkFactory(string providerName, string connectionString, IEnumerable<IRepositoryInterceptor> interceptors)
        {
            if (providerName == null)
                throw new ArgumentNullException(nameof(providerName));

            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            _connectionString = connectionString;
            _providerName = providerName;
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
            var args = new List<object>();

            if (!string.IsNullOrEmpty(_providerName))
                args.Add(_providerName);

            args.Add(_connectionString);

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
            return CreateInstance<AdoNetUnitOfWork>();
        }

        #endregion
    }
}
