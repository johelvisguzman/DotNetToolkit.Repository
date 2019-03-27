﻿#if !NETSTANDARD1_3

namespace DotNetToolkit.Repository.Internal.ConfigFile
{
    using Configuration.Mapper;
    using System;
    using System.Configuration;

    internal class MappingProviderElement : ConfigurationElement
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

        public IMapperProvider GetTypedValue()
        {
            if (string.IsNullOrEmpty(TypeName))
                return null;

            var type = Type.GetType(TypeName, throwOnError: true);
            var args = Parameters.GetTypedParameterValues();

            var defaultFactory = ConfigurationProvider.GetDefaultFactory();

            if (defaultFactory != null)
                return (IMapperProvider)defaultFactory(type);

            return (IMapperProvider)Activator.CreateInstance(type, args);
        }
    }
}

#endif
