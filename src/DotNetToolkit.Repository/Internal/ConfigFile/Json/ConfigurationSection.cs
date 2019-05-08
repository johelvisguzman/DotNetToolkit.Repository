namespace DotNetToolkit.Repository.Internal.ConfigFile.Json
{
    using Configuration.Caching;
    using Configuration.Interceptors;
    using Configuration.Logging;
    using Configuration.Mapper;
    using Extensions;
    using Factories;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Utility;

    /// <summary>
    /// Represents a configuration section handler for configuring repositories from a file.
    /// </summary>
    internal class ConfigurationSection : ConfigFile.IConfigurationSection
    {
        #region Fields

        public const string SectionName = "repository";

        private const string DefaultContextFactorySectionKey = "defaultContextFactory";
        private const string LoggingProviderSectionKey = "loggingProvider";
        private const string CachingProviderSectionKey = "cachingProvider";
        private const string MappingProviderSectionKey = "mappingProvider";
        private const string InterceptorCollectionSectionKey = "interceptors";
        private const string ParameterCollectionSectionKey = "parameters";
        private const string TypeKey = "type";

        private readonly IConfigurationSection _root;

        #endregion

        #region Constructors

        public ConfigurationSection([NotNull] IConfigurationSection root)
        {
            _root = Guard.NotNull(root, nameof(root));
        }

        #endregion

        #region Public Methods

        public IRepositoryContextFactory GetDefaultContextFactory()
        {
            var section = _root.GetSection(DefaultContextFactorySectionKey);

            if (section != null)
            {
                return GetTypedValue<IRepositoryContextFactory>(section);
            }

            return null;
        }

        public ILoggerProvider GetLoggerProvider()
        {
            var section = _root.GetSection(LoggingProviderSectionKey);

            if (section != null)
            {
                return GetTypedValue<ILoggerProvider>(section);
            }

            return null;
        }

        public ICacheProvider GetCachingProvider()
        {
            var section = _root.GetSection(CachingProviderSectionKey);

            if (section != null)
            {
                var value = GetTypedValue<ICacheProvider>(section);

                return value;
            }

            return null;
        }

        public IMapperProvider GetMappingProvider()
        {
            var section = _root.GetSection(MappingProviderSectionKey);

            if (section != null)
            {
                return GetTypedValue<IMapperProvider>(section);
            }

            return null;
        }

        public Dictionary<Type, Func<IRepositoryInterceptor>> GetInterceptors()
        {
            var interceptorsDict = new Dictionary<Type, Func<IRepositoryInterceptor>>();
            var section = _root.GetSection(InterceptorCollectionSectionKey);

            if (section != null)
            {
                foreach (var subSection in section.GetChildren())
                {
                    if (subSection != null)
                    {
                        var type = ExtractType(subSection, isRequired: true);

                        interceptorsDict.Add(type, () => GetTypedValue<IRepositoryInterceptor>(section, type));
                    }
                }
            }

            return interceptorsDict;
        }

        #endregion

        #region Private Methods

        private static Dictionary<string, string> ExtractParameters([NotNull] IConfigurationSection section)
        {
            Guard.NotNull(section, nameof(section));

            var parameterCollectionSection = section.GetSection(ParameterCollectionSectionKey);

            if (parameterCollectionSection != null)
            {
                return parameterCollectionSection
                    .GetChildren()
                    .Select(ExtractKeyValue)
                    .ToDictionary(x => x.Key, x => x.Value);
            }

            return null;
        }

        private static Type ExtractType([NotNull] IConfigurationSection section, bool isRequired)
        {
            Guard.NotNull(section, nameof(section));

            var value = Extract(section, TypeKey, isRequired);

            if (string.IsNullOrEmpty(value))
                value = "System.String";

            return Type.GetType(value, throwOnError: true);
        }

        private static KeyValuePair<string, string> ExtractKeyValue([NotNull] IConfigurationSection section)
        {
            Guard.NotNull(section, nameof(section));

            var key = section.Key;
            var value = section.Value;

            if (string.IsNullOrEmpty(key))
                throw new InvalidOperationException($"The key is missing for '{section.Path}' section.");

            if (string.IsNullOrEmpty(value))
                throw new InvalidOperationException($"The value for '{key}' key is missing for '{section.Path}' section.");

            return new KeyValuePair<string, string>(key, value);
        }

        private static string Extract([NotNull] IConfigurationSection section, [NotNull] string key, bool isRequired = true)
        {
            Guard.NotNull(section, nameof(section));
            Guard.NotEmpty(key, nameof(key));

            if (section[key] == null && isRequired)
                throw new InvalidOperationException($"The value for '{key}' key is missing for '{section.Path}' section.");

            return section[key];
        }

        private T GetTypedValue<T>([NotNull] IConfigurationSection section, [CanBeNull] Type type = null)
        {
            Guard.NotNull(section, nameof(section));

            if (type == null)
                type = ExtractType(section, isRequired: true);

            var keyValues = ExtractParameters(section);

            if (!keyValues.Any())
            {
                try
                {
                    return (T)RepositoryDependencyResolver.Current.Resolve(type);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(string.Format(
                            Properties.Resources.UnableToResolveTypeWithDependencyResolver_ConfigFile,
                            type.FullName,
                            typeof(RepositoryDependencyResolver).Name),
                        ex.InnerException);
                }
            }

            return (T)type.InvokeConstructor(keyValues);
        }

        #endregion
    }
}
