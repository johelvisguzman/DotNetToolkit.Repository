namespace DotNetToolkit.Repository.Configuration.Caching.Internal
{
    using JetBrains.Annotations;
    using Utility;

    internal class DefaultCacheKeyTransformer
    {
        public string Transform([NotNull] string key)
        {
            return Hasher.ComputeMD5(Guard.NotEmpty(key, nameof(key)));
        }
    }
}
