#if !NETSTANDARD1_3

namespace DotNetToolkit.Repository.Internal.ConfigFile
{
    using Extensions;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    internal abstract class TypedConfigurationElementBase<T> : ConfigurationElement
    {
        private const string TypeKey = "type";
        private const string ParametersKey = "parameters";
        private const string KeyValuesKey = "keyValues";

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
            get
            {
                if (_type == null)
                {
                    if (string.IsNullOrEmpty(TypeName))
                        return null;

                    _type = Type.GetType(TypeName, throwOnError: true);
                }

                return _type;
            }
        }

        [ConfigurationProperty(ParametersKey, IsRequired = false)]
        public ParameterCollection Parameters
        {
            get { return (ParameterCollection)this[ParametersKey]; }
        }

        [ConfigurationProperty(KeyValuesKey, IsDefaultCollection = false)]
        public KeyValueConfigurationCollection KeyValues
        {
            get { return (KeyValueConfigurationCollection)this[KeyValuesKey]; }
        }

        public virtual T GetTypedValue()
        {
            var type = Type;

            if (type == null)
                return default(T);

            var defaultFactory = ConfigurationProvider.GetDefaultFactory();

            if (defaultFactory != null)
                return (T)defaultFactory(type);

            var args = GetArguments();

            if (args != null && args.Any())
                return (T)Activator.CreateInstance(type, args);

            return (T)Activator.CreateInstance(type);
        }

        private object[] GetArguments()
        {
            var parameters = Parameters.GetTypedParameterValues();
            if (parameters != null && parameters.Any())
                return parameters;

            var keyValues = KeyValues;
            if (keyValues == null || keyValues.Count == 0)
                return null;

            var type = Type;
            var keys = keyValues.AllKeys;
            var matchedCtorParams = type
                .GetConstructors()
                .Select(x => x.GetParameters())
                .FirstOrDefault(pi => pi
                    .Select(x => x.Name)
                    .OrderBy(x => x)
                    .SequenceEqual(keys.OrderBy(x => x)));

            if (matchedCtorParams == null)
                return null;

            var paramList = new List<object>();

            paramList.AddRange(matchedCtorParams
                .Select(ctorParam => ctorParam.ParameterType.ConvertTo(keyValues[ctorParam.Name].Value)));

            return paramList.ToArray();
        }
    }
}

#endif
