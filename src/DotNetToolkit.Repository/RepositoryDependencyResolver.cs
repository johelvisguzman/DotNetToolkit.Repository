namespace DotNetToolkit.Repository
{
    using JetBrains.Annotations;
    using System;
    using System.Reflection;
    using Utility;

    /// <summary>
    /// Provides a registration point for dependency resolvers that implement <see cref="IRepositoryDependencyResolver"/>.
    /// </summary>
    public class RepositoryDependencyResolver
    {
        #region Fields

        private static readonly RepositoryDependencyResolver _instance = new RepositoryDependencyResolver();
        private IRepositoryDependencyResolver _current;

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="RepositoryDependencyResolver" /> class from being created.
        /// </summary>
        private RepositoryDependencyResolver()
        {
            InnerSetResolver(new DefaultDependencyResolver());
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current dependency resolver.
        /// If a dependency resolver has not been set with the <see cref="RepositoryDependencyResolver.SetResolver(IRepositoryDependencyResolver)"/> method, then a default dependency resolver will be used instead.
        /// </summary>
        public static IRepositoryDependencyResolver Current
        {
            get { return _instance._current; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the current dependency resolver.
        /// </summary>
        /// <param name="resolver">The current dependency resolver to set.</param>
        public static void SetResolver([NotNull] IRepositoryDependencyResolver resolver)
        {
            _instance.InnerSetResolver(Guard.NotNull(resolver, nameof(resolver)));
        }

        /// <summary>
        /// Sets the dependency resolver.
        /// </summary>
        /// <param name="getService">A function for resolving a instance for a specified type.</param>
        public static void SetResolver([NotNull] Func<Type, object> getService)
        {
            _instance.InnerSetResolver(Guard.NotNull(getService, nameof(getService)));
        }

        #endregion

        #region Private Methods

        private void InnerSetResolver(IRepositoryDependencyResolver resolver)
        {
            _current = resolver;
        }

        private void InnerSetResolver(Func<Type, object> getService)
        {
            InnerSetResolver(new DelegateDependencyResolver(getService));
        }

        #endregion

        #region Nested type: DefaultDependencyResolver

        /// <summary>
        /// An implementation of <see cref="DefaultDependencyResolver" />.
        /// </summary>
        internal class DefaultDependencyResolver : IRepositoryDependencyResolver
        {
            public T Resolve<T>()
            {
                return (T)Resolve(typeof(T));
            }

            public object Resolve([NotNull] Type type)
            {
                Guard.NotNull(type, nameof(type));

                var typeInfo = type.GetTypeInfo();

                object result;

                Exception innerException = null;

                if (typeInfo.IsInterface || typeInfo.IsAbstract)
                {
                    result = null;
                }
                else
                {
                    try
                    {
                        result = FastActivator.CreateInstance(type);
                    }
                    catch (Exception ex)
                    {
                        innerException = ex;
                        result = null;
                    }
                }

                if (result == null)
                {
                    throw new InvalidOperationException(string.Format(
                            Properties.Resources.UnableToResolveTypeWithDependencyResolver,
                            type.FullName,
                            typeof(RepositoryDependencyResolver).Name),
                        innerException);
                }

                return result;
            }
        }

        #endregion

        #region Nested type: DelegateDependencyResolver

        /// <summary>
        /// An implementation of <see cref="IRepositoryDependencyResolver" />.
        /// </summary>
        private class DelegateDependencyResolver : IRepositoryDependencyResolver
        {
            private readonly Func<Type, object> _getService;

            public DelegateDependencyResolver([NotNull] Func<Type, object> getService)
            {
                _getService = Guard.NotNull(getService, nameof(getService));
            }

            public T Resolve<T>()
            {
                return (T)Resolve(typeof(T));
            }

            public object Resolve([NotNull] Type type)
            {
                return _getService(Guard.NotNull(type, nameof(type)));
            }
        }

        #endregion
    }
}
