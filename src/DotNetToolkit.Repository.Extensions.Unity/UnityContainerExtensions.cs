namespace DotNetToolkit.Repository.Extensions.Unity
{
    using Configuration.Interceptors;
    using Configuration.Options;
    using Configuration.Options.Internal;
    using Factories;
    using global::Unity;
    using JetBrains.Annotations;
    using Services;
    using System;
    using System.Collections.Generic;
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
        /// <param name="container">The unity container.</param>
        /// <param name="optionsAction">A builder action used to create or modify options for the repositories.</param>
        /// <param name="assembliesToScan">The assemblies to scan.</param>
        /// <remarks>
        /// This method will scan for repositories and interceptors from the specified assemblies collection, and will register them to the container.
        /// </remarks>
        public static void RegisterRepositories([NotNull] this IUnityContainer container, [NotNull] Action<RepositoryOptionsBuilder> optionsAction, [NotNull] params Assembly[] assembliesToScan)
        {
            Guard.NotNull(container, nameof(container));
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
                        if (container.IsRegistered(implementationType) || optionsBuilder.Options.Interceptors.ContainsKey(implementationType))
                            continue;

                        container.RegisterType(implementationType, implementationType);
                        container.RegisterType(serviceType, implementationType, implementationType.FullName);
                        registeredInterceptorTypes.Add(implementationType);
                    }
                    else
                    {
                        container.RegisterType(serviceType, implementationType);
                    }
                }
            }

            // Register other services
            container.RegisterFactory<IRepositoryFactory>(c => new RepositoryFactory());
            container.RegisterFactory<IUnitOfWork>(c => new UnitOfWork());
            container.RegisterFactory<IUnitOfWorkFactory>(c => new UnitOfWorkFactory());
            container.RegisterFactory<IRepositoryOptions>(c =>
            {
                var options = new RepositoryOptions(optionsBuilder.Options);

                foreach (var interceptorType in registeredInterceptorTypes)
                {
                    options = options.With(interceptorType, () => (IRepositoryInterceptor)c.Resolve(interceptorType));
                }

                return options;
            });

            // Register resolver
            RepositoryDependencyResolver.SetResolver(type => container.Resolve(type));
        }

        /// <summary>
        /// Register all the repositories services using the specified options builder.
        /// </summary>
        /// <param name="container">The unity container.</param>
        /// <param name="optionsAction">A builder action used to create or modify options for the repositories.</param>
        /// <remarks>
        /// This method will scan for repositories and interceptors from the assemblies that have been loaded into the
        /// execution context of this application domain, and will register them to the container.
        /// </remarks>
        public static void RegisterRepositories([NotNull] this IUnityContainer container, [NotNull] Action<RepositoryOptionsBuilder> optionsAction)
        {
            RegisterRepositories(container, optionsAction, AppDomain.CurrentDomain.GetAssemblies());
        }
    }
}
