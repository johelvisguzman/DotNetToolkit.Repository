#if !NETSTANDARD1_3

namespace DotNetToolkit.Repository.Internal.ConfigFile
{
    using Configuration.Logging;

    internal class LoggingProviderElement : TypedConfigurationElementBase<ILoggerProvider>
    {
    }
}

#endif
