namespace DotNetToolkit.Repository.AzureStorageBlob.Internal
{
    using Configuration;
    using Configuration.Conventions;
    using Extensions;
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Blob;
    using Newtonsoft.Json;
    using Query;
    using Query.Strategies;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="IAzureStorageBlobRepositoryContext" />.
    /// </summary>
    /// <seealso cref="IAzureStorageBlobRepositoryContext" />
    internal class AzureStorageBlobRepositoryContext : LinqRepositoryContextBase, IAzureStorageBlobRepositoryContext
    {
        #region Properties

        /// <summary>
        /// Gest the cloud blob container.
        /// </summary>
        public CloudBlobContainer BlobContainer { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureStorageBlobRepositoryContext" /> class.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        /// <param name="container">The name of the container.</param>
        /// <param name="createIfNotExists">Creates the container if it does not exist.</param>
        public AzureStorageBlobRepositoryContext(string nameOrConnectionString, string container = null, bool createIfNotExists = false)
        {
            Guard.NotEmpty(nameOrConnectionString, nameof(nameOrConnectionString));

            Conventions = RepositoryConventions.Default<AzureStorageBlobRepositoryContext>();

            var css = GetConnectionStringSettings(nameOrConnectionString);

            var connectionString = css != null
                ? css.ConnectionString
                : nameOrConnectionString;

            var account = CloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudBlobClient();

            if (string.IsNullOrEmpty(container))
                container = GetType().Name.ToLower();

            BlobContainer = client.GetContainerReference(container);

            if (createIfNotExists)
                BlobContainer.CreateIfNotExists();
        }

        #endregion

        #region Private Methods

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

        private CloudBlockBlob GetBlobkBlobReference(params object[] keyValues)
        {
            var key = string.Join(":", keyValues);
            var blob = BlobContainer.GetBlockBlobReference(key);

            return blob;
        }

        private CloudBlockBlob GetBlobkBlobReference<TEntity>(TEntity entity) where TEntity : class
        {
            return GetBlobkBlobReference(Conventions.GetPrimaryKeyValues(entity));
        }

        private IEnumerable<CloudBlockBlob> GetBlobs()
        {
            BlobContinuationToken continuationToken = null;

            do
            {
                var response = BlobContainer.ListBlobsSegmented(continuationToken);

                continuationToken = response.ContinuationToken;

                foreach (var blob in response.Results.OfType<CloudBlockBlob>())
                {
                    yield return blob;
                }

            } while (continuationToken != null);
        }

        private static string Serialize(object o)
        {
            if (o == null)
                return null;

            return JsonConvert.SerializeObject(o, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        private static T Deserialize<T>(string v)
        {
            if (string.IsNullOrEmpty(v))
                return default(T);

            return JsonConvert.DeserializeObject<T>(v);
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
            return GetBlobs()
                .Select(x =>
                {
                    try
                    {
                        return Deserialize<TEntity>(x.DownloadText());
                    }
                    catch (StorageException storageException)
                    {
                        if (storageException.RequestInformation.HttpStatusCode == 404)
                            return default(TEntity);

                        throw;
                    }
                })
                .AsQueryable();
        }

        /// <summary>
        /// Apply a fetching options to the specified entity's query.
        /// </summary>
        /// <returns>The entity's query with the applied options.</returns>
        protected override IQueryable<TEntity> ApplyFetchingOptions<TEntity>(IQueryable<TEntity> query, IQueryOptions<TEntity> options)
        {
            if (options?.FetchStrategy?.PropertyPaths?.Any() == true)
                Logger.Debug("The azure storage blob context does not support fetching strategy.");

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

            var blob = GetBlobkBlobReference(entity);

            blob.UploadText(Serialize(entity));
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be updated in the database when <see cref="SaveChanges" /> is called.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public override void Update<TEntity>(TEntity entity)
        {
            Guard.NotNull(entity, nameof(entity));

            var blob = GetBlobkBlobReference(entity);

            blob.UploadText(Serialize(entity));
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be removed from the database when <see cref="SaveChanges" /> is called.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public override void Remove<TEntity>(TEntity entity)
        {
            Guard.NotNull(entity, nameof(entity));

            var blob = GetBlobkBlobReference(entity);

            blob.DeleteIfExists();
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

            try
            {
                var blob = GetBlobkBlobReference(keyValues);

                return Deserialize<TEntity>(blob.DownloadText());
            }
            catch (StorageException storageException)
            {
                if (storageException.RequestInformation.HttpStatusCode == 404)
                    return default(TEntity);

                throw;
            }
        }

        #endregion
    }
}
