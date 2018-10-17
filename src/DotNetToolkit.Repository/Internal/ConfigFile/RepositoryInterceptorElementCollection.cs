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
            return ((RepositoryInterceptorElement)element).Type;
        }

        public RepositoryInterceptorElement AddInterceptor(string type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var element = (RepositoryInterceptorElement)CreateNewElement();

            base.BaseAdd(element);

            element.Type = type;

            return element;
        }

        public IEnumerable<IRepositoryInterceptor> GetTypedValues()
        {
            var defaultFactory = RepositoryInterceptorProvider.GetDefaultFactory();

            return this.Cast<RepositoryInterceptorElement>()
                .Select(x =>
                {
                    var type = Type.GetType(x.Type, throwOnError: true);

                    if (defaultFactory != null)
                        return (IRepositoryInterceptor)defaultFactory(type);

                    var args = x.Parameters.GetTypedParameterValues();

                    return (IRepositoryInterceptor)Activator.CreateInstance(type, args);
                })
                .ToList();
        }
    }
}

#endif
