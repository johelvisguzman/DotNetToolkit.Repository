namespace DotNetToolkit.Repository.Configuration.Options
{
    using Caching;
    using Factories;
    using Interceptors;
    using Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An implementation of <see cref="IRepositoryOptions" />.
    /// </summary>
    public class RepositoryOptions : IRepositoryOptions
    {
        #region Fields

        private Dictionary<Type, Lazy<IRepositoryInterceptor>> _interceptors = new Dictionary<Type, Lazy<IRepositoryInterceptor>>();
        private IRepositoryContextFactory _contextFactory;
        private ILoggerProvider _loggerProvider;
        private ICacheProvider _cachingProvider;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the configured interceptors.
        /// </summary>
        public virtual IReadOnlyDictionary<Type, Lazy<IRepositoryInterceptor>> Interceptors { get { return _interceptors; } }

        /// <summary>
        /// Gets the configured logger provider.
        /// </summary>
        public virtual ILoggerProvider LoggerProvider { get { return _loggerProvider; } }

        /// <summary>
        /// Gets the configured caching provider.
        /// </summary>
        public virtual ICacheProvider CachingProvider { get { return _cachingProvider; } }

        /// <summary>
        /// Gets the configured internal context factory.
        /// </summary>
        public virtual IRepositoryContextFactory ContextFactory { get { return _contextFactory; } }

        /// <summary>
        /// Gets a value indicating whether any options have been configured.
        /// </summary>
        public virtual bool IsConfigured
        {
            get
            {
                return ContextFactory != null ||
                       LoggerProvider != null ||
                       CachingProvider != null ||
                       Interceptors.Any();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryOptions" /> class.
        /// </summary>
        public RepositoryOptions() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryOptions" /> class.
        /// </summary>
        /// <param name="options">The repository options to clone.</param>
        public RepositoryOptions(IRepositoryOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            Map(options, this);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clones the current configured options to a new instance.
        /// </summary>
        /// <returns>The new clone instance.</returns>
        public virtual RepositoryOptions Clone()
        {
            var clone = new RepositoryOptions();

            Map(this, clone);

            return clone;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Returns the option instance with a configured interceptor.
        /// </summary>
        /// <param name="underlyingType">The type of interceptor.</param>
        /// <param name="interceptorFactory">The interceptor factory.</param>
        /// <returns>The same option instance.</returns>
        internal virtual RepositoryOptions With(Type underlyingType, Func<IRepositoryInterceptor> interceptorFactory)
        {
            if (underlyingType == null)
                throw new ArgumentNullException(nameof(underlyingType));

            if (interceptorFactory == null)
                throw new ArgumentNullException(nameof(interceptorFactory));

            var lazy = new Lazy<IRepositoryInterceptor>(interceptorFactory);

            if (_interceptors.ContainsKey(underlyingType))
                _interceptors[underlyingType] = lazy;
            else
                _interceptors.Add(underlyingType, lazy);

            return this;
        }

        /// <summary>
        /// Returns the option instance with a configured context factory.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <returns>The same option instance.</returns>
        internal virtual RepositoryOptions With(IRepositoryContextFactory contextFactory)
        {
            if (contextFactory == null)
                throw new ArgumentNullException(nameof(contextFactory));

            _contextFactory = contextFactory;

            return this;
        }

        /// <summary>
        /// Returns the option instance with a configured logger provider for logging messages within the repository.
        /// </summary>
        /// <param name="loggerProvider">The logger factory.</param>
        /// <returns>The same option instance.</returns>
        internal virtual RepositoryOptions With(ILoggerProvider loggerProvider)
        {
            if (loggerProvider == null)
                throw new ArgumentNullException(nameof(loggerProvider));

            _loggerProvider = loggerProvider;

            return this;
        }

        /// <summary>
        /// Returns the option instance with a configured caching provider for caching queries within the repository.
        /// </summary>
        /// <param name="cacheProvider">The caching provider.</param>
        /// <returns>The same option instance.</returns>
        internal virtual RepositoryOptions With(ICacheProvider cacheProvider)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            _cachingProvider = cacheProvider;

            return this;
        }

        /// <summary>
        /// Determines whether the specified interceptor exists within the collection.
        /// </summary>
        /// <returns><c>true</c> if able to find an interceptor of the specified type; otherwise, <c>false</c>.</returns>
        internal virtual bool ContainsInterceptorOfType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return _interceptors.ContainsKey(type);
        }
        
        #endregion

        #region Private Methods

        private void Map(IRepositoryOptions source, RepositoryOptions target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target._interceptors = source.Interceptors.ToDictionary(x => x.Key, x => x.Value);
            target._cachingProvider = source.CachingProvider;
            target._loggerProvider = source.LoggerProvider;
            target._contextFactory = source.ContextFactory;
        }

        #endregion
    }
}
