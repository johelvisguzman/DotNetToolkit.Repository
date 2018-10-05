﻿#if !NETSTANDARD

namespace DotNetToolkit.Repository.Internal.ConfigFile
{
    using System;
    using System.Configuration;
    using System.Globalization;

    /// <summary>
    /// Represents a parameter to be passed to a method
    /// </summary>
    internal class ParameterElement : ConfigurationElement
    {
        private const string ValueKey = "value";
        private const string TypeKey = "type";

        public ParameterElement(int key)
        {
            Key = key;
        }

        internal int Key { get; private set; }

        [ConfigurationProperty(ValueKey, IsRequired = true)]
        public string ValueString
        {
            get { return (string)this[ValueKey]; }
            set { this[ValueKey] = value; }
        }

        [ConfigurationProperty(TypeKey, DefaultValue = "System.String")]
        public string TypeName
        {
            get { return (string)this[TypeKey]; }
            set { this[TypeKey] = value; }
        }

        public object GetTypedParameterValue()
        {
            var type = Type.GetType(TypeName, throwOnError: true);

            return Convert.ChangeType(ValueString, type, CultureInfo.InvariantCulture);
        }
    }
}

#endif