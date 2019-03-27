#if !NETSTANDARD1_3

namespace DotNetToolkit.Repository.Internal.ConfigFile
{
    using System;
    using System.Configuration;

    internal abstract class TypedConfigurationElementBase<T> : ConfigurationElement
    {
        private const string TypeKey = "type";
        private const string ParametersKey = "parameters";

        private Type _type;

        [ConfigurationProperty(TypeKey, IsKey = true, IsRequired = true)]
        public string TypeName
        {
            get => (string)this[TypeKey];
            set
            {
                this[TypeKey] = value;

                _type = null;
            }
        }

        public Type Type
        {
            get => _type ?? (_type = Type.GetType(TypeName, throwOnError: true));
        }

        [ConfigurationProperty(ParametersKey, IsRequired = false)]
        public ParameterCollection Parameters
        {
            get => (ParameterCollection)this[ParametersKey];
            set => this[ParametersKey] = value;
        }

        public virtual T GetTypedValue()
        {
            var type = Type;
            var args = Parameters.GetTypedParameterValues();

            var defaultFactory = ConfigurationProvider.GetDefaultFactory();

            if (defaultFactory != null)
                return (T)defaultFactory(type);

            return (T)Activator.CreateInstance(type, args);
        }
    }
}

#endif
