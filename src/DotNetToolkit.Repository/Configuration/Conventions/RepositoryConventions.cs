namespace DotNetToolkit.Repository.Configuration.Conventions
{
    using Internal;
    using JetBrains.Annotations;
    using System;
    using System.Reflection;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="IRepositoryConventions" />.
    /// </summary>
    public sealed class RepositoryConventions : IRepositoryConventions
    {
        /// <summary>
        /// Gets or sets a callback function for getting a collection of primary keys for the specified type.
        /// </summary>
        public Func<Type, PropertyInfo[]> PrimaryKeysCallback { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryConventions"/> class.
        /// </summary>
        public RepositoryConventions() { }

        /// <summary>
        /// Gets the default conventions.
        /// </summary>
        public static IRepositoryConventions Default()
        {
            var conventions = new RepositoryConventions
            {
                PrimaryKeysCallback = pi => PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(pi)
            };

            return conventions;
        }

        /// <summary>
        /// Combines the source conventions into the target.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        public static void Combine<T>([NotNull] T source, [NotNull] T target) where T : IRepositoryConventions
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotNull(target, nameof(target));

            if (source.PrimaryKeysCallback != null)
            {
                target.PrimaryKeysCallback = source.PrimaryKeysCallback;
            }
        }
    }
}
