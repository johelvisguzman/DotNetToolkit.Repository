namespace DotNetToolkit.Repository.Configuration.Options
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// An implementation of <see cref="IRepositoryOptions" />.
    /// </summary>
    public class RepositoryOptions : IRepositoryOptions
    {
        private Dictionary<Type, IRepositoryOptionsExtensions> _extensions = new Dictionary<Type, IRepositoryOptionsExtensions>();

        /// <summary>
        /// Gets the repository extensions that store the configured options.
        /// </summary>
        public IEnumerable<IRepositoryOptionsExtensions> Extensions { get { return _extensions.Values; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryOptions"/> class.
        /// </summary>
        public RepositoryOptions() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity}"/> class.
        /// </summary>
        /// <param name="options">The options to copy to this instance.</param>
        public RepositoryOptions(RepositoryOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            Map(options, this);
        }

        /// <summary>
        /// Adds the specified extension to the collection.
        /// </summary>
        /// <typeparam name="TExtension">The type of the extension to add.</typeparam>
        /// <param name="extension">The extension.</param>
        public virtual void AddOrUpdateExtension<TExtension>(TExtension extension) where TExtension : class, IRepositoryOptionsExtensions
        {
            if (extension == null)
                throw new ArgumentNullException(nameof(extension));

            var key = typeof(TExtension);

            if (_extensions.ContainsKey(key))
                _extensions[key] = extension;
            else
                _extensions.Add(key, extension);
        }

        /// <summary>
        /// Removes the specified extension to the collection.
        /// </summary>
        /// <typeparam name="TExtension">The type of the extension to remove.</typeparam>
        public virtual bool TryRemoveExtensions<TExtension>() where TExtension : class, IRepositoryOptionsExtensions
        {
            if (_extensions.TryGetValue(typeof(TExtension), out _))
            {
                _extensions.Remove(typeof(TExtension));

                return true;
            }

            var result = false;

            foreach (var extensionType in _extensions.Keys.ToList())
            {
                if (typeof(TExtension).IsAssignableFrom(extensionType))
                {
                    _extensions.Remove(extensionType);

                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the extension of the specified type. Returns null if no extension of the specified type is configured.
        /// </summary>
        /// <typeparam name="TExtension">The type of the extension to get.</typeparam>
        /// <returns>The extension, or null if none was found.</returns>
        public virtual TExtension FindExtension<TExtension>() where TExtension : class, IRepositoryOptionsExtensions
        {
            if (this._extensions.TryGetValue(typeof(TExtension), out var optionsExtension))
                return (TExtension)optionsExtension;

            foreach (var extensionType in _extensions.Keys)
            {
                if (typeof(TExtension).IsAssignableFrom(extensionType))
                    return (TExtension)_extensions[extensionType];
            }

            return default(TExtension);
        }

        /// <summary>
        /// Clones the current configured options to a new instance.
        /// </summary>
        /// <returns>The new clone instance.</returns>
        public RepositoryOptions Clone()
        {
            var clone = new RepositoryOptions();

            Map(this, clone);

            return clone;
        }

        private void Map(RepositoryOptions source, RepositoryOptions target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target._extensions = source._extensions.ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
