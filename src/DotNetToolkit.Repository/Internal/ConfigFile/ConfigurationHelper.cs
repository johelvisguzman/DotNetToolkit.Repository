#if !NETSTANDARD

namespace DotNetToolkit.Repository.Internal.ConfigFile
{
    using Configuration;
    using System.Configuration;

    internal static class ConfigurationHelper
    {
        public static IRepositoryConfigurationOptions GetRequiredConfigurationOptions()
        {
            return (IRepositoryConfigurationOptions)ConfigurationManager.GetSection(ConfigurationSection.SectionName);
        }
    }
}

#endif