#if !NETSTANDARD

namespace DotNetToolkit.Repository.Internal.ConfigFile
{
    using Configuration;
    using Configuration.Interceptors;
    using Factories;
    using System.Collections.Generic;
    using System.Configuration;

    /// <summary>
    /// Represents a configuration section for configuring repositories from App.config.
    /// </summary>
    internal class ConfigurationSection : System.Configuration.ConfigurationSection, IRepositoryConfigurationOptions
    {
        public const string SectionName = "repository";
        private const string InterceptorsKey = "interceptors";
        private const string DefaultContextFactoryKey = "defaultContextFactory";

        [ConfigurationProperty(InterceptorsKey)]
        public virtual  RepositoryInterceptorElementCollection Interceptors
        {
            get => (RepositoryInterceptorElementCollection)this[InterceptorsKey];
        }

        [ConfigurationProperty(DefaultContextFactoryKey, IsRequired = true)]
        public virtual  RepositoryContextFactoryElement DefaultContextFactory
        {
            get => (RepositoryContextFactoryElement)this[DefaultContextFactoryKey];
        }

        public IRepositoryContextFactory GetContextFactory()
        {
            return DefaultContextFactory.GetTypedValue();
        }

        public IEnumerable<IRepositoryInterceptor> GetInterceptors()
        {
            return Interceptors.GetTypedValues();
        }
    }
}

#endif
