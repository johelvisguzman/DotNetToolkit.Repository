namespace DotNetToolkit.Repository.AzureStorageBlob
{
    using Configuration;
    using Microsoft.Azure.Storage.Blob;

    /// <summary>
    /// Represents a repository context for the Microsoft Azure Storage Blob service for storing binary and text data.
    /// </summary>
    /// <seealso cref="IRepositoryContext" />
    public interface IAzureStorageBlobRepositoryContext : IRepositoryContext
    {
        /// <summary>
        /// Gest the cloud blob container.
        /// </summary>
        CloudBlobContainer BlobContainer { get; }
    }
}
