#if !NETSTANDARD

namespace DotNetToolkit.Repository.Internal.ConfigFile
{
    using System.Configuration;

    /// <summary>
    /// Represents a repository interceptor element.
    /// </summary>
    internal class RepositoryInterceptorElement : ConfigurationElement
    {
        private const string TypeKey = "type";
        private const string ParametersKey = "parameters";

        [ConfigurationProperty(TypeKey, IsRequired = true)]
        public string Type
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
    }
}

#endif