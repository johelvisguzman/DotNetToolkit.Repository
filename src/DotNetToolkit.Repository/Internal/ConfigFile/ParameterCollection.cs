#if !NETSTANDARD1_3

namespace DotNetToolkit.Repository.Internal.ConfigFile
{
    using System.Configuration;
    using System.Linq;

    /// <summary>
    /// Represents a collection of parameter elements.
    /// </summary>
    internal class ParameterCollection : ConfigurationElementCollection
    {
        private const string ParameterKey = "parameter";
        private int _nextKey;

        protected override ConfigurationElement CreateNewElement()
        {
            var element = new ParameterElement(_nextKey);
            _nextKey++;
            return element;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ParameterElement)element).Key;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get { return ParameterKey; }
        }

        public virtual object[] GetTypedParameterValues()
        {
            return this.Cast<ParameterElement>()
                .Select(e => e.GetTypedParameterValue())
                .ToArray();
        }

        internal ParameterElement NewElement()
        {
            var element = CreateNewElement();
            base.BaseAdd(element);
            return (ParameterElement)element;
        }
    }
}

#endif