namespace DotNetToolkit.Repository.Extensions.Microsoft.DependencyInjection
{
    using Configuration.Interceptors;
    using Configuration.Options;
    using Configuration.Options.Internal;
    using Extensions.Internal;
    using Factories;
    using global::Microsoft.Extensions.DependencyInjection;
    using JetBrains.Annotations;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Transactions;
    using Utility;

    /// <summary>
    /// Contains various extension methods for <see cref="IServiceCollection" />
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all the repository services using the specified options builder.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="optionsAction">A builder action used to create or modify options for the repositories.</param>
        /// <param name="assembliesToScan">The assemblies to scan.</param>
        /// <returns>The same instance of the service collection which has been configured with the repositories.</returns>
        /// <remarks>
        /// This method will scan for repositories and interceptors from the specified assemblies collection, and will register them to the container.
        /// </remarks>
        public static IServiceCollection AddRepositories([NotNull] this IServiceCollection services, [NotNull] Action<RepositoryOptionsBuilder> optionsAction, [NotNull] params Assembly[] assembliesToScan)
        {
            Guard.NotNull(services, nameof(services));
            Guard.NotNull(optionsAction, nameof(optionsAction));
            Guard.NotEmpty(assembliesToScan, nameof(assembliesToScan));

            var baseAssembly = Assembly.Load("DotNetToolkit.Repository");

            var assToScan = !assembliesToScan.Any(x => x.FullName.Equals(baseAssembly.FullName))
                ? assembliesToScan.Concat(new[] { baseAssembly })
                : assembliesToScan;

            var types = assToScan.SelectMany(x => x.GetAccessibleTypes());

            var interfaceTypesToScan = new[]
            {
                typeof(IService<>),
                typeof(IService<,>),
                typeof(IService<,,>),
                typeof(IService<,,,>),
                typeof(IRepository<>),
                typeof(IRepository<,>),
                typeof(IRepository<,,>),
                typeof(IRepository<,,,>),
                typeof(IReadOnlyService<>),
                typeof(IReadOnlyService<,>),
                typeof(IReadOnlyService<,,>),
                typeof(IReadOnlyService<,,,>),
                typeof(IReadOnlyRepository<>),
                typeof(IReadOnlyRepository<,>),
                typeof(IReadOnlyRepository<,,>),
                typeof(IReadOnlyRepository<,,,>),
                typeof(IRepositoryInterceptor)
            };

            // Gets all the interfaces that inherent from IRepository<> or IRepositoryInterceptor
            var interfaceTypes = interfaceTypesToScan
                .SelectMany(interfaceType => types.Where(t => !t.IsClass && t.ImplementsInterface(interfaceType)))
                .Distinct();

            var serviceTypesMapping = interfaceTypes
                .SelectMany(interfaceType => types.Where(t => t.IsClass && !t.IsAbstract && t.ImplementsInterface(interfaceType))
                .GroupBy(t => interfaceType, t => t));

            // Register the repositories and interceptors that have been scanned
            var optionsBuilder = new RepositoryOptionsBuilder();

            optionsAction(optionsBuilder);

            var registeredInterceptorTypes = new List<Type>();

            foreach (var t in serviceTypesMapping)
            {
                var serviceType = t.Key;
                var implementationTypes = t.Where(x => x.IsGenericType == serviceType.IsGenericType &&
                                                       x.GetGenericArguments().Length == serviceType.GetGenericArguments().Length &&
                                                       x.IsVisible && !x.IsAbstract);

                foreach (var implementationType in implementationTypes)
                {
                    if (serviceType == typeof(IRepositoryInterceptor))
                    {
                        if (services.Any(x => x.ServiceType == implementationType) || optionsBuilder.Options.Interceptors.ContainsKey(implementationType))
                            continue;

                        services.AddScoped(implementationType, implementationType);
                        services.AddScoped(serviceType, implementationType);
                        registeredInterceptorTypes.Add(implementationType);
                    }
                    else
                    {
                        services.AddScoped(serviceType, implementationType);
                    }
                }
            }

            // Register other services
            services.AddScoped<IRepositoryFactory, RepositoryFactory>(sp => new RepositoryFactory(sp.GetService<IRepositoryOptions>()));
            services.AddScoped<IUnitOfWork, UnitOfWork>(sp => new UnitOfWork(sp.GetService<IRepositoryOptions>()));
            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>(sp => new UnitOfWorkFactory(sp.GetService<IRepositoryOptions>()));
            services.AddScoped<IRepositoryOptions>(sp =>
            {
                var options = new RepositoryOptions(optionsBuilder.Options);

                foreach (var interceptorType in registeredInterceptorTypes)
                {
                    options = options.With(interceptorType, () => (IRepositoryInterceptor)sp.GetService(interceptorType));
                }

                return options;
            });

            // Register resolver
            RepositoryDependencyResolver.SetResolver(type =>
            {
                return services
                    .BuildServiceProvider()
                    .GetService(type);
            });

            services.AddScoped<IRepositoryDependencyResolver>(sp => RepositoryDependencyResolver.Current);

            return services;
        }

        /// <summary>
        /// Adds all the repository services using the specified options builder.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="optionsAction">A builder action used to create or modify options for the repositories.</param>
        /// <returns>The same instance of the service collection which has been configured with the repositories.</returns>
        /// <remarks>
        /// This method will scan for repositories and interceptors from the assemblies that have been loaded into the
        /// execution context of this application domain, and will register them to the container.
        /// </remarks>
        public static IServiceCollection AddRepositories([NotNull] this IServiceCollection services, [NotNull] Action<RepositoryOptionsBuilder> optionsAction)
        {
            return AddRepositories(services, optionsAction, AppDomain.CurrentDomain.GetAssemblies());
        }
    }
}