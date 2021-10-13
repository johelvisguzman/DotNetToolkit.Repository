namespace DotNetToolkit.Repository.AzureStorageBlob.Internal
{
    using Azure;
    using Azure.Storage.Blobs;
    using Configuration;
    using Extensions;
    using Newtonsoft.Json;
    using Properties;
    using Query;
    using Query.Strategies;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Utility;

    internal class AzureStorageBlobRepositoryContext : LinqEnumerableRepositoryContextBase, IAzureStorageBlobRepositoryContext
    {
        #region Fields

        private readonly IAzureStorageBlobContainerNameBuilder _containerNameBuilder;
        private readonly bool _createContainerIfNotExists;
        private readonly JsonSerializerSettings _serializerSettings;

        #endregion

        #region Constructors

        public AzureStorageBlobRepositoryContext(string connectionString, IAzureStorageBlobContainerNameBuilder containerNameBuilder = null, bool createIfNotExists = false, JsonSerializerSettings serializerSettings = null)
        {
            Guard.NotEmpty(connectionString, nameof(connectionString));

            _containerNameBuilder = containerNameBuilder ?? new DefaultContainerNameBuilder();
            _createContainerIfNotExists = createIfNotExists;
            _serializerSettings = serializerSettings;

            Client = new BlobServiceClient(connectionString);
        }

        #endregion

        #region Private Methods

        private JsonSerializer GetJsonSerializer()
        {
            var settings = _serializerSettings ?? new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultJsonSerializeContractResolver(),
                PreserveReferencesHandling = PreserveReferencesHandling.None
            };

            return JsonSerializer.Create(settings);
        }

        private Stream Serialize<TEntity>(TEntity entity)
        {
            var serializer = GetJsonSerializer();
            var stream = new MemoryStream();

            using (var sw = new StreamWriter(stream: stream, encoding: Encoding.UTF8, bufferSize: 4096, leaveOpen: true))
            using (var jsonTextWriter = new JsonTextWriter(sw))
            {
                serializer.Serialize(jsonTextWriter, entity);

                sw.Flush();
                stream.Seek(0, SeekOrigin.Begin);

                return stream;
            }
        }

        private TEntity Deserialize<TEntity>(Stream stream)
        {
            var serializer = GetJsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<TEntity>(jsonTextReader);
            }
        }

        private BlobContainerClient GetBlobContainer<TEntity>()
        {
            var container = _containerNameBuilder.Build<TEntity>();
            var blobContainer = Client.GetBlobContainerClient(container);

            if (_createContainerIfNotExists)
                blobContainer.CreateIfNotExists();

            return blobContainer;
        }

        private IAsyncEnumerable<TEntity> AsAsyncEnumerable<TEntity>() where TEntity : class
        {
            return DownloadEntitiesAsync<TEntity>();
        }

        private BlobClient GetBlobClient<TEntity>(TEntity entity) where TEntity : class
        {
            var keyValues = Conventions.GetPrimaryKeyValues(entity);
            var key = string.Join(":", keyValues);
            var blobContainer = GetBlobContainer<TEntity>();

            return blobContainer.GetBlobClient(key);
        }

        private TEntity DownloadEntity<TEntity>(BlobContainerClient blobContainer, string blobName) where TEntity : class
        {
            try
            {
                var blob = blobContainer.GetBlobClient(blobName);
                var contentResult = blob.DownloadContent();
                var contentStream = contentResult.Value.Content.ToStream();
                var result = Deserialize<TEntity>(contentStream);

                return result;
            }
            catch (RequestFailedException)
            {
                return default(TEntity);
            }
        }

        private IEnumerable<TEntity> DownloadEntities<TEntity>() where TEntity : class
        {
            var blobContainer = GetBlobContainer<TEntity>();
            foreach (var blobItem in blobContainer.GetBlobs())
            {
                yield return DownloadEntity<TEntity>(blobContainer, blobItem.Name);
            }
        }

        private async Task<BlobContainerClient> GetBlobContainerAsync<TEntity>()
        {
            var container = _containerNameBuilder.Build<TEntity>();
            var blobContainer = Client.GetBlobContainerClient(container);

            if (_createContainerIfNotExists)
                await blobContainer.CreateIfNotExistsAsync();

            return blobContainer;
        }

        private async Task<BlobClient> GetBlobClientAsync<TEntity>(TEntity entity) where TEntity : class
        {
            var keyValues = Conventions.GetPrimaryKeyValues(entity);
            var key = string.Join(":", keyValues);
            var blobContainer = await GetBlobContainerAsync<TEntity>();

            return blobContainer.GetBlobClient(key);
        }

        private async Task<TEntity> DownloadEntityAsync<TEntity>(BlobContainerClient blobContainer, string blobName, CancellationToken cancellationToken = default) where TEntity : class
        {
            try
            {
                var blob = blobContainer.GetBlobClient(blobName);
                var contentResult = await blob.DownloadContentAsync(cancellationToken);
                var contentStream = contentResult.Value.Content.ToStream();
                var result = Deserialize<TEntity>(contentStream);

                return result;
            }
            catch (RequestFailedException)
            {
                return default(TEntity);
            }
        }

        private async IAsyncEnumerable<TEntity> DownloadEntitiesAsync<TEntity>() where TEntity : class
        {
            var blobContainer = await GetBlobContainerAsync<TEntity>();
            await foreach (var blobItem in blobContainer.GetBlobsAsync())
            {
                yield return await DownloadEntityAsync<TEntity>(blobContainer, blobItem.Name);
            }
        }

        private void UploadEntity<TEntity>(TEntity entity) where TEntity : class
        {
            var blob = GetBlobClient(entity);
            using (var stream = Serialize<TEntity>(entity))
            {
                var binaryData = BinaryData.FromStream(stream);

                blob.Upload(binaryData, overwrite: true);
            }
        }

        private async Task UploadEntityAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            var blob = await GetBlobClientAsync(entity);
            using (var stream = Serialize<TEntity>(entity))
            {
                var binaryData = await BinaryData.FromStreamAsync(stream, cancellationToken);

                await blob.UploadAsync(binaryData, overwrite: true, cancellationToken);
            }
        }

        #endregion

        #region Overrides of LinqEnumerableRepositoryContextBase

        protected override IEnumerable<TEntity> AsEnumerable<TEntity>(IFetchQueryStrategy<TEntity> fetchStrategy)
        {
            return DownloadEntities<TEntity>();
        }

        #endregion

        #region Implementation of IAzureStorageBlobRepositoryContext

        public BlobServiceClient Client { get; }

        #endregion

        #region Implementation of IRepositoryContext

        public override void Add<TEntity>(TEntity entity)
        {
            UploadEntity(Guard.NotNull(entity, nameof(entity)));
        }

        public override void Update<TEntity>(TEntity entity)
        {
            UploadEntity(Guard.NotNull(entity, nameof(entity)));
        }

        public override void Remove<TEntity>(TEntity entity)
        {
            Guard.NotNull(entity, nameof(entity));

            var blob = GetBlobClient(entity);

            blob.DeleteIfExists();
        }

        public override int SaveChanges() => -1;

        public override TEntity Find<TEntity>(IFetchQueryStrategy<TEntity> fetchStrategy, params object[] keyValues) where TEntity : class
        {
            Guard.NotEmpty(keyValues, nameof(keyValues));

            var key = string.Join(":", keyValues);
            var blobContainer = GetBlobContainer<TEntity>();
            var result = DownloadEntity<TEntity>(blobContainer, key);

            return result;
        }

        #endregion

        #region Implementation of IRepositoryContextAsync

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync<TEntity>(string sql, CommandType cmdType, Dictionary<string, object> parameters, Func<IDataReader, TEntity> projector, CancellationToken cancellationToken = default) where TEntity : class
        {
            throw new NotSupportedException(Resources.QueryExecutionNotSupported);
        }

        public Task<int> ExecuteSqlCommandAsync(string sql, CommandType cmdType, Dictionary<string, object> parameters, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException(Resources.QueryExecutionNotSupported);
        }

        public Task AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            return UploadEntityAsync(Guard.NotNull(entity, nameof(entity)), cancellationToken);
        }

        public Task UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            return UploadEntityAsync(Guard.NotNull(entity, nameof(entity)), cancellationToken);
        }

        public Task RemoveAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            Guard.NotNull(entity, nameof(entity));

            var blob = GetBlobClient(entity);

            return blob.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(-1);

        public async Task<TEntity> FindAsync<TEntity>(CancellationToken cancellationToken, IFetchQueryStrategy<TEntity> fetchStrategy, params object[] keyValues) where TEntity : class
        {
            Guard.NotEmpty(keyValues, nameof(keyValues));

            var key = string.Join(":", keyValues);
            var blobContainer = await GetBlobContainerAsync<TEntity>();
            var result = await DownloadEntityAsync<TEntity>(blobContainer, key, cancellationToken);

            return result;
        }

        public async Task<TResult> FindAsync<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default) where TEntity : class
        {
            Guard.NotNull(selector, nameof(selector));

            var selectorFunc = selector.Compile();

            var result = await AsAsyncEnumerable<TEntity>()
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(Conventions, options)
                .ApplyPagingOptions(options)
                .Select(selectorFunc)
                .FirstOrDefaultAsync(cancellationToken);

            return result;
        }

        public async Task<PagedQueryResult<IEnumerable<TResult>>> FindAllAsync<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default) where TEntity : class
        {
            Guard.NotNull(selector, nameof(selector));

            var selectorFunc = selector.Compile();

            var query = AsAsyncEnumerable<TEntity>()
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(Conventions, options);

            var total = await query.CountAsync(cancellationToken);

            var result = await query
                .ApplyPagingOptions(options)
                .Select(selectorFunc)
                .ToListAsync(cancellationToken);

            return new PagedQueryResult<IEnumerable<TResult>>(result, total);
        }

        public async Task<int> CountAsync<TEntity>(IQueryOptions<TEntity> options, CancellationToken cancellationToken = default) where TEntity : class
        {
            var result = await AsAsyncEnumerable<TEntity>()
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(Conventions, options)
                .ApplyPagingOptions(options)
                .CountAsync(cancellationToken);

            return result;
        }

        public async Task<bool> ExistsAsync<TEntity>(IQueryOptions<TEntity> options, CancellationToken cancellationToken = default) where TEntity : class
        {
            Guard.NotNull(options, nameof(options));

            var result = await AsAsyncEnumerable<TEntity>()
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(Conventions, options)
                .ApplyPagingOptions(options)
                .AnyAsync(cancellationToken);

            return result;
        }

        public async Task<PagedQueryResult<Dictionary<TDictionaryKey, TElement>>> ToDictionaryAsync<TEntity, TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = default) where TEntity : class
        {
            Guard.NotNull(keySelector, nameof(keySelector));
            Guard.NotNull(elementSelector, nameof(elementSelector));

            var keySelectFunc = keySelector.Compile();
            var elementSelectorFunc = elementSelector.Compile();

            var query = AsAsyncEnumerable<TEntity>()
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(Conventions, options);

            Dictionary<TDictionaryKey, TElement> result;
            int total;

            if (options != null && options.PageSize != -1)
            {
                total = await query.CountAsync(cancellationToken);

                result = await query
                    .ApplyPagingOptions(options)
                    .ToDictionaryAsync(keySelectFunc, elementSelectorFunc, cancellationToken);
            }
            else
            {
                // Gets the total count from memory
                result = await query.ToDictionaryAsync(keySelectFunc, elementSelectorFunc, cancellationToken);
                total = result.Count;
            }

            return new PagedQueryResult<Dictionary<TDictionaryKey, TElement>>(result, total);
        }

        public Task<PagedQueryResult<IEnumerable<TResult>>> GroupByAsync<TEntity, TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<IGrouping<TGroupKey, TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = default) where TEntity : class
        {
            return Task.FromResult(GroupBy<TEntity, TGroupKey, TResult>(options, keySelector, resultSelector));
        }

        #endregion
    }
}
