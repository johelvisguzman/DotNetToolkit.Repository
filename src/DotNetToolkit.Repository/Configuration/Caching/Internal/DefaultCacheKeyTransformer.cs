namespace DotNetToolkit.Repository.Configuration.Caching.Internal
{
    using JetBrains.Annotations;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="ICacheKeyTransformer" />.
    /// </summary>
    public class DefaultCacheKeyTransformer : ICacheKeyTransformer
    {
        /// <summary>
        /// Transforms the specified key into a custom format.
        /// </summary>
        /// <param name="key">The key to be transformed.</param>
        /// <returns>The transformed key.</returns>
        public string Transform([NotNull] string key)
        {
            return Hasher.ComputeMD5(Guard.NotEmpty(key, nameof(key)));
        }
    }
}
