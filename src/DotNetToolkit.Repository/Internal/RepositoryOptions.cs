namespace DotNetToolkit.Repository.Internal
{
    using Configuration;
    using Configuration.Caching;
    using Configuration.Conventions;
    using Configuration.Interceptors;
    using JetBrains.Annotations;
    using Configuration.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="IRepositoryOptions" />.
    /// </summary>
    internal class RepositoryOptions : IRepositoryOptions
    {
        #region Fields

        private Dictionary<Type, Lazy<IRepositoryInterceptor>> _interceptors = new Dictionary<Type, Lazy<IRepositoryInterceptor>>();
        private IRepositoryContextFactory _contextFactory;
        private ILoggerProvider _loggerProvider;
        private ICacheProvider _cachingProvider;
        private IRepositoryConventions _conventions;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the configured interceptors.
        /// </summary>
        public IReadOnlyDictionary<Type, Lazy<IRepositoryInterceptor>> Interceptors { get { return _interceptors; } }

        /// <summary>
        /// Gets the configured logger provider.
        /// </summary>
        public ILoggerProvider LoggerProvider { get { return _loggerProvider; } }

        /// <summary>
        /// Gets the configured caching provider.
        /// </summary>
        public ICacheProvider CachingProvider { get { return _cachingProvider; } }

        /// <summary>
        /// Gets the configured internal context factory.
        /// </summary>
        public IRepositoryContextFactory ContextFactory { get { return _contextFactory; } }

        /// <summary>
        /// Gets the configured conventions.
        /// </summary>
        public IRepositoryConventions Conventions { get { return _conventions; } }

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
        public RepositoryOptions([NotNull] IRepositoryOptions options)
        {
            Guard.NotNull(options, nameof(options));

            _interceptors = options.Interceptors.ToDictionary(x => x.Key, x => x.Value);
            _cachingProvider = options.CachingProvider;
            _loggerProvider = options.LoggerProvider;
            _contextFactory = options.ContextFactory;
            _conventions = options.Conventions;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clones the current configured options to a new instance.
        /// </summary>
        /// <returns>The new clone instance.</returns>
        public RepositoryOptions Clone() => new RepositoryOptions(this);

        /// <summary>
        /// Returns the option instance with a configured interceptor.
        /// </summary>
        /// <param name="underlyingType">The type of interceptor.</param>
        /// <param name="interceptorFactory">The interceptor factory.</param>
        /// <returns>The same option instance.</returns>
        public RepositoryOptions With([NotNull] Type underlyingType, Func<IRepositoryInterceptor> interceptorFactory)
        {
            Guard.NotNull(underlyingType, nameof(underlyingType));
            Guard.NotNull(interceptorFactory, nameof(interceptorFactory));

            var clone = Clone();

            var lazy = new Lazy<IRepositoryInterceptor>(interceptorFactory);

            if (clone._interceptors.ContainsKey(underlyingType))
                clone._interceptors[underlyingType] = lazy;
            else
                clone._interceptors.Add(underlyingType, lazy);

            return clone;
        }

        /// <summary>
        /// Returns the option instance with a configured context factory.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <returns>The same option instance.</returns>
        public RepositoryOptions With([NotNull] IRepositoryContextFactory contextFactory)
        {
            var clone = Clone();

            clone._contextFactory = Guard.NotNull(contextFactory, nameof(contextFactory));

            return clone;
        }

        /// <summary>
        /// Returns the option instance with a configured logger provider for logging messages within the repository.
        /// </summary>
        /// <param name="loggerProvider">The logger factory.</param>
        /// <returns>The same option instance.</returns>
        public RepositoryOptions With([NotNull] ILoggerProvider loggerProvider)
        {
            var clone = Clone();

            clone._loggerProvider = Guard.NotNull(loggerProvider, nameof(loggerProvider));

            return clone;
        }

        /// <summary>
        /// Returns the option instance with a configured caching provider for caching queries within the repository.
        /// </summary>
        /// <param name="cacheProvider">The caching provider.</param>
        /// <returns>The same option instance.</returns>
        public RepositoryOptions With([NotNull] ICacheProvider cacheProvider)
        {
            var clone = Clone();

            clone._cachingProvider = Guard.NotNull(cacheProvider, nameof(cacheProvider));

            return clone;
        }

        /// <summary>
        /// Returns the option instance with a configured conventions.
        /// </summary>
        /// <param name="conventions">The configurable conventions.</param>
        /// <returns>The same option instance.</returns>
        public RepositoryOptions With([NotNull] IRepositoryConventions conventions)
        {
            var clone = Clone();

            clone._conventions = Guard.NotNull(conventions, nameof(conventions));

            return clone;
        }

        #endregion
    }
}
