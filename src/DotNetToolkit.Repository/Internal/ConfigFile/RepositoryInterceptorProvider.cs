#if !NETSTANDARD1_3

namespace DotNetToolkit.Repository.Internal.ConfigFile
{
    using System;

    /// <summary>
    /// Represents a repository interceptor provider for providing a factory for constructing a repository interceptor from the App.config file.
    /// </summary>
    public sealed class RepositoryInterceptorProvider
    {
        private static Func<Type, object> _factory;

        /// <summary>
        /// Sets a default factory for constructing a repository interceptor.
        /// </summary>
        public static void SetDefaultFactory(Func<Type, object> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            _factory = factory;
        }

        /// <summary>
        /// Gets a default factory for constructing a repository interceptor.
        /// </summary>
        public static Func<Type, object> GetDefaultFactory()
        {
            return _factory;
        }
    }
}

#endif
