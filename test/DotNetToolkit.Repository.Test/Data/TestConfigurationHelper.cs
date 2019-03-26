namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using Microsoft.Extensions.Configuration;
    using System;

    public class TestConfigurationHelper
    {
        public static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }
    }
}
