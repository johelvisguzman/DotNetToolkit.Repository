namespace DotNetToolkit.Repository.Internal
{
    using Configuration.Interceptors;
    using Configuration.Options;
    using System;
    using System.Collections.Generic;

    internal class RepositoryOptionsCoreExtension : IRepositoryOptionsExtensions
    {
        private readonly Dictionary<Type, Lazy<IRepositoryInterceptor>> _interceptors = new Dictionary<Type, Lazy<IRepositoryInterceptor>>();

        /// <summary>
        /// Gets the configured interceptors.
        /// </summary>
        public IEnumerable<Lazy<IRepositoryInterceptor>> Interceptors { get { return _interceptors.Values; } }

        /// <summary>
        /// Returns the extension instance with a configured interceptor.
        /// </summary>
        /// <param name="underlyingType">The type of interceptor.</param>
        /// <param name="interceptorFactory">The interceptor factory.</param>
        /// <returns>The same extension instance.</returns>
        public RepositoryOptionsCoreExtension WithInterceptor(Type underlyingType, Func<IRepositoryInterceptor> interceptorFactory)
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
        public bool ContainsInterceptorOfType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return _interceptors.ContainsKey(type);
        }
    }
}
