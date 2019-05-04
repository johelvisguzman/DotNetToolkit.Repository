namespace DotNetToolkit.Repository.Internal.ConfigFile
{
    using JetBrains.Annotations;
    using System;
    using Utility;

    /// <summary>
    /// Represents configuration provider for providing a factory for constructing the elements from the config file.
    /// </summary>
    public class ConfigurationProvider
    {
        private static Func<Type, object> _factory;

        /// <summary>
        /// Sets a default factory for constructing the elements from the config file.
        /// </summary>
        public static void SetDefaultFactory([NotNull] Func<Type, object> factory)
        {
            _factory = Guard.NotNull(factory, nameof(factory));
        }

        /// <summary>
        /// Gets a default factory for constructing the elements from the config file.
        /// </summary>
        public static Func<Type, object> GetDefaultFactory()
        {
            return _factory;
        }
    }
}
