#if !NETSTANDARD1_3

namespace DotNetToolkit.Repository.Internal.ConfigFile
{
    using Configuration.Caching;

    internal class CachingProviderElement : TypedConfigurationElementBase<ICacheProvider>
    {
    }
}

#endif
