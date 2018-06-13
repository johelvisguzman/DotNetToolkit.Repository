namespace DotNetToolkit.Repository.AdoNet
{
    using Factories;
    using Interceptors;
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetUnitOfWorkFactory(string connectionString, IEnumerable<IRepositoryInterceptor> interceptors = null)
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
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetUnitOfWorkFactory(string providerName, string connectionString, IEnumerable<IRepositoryInterceptor> interceptors = null)
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

        #endregion

        #region Implementation of IUnitOfWorkFactoryAsync

        /// <summary>
        /// Creates a new asynchronous unit of work.
        /// </summary>
        /// <returns>The new asynchronous unit of work.</returns>
        public IUnitOfWorkAsync CreateAsync()
        {
            return string.IsNullOrEmpty(_providerName)
                ? new AdoNetUnitOfWork(_connectionString, _interceptors)
                : new AdoNetUnitOfWork(_providerName, _connectionString, _interceptors);
        }

        #endregion
    }
}
