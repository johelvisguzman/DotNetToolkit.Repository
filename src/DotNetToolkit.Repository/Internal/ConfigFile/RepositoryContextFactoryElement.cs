#if !NETSTANDARD1_3

namespace DotNetToolkit.Repository.Internal.ConfigFile
{
    using Factories;

    internal class RepositoryContextFactoryElement : TypedConfigurationElementBase<IRepositoryContextFactory>
    {
    }
}

#endif