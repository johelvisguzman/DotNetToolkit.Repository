namespace DotNetToolkit.Repository.AzureStorageTable
{
    using Configuration;
    using Microsoft.Azure.CosmosDB.Table;

    /// <summary>
    /// Represents a repository context provider for the Microsoft Azure Storage Table service for storing structured NoSQL data in the cloud.
    /// </summary>
    /// <seealso cref="IRepositoryContext" />
    public interface IAzureStorageTableRepositoryContext : IRepositoryContext
    {
        /// <summary>
        /// Gest the cloud Table.
        /// </summary>
        CloudTable Table { get; }
    }
}
