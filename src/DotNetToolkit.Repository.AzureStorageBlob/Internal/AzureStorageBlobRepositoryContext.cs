namespace DotNetToolkit.Repository.AzureStorageBlob.Internal
{
    using Azure;
    using Azure.Storage.Blobs;
    using Configuration.Conventions;
    using Configuration.Logging;
    using Extensions;
    using Properties;
    using Query;
    using Query.Strategies;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Transactions;
    using Utility;

    internal class AzureStorageBlobRepositoryContext : IAzureStorageBlobRepositoryContext
    {
        #region Constructors

        public AzureStorageBlobRepositoryContext(string connectionString, string container = null, bool createIfNotExists = false)
        {
            Guard.NotEmpty(connectionString, nameof(connectionString));

            var client = new BlobServiceClient(connectionString);

            if (string.IsNullOrEmpty(container))
                container = GetType().Name.ToLower();

            BlobContainer = client.GetBlobContainerClient(container);

            if (createIfNotExists)
                BlobContainer.CreateIfNotExists();

            Conventions = RepositoryConventions.Default();
        }

        #endregion

        #region Private Methods

        private BlobClient GetBlobClient<TEntity>(TEntity entity) where TEntity : class
        {
            var keyValues = Conventions.GetPrimaryKeyValues(entity);
            var key = string.Join(":", keyValues);

            return BlobContainer.GetBlobClient(key);
        }

        private TEntity DownloadEntity<TEntity>(string blobName) where TEntity : class
        {
            try
            {
                var blob = BlobContainer.GetBlobClient(blobName);
                var contentResult = blob.DownloadContent();
                var result = contentResult.Value.Content.ToObjectFromJson<TEntity>();

                return result;
            }
            catch (RequestFailedException)
            {
                return default(TEntity);
            }
        }

        private async Task<TEntity> DownloadEntityAsync<TEntity>(string blobName, CancellationToken cancellationToken = default) where TEntity : class
        {
            try
            {
                var blob = BlobContainer.GetBlobClient(blobName);
                var contentResult = await blob.DownloadContentAsync(cancellationToken: cancellationToken);
                var result = contentResult.Value.Content.ToObjectFromJson<TEntity>();

                return result;
            }
            catch (RequestFailedException)
            {
                return default(TEntity);
            }
        }

        private IEnumerable<TEntity> DownloadEntities<TEntity>() where TEntity : class
        {
            foreach (var blobItem in BlobContainer.GetBlobs())
            {
                yield return DownloadEntity<TEntity>(blobItem.Name);
            }
        }

        private async IAsyncEnumerable<TEntity> DownloadEntitiesAsync<TEntity>() where TEntity : class
        {
            await foreach (var blobItem in BlobContainer.GetBlobsAsync())
            {
                yield return DownloadEntity<TEntity>(blobItem.Name);
            }
        }

        private void UploadEntity<TEntity>(TEntity entity) where TEntity : class
        {
            var blob = GetBlobClient(entity);
            var binaryData = BinaryData.FromObjectAsJson<TEntity>(entity);

            blob.Upload(binaryData, overwrite: true);
        }

        #endregion

        #region Implementation of IAzureStorageBlobRepositoryContext

        public BlobContainerClient BlobContainer { get; }

        #endregion

        #region Implementation of IRepositoryContext

        public ITransactionManager CurrentTransaction { get; set; }

        public IRepositoryConventions Conventions { get; }

        public ILoggerProvider LoggerProvider { get; set; } = NullLoggerProvider.Instance;

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            UploadEntity(Guard.NotNull(entity, nameof(entity)));
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            UploadEntity(Guard.NotNull(entity, nameof(entity)));
        }

        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            Guard.NotNull(entity, nameof(entity));

            var blob = GetBlobClient(entity);

            blob.DeleteIfExists();
        }

        public int SaveChanges() => -1;

        public IEnumerable<TEntity> ExecuteSqlQuery<TEntity>(string sql, CommandType cmdType, Dictionary<string, object> parameters, Func<IDataReader, TEntity> projector) where TEntity : class
        {
            throw new NotSupportedException(Resources.QueryExecutionNotSupported);
        }

        public int ExecuteSqlCommand(string sql, CommandType cmdType, Dictionary<string, object> parameters)
        {
            throw new NotSupportedException(Resources.QueryExecutionNotSupported);
        }

        public ITransactionManager BeginTransaction()
        {
            throw new NotSupportedException(Resources.TransactionNotSupported);
        }

        public TEntity Find<TEntity>(IFetchQueryStrategy<TEntity> fetchStrategy, params object[] keyValues) where TEntity : class
        {
            Guard.NotEmpty(keyValues, nameof(keyValues));

            var key = string.Join(":", keyValues);
            var result = DownloadEntity<TEntity>(key);

            return result;
        }

        public TResult Find<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            Guard.NotNull(selector, nameof(selector));

            var result = DownloadEntities<TEntity>().AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(Conventions, options)
                .ApplyPagingOptions(options)
                .Select(selector)
                .FirstOrDefault();

            return result;
        }

        public PagedQueryResult<IEnumerable<TResult>> FindAll<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            Guard.NotNull(selector, nameof(selector));

            var query = DownloadEntities<TEntity>().AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(Conventions, options);

            var total = query.Count();

            var result = query
                .ApplyPagingOptions(options)
                .Select(selector)
                .ToList();

            return new PagedQueryResult<IEnumerable<TResult>>(result, total);
        }

        public int Count<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            var result = DownloadEntities<TEntity>().AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(Conventions, options)
                .ApplyPagingOptions(options)
                .Count();

            return result;
        }

        public bool Exists<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            Guard.NotNull(options, nameof(options));

            var result = DownloadEntities<TEntity>().AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(Conventions, options)
                .ApplyPagingOptions(options)
                .Any();

            return result;
        }

        public PagedQueryResult<Dictionary<TDictionaryKey, TElement>> ToDictionary<TEntity, TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector) where TEntity : class
        {
            Guard.NotNull(keySelector, nameof(keySelector));
            Guard.NotNull(elementSelector, nameof(elementSelector));

            var keySelectFunc = keySelector.Compile();
            var elementSelectorFunc = elementSelector.Compile();

            var query = DownloadEntities<TEntity>().AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(Conventions, options);

            Dictionary<TDictionaryKey, TElement> result;
            int total;

            if (options != null && options.PageSize != -1)
            {
                total = query.Count();

                result = query
                    .ApplyPagingOptions(options)
                    .ToDictionary(keySelectFunc, elementSelectorFunc);
            }
            else
            {
                // Gets the total count from memory
                result = query.ToDictionary(keySelectFunc, elementSelectorFunc);
                total = result.Count;
            }

            return new PagedQueryResult<Dictionary<TDictionaryKey, TElement>>(result, total);
        }

        public PagedQueryResult<IEnumerable<TResult>> GroupBy<TEntity, TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector) where TEntity : class
        {
            Guard.NotNull(keySelector, nameof(keySelector));
            Guard.NotNull(resultSelector, nameof(resultSelector));

            var keySelectFunc = keySelector.Compile();
            var resultSelectorFunc = resultSelector.Compile();

            var query = DownloadEntities<TEntity>().AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(Conventions, options);

            var total = query.Count();

            var result = query
                .ApplyPagingOptions(options)
                .GroupBy(keySelectFunc, resultSelectorFunc)
                .ToList();

            return new PagedQueryResult<IEnumerable<TResult>>(result, total);
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

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(-1);

        public Task<TEntity> FindAsync<TEntity>(CancellationToken cancellationToken, IFetchQueryStrategy<TEntity> fetchStrategy, params object[] keyValues) where TEntity : class
        {
            Guard.NotEmpty(keyValues, nameof(keyValues));

            var key = string.Join(":", keyValues);
            var result = DownloadEntityAsync<TEntity>(key, cancellationToken);

            return result;
        }

        public async Task<TResult> FindAsync<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default) where TEntity : class
        {
            Guard.NotNull(selector, nameof(selector));

            var selectorFunc = selector.Compile();

            var result = await DownloadEntitiesAsync<TEntity>()
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

            var query = DownloadEntitiesAsync<TEntity>()
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
            var result = await DownloadEntitiesAsync<TEntity>()
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(Conventions, options)
                .ApplyPagingOptions(options)
                .CountAsync(cancellationToken);

            return result;
        }

        public async Task<bool> ExistsAsync<TEntity>(IQueryOptions<TEntity> options, CancellationToken cancellationToken = default) where TEntity : class
        {
            Guard.NotNull(options, nameof(options));

            var result = await DownloadEntitiesAsync<TEntity>()
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

            var query = DownloadEntitiesAsync<TEntity>()
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

        public Task<PagedQueryResult<IEnumerable<TResult>>> GroupByAsync<TEntity, TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = default) where TEntity : class
        {
            return Task.FromResult(GroupBy<TEntity, TGroupKey, TResult>(options, keySelector, resultSelector));
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            CurrentTransaction = null;
        }

        #endregion
    }
}
