namespace DotNetToolkit.Repository.AzureStorageBlob.Internal
{
    using Configuration.Conventions.Internal;

    internal class DefaultContainerNameBuilder : IAzureStorageBlobContainerNameBuilder
    {
        public string Build<T>()
        {
            return ModelConventionHelper.GetTableName<T>().ToLower();
        }
    }
}
