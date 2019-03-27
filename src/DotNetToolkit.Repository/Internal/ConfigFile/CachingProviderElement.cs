#if !NETSTANDARD1_3

namespace DotNetToolkit.Repository.Internal.ConfigFile
{
    using Configuration.Caching;
    using System;
    using System.Configuration;

    internal class CachingProviderElement : TypedConfigurationElementBase<ICacheProvider>
    {
        private const string ExpiryKey = "expiry";
        
        [ConfigurationProperty(ExpiryKey, IsRequired = false)]
        public TimeSpan? Expiry
        {
            get => (TimeSpan?)this[ExpiryKey];
            set => this[ExpiryKey] = value;
        }

        public override ICacheProvider GetTypedValue()
        {
            var provider = base.GetTypedValue();

            if (Expiry != null)
                provider.CacheExpiration = Expiry;

            return provider;
        }
    }
}

#endif
