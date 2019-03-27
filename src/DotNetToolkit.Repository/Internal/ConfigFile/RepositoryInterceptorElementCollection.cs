#if !NETSTANDARD1_3

namespace DotNetToolkit.Repository.Internal.ConfigFile
{
    using Configuration.Interceptors;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    /// <summary>
    /// Represents collection of repository interceptors elements.
    /// </summary>
    internal class RepositoryInterceptorElementCollection : ConfigurationElementCollection
    {
        private const string InterceptorKey = "interceptor";

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get { return InterceptorKey; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new RepositoryInterceptorElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RepositoryInterceptorElement)element).TypeName;
        }

        public Dictionary<Type, Func<IRepositoryInterceptor>> GetTypedValues()
        {
            var defaultFactory = ConfigurationProvider.GetDefaultFactory();

            return this
                .Cast<RepositoryInterceptorElement>()
                .ToDictionary(
                    x => x.Type,
                    x => (Func<IRepositoryInterceptor>)x.GetTypedValue);
        }
    }
}

#endif
