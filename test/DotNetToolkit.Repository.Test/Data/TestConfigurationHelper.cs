#if NETSTANDARD2_0
namespace DotNetToolkit.Repository.Test.Data
{
    using Microsoft.Extensions.Configuration;
    using System;

    public class TestConfigurationHelper
    {
        public static IConfigurationRoot GetConfiguration(string fileName = "appsettings.json")
        {
            return new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(fileName, optional: false, reloadOnChange: true)
                .Build();
        }
    }
}
#endif