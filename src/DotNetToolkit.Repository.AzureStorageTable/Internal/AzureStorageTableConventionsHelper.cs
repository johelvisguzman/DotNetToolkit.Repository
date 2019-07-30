namespace DotNetToolkit.Repository.AzureStorageTable.Internal
{
    using Microsoft.Azure.CosmosDB.Table;
    using System;
    using System.Reflection;
    using Utility;

    internal static class AzureStorageTableConventionsHelper
    {
        public static PropertyInfo[] GetPrimaryKeyPropertyInfos(Type entityType)
        {
            Guard.NotNull(entityType, nameof(entityType));

            var partitionKey = entityType.GetProperty(nameof(ITableEntity.PartitionKey));
            var rowKey = entityType.GetProperty(nameof(ITableEntity.RowKey));

            return new[] { partitionKey, rowKey };
        }
    }
}
