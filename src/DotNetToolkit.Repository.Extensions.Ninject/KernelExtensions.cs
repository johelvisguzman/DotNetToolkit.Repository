namespace DotNetToolkit.Repository.Extensions.Ninject
{
    using Configuration.Caching;
    using Configuration.Interceptors;
    using Configuration.Logging;
    using global::Ninject;
    using JetBrains.Annotations;
    using Repository.Internal;
    using System;
    using System.Linq;
    using System.Reflection;
    using Utility;

    /// <summary>
    /// Contains various extension methods for <see cref="IKernel" />
    /// </summary>
    public static class KernelExtensions
    {
        /// <summary>
        /// Binds all the repositories services using the specified options builder.
        /// </summary>
        /// <typeparam name="T">Used for scanning the assembly containing the specified type.</typeparam>
        /// <param name="kernel">The kernel.</param>
        /// <param name="optionsAction">A builder action used to create or modify options for the repositories.</param>
        /// <remarks>
        /// This method will scan for repositories and interceptors from the assemblies that have been loaded into the
        /// execution context of this application domain, and will bind them to the container.
        /// </remarks>
        public static void BindRepositories<T>([NotNull] this IKernel kernel, [NotNull] Action<RepositoryOptionsBuilder> optionsAction)
        {
            BindRepositories(kernel, optionsAction, new[] { typeof(T).GetTypeInfo().Assembly });
        }

        /// <summary>
        /// Binds all the repositories services using the specified options builder.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="optionsAction">A builder action used to create or modify options for the repositories.</param>
        /// <remarks>
        /// This method will scan for repositories and interceptors from the assemblies that have been loaded into the
        /// execution context of this application domain, and will bind them to the container.
        /// </remarks>
        public static void BindRepositories([NotNull] this IKernel kernel, [NotNull] Action<RepositoryOptionsBuilder> optionsAction)
        {
            BindRepositories(kernel, optionsAction, AppDomain.CurrentDomain.GetAssemblies());
        }

        /// <summary>
        /// Binds all the repositories services using the specified options builder.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="optionsAction">A builder action used to create or modify options for the repositories.</param>
        /// <param name="assembliesToScan">The assemblies to scan.</param>
        /// <remarks>
        /// This method will scan for repositories and interceptors from the specified assemblies collection, and will bind them to the container.
        /// </remarks>
        public static void BindRepositories([NotNull] this IKernel kernel, [NotNull] Action<RepositoryOptionsBuilder> optionsAction, [NotNull] params Assembly[] assembliesToScan)
        {
            Guard.NotNull(kernel, nameof(kernel));
            Guard.NotNull(optionsAction, nameof(optionsAction));
            Guard.NotEmpty(assembliesToScan, nameof(assembliesToScan));

            var optionsBuilder = new RepositoryOptionsBuilder();

            optionsAction(optionsBuilder);

            var scanResults = AssemblyScanner.FindRepositoriesFromAssemblies(assembliesToScan);

            // Binds scanned types
            scanResults.ForEach(scanResult =>
            {
                foreach (var implementationType in scanResult.ImplementationTypes)
                {
                    kernel.Bind(implementationType).ToSelf();
                    kernel.Bind(scanResult.InterfaceType).To(implementationType);
                }
            });

            // Binds options services
            kernel.Bind<IRepositoryOptions>().ToMethod(c =>
            {
                var options = new RepositoryOptions(optionsBuilder.Options);

                foreach (var interceptorType in scanResults.OfType<IRepositoryInterceptor>())
                {
                    if (!options.Interceptors.ContainsKey(interceptorType))
                    {
                        options = options.With(interceptorType, () => (IRepositoryInterceptor)c.Kernel.Get(interceptorType));
                    }
                }

                if (options.LoggerProvider == null)
                {
                    var loggerProviderType = scanResults.OfType<ILoggerProvider>().FirstOrDefault();

                    if (loggerProviderType != null)
                    {
                        options = options.With((ILoggerProvider)c.Kernel.Get(loggerProviderType));
                    }
                }

                if (options.CachingProvider == null)
                {
                    var cacheProviderType = scanResults.OfType<ICacheProvider>().FirstOrDefault();

                    if (cacheProviderType != null)
                    {
                        options = options.With((ICacheProvider)c.Kernel.Get(cacheProviderType));
                    }
                }

                return options;
            });

            // Binds resolver
            RepositoryDependencyResolver.SetResolver(type => kernel.Get(type));

            kernel.Bind<IRepositoryDependencyResolver>().ToMethod(c => RepositoryDependencyResolver.Current);
        }
    }
}
