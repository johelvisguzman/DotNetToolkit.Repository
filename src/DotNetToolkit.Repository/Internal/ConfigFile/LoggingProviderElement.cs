#if !NETSTANDARD1_3

namespace DotNetToolkit.Repository.Internal.ConfigFile
{
    using Configuration.Logging;
    using System;
    using System.Configuration;

    internal class LoggingProviderElement : ConfigurationElement
    {
        private const string TypeKey = "type";
        private const string ParametersKey = "parameters";

        [ConfigurationProperty(TypeKey, IsKey = true, IsRequired = true)]
        public string TypeName
        {
            get => (string)this[TypeKey];
            set => this[TypeKey] = value;
        }

        [ConfigurationProperty(ParametersKey, IsRequired = false)]
        public ParameterCollection Parameters
        {
            get => (ParameterCollection)this[ParametersKey];
            set => this[ParametersKey] = value;
        }

        public ILoggerProvider GetTypedValue()
        {
            if (string.IsNullOrEmpty(TypeName))
                return null;

            var type = Type.GetType(TypeName, throwOnError: true);
            var args = Parameters.GetTypedParameterValues();

            var defaultFactory = ConfigurationProvider.GetDefaultFactory();

            if (defaultFactory != null)
                return (ILoggerProvider)defaultFactory(type);

            return (ILoggerProvider)Activator.CreateInstance(type, args);
        }
    }
}

#endif
