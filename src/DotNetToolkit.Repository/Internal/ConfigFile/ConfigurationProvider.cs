namespace DotNetToolkit.Repository.Internal.ConfigFile
{
    using System;

    /// <summary>
    /// Represents configuration provider for providing a factory for constructing the elements from the config file.
    /// </summary>
    public class ConfigurationProvider
    {
        private static Func<Type, object> _factory;

        /// <summary>
        /// Sets a default factory for constructing the elements from the config file.
        /// </summary>
        public static void SetDefaultFactory(Func<Type, object> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            _factory = factory;
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
