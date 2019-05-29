namespace DotNetToolkit.Repository.Extensions
{
    using Configuration.Caching;
    using Configuration.Caching.Internal;
    using Configuration.Conventions;
    using Configuration.Logging;
    using Extensions.Internal;
    using JetBrains.Annotations;
    using Queries;
    using Queries.Internal;
    using Queries.Strategies;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;
    using Utility;

    /// <summary>
    /// Contains various extension methods for <see cref="ICacheProvider" />
    /// </summary>
    public static class CachingProviderExtensions
    {
        private static readonly object _syncRoot = new object();
        private static string Name<T>() => typeof(T).FullName;
        private static string Glue => CacheProviderManager.CachePrefixGlue;

        /// <summary>
        /// Gets or sets a cached query result that matches the specified key.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="cacheProvider">The caching provider.</param>
        /// <param name="key">The caching key (un-hashed).</param>
        /// <param name="getter">A function for getting a result to cache.</param>
        /// <param name="expiry">The expiration time.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>The cached query result.</returns>
        public static ICacheQueryResult<TResult> GetOrSet<TEntity, TResult>(
            [NotNull] this ICacheProvider cacheProvider,
            [NotNull] string key,
            [NotNull] Func<TResult> getter,
            [CanBeNull] TimeSpan? expiry,
            [NotNull] ILogger logger)
        {
            Guard.NotNull(cacheProvider, nameof(cacheProvider));
            Guard.NotEmpty(key, nameof(key));
            Guard.NotNull(logger, nameof(logger));

            var hashedKey = FormatHashedKey<TEntity>(cacheProvider, key);
            var cacheUsed = false;

            if (!cacheProvider.TryGetValue<TResult>(hashedKey, out var value))
            {
                value = getter();

                cacheProvider.SetValue<TResult>(hashedKey, key, value, expiry, logger);
            }
            else
            {
                cacheUsed = true;
            }

            return new CacheQueryResult<TResult>(value)
            {
                CacheUsed = cacheUsed
            };
        }

        /// <summary>
        /// Gets or sets a cached paged query result that matches the specified key.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="cacheProvider">The caching provider.</param>
        /// <param name="key">The caching key (un-hashed).</param>
        /// <param name="getter">A function for getting a result to cache.</param>
        /// <param name="expiry">The expiration time.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>The cached query result.</returns>
        public static ICachePagedQueryResult<TResult> GetOrSet<TEntity, TResult>(
            [NotNull] this ICacheProvider cacheProvider,
            [NotNull] string key,
            [NotNull] Func<IPagedQueryResult<TResult>> getter,
            [CanBeNull] TimeSpan? expiry,
            [NotNull] ILogger logger)
        {
            Guard.NotNull(cacheProvider, nameof(cacheProvider));
            Guard.NotEmpty(key, nameof(key));
            Guard.NotNull(getter, nameof(getter));
            Guard.NotNull(logger, nameof(logger));

            var hashedKey = FormatHashedKey<TEntity>(cacheProvider, key);
            var cacheUsed = false;

            if (!cacheProvider.TryGetValue<PagedQueryResult<TResult>>(hashedKey, out var value))
            {
                value = new PagedQueryResult<TResult>(getter());

                cacheProvider.SetValue<PagedQueryResult<TResult>>(hashedKey, key, value, expiry, logger);
            }
            else
            {
                cacheUsed = true;
            }

            return new CachePagedQueryResult<TResult>(value)
            {
                CacheUsed = cacheUsed
            };
        }

        /// <summary>
        /// Gets or sets a cached query result that matches the specified key.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="cacheProvider">The caching provider.</param>
        /// <param name="key">The caching key (un-hashed).</param>
        /// <param name="getter">A function for getting a result to cache.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>The cached query result.</returns>
        public static ICacheQueryResult<TResult> GetOrSet<TEntity, TResult>(
            [NotNull] this ICacheProvider cacheProvider,
            [NotNull] string key,
            [NotNull] Func<TResult> getter,
            [NotNull] ILogger logger)
            => GetOrSet<TEntity, TResult>(
                cacheProvider,
                key,
                getter,
                cacheProvider.Expiry,
                logger);

        /// <summary>
        /// Gets or sets a cached paged query result that matches the specified key.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="cacheProvider">The caching provider.</param>
        /// <param name="key">The caching key (un-hashed).</param>
        /// <param name="getter">A function for getting a result to cache.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>The cached query result.</returns>
        public static ICachePagedQueryResult<TResult> GetOrSet<TEntity, TResult>(
            [NotNull] this ICacheProvider cacheProvider,
            [NotNull] string key,
            [NotNull] Func<IPagedQueryResult<TResult>> getter,
            [NotNull] ILogger logger)
            => GetOrSet<TEntity, TResult>(
                cacheProvider,
                key,
                getter,
                cacheProvider.Expiry,
                logger);

        /// <summary>
        /// Asynchronously gets or sets a cached query result that matches the specified key.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="cacheProvider">The caching provider.</param>
        /// <param name="key">The caching key (un-hashed).</param>
        /// <param name="getter">A function for getting a result to cache.</param>
        /// <param name="expiry">The expiration time.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>The cached query result.</returns>
        public static async Task<ICacheQueryResult<TResult>> GetOrSetAsync<TEntity, TResult>(
            [NotNull] this ICacheProvider cacheProvider,
            [NotNull] string key,
            [NotNull] Func<Task<TResult>> getter,
            [CanBeNull] TimeSpan? expiry,
            [NotNull] ILogger logger)
        {
            Guard.NotNull(cacheProvider, nameof(cacheProvider));
            Guard.NotEmpty(key, nameof(key));
            Guard.NotNull(getter, nameof(getter));
            Guard.NotNull(logger, nameof(logger));

            var hashedKey = FormatHashedKey<TEntity>(cacheProvider, key);
            var cacheUsed = false;

            if (!cacheProvider.TryGetValue<TResult>(hashedKey, out var value))
            {
                value = await getter();

                cacheProvider.SetValue<TResult>(hashedKey, key, value, expiry, logger);
            }
            else
            {
                cacheUsed = true;
            }

            return new CacheQueryResult<TResult>(value)
            {
                CacheUsed = cacheUsed
            };
        }

        /// <summary>
        /// Asynchronously gets or sets a cached paged query result that matches the specified key.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="cacheProvider">The caching provider.</param>
        /// <param name="key">The caching key (un-hashed).</param>
        /// <param name="getter">A function for getting a result to cache.</param>
        /// <param name="expiry">The expiration time.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>The cached query result.</returns>
        public static async Task<ICachePagedQueryResult<TResult>> GetOrSetAsync<TEntity, TResult>(
            [NotNull] this ICacheProvider cacheProvider,
            [NotNull] string key,
            [NotNull] Func<Task<IPagedQueryResult<TResult>>> getter,
            [CanBeNull] TimeSpan? expiry,
            [NotNull] ILogger logger)
        {
            Guard.NotNull(cacheProvider, nameof(cacheProvider));
            Guard.NotEmpty(key, nameof(key));
            Guard.NotNull(getter, nameof(getter));
            Guard.NotNull(logger, nameof(logger));

            var hashedKey = FormatHashedKey<TEntity>(cacheProvider, key);
            var cacheUsed = false;

            if (!cacheProvider.TryGetValue<PagedQueryResult<TResult>>(hashedKey, out var value))
            {
                value = new PagedQueryResult<TResult>(await getter());

                cacheProvider.SetValue<PagedQueryResult<TResult>>(hashedKey, key, value, expiry, logger);
            }
            else
            {
                cacheUsed = true;
            }

            return new CachePagedQueryResult<TResult>(value)
            {
                CacheUsed = cacheUsed
            };
        }

        /// <summary>
        /// Asynchronously gets or sets a cached query result that matches the specified key.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="cacheProvider">The caching provider.</param>
        /// <param name="key">The caching key (un-hashed).</param>
        /// <param name="getter">A function for getting a result to cache.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>The cached query result.</returns>
        public static Task<ICacheQueryResult<TResult>> GetOrSetAsync<TEntity, TResult>(
            [NotNull] this ICacheProvider cacheProvider,
            [NotNull] string key,
            [NotNull] Func<Task<TResult>> getter,
            [NotNull] ILogger logger)
            => GetOrSetAsync<TEntity, TResult>(
                cacheProvider,
                key,
                getter,
                cacheProvider.Expiry,
                logger);

        /// <summary>
        /// Asynchronously gets or sets a cached query result that matches the specified key.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="cacheProvider">The caching provider.</param>
        /// <param name="key">The caching key (un-hashed).</param>
        /// <param name="getter">A function for getting a result to cache.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>The cached query result.</returns>
        public static Task<ICachePagedQueryResult<TResult>> GetOrSetAsync<TEntity, TResult>(
            [NotNull] this ICacheProvider cacheProvider,
            [NotNull] string key,
            [NotNull] Func<Task<IPagedQueryResult<TResult>>> getter,
            [NotNull] ILogger logger)
            => GetOrSetAsync<TEntity, TResult>(
                cacheProvider,
                key,
                getter,
                cacheProvider.Expiry,
                logger);

        internal static ICacheQueryResult<IEnumerable<T>> GetOrSetExecuteSqlQuery<T>(
            [NotNull] this ICacheProvider cacheProvider,
            [NotNull] string sql, CommandType cmdType,
            [CanBeNull] Dictionary<string, object> parameters,
            [NotNull] Func<IDataReader, IRepositoryConventions, T> projector,
            [NotNull] Func<IEnumerable<T>> getter,
            [NotNull] ILogger logger)
            => GetOrSet<T, IEnumerable<T>>(
                cacheProvider,
                FormatGetOrSetExecuteSqlQueryKey<T>(sql, cmdType, parameters),
                getter,
                logger);

        internal static ICacheQueryResult<IEnumerable<T>> GetOrSetExecuteSqlQuery<T>(
            [NotNull] this ICacheProvider cacheProvider,
            [NotNull] string sql, CommandType cmdType,
            [CanBeNull] Dictionary<string, object> parameters,
            [NotNull] Func<IDataReader, T> projector,
            [NotNull] Func<IEnumerable<T>> getter,
            [NotNull] ILogger logger)
            => GetOrSet<T, IEnumerable<T>>(
                cacheProvider,
                FormatGetOrSetExecuteSqlQueryKey<T>(sql, cmdType, parameters),
                getter,
                logger);

        internal static ICacheQueryResult<int> GetOrSetExecuteSqlCommand<T>(
            [NotNull] this ICacheProvider cacheProvider,
            [NotNull] string sql, CommandType cmdType,
            [CanBeNull] Dictionary<string, object> parameters,
            [NotNull] Func<int> getter,
            [NotNull] ILogger logger)
            => GetOrSet<T, int>(
                cacheProvider,
                FormatGetOrSetExecuteSqlCommandKey<T>(sql, cmdType, parameters),
                getter,
                logger);

        internal static ICacheQueryResult<T> GetOrSetFind<T>(
            [NotNull] this ICacheProvider cacheProvider,
            [NotNull] object[] keys,
            [CanBeNull] IFetchQueryStrategy<T> fetchStrategy,
            [NotNull] Func<T> getter,
            [NotNull] ILogger logger)
            => GetOrSet<T, T>(
                cacheProvider,
                FormatGetOrSetFindKey<T>(keys, fetchStrategy),
                getter,
                logger);

        internal static ICacheQueryResult<TResult> GetOrSetFind<T, TResult>(
            [NotNull] this ICacheProvider cacheProvider,
            [CanBeNull] IQueryOptions<T> options,
            [NotNull] Expression<Func<T, TResult>> selector,
            [NotNull] Func<TResult> getter,
            [NotNull] ILogger logger)
            => GetOrSet<T, TResult>(
                cacheProvider,
                FormatGetOrSetFindKey<T, TResult>(options, selector),
                getter,
                logger);

        internal static ICachePagedQueryResult<IEnumerable<TResult>> GetOrSetFindAll<T, TResult>(
            [NotNull] this ICacheProvider cacheProvider,
            [CanBeNull] IQueryOptions<T> options,
            [NotNull] Expression<Func<T, TResult>> selector,
            [NotNull] Func<IPagedQueryResult<IEnumerable<TResult>>> getter,
            [NotNull] ILogger logger)
            => GetOrSet<T, IEnumerable<TResult>>(
                cacheProvider,
                FormatGetOrSetFindAllKey<T, TResult>(options, selector),
                getter,
                logger);

        internal static ICacheQueryResult<int> GetOrSetCount<T>(
            [NotNull] this ICacheProvider cacheProvider,
            [CanBeNull] IQueryOptions<T> options,
            [NotNull] Func<int> getter,
            [NotNull] ILogger logger)
            => GetOrSet<T, int>(
                cacheProvider,
                FormatGetOrSetCountKey<T>(options),
                getter,
                logger);

        internal static ICachePagedQueryResult<Dictionary<TDictionaryKey, TElement>> GetOrSetDictionary<T, TDictionaryKey, TElement>(
            [NotNull] this ICacheProvider cacheProvider,
            [CanBeNull] IQueryOptions<T> options,
            [NotNull] Expression<Func<T, TDictionaryKey>> keySelector,
            [NotNull] Expression<Func<T, TElement>> elementSelector,
            [NotNull] Func<IPagedQueryResult<Dictionary<TDictionaryKey, TElement>>> getter,
            [NotNull] ILogger logger)
            => GetOrSet<T, Dictionary<TDictionaryKey, TElement>>(
                cacheProvider,
                FormatGetOrSetDictionaryKey<T, TDictionaryKey, TElement>(options, keySelector, elementSelector),
                getter,
                logger);

        internal static ICachePagedQueryResult<IEnumerable<TResult>> GetOrSetGroup<T, TGroupKey, TResult>(
            [NotNull] this ICacheProvider cacheProvider,
            [CanBeNull] IQueryOptions<T> options,
            [NotNull] Expression<Func<T, TGroupKey>> keySelector,
            [NotNull] Expression<Func<TGroupKey, IEnumerable<T>, TResult>> resultSelector,
            [NotNull] Func<IPagedQueryResult<IEnumerable<TResult>>> getter,
            [NotNull] ILogger logger)
            => GetOrSet<T, IEnumerable<TResult>>(
                cacheProvider,
                FormatGetOrSetGroupKey<T, TGroupKey, TResult>(options, keySelector, resultSelector),
                getter,
                logger);

        internal static Task<ICacheQueryResult<IEnumerable<T>>> GetOrSetExecuteSqlQueryAsync<T>(
            [NotNull] this ICacheProvider cacheProvider,
            [NotNull] string sql,
            CommandType cmdType,
            [CanBeNull] Dictionary<string, object> parameters,
            [NotNull] Func<IDataReader, IRepositoryConventions, T> projector,
            [NotNull] Func<Task<IEnumerable<T>>> getter,
            [NotNull] ILogger logger)
            => GetOrSetAsync<T, IEnumerable<T>>(
                cacheProvider,
                FormatGetOrSetExecuteSqlQueryKey<T>(sql, cmdType, parameters),
                getter,
                logger);

        internal static Task<ICacheQueryResult<IEnumerable<T>>> GetOrSetExecuteSqlQueryAsync<T>(
            [NotNull] this ICacheProvider cacheProvider,
            [NotNull] string sql,
            CommandType cmdType,
            [CanBeNull] Dictionary<string, object> parameters,
            [NotNull] Func<IDataReader, T> projector,
            [NotNull] Func<Task<IEnumerable<T>>> getter,
            [NotNull] ILogger logger)
            => GetOrSetAsync<T, IEnumerable<T>>(
                cacheProvider,
                FormatGetOrSetExecuteSqlQueryKey<T>(sql, cmdType, parameters),
                getter,
                logger);

        internal static Task<ICacheQueryResult<int>> GetOrSetExecuteSqlCommandAsync<T>(
            [NotNull] this ICacheProvider cacheProvider,
            [NotNull] string sql,
            CommandType cmdType,
            [CanBeNull] Dictionary<string, object> parameters,
            [NotNull] Func<Task<int>> getter,
            [NotNull] ILogger logger)
            => GetOrSetAsync<T, int>(
                cacheProvider,
                FormatGetOrSetExecuteSqlCommandKey<T>(sql, cmdType, parameters),
                getter,
                logger);

        internal static Task<ICacheQueryResult<T>> GetOrSetFindAsync<T>(
            [NotNull] this ICacheProvider cacheProvider,
            [NotNull] object[] keys,
            [CanBeNull] IFetchQueryStrategy<T> fetchStrategy,
            [NotNull] Func<Task<T>> getter,
            [NotNull] ILogger logger)
            => GetOrSetAsync<T, T>(
                cacheProvider,
                FormatGetOrSetFindKey<T>(keys, fetchStrategy),
                getter,
                logger);

        internal static Task<ICacheQueryResult<TResult>> GetOrSetFindAsync<T, TResult>(
            [NotNull] this ICacheProvider cacheProvider,
            [CanBeNull] IQueryOptions<T> options,
            [NotNull] Expression<Func<T, TResult>> selector,
            [NotNull] Func<Task<TResult>> getter,
            [NotNull] ILogger logger)
            => GetOrSetAsync<T, TResult>(
                cacheProvider,
                FormatGetOrSetFindKey<T, TResult>(options, selector),
                getter,
                logger);

        internal static Task<ICachePagedQueryResult<IEnumerable<TResult>>> GetOrSetFindAllAsync<T, TResult>(
            [NotNull] this ICacheProvider cacheProvider,
            [CanBeNull] IQueryOptions<T> options,
            [NotNull] Expression<Func<T, TResult>> selector,
            [NotNull] Func<Task<IPagedQueryResult<IEnumerable<TResult>>>> getter,
            [NotNull] ILogger logger)
            => GetOrSetAsync<T, IEnumerable<TResult>>(
                cacheProvider,
                FormatGetOrSetFindAllKey<T, TResult>(options, selector),
                getter,
                logger);

        internal static Task<ICacheQueryResult<int>> GetOrSetCountAsync<T>(
            [NotNull] this ICacheProvider cacheProvider,
            [CanBeNull] IQueryOptions<T> options,
            [NotNull] Func<Task<int>> getter,
            [NotNull] ILogger logger)
            => GetOrSetAsync<T, int>(
                cacheProvider,
                FormatGetOrSetCountKey<T>(options),
                getter,
                logger);

        internal static Task<ICachePagedQueryResult<Dictionary<TDictionaryKey, TElement>>> GetOrSetDictionaryAsync<T, TDictionaryKey, TElement>(
            [NotNull] this ICacheProvider cacheProvider,
            [CanBeNull] IQueryOptions<T> options,
            [NotNull] Expression<Func<T, TDictionaryKey>> keySelector,
            [NotNull] Expression<Func<T, TElement>> elementSelector,
            [NotNull] Func<Task<IPagedQueryResult<Dictionary<TDictionaryKey, TElement>>>> getter,
            [NotNull] ILogger logger)
            => GetOrSetAsync<T, Dictionary<TDictionaryKey, TElement>>(
                cacheProvider,
                FormatGetOrSetDictionaryKey<T, TDictionaryKey, TElement>(options, keySelector, elementSelector),
                getter,
                logger);

        internal static Task<ICachePagedQueryResult<IEnumerable<TResult>>> GetOrSetGroupAsync<T, TGroupKey, TResult>(
            [NotNull] this ICacheProvider cacheProvider,
            [CanBeNull] IQueryOptions<T> options,
            [NotNull] Expression<Func<T, TGroupKey>> keySelector,
            [NotNull] Expression<Func<TGroupKey, IEnumerable<T>, TResult>> resultSelector,
            [NotNull] Func<Task<IPagedQueryResult<IEnumerable<TResult>>>> getter,
            [NotNull] ILogger logger)
            => GetOrSetAsync<T, IEnumerable<TResult>>(
                cacheProvider,
                FormatGetOrSetGroupKey<T, TGroupKey, TResult>(options, keySelector, resultSelector),
                getter,
                logger);

        internal static void IncrementCounter<T>([NotNull] this ICacheProvider cacheProvider)
        {
            Guard.NotNull(cacheProvider, nameof(cacheProvider));
            Guard.EnsureNotNull(cacheProvider.Cache, "The caching cannot be null.");

            lock (_syncRoot)
            {
                cacheProvider.Cache.Increment(FormatCachePrefixCounterKey<T>(), 1, 1);
            }
        }

        private static void SetValue<T>(
            [NotNull] this ICacheProvider cacheProvider,
            [NotNull] string hashedKey,
            [NotNull] string key,
            [NotNull] T value,
            [CanBeNull] TimeSpan? expiry,
            [NotNull] ILogger logger)
        {
            Guard.NotNull(cacheProvider, nameof(cacheProvider));
            Guard.NotEmpty(hashedKey, nameof(hashedKey));
            Guard.NotEmpty(key, nameof(key));
            Guard.NotNull(logger, nameof(logger));
            Guard.EnsureNotNull(cacheProvider.Cache, "The caching cannot be null.");

            lock (_syncRoot)
            {
                logger.Debug(expiry.HasValue
                        ? $"Setting up cache for '{hashedKey}' expire handling in {expiry.Value.TotalSeconds} seconds"
                        : $"Setting up cache for '{hashedKey}'");

                cacheProvider.Cache.Set<T>(
                    hashedKey,
                    value,
                    expiry,
                    reason => logger.Debug($"Cache for '{hashedKey}' has expired. Evicting from cache for '{reason}'"));
            }
        }

        private static bool TryGetValue<T>([NotNull] this ICacheProvider cacheProvider, [NotNull] string key, out T value)
        {
            Guard.NotNull(cacheProvider, nameof(cacheProvider));
            Guard.NotEmpty(key, nameof(key));
            Guard.EnsureNotNull(cacheProvider.Cache, "The caching cannot be null.");

            lock (_syncRoot)
            {
                return cacheProvider.Cache.TryGetValue<T>(key, out value);
            }
        }

        private static int GetCachingPrefixCounter<T>([NotNull] this ICacheProvider cacheProvider)
        {
            Guard.NotNull(cacheProvider, nameof(cacheProvider));

            return !cacheProvider.TryGetValue<int>(FormatCachePrefixCounterKey<T>(), out var key) ? 1 : key;
        }

        private static string FormatCachePrefixCounterKey<T>()
        {
            return string.Format("{1}{0}{2}",
                Glue,
                Name<T>(),
                CacheProviderManager.CacheCounterPrefix);
        }

        private static string FormatHashedKey<T>([NotNull] this ICacheProvider cacheProvider, [NotNull] string key)
        {
            Guard.NotNull(cacheProvider, nameof(cacheProvider));
            Guard.NotEmpty(key, nameof(key));

            var cacheKeyTransformer = cacheProvider.KeyTransformer ?? new DefaultCacheKeyTransformer();

            return string.Format("{1}{0}{2}{0}{3}{0}{4}",
                Glue,
                CacheProviderManager.GlobalCachingPrefixCounter,
                cacheProvider.GetCachingPrefixCounter<T>(),
                cacheKeyTransformer.Transform(key),
                CacheProviderManager.CachePrefix);
        }

        private static string FormatQueryOptions<T>([CanBeNull] IQueryOptions<T> options)
        {
            return options != null ? options.ToString() : $"QueryOptions<{Name<T>()}>: [ null ]";
        }

        private static string FormatFetchQueryStrategy<T>([CanBeNull] IFetchQueryStrategy<T> fetchStrategy)
        {
            return fetchStrategy != null ? fetchStrategy.ToString() : $"FetchQueryStrategy<{Name<T>()}>: [ null ]";
        }

        private static string FormatGetOrSetExecuteSqlQueryKey<T>([NotNull] string sql, CommandType cmdType, [CanBeNull] Dictionary<string, object> parameters)
        {
            Guard.NotEmpty(sql, nameof(sql));

            var sb = new StringBuilder();

            sb.Append($"GetOrSetExecuteSqlQuery<{Name<T>()}>: [ \n\tSql = {sql},");

            if (parameters != null && parameters.Any())
            {
                sb.Append($"\n\tParameters = {parameters.ToDebugString()},");
            }

            sb.Append($"\n\tCommandType = {cmdType} ]");

            return sb.ToString();
        }

        private static string FormatGetOrSetExecuteSqlCommandKey<T>([NotNull] string sql, CommandType cmdType, [CanBeNull] Dictionary<string, object> parameters)
        {
            Guard.NotEmpty(sql, nameof(sql));

            var sb = new StringBuilder();

            sb.Append($"GetOrSetExecuteSqlCommand<{Name<T>()}>: [ \n\tSql = {sql},");

            if (parameters != null && parameters.Any())
            {
                sb.Append($"\n\tParameters = {parameters.ToDebugString()},");
            }

            sb.Append($"\n\tCommandType = {cmdType} ]");

            return sb.ToString();
        }

        private static string FormatGetOrSetFindKey<T>([NotNull] object[] keys, [CanBeNull] IFetchQueryStrategy<T> fetchStrategy)
        {
            Guard.NotEmpty(keys, nameof(keys));

            var f = FormatFetchQueryStrategy(fetchStrategy);

            return $"GetOrSetFind<{Name<T>()}>: [ \n\tKeys = {string.Join(", ", keys.Select(x => x.ToString()).ToArray())},\n\t{f} ]";
        }

        private static string FormatGetOrSetFindAllKey<T, TResult>([CanBeNull] IQueryOptions<T> options, [NotNull] Expression<Func<T, TResult>> selector)
        {
            Guard.NotNull(selector, nameof(selector));

            var o = FormatQueryOptions<T>(options);

            return $"GetOrSetFindAll<{Name<T>()}>: [ \n\t{o},\n\tSelector = {ExpressionHelper.TranslateToString(selector)} ]";
        }

        private static string FormatGetOrSetFindKey<T, TResult>([CanBeNull] IQueryOptions<T> options, [NotNull] Expression<Func<T, TResult>> selector)
        {
            Guard.NotNull(selector, nameof(selector));

            var o = FormatQueryOptions<T>(options);

            return $"GetOrSetFind<{Name<T>()}>: [ \n\t{o},\n\tSelector = {ExpressionHelper.TranslateToString(selector)} ]";
        }

        private static string FormatGetOrSetCountKey<T>([CanBeNull] IQueryOptions<T> options)
        {
            var o = FormatQueryOptions<T>(options);

            return $"GetOrSetCount<{Name<T>()}>: [ \n\t{o} ]";
        }

        private static string FormatGetOrSetDictionaryKey<T, TDictionaryKey, TElement>([CanBeNull] IQueryOptions<T> options, [NotNull] Expression<Func<T, TDictionaryKey>> keySelector, [NotNull] Expression<Func<T, TElement>> elementSelector)
        {
            Guard.NotNull(keySelector, nameof(keySelector));
            Guard.NotNull(elementSelector, nameof(elementSelector));

            var o = FormatQueryOptions<T>(options);

            return $"GetOrSetDictionary<{Name<T>()}, {typeof(TDictionaryKey).Name}, {typeof(TElement).Name}>: [ \n\t{o},\n\tKeySelector = {ExpressionHelper.TranslateToString(keySelector)},\n\tElementSelector = {ExpressionHelper.TranslateToString(elementSelector)} ]";
        }

        private static string FormatGetOrSetGroupKey<T, TGroupKey, TResult>([CanBeNull] IQueryOptions<T> options, [NotNull] Expression<Func<T, TGroupKey>> keySelector, [NotNull] Expression<Func<TGroupKey, IEnumerable<T>, TResult>> resultSelector)
        {
            Guard.NotNull(keySelector, nameof(keySelector));
            Guard.NotNull(resultSelector, nameof(resultSelector));

            var o = FormatQueryOptions<T>(options);

            return $"GetOrSetGroup<{Name<T>()}, {typeof(TGroupKey).Name}, {typeof(TResult).Name}>: [ \n\t{o},\n\tKeySelector = {ExpressionHelper.TranslateToString(keySelector)},\n\tResultSelector = {ExpressionHelper.TranslateToString(resultSelector)} ]";
        }
    }
}