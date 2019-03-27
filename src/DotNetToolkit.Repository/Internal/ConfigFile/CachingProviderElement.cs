#if !NETSTANDARD1_3

namespace DotNetToolkit.Repository.Internal.ConfigFile
{
    using Configuration.Caching;
    using System;
    using System.Configuration;

    internal class CachingProviderElement : ConfigurationElement
    {
        private const string TypeKey = "type";
        private const string ExpiryKey = "expiry";
        private const string ParametersKey = "parameters";

        [ConfigurationProperty(TypeKey, IsKey = true, IsRequired = true)]
        public string TypeName
        {
            get => (string)this[TypeKey];
            set => this[TypeKey] = value;
        }

        [ConfigurationProperty(ExpiryKey, IsRequired = false)]
        public TimeSpan? Expiry
        {
            get => (TimeSpan?)this[ExpiryKey];
            set => this[ExpiryKey] = value;
        }

        [ConfigurationProperty(ParametersKey, IsRequired = false)]
        public ParameterCollection Parameters
        {
            get => (ParameterCollection)this[ParametersKey];
            set => this[ParametersKey] = value;
        }

        public ICacheProvider GetTypedValue()
        {
            if (string.IsNullOrEmpty(TypeName))
                return null;

            var type = Type.GetType(TypeName, throwOnError: true);
            var args = Parameters.GetTypedParameterValues();

            var provider = (ICacheProvider)Activator.CreateInstance(type, args);

            if (Expiry != null)
                provider.CacheExpiration = Expiry;

            return provider;
        }
    }
}

#endif
