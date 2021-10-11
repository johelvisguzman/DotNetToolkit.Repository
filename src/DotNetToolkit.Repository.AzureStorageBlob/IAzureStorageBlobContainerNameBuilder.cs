namespace DotNetToolkit.Repository.AzureStorageBlob
{
    /// <summary>
    /// Builds a container name for the azure storage blob for the specified type.
    /// </summary>
    public interface IAzureStorageBlobContainerNameBuilder
    {
        /// <summary>
        /// Returns a custom container name for the specified type.
        /// </summary>
        string Build<T>();
    }
}
