#if !NETSTANDARD1_3

namespace DotNetToolkit.Repository.Internal.ConfigFile
{
    using Configuration;
    using Configuration.Caching;
    using Configuration.Interceptors;
    using Configuration.Logging;
    using Extensions.Internal;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    /// <summary>
    /// Represents a configuration section for configuring repositories from App.config.
    /// </summary>
    internal class ConfigurationSection : System.Configuration.ConfigurationSection, ConfigFile.IConfigurationSection
    {
        #region Fields

        public const string SectionName = "repository";

        private const string InterceptorsKey = "interceptors";
        private const string DefaultContextFactoryKey = "defaultContextFactory";
        private const string LoggingProviderKey = "loggingProvider";
        private const string CachingProviderKey = "cachingProvider";
        private const string MappingProviderKey = "mappingProvider";

        #endregion

        #region Properties

        [ConfigurationProperty(InterceptorsKey)]
        public virtual RepositoryInterceptorElementCollection Interceptors
        {
            get => (RepositoryInterceptorElementCollection)this[InterceptorsKey];
        }

        [ConfigurationProperty(DefaultContextFactoryKey, IsRequired = false)]
        public virtual RepositoryContextFactoryElement DefaultContextFactory
        {
            get => (RepositoryContextFactoryElement)this[DefaultContextFactoryKey];
        }

        [ConfigurationProperty(LoggingProviderKey, IsRequired = false)]
        public virtual LoggingProviderElement LoggingProvider
        {
            get => (LoggingProviderElement)this[LoggingProviderKey];
        }

        [ConfigurationProperty(CachingProviderKey, IsRequired = false)]
        public virtual CachingProviderElement CachingProvider
        {
            get => (CachingProviderElement)this[CachingProviderKey];
        }

        #endregion

        #region Implementation of IConfigurationSection

        public IRepositoryContextFactory GetDefaultContextFactory()
        {
            return DefaultContextFactory.GetTypedValue();
        }

        public ILoggerProvider GetLoggerProvider()
        {
            return LoggingProvider.GetTypedValue();
        }

        public ICacheProvider GetCachingProvider()
        {
            return CachingProvider.GetTypedValue();
        }

        public IReadOnlyDictionary<Type, Func<IRepositoryInterceptor>> GetInterceptors()
        {
            return Interceptors.GetTypedValues();
        }

        #endregion
    }

    class LoggingProviderElement : TypedConfigurationElementBase<ILoggerProvider> { }

    class CachingProviderElement : TypedConfigurationElementBase<ICacheProvider> { }

    class RepositoryContextFactoryElement : TypedConfigurationElementBase<IRepositoryContextFactory> { }

    class RepositoryInterceptorElement : TypedConfigurationElementBase<IRepositoryInterceptor> { }

    class RepositoryInterceptorElementCollection : ConfigurationElementCollection
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
            return this.Cast<RepositoryInterceptorElement>()
                .ToDictionary(
                    x => x.Type,
                    x => (Func<IRepositoryInterceptor>)x.GetTypedValue);
        }
    }

    class ParameterElement : ConfigurationElement
    {
        private const string ValueKey = "value";
        private const string NameKey = "name";

        [ConfigurationProperty(NameKey, IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this[NameKey]; }
            set { this[NameKey] = value; }
        }

        [ConfigurationProperty(ValueKey, IsRequired = true)]
        public string ValueString
        {
            get { return (string)this[ValueKey]; }
            set { this[ValueKey] = value; }
        }
    }

    class ParameterCollection : ConfigurationElementCollection
    {
        private const string ParameterKey = "parameter";

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ParameterElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ParameterElement)element).Name;
        }

        protected override string ElementName
        {
            get { return ParameterKey; }
        }
    }

    abstract class TypedConfigurationElementBase<T> : ConfigurationElement
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

        [ConfigurationProperty(ParametersKey, IsDefaultCollection = false)]
        public ParameterCollection Parameters
        {
            get { return (ParameterCollection)this[ParametersKey]; }
        }

        public virtual T GetTypedValue()
        {
            var type = Type;

            if (type == null)
                return default(T);

            var keyValues = Parameters
                .Cast<ParameterElement>()
                .ToDictionary(x => x.Name, x => x.ValueString);

            if (!keyValues.Any())
            {
                try
                {
                    return (T)RepositoryDependencyResolver.Current.Resolve(type);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(string.Format(
                            Repository.Properties.Resources.UnableToResolveTypeWithDependencyResolver_ConfigFile,
                            type.FullName,
                            typeof(RepositoryDependencyResolver).Name),
                        ex.InnerException);
                }
            }

            return (T)type.InvokeConstructor(keyValues);
        }
    }
}

#endif
