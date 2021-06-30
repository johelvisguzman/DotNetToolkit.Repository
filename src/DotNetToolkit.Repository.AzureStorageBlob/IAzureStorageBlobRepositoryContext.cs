namespace DotNetToolkit.Repository.AzureStorageBlob
{
    using Azure.Storage.Blobs;
    using Configuration;

    /// <summary>
    /// Represents a repository context for the Microsoft Azure Storage Blob service for storing binary and text data.
    /// </summary>
    /// <seealso cref="IRepositoryContextAsync" />
    public interface IAzureStorageBlobRepositoryContext : IRepositoryContextAsync
    {
        /// <summary>
        /// Gest the cloud blob container.
        /// </summary>
        BlobContainerClient BlobContainer { get; }
    }
}
