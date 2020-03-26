namespace DotNetToolkit.Repository.Extensions.Microsoft.DependencyInjection
{
    using Configuration.Interceptors;
    using Configuration.Logging;
    using Configuration.Options;
    using Configuration.Options.Internal;
    using global::Microsoft.Extensions.DependencyInjection;
    using JetBrains.Annotations;
    using System;
    using System.Linq;
    using System.Reflection;
    using Utility;

    /// <summary>
    /// Contains various extension methods for <see cref="IServiceCollection" />
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all the repository services using the specified options builder.
        /// </summary>
        /// <typeparam name="T">Used for scanning the assembly containing the specified type.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="optionsAction">A builder action used to create or modify options for the repositories.</param>
        /// <param name="serviceLifetime">The Microsoft.Extensions.DependencyInjection.ServiceLifetime of the service.</param>
        /// <returns>The same instance of the service collection which has been configured with the repositories.</returns>
        /// <remarks>
        /// This method will scan for repositories and interceptors from the assemblies that have been loaded into the
        /// execution context of this application domain, and will register them to the container.
        /// </remarks>
        public static IServiceCollection AddRepositories<T>([NotNull] this IServiceCollection services, [NotNull] Action<RepositoryOptionsBuilder> optionsAction, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            return AddRepositories(services, optionsAction, new[] { typeof(T).GetTypeInfo().Assembly }, serviceLifetime);
        }

        /// <summary>
        /// Adds all the repository services using the specified options builder.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="optionsAction">A builder action used to create or modify options for the repositories.</param>
        /// <param name="serviceLifetime">The Microsoft.Extensions.DependencyInjection.ServiceLifetime of the service.</param>
        /// <returns>The same instance of the service collection which has been configured with the repositories.</returns>
        /// <remarks>
        /// This method will scan for repositories and interceptors from the assemblies that have been loaded into the
        /// execution context of this application domain, and will register them to the container.
        /// </remarks>
        public static IServiceCollection AddRepositories([NotNull] this IServiceCollection services, [NotNull] Action<RepositoryOptionsBuilder> optionsAction, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            return AddRepositories(services, optionsAction, AppDomain.CurrentDomain.GetAssemblies(), serviceLifetime);
        }

        /// <summary>
        /// Adds all the repository services using the specified options builder.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="optionsAction">A builder action used to create or modify options for the repositories.</param>
        /// <param name="assembliesToScan">The assemblies to scan.</param>
        /// <param name="serviceLifetime">The Microsoft.Extensions.DependencyInjection.ServiceLifetime of the service.</param>
        /// <returns>The same instance of the service collection which has been configured with the repositories.</returns>
        /// <remarks>
        /// This method will scan for repositories and interceptors from the specified assemblies collection, and will register them to the container.
        /// </remarks>
        public static IServiceCollection AddRepositories([NotNull] this IServiceCollection services, [NotNull] Action<RepositoryOptionsBuilder> optionsAction, [NotNull] Assembly[] assembliesToScan, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            Guard.NotNull(services, nameof(services));
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
                    // Register as interface
                    services.Add(new ServiceDescriptor(
                        scanResult.InterfaceType,
                        implementationType,
                        serviceLifetime));

                    // Register as self
                    services.Add(new ServiceDescriptor(
                        implementationType,
                        implementationType,
                        serviceLifetime));
                }
            });

            // Register options services
            services.Add(new ServiceDescriptor(
                typeof(IRepositoryOptions),
                sp =>
                {
                    var options = new RepositoryOptions(optionsBuilder.Options);

                    foreach (var interceptorType in scanResults.OfType<IRepositoryInterceptor>())
                    {
                        if (!optionsBuilder.Options.Interceptors.ContainsKey(interceptorType))
                        {
                            options = options.With(interceptorType, () => (IRepositoryInterceptor)sp.GetService(interceptorType));
                        }
                    }

                    if (optionsBuilder.Options.LoggerProvider == null)
                    {
                        var loggerProviderType = scanResults.OfType<ILoggerProvider>().FirstOrDefault();

                        options = options.With((ILoggerProvider)sp.GetService(loggerProviderType));
                    }

                    return options;
                },
                serviceLifetime));

            // Register resolver
            RepositoryDependencyResolver.SetResolver(type => services.BuildServiceProvider().GetService(type));

            services.AddSingleton<IRepositoryDependencyResolver>(sp => RepositoryDependencyResolver.Current);

            return services;
        }
    }
}