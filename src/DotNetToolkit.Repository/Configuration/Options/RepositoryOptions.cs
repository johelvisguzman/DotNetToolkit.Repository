namespace DotNetToolkit.Repository.Configuration.Options
{
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
        private IRepositoryContext _context;
        private ILoggerProvider _loggerProvider;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the configured interceptors.
        /// </summary>
        public virtual IEnumerable<Lazy<IRepositoryInterceptor>> Interceptors { get { return _interceptors.Values; } }

        /// <summary>
        /// Gets the configured logger provider.
        /// </summary>
        public virtual ILoggerProvider LoggerProvider { get { return _loggerProvider; } }

        /// <summary>
        /// Gets the configured internal context factory.
        /// </summary>
        internal virtual IRepositoryContextFactory ContextFactory { get { return _contextFactory; } }

        /// <summary>
        /// Gets the configured internal shared context.
        /// </summary>
        internal virtual IRepositoryContext Context { get { return _context; } }

        /// <summary>
        /// Gets a value indicating whether any options have been configured.
        /// </summary>
        public virtual bool IsConfigured
        {
            get
            {
                return Context != null ||
                       ContextFactory != null ||
                       LoggerProvider != null ||
                       Interceptors.Any();
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Returns the option instance with a configured interceptor.
        /// </summary>
        /// <param name="underlyingType">The type of interceptor.</param>
        /// <param name="interceptorFactory">The interceptor factory.</param>
        /// <returns>The same option instance.</returns>
        internal virtual RepositoryOptions WithInterceptor(Type underlyingType, Func<IRepositoryInterceptor> interceptorFactory)
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
        /// Determines whether the specified interceptor exists within the collection.
        /// </summary>
        /// <returns><c>true</c> if able to find an interceptor of the specified type; otherwise, <c>false</c>.</returns>
        internal virtual bool ContainsInterceptorOfType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return _interceptors.ContainsKey(type);
        }

        /// <summary>
        /// Clones the current configured options to a new instance.
        /// </summary>
        /// <returns>The new clone instance.</returns>
        internal virtual RepositoryOptions Clone()
        {
            var clone = new RepositoryOptions();

            Map(this, clone);

            return clone;
        }

        /// <summary>
        /// Returns the option instance with a configured internal context factory.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <returns>The same option instance.</returns>
        internal virtual RepositoryOptions WithInternalContextFactory(IRepositoryContextFactory contextFactory)
        {
            if (contextFactory == null)
                throw new ArgumentNullException(nameof(contextFactory));

            _contextFactory = contextFactory;

            return this;
        }

        /// <summary>
        /// Returns the option instance with a configured internal shared context (usually setup by the unit of work).
        /// </summary>
        /// <param name="sharedContext">The context.</param>
        /// <returns>The same option instance.</returns>
        internal virtual RepositoryOptions WithInternalSharedContext(IRepositoryContext sharedContext)
        {
            if (sharedContext == null)
                throw new ArgumentNullException(nameof(sharedContext));

            _context = sharedContext;

            return this;
        }

        /// <summary>
        /// Returns the option instance with a configured logger provider for logging messages within the repository.
        /// </summary>
        /// <param name="loggerProvider">The logger factory.</param>
        /// <returns>The same option instance.</returns>
        internal virtual RepositoryOptions WithLoggerProvider(ILoggerProvider loggerProvider)
        {
            if (loggerProvider == null)
                throw new ArgumentNullException(nameof(loggerProvider));

            _loggerProvider = loggerProvider;

            return this;
        }

        #endregion

        #region Private Methods

        private void Map(RepositoryOptions source, RepositoryOptions target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target._interceptors = source._interceptors.ToDictionary(x => x.Key, x => x.Value);
            target._loggerProvider = source._loggerProvider;
            target._contextFactory = source._contextFactory;
            target._context = source._context;
        }

        #endregion
    }
}
