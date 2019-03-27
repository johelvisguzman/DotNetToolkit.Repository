#if !NETSTANDARD1_3

namespace DotNetToolkit.Repository.Internal.ConfigFile
{
    using Configuration.Interceptors;

    internal class RepositoryInterceptorElement : TypedConfigurationElementBase<IRepositoryInterceptor>
    {
    }
}

#endif