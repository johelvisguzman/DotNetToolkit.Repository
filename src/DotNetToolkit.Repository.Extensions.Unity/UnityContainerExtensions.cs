namespace DotNetToolkit.Repository.Extensions.Unity
{
    using Configuration.Interceptors;
    using Configuration.Logging;
    using Configuration.Options;
    using Configuration.Options.Internal;
    using global::Unity;
    using global::Unity.Lifetime;
    using JetBrains.Annotations;
    using Services;
    using System;
    using System.Linq;
    using System.Reflection;
    using Transactions;
    using Utility;

    /// <summary>
    /// Contains various extension methods for <see cref="IUnityContainer" />
    /// </summary>
    public static class UnityContainerExtensions
    {
        /// <summary>
        /// Register all the repositories services using the specified options builder.
        /// </summary>
        /// <typeparam name="T">Used for scanning the assembly containing the specified type.</typeparam>
        /// <param name="container">The unity container.</param>
        /// <param name="optionsAction">A builder action used to create or modify options for the repositories.</param>
        /// <param name="typelifetimeManager">The type lifetime manager for the service.</param>
        /// <param name="factorylifetimeManager">The factory lifetime manager for the service.</param>
        /// <remarks>
        /// This method will scan for repositories and interceptors from the assemblies that have been loaded into the
        /// execution context of this application domain, and will register them to the container.
        /// </remarks>
        public static void RegisterRepositories<T>([NotNull] this IUnityContainer container, [NotNull] Action<RepositoryOptionsBuilder> optionsAction, ITypeLifetimeManager typelifetimeManager = null, IFactoryLifetimeManager factorylifetimeManager = null)
        {
            RegisterRepositories(container, optionsAction, new[] { typeof(T).GetTypeInfo().Assembly }, typelifetimeManager);
        }

        /// <summary>
        /// Register all the repositories services using the specified options builder.
        /// </summary>
        /// <param name="container">The unity container.</param>
        /// <param name="optionsAction">A builder action used to create or modify options for the repositories.</param>
        /// <param name="typelifetimeManager">The type lifetime manager for the service.</param>
        /// <param name="factorylifetimeManager">The factory lifetime manager for the service.</param>
        /// <remarks>
        /// This method will scan for repositories and interceptors from the assemblies that have been loaded into the
        /// execution context of this application domain, and will register them to the container.
        /// </remarks>
        public static void RegisterRepositories([NotNull] this IUnityContainer container, [NotNull] Action<RepositoryOptionsBuilder> optionsAction, ITypeLifetimeManager typelifetimeManager = null, IFactoryLifetimeManager factorylifetimeManager = null)
        {
            RegisterRepositories(container, optionsAction, AppDomain.CurrentDomain.GetAssemblies(), typelifetimeManager);
        }

        /// <summary>
        /// Register all the repositories services using the specified options builder.
        /// </summary>
        /// <param name="container">The unity container.</param>
        /// <param name="optionsAction">A builder action used to create or modify options for the repositories.</param>
        /// <param name="assembliesToScan">The assemblies to scan.</param>
        /// <param name="typelifetimeManager">The type lifetime manager for the service.</param>
        /// <param name="factorylifetimeManager">The factory lifetime manager for the service.</param>
        /// <remarks>
        /// This method will scan for repositories and interceptors from the specified assemblies collection, and will register them to the container.
        /// </remarks>
        public static void RegisterRepositories([NotNull] this IUnityContainer container, [NotNull] Action<RepositoryOptionsBuilder> optionsAction, [NotNull] Assembly[] assembliesToScan, ITypeLifetimeManager typelifetimeManager = null, IFactoryLifetimeManager factorylifetimeManager = null)
        {
            Guard.NotNull(container, nameof(container));
            Guard.NotNull(optionsAction, nameof(optionsAction));
            Guard.NotEmpty(assembliesToScan, nameof(assembliesToScan));

            var optionsBuilder = new RepositoryOptionsBuilder();

            optionsAction(optionsBuilder);

            var scanResults = AssemblyScanner.FindRepositoriesFromAssemblies(assembliesToScan);

            // Register scanned types
            scanResults.ForEach(scanResult =>
            {
                foreach (var implementationType in scanResult.ImplementationTypes)
                {
                    container.RegisterType(implementationType);
                    
                    if (scanResult.InterfaceType == typeof(IRepositoryInterceptor))
                    {
                        container.RegisterType(scanResult.InterfaceType, implementationType, implementationType.FullName);
                    }
                    else
                    {
                        container.RegisterType(scanResult.InterfaceType, implementationType);
                    }
                }
            });

            // Register options services
            container.RegisterFactory<IRepositoryOptions>(c =>
            {
                var options = new RepositoryOptions(optionsBuilder.Options);

                foreach (var interceptorType in scanResults.OfType<IRepositoryInterceptor>())
                {
                    if (!optionsBuilder.Options.Interceptors.ContainsKey(interceptorType))
                    {
                        options = options.With(interceptorType, () => (IRepositoryInterceptor)c.Resolve(interceptorType));
                    }
                }

                if (optionsBuilder.Options.LoggerProvider == null)
                {
                    var loggerProviderType = scanResults.OfType<ILoggerProvider>().FirstOrDefault();

                    if (loggerProviderType != null)
                    {
                        options = options.With((ILoggerProvider)c.Resolve(loggerProviderType));
                    }
                }

                return options;
            }, factorylifetimeManager);

            // Register resolver
            RepositoryDependencyResolver.SetResolver(type => container.Resolve(type));

            container.RegisterFactory<IRepositoryDependencyResolver>(c => RepositoryDependencyResolver.Current, new ContainerControlledLifetimeManager());
        }
    }
}
