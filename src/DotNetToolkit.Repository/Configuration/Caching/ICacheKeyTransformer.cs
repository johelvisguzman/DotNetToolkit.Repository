namespace DotNetToolkit.Repository.Configuration.Caching
{
    /// <summary>
    /// Represents a transformer which converts a caching key into a custom format.
    /// </summary>
    public interface ICacheKeyTransformer
    {
        /// <summary>
        /// Transforms the specified key into a custom format.
        /// </summary>
        /// <param name="key">The caching key to be transformed.</param>
        /// <returns>The transformed key.</returns>
        string Transform(string key);
    }
}
