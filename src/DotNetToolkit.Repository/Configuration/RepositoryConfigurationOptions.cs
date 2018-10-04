namespace DotNetToolkit.Repository.Configuration
{
    using Factories;
    using Interceptors;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class RepositoryConfigurationOptions : IRepositoryConfigurationOptions
    {
        private readonly IRepositoryContextFactory _contextFactory;
        private readonly IEnumerable<IRepositoryInterceptor> _interceptors;

        public RepositoryConfigurationOptions(IRepositoryContextFactory contextFactory) : this(contextFactory, (IEnumerable<IRepositoryInterceptor>)null) { }

        public RepositoryConfigurationOptions(IRepositoryContextFactory contextFactory, IEnumerable<IRepositoryInterceptor> interceptors)
        {
            if (contextFactory == null)
                throw new ArgumentNullException(nameof(contextFactory));

            _contextFactory = contextFactory;
            _interceptors = interceptors ?? Enumerable.Empty<IRepositoryInterceptor>();
        }

        public IRepositoryContextFactory GetContextFactory()
        {
            return _contextFactory;
        }

        public IEnumerable<IRepositoryInterceptor> GetInterceptors()
        {
            return _interceptors;
        }
    }
}
