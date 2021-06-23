namespace DotNetToolkit.Repository.AzureStorageTable.Internal
{
    using Configuration;
    using Configuration.Conventions;
    using Extensions;
    using Extensions.Internal;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using Microsoft.WindowsAzure.Storage.Table.Queryable;
    using Query;
    using Query.Strategies;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Reflection;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="IAzureStorageTableRepositoryContext" />.
    /// </summary>
    /// <seealso cref="IAzureStorageTableRepositoryContext" />
    internal class AzureStorageTableRepositoryContext : LinqRepositoryContextBase, IAzureStorageTableRepositoryContext
    {
        #region Properties

        /// <summary>
        /// Gest the cloud Table.
        /// </summary>
        public CloudTable Table { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureStorageTableRepositoryContext" /> class.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="createIfNotExists">Creates the container if it does not exist.</param>
        public AzureStorageTableRepositoryContext(string nameOrConnectionString, string tableName = null, bool createIfNotExists = false)
        {
            Guard.NotEmpty(nameOrConnectionString, nameof(nameOrConnectionString));

            ConfigureConventions();

            var css = GetConnectionStringSettings(nameOrConnectionString);

            var connectionString = css != null
                ? css.ConnectionString
                : nameOrConnectionString;

            var account = CloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudTableClient();

            if (string.IsNullOrEmpty(tableName))
                tableName = GetType().Name;

            Table = client.GetTableReference(tableName);

            if (createIfNotExists)
                Table.CreateIfNotExists();
        }

        #endregion

        #region Private Methods

        private void ConfigureConventions()
        {
            Conventions = new RepositoryConventions()
            {
                PrimaryKeysCallback = type => AzureStorageTableConventionsHelper.GetPrimaryKeyPropertyInfos(type)
            };
        }

        private IEnumerable<TElement> ExecuteTableQuery<TElement>(TableQuery<TElement> tableQuery)
        {
            var nextQuery = tableQuery;
            var continuationToken = default(TableContinuationToken);
            var results = new List<TElement>();

            do
            {
                var queryResult = nextQuery.ExecuteSegmented(continuationToken);

                results.Capacity += queryResult.Results.Count;
                results.AddRange(queryResult.Results);

                continuationToken = queryResult.ContinuationToken;

                if (continuationToken != null && tableQuery.TakeCount.HasValue)
                {
                    var itemsToLoad = tableQuery.TakeCount.Value - results.Count;

                    nextQuery = itemsToLoad > 0
                        ? tableQuery.Take<TElement>(itemsToLoad).AsTableQuery()
                        : null;
                }

            } while (continuationToken != null && nextQuery != null);

            return results;
        }

        private TableQuery<TEntity> InvokeCreateQuery<TEntity>()
        {
            return (TableQuery<TEntity>)typeof(CloudTable)
                .GetRuntimeMethods()
                .Single(x => x.Name == nameof(CloudTable.CreateQuery) &&
                             x.IsGenericMethodDefinition &&
                             x.GetGenericArguments().Length == 1 &&
                             x.GetParameters().Length == 0)
                .MakeGenericMethod(typeof(TEntity))
                .Invoke(Table, null);
        }

        private TableOperation InvokeRetrieve<TEntity>(string partitionKey, string rowKey)
        {
            return (TableOperation)typeof(TableOperation)
                .GetRuntimeMethods()
                .Single(x => x.Name == nameof(TableOperation.Retrieve) &&
                             x.IsGenericMethodDefinition &&
                             x.GetGenericArguments().Length == 1 &&
                             x.GetParameters().Length == 3)
                .MakeGenericMethod(typeof(TEntity))
                .Invoke(Table, new[] { partitionKey, rowKey, null });
        }

        private static void ThrowsIfNotTableEntity<TEntity>()
        {
            if (!typeof(TEntity).ImplementsInterface(typeof(ITableEntity)))
                throw new InvalidOperationException($"The specified '{typeof(TEntity).FullName}' type must implement '{typeof(ITableEntity).FullName}.'");
        }

        private static ConnectionStringSettings GetConnectionStringSettings(string nameOrConnectionString)
        {
            var css = ConfigurationManager.ConnectionStrings[nameOrConnectionString];

            if (css != null)
                return css;

            for (var i = 0; i < ConfigurationManager.ConnectionStrings.Count; i++)
            {
                css = ConfigurationManager.ConnectionStrings[i];

                if (css.ConnectionString.Equals(nameOrConnectionString))
                    return css;
            }

            return null;
        }

        #endregion

        #region Overrides of LinqRepositoryContextBase

        /// <summary>
        /// Returns the entity's query.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <returns>The entity's query.</returns>
        protected override IQueryable<TEntity> AsQueryable<TEntity>()
        {
            ThrowsIfNotTableEntity<TEntity>();

            return ExecuteTableQuery<TEntity>(
                InvokeCreateQuery<TEntity>())
                .AsQueryable();
        }

        /// <summary>
        /// Apply a fetching options to the specified entity's query.
        /// </summary>
        /// <returns>The entity's query with the applied options.</returns>
        protected override IQueryable<TEntity> ApplyFetchingOptions<TEntity>(IQueryable<TEntity> query, IQueryOptions<TEntity> options)
        {
            ThrowsIfNotTableEntity<TEntity>();

            if (options?.FetchStrategy?.PropertyPaths?.Any() == true)
                Logger.Debug("The azure storage table context does not support fetching strategy.");

            return query;
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be inserted into the database when <see cref="SaveChanges" /> is called.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public override void Add<TEntity>(TEntity entity)
        {
            Guard.NotNull(entity, nameof(entity));

            ThrowsIfNotTableEntity<TEntity>();

            var operation = TableOperation.InsertOrReplace((ITableEntity)entity);

            Table.Execute(operation);
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be updated in the database when <see cref="SaveChanges" /> is called.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public override void Update<TEntity>(TEntity entity)
        {
            Guard.NotNull(entity, nameof(entity));

            ThrowsIfNotTableEntity<TEntity>();

            var operation = TableOperation.Replace((ITableEntity)entity);

            Table.Execute(operation);
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be removed from the database when <see cref="SaveChanges" /> is called.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public override void Remove<TEntity>(TEntity entity)
        {
            Guard.NotNull(entity, nameof(entity));

            ThrowsIfNotTableEntity<TEntity>();

            var operation = TableOperation.Delete((ITableEntity)entity);

            Table.Execute(operation);
        }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        public override int SaveChanges()
        {
            return -1;
        }

        /// <summary>
        /// Finds an entity with the given primary key values in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity.</param>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The entity found in the repository.</returns>
        public override TEntity Find<TEntity>(IFetchQueryStrategy<TEntity> fetchStrategy, params object[] keyValues)
        {
            Guard.NotEmpty(keyValues, nameof(keyValues));

            ThrowsIfNotTableEntity<TEntity>();

            string partitionKey = string.Empty;
            string rowKey;

            if (keyValues.Length == 1)
            {
                rowKey = keyValues[0].ToString();
            }
            else
            {
                partitionKey = keyValues[0].ToString();
                rowKey = keyValues[1].ToString();
            }

            var opertation = InvokeRetrieve<TEntity>(partitionKey, rowKey);
            var tableResult = Table.Execute(opertation);

            return tableResult.Result as TEntity;
        }

        #endregion
    }
}
