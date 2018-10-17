#if !NETSTANDARD1_3

namespace DotNetToolkit.Repository.Internal.ConfigFile
{
    using Factories;
    using System;
    using System.Configuration;

    /// <summary>
    /// Represents a repository context factory element.
    /// </summary>
    internal class RepositoryContextFactoryElement : ConfigurationElement
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

        public IRepositoryContextFactory GetTypedValue()
        {
            var type = Type.GetType(TypeName, throwOnError: true);
            var args = Parameters.GetTypedParameterValues();

            return (IRepositoryContextFactory)Activator.CreateInstance(type, args);
        }
    }
}

#endif