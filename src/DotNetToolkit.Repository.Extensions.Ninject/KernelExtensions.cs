namespace DotNetToolkit.Repository.Extensions.Ninject
{
    using Configuration.Interceptors;
    using Configuration.Options;
    using Configuration.Options.Internal;
    using Extensions.Internal;
    using Factories;
    using global::Ninject;
    using JetBrains.Annotations;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Transactions;
    using Utility;

    /// <summary>
    /// Contains various extension methods for <see cref="IKernel" />
    /// </summary>
    public static class KernelExtensions
    {
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

            // Binds the repositories and interceptors that have been scanned
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
                        if (kernel.GetBindings(implementationType).Any() || optionsBuilder.Options.Interceptors.ContainsKey(implementationType))
                            continue;

                        kernel.Bind(implementationType).ToSelf();
                        kernel.Bind(serviceType).To(implementationType);
                        registeredInterceptorTypes.Add(implementationType);
                    }
                    else
                    {
                        kernel.Bind(serviceType).To(implementationType);
                    }
                }
            }

            // Binds other services
            kernel.Bind<IRepositoryFactory>().ToMethod(c => new RepositoryFactory(c.Kernel.Get<IRepositoryOptions>()));
            kernel.Bind<IUnitOfWork>().ToMethod(c => new UnitOfWork(c.Kernel.Get<IRepositoryOptions>()));
            kernel.Bind<IUnitOfWorkFactory>().ToMethod(c => new UnitOfWorkFactory(c.Kernel.Get<IRepositoryOptions>()));
            kernel.Bind<IRepositoryOptions>().ToMethod(c =>
            {
                var options = new RepositoryOptions(optionsBuilder.Options);

                foreach (var interceptorType in registeredInterceptorTypes)
                {
                    options = options.With(interceptorType, () => (IRepositoryInterceptor)c.Kernel.Get(interceptorType));
                }

                return options;
            });

            // Binds resolver
            RepositoryDependencyResolver.SetResolver(type => kernel.Get(type));

            kernel.Bind<IRepositoryDependencyResolver>().ToMethod(c => RepositoryDependencyResolver.Current);
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
        public static void RegisterRepositories([NotNull] this IKernel kernel, [NotNull] Action<RepositoryOptionsBuilder> optionsAction)
        {
            BindRepositories(kernel, optionsAction, AppDomain.CurrentDomain.GetAssemblies());
        }
    }
}
