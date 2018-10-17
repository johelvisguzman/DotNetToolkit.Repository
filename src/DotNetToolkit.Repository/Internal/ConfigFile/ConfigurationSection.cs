#if !NETSTANDARD1_3

namespace DotNetToolkit.Repository.Internal.ConfigFile
{
    using System.Configuration;

    /// <summary>
    /// Represents a configuration section for configuring repositories from App.config.
    /// </summary>
    internal class ConfigurationSection : System.Configuration.ConfigurationSection
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
    }
}

#endif
