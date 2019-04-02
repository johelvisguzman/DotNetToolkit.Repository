namespace DotNetToolkit.Repository.Extensions
{
    using Configuration.Caching;
    using Configuration.Logging;
    using Helpers;
    using Microsoft.Extensions.Caching.Memory;
    using Queries;
    using Queries.Strategies;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains various extension methods for <see cref="ICacheProvider" />
    /// </summary>
    internal static class CachingProviderExtensions
    {
        private static readonly object _syncRoot = new object();

        public static IQueryResult<IEnumerable<T>> GetOrSetExecuteSqlQuery<T>(this ICacheProvider cacheProvider, string sql, CommandType cmdType, Dictionary<string, object> parameters, Func<IDataReader, T> projector, Func<IQueryResult<IEnumerable<T>>> getter, ILogger logger)
            => GetOrSet<IEnumerable<T>>(
                cacheProvider,
                FormatGetOrSetExecuteSqlQueryKey<T>(sql, cmdType, parameters),
                getter,
                logger);

        public static IQueryResult<int> GetOrSetExecuteSqlCommand<T>(this ICacheProvider cacheProvider, string sql, CommandType cmdType, Dictionary<string, object> parameters, Func<IQueryResult<int>> getter, ILogger logger)
            => GetOrSet<int>(
                cacheProvider,
                FormatGetOrSetExecuteSqlCommandKey<T>(sql, cmdType, parameters),
                getter,
                logger);

        public static IQueryResult<T> GetOrSetFind<T>(this ICacheProvider cacheProvider, object[] keys, IFetchQueryStrategy<T> fetchStrategy, Func<IQueryResult<T>> getter, ILogger logger)
            => GetOrSet<T>(
                cacheProvider,
                FormatGetOrSetFindKey<T>(keys, fetchStrategy),
                getter,
                logger);

        public static IQueryResult<TResult> GetOrSetFind<T, TResult>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TResult>> selector, Func<IQueryResult<TResult>> getter, ILogger logger)
            => GetOrSet<TResult>(
                cacheProvider,
                FormatGetOrSetFindKey<T, TResult>(options, selector),
                getter,
                logger);

        public static IPagedQueryResult<IEnumerable<TResult>> GetOrSetFindAll<T, TResult>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TResult>> selector, Func<IPagedQueryResult<IEnumerable<TResult>>> getter, ILogger logger)
            => GetOrSet<IEnumerable<TResult>>(
                cacheProvider,
                FormatGetOrSetFindAllKey<T, TResult>(options, selector),
                getter,
                logger);

        public static IQueryResult<int> GetOrSetCount<T>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Func<IQueryResult<int>> getter, ILogger logger)
            => GetOrSet<int>(
                cacheProvider,
                FormatGetOrSetCountKey<T>(options),
                getter,
                logger);

        public static IPagedQueryResult<Dictionary<TDictionaryKey, TElement>> GetOrSetDictionary<T, TDictionaryKey, TElement>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TDictionaryKey>> keySelector, Expression<Func<T, TElement>> elementSelector, Func<IPagedQueryResult<Dictionary<TDictionaryKey, TElement>>> getter, ILogger logger)
            => GetOrSet<Dictionary<TDictionaryKey, TElement>>(
                cacheProvider,
                FormatGetOrSetDictionaryKey<T, TDictionaryKey, TElement>(options, keySelector, elementSelector),
                getter,
                logger);

        public static IPagedQueryResult<IEnumerable<TResult>> GetOrSetGroup<T, TGroupKey, TResult>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<T>, TResult>> resultSelector, Func<IPagedQueryResult<IEnumerable<TResult>>> getter, ILogger logger)
            => GetOrSet<IEnumerable<TResult>>(
                cacheProvider,
                FormatGetOrSetGroupKey<T, TGroupKey, TResult>(options, keySelector, resultSelector),
                getter,
                logger);

        public static Task<IQueryResult<IEnumerable<T>>> GetOrSetExecuteSqlQueryAsync<T>(this ICacheProvider cacheProvider, string sql, CommandType cmdType, Dictionary<string, object> parameters, Func<IDataReader, T> projector, Func<Task<IQueryResult<IEnumerable<T>>>> getter, ILogger logger)
            => GetOrSetAsync<IEnumerable<T>>(
                cacheProvider,
                FormatGetOrSetExecuteSqlQueryKey<T>(sql, cmdType, parameters),
                getter,
                logger);

        public static Task<IQueryResult<int>> GetOrSetExecuteSqlCommandAsync<T>(this ICacheProvider cacheProvider, string sql, CommandType cmdType, Dictionary<string, object> parameters, Func<Task<IQueryResult<int>>> getter, ILogger logger)
            => GetOrSetAsync<int>(
                cacheProvider,
                FormatGetOrSetExecuteSqlCommandKey<T>(sql, cmdType, parameters),
                getter,
                logger);

        public static Task<IQueryResult<T>> GetOrSetFindAsync<T>(this ICacheProvider cacheProvider, object[] keys, IFetchQueryStrategy<T> fetchStrategy, Func<Task<IQueryResult<T>>> getter, ILogger logger)
            => GetOrSetAsync<T>(
                cacheProvider,
                FormatGetOrSetFindKey<T>(keys, fetchStrategy),
                getter,
                logger);

        public static Task<IQueryResult<TResult>> GetOrSetFindAsync<T, TResult>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TResult>> selector, Func<Task<IQueryResult<TResult>>> getter, ILogger logger)
            => GetOrSetAsync<TResult>(
                cacheProvider,
                FormatGetOrSetFindKey<T, TResult>(options, selector),
                getter,
                logger);

        public static Task<IPagedQueryResult<IEnumerable<TResult>>> GetOrSetFindAllAsync<T, TResult>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TResult>> selector, Func<Task<IPagedQueryResult<IEnumerable<TResult>>>> getter, ILogger logger)
            => GetOrSetAsync<IEnumerable<TResult>>(
                cacheProvider,
                FormatGetOrSetFindAllKey<T, TResult>(options, selector),
                getter,
                logger);

        public static Task<IQueryResult<int>> GetOrSetCountAsync<T>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Func<Task<IQueryResult<int>>> getter, ILogger logger)
            => GetOrSetAsync<int>(
                cacheProvider,
                FormatGetOrSetCountKey<T>(options),
                getter,
                logger);

        public static Task<IPagedQueryResult<Dictionary<TDictionaryKey, TElement>>> GetOrSetDictionaryAsync<T, TDictionaryKey, TElement>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TDictionaryKey>> keySelector, Expression<Func<T, TElement>> elementSelector, Func<Task<IPagedQueryResult<Dictionary<TDictionaryKey, TElement>>>> getter, ILogger logger)
            => GetOrSetAsync<Dictionary<TDictionaryKey, TElement>>(
                cacheProvider,
                FormatGetOrSetDictionaryKey<T, TDictionaryKey, TElement>(options, keySelector, elementSelector),
                getter,
                logger);

        public static Task<IPagedQueryResult<IEnumerable<TResult>>> GetOrSetGroupAsync<T, TGroupKey, TResult>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<T>, TResult>> resultSelector, Func<Task<IPagedQueryResult<IEnumerable<TResult>>>> getter, ILogger logger)
            => GetOrSetAsync<IEnumerable<TResult>>(
                cacheProvider,
                FormatGetOrSetGroupKey<T, TGroupKey, TResult>(options, keySelector, resultSelector),
                getter,
                logger);

        private static Task<IQueryResult<T>> GetOrSetAsync<T>(ICacheProvider cacheProvider, string key, Func<Task<IQueryResult<T>>> getter, TimeSpan? expiry, ILogger logger)
            => GetOrSetAsync<T>(
                cacheProvider,
                key,
                getter,
                CacheItemPriority.Normal,
                expiry,
                logger);

        private static Task<IQueryResult<T>> GetOrSetAsync<T>(ICacheProvider cacheProvider, string key, Func<Task<IQueryResult<T>>> getter, ILogger logger)
            => GetOrSetAsync<T>(
                cacheProvider,
                key,
                getter,
                cacheProvider.Expiry,
                logger);

        private static IQueryResult<T> GetOrSet<T>(ICacheProvider cacheProvider, string key, Func<IQueryResult<T>> getter, TimeSpan? expiry, ILogger logger)
            => GetOrSet<T>(
                cacheProvider,
                key,
                getter,
                CacheItemPriority.Normal,
                expiry,
                logger);

        private static IQueryResult<T> GetOrSet<T>(ICacheProvider cacheProvider, string key, Func<IQueryResult<T>> getter, ILogger logger)
            => GetOrSet<T>(
                cacheProvider,
                key,
                getter,
                cacheProvider.Expiry,
                logger);

        private static Task<IPagedQueryResult<T>> GetOrSetAsync<T>(ICacheProvider cacheProvider, string key, Func<Task<IPagedQueryResult<T>>> getter, TimeSpan? expiry, ILogger logger)
            => GetOrSetAsync<T>(
                cacheProvider,
                key,
                getter,
                CacheItemPriority.Normal,
                expiry,
                logger);

        private static Task<IPagedQueryResult<T>> GetOrSetAsync<T>(ICacheProvider cacheProvider, string key, Func<Task<IPagedQueryResult<T>>> getter, ILogger logger)
            => GetOrSetAsync<T>(
                cacheProvider,
                key,
                getter,
                cacheProvider.Expiry,
                logger);

        private static IPagedQueryResult<T> GetOrSet<T>(ICacheProvider cacheProvider, string key, Func<IPagedQueryResult<T>> getter, TimeSpan? expiry, ILogger logger)
            => GetOrSet<T>(
                cacheProvider,
                key,
                getter,
                CacheItemPriority.Normal,
                expiry,
                logger);

        private static IPagedQueryResult<T> GetOrSet<T>(ICacheProvider cacheProvider, string key, Func<IPagedQueryResult<T>> getter, ILogger logger)
            => GetOrSet<T>(
                cacheProvider,
                key,
                getter,
                cacheProvider.Expiry,
                logger);

        private static PagedQueryResult<T> CreatePagedQueryResult<T>(IPagedQueryResult<T> oldValue, bool cachedUsed = false)
            => new PagedQueryResult<T>(oldValue.Result, oldValue.Total)
            {
                CacheUsed = cachedUsed
            };

        private static QueryResult<T> CreateQueryResult<T>(IQueryResult<T> oldValue, bool cachedUsed = false)
            => new QueryResult<T>(oldValue.Result)
            {
                CacheUsed = cachedUsed
            };

        private static void SetValue<T>(this ICacheProvider cacheProvider, string hashedKey, string key, IQueryResult<T> value, CacheItemPriority priority, TimeSpan? expiry, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (hashedKey == null)
                throw new ArgumentNullException(nameof(hashedKey));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            lock (_syncRoot)
            {
                logger.Debug(expiry.HasValue
                        ? $"Setting up cache for '{hashedKey}' expire handling in {expiry.Value.TotalSeconds} seconds"
                        : $"Setting up cache for '{hashedKey}'");

                cacheProvider.Cache.Set<IQueryResult<T>>(
                    hashedKey,
                    value,
                    priority,
                    expiry,
                    reason => logger.Debug($"Cache for '{hashedKey}' has expired. Evicting from cache for '{reason}'"));
            }
        }

        private static QueryResult<T> GetOrSet<T>(ICacheProvider cacheProvider, string key, Func<IQueryResult<T>> getter, CacheItemPriority priority, TimeSpan? expiry, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (getter == null)
                throw new ArgumentNullException(nameof(getter));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            var hashedKey = FormatHashedKey<T>(cacheProvider, key);

            if (!cacheProvider.Cache.TryGetValue<QueryResult<T>>(hashedKey, out var value))
            {
                value = CreateQueryResult(getter(), cachedUsed: false);

                cacheProvider.SetValue(hashedKey, key, value, priority, expiry, logger);
            }
            else
            {
                value = CreateQueryResult(value, cachedUsed: true);
            }

            return value;
        }

        private static async Task<IQueryResult<T>> GetOrSetAsync<T>(ICacheProvider cacheProvider, string key, Func<Task<IQueryResult<T>>> getter, CacheItemPriority priority, TimeSpan? expiry, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (getter == null)
                throw new ArgumentNullException(nameof(getter));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            var hashedKey = FormatHashedKey<T>(cacheProvider, key);

            if (!cacheProvider.Cache.TryGetValue<QueryResult<T>>(hashedKey, out var value))
            {
                value = CreateQueryResult(await getter(), cachedUsed: false);

                cacheProvider.SetValue(hashedKey, key, value, priority, expiry, logger);
            }
            else
            {
                value = CreateQueryResult(value, cachedUsed: true);
            }

            return value;
        }

        private static IPagedQueryResult<T> GetOrSet<T>(ICacheProvider cacheProvider, string key, Func<IPagedQueryResult<T>> getter, CacheItemPriority priority, TimeSpan? expiry, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (getter == null)
                throw new ArgumentNullException(nameof(getter));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            var hashedKey = FormatHashedKey<T>(cacheProvider, key);

            if (!cacheProvider.Cache.TryGetValue<PagedQueryResult<T>>(hashedKey, out var value))
            {
                value = CreatePagedQueryResult(getter(), cachedUsed: false);

                cacheProvider.SetValue(hashedKey, key, value, priority, expiry, logger);
            }
            else
            {
                value = CreatePagedQueryResult(value, cachedUsed: true);
            }

            return value;
        }

        private static async Task<IPagedQueryResult<T>> GetOrSetAsync<T>(ICacheProvider cacheProvider, string key, Func<Task<IPagedQueryResult<T>>> getter, CacheItemPriority priority, TimeSpan? expiry, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (getter == null)
                throw new ArgumentNullException(nameof(getter));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            var hashedKey = FormatHashedKey<T>(cacheProvider, key);

            if (!cacheProvider.Cache.TryGetValue<PagedQueryResult<T>>(hashedKey, out var value))
            {
                value = CreatePagedQueryResult(await getter(), cachedUsed: false);

                cacheProvider.SetValue(hashedKey, key, value, priority, expiry, logger);
            }
            else
            {
                value = CreatePagedQueryResult(value, cachedUsed: true);
            }

            return value;
        }

        public static void IncrementCounter<T>(this ICacheProvider cacheProvider)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            cacheProvider.Cache.Increment(FormatCachePrefixCounterKey<T>(), 1, 1, CacheItemPriority.NeverRemove);
        }

        private static int GetCachingPrefixCounter<T>(this ICacheProvider cacheProvider)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            return !cacheProvider.Cache.TryGetValue<int>(FormatCachePrefixCounterKey<T>(), out var key) ? 1 : key;
        }

        private static string Name<T>() => typeof(T).FullName;
        private static string CacheGlue => CacheProviderManager.CachePrefixGlue;

        private static string FormatCachePrefixCounterKey<T>()
        {
            return string.Format("{1}{0}{2}",
                CacheGlue,
                Name<T>(),
                CacheProviderManager.CacheCounterPrefix);
        }

        private static string FormatHashedKey<T>(ICacheProvider cacheProvider, string key)
        {
            return string.Format("{1}{0}{2}{0}{3}{0}{4}",
                CacheGlue,
                CacheProviderManager.GlobalCachingPrefixCounter,
                cacheProvider.GetCachingPrefixCounter<T>(),
                key.ToSHA256(),
                CacheProviderManager.CachePrefix);
        }

        private static string FormatQueryOptions<T>(IQueryOptions<T> options)
        {
            return options != null ? options.ToString() : $"QueryOptions<{Name<T>()}>: [ null ]";
        }

        private static string FormatFetchQueryStrategy<T>(IFetchQueryStrategy<T> fetchStrategy)
        {
            return fetchStrategy != null ? fetchStrategy.ToString() : $"FetchQueryStrategy<{Name<T>()}>: [ null ]";
        }

        private static string FormatGetOrSetExecuteSqlQueryKey<T>(string sql, CommandType cmdType, Dictionary<string, object> parameters)
        {
            var sb = new StringBuilder();

            sb.Append($"GetOrSetExecuteSqlQuery<{Name<T>()}>: [ \n\tSql = {sql},");

            if (parameters != null && parameters.Any())
            {
                sb.Append($"\n\tParameters = {parameters.ToDebugString()},");
            }

            sb.Append($"\n\tCommandType = {cmdType} ]");

            return sb.ToString();
        }

        private static string FormatGetOrSetExecuteSqlCommandKey<T>(string sql, CommandType cmdType, Dictionary<string, object> parameters)
        {
            var sb = new StringBuilder();

            sb.Append($"GetOrSetExecuteSqlCommand<{Name<T>()}>: [ \n\tSql = {sql},");

            if (parameters != null && parameters.Any())
            {
                sb.Append($"\n\tParameters = {parameters.ToDebugString()},");
            }

            sb.Append($"\n\tCommandType = {cmdType} ]");

            return sb.ToString();
        }

        private static string FormatGetOrSetFindKey<T>(object[] keys, IFetchQueryStrategy<T> fetchStrategy)
        {
            var f = FormatFetchQueryStrategy(fetchStrategy);

            return $"GetOrSetFind<{Name<T>()}>: [ \n\tKeys = {string.Join(", ", keys.Select(x => x.ToString()).ToArray())},\n\t{f} ]";
        }

        private static string FormatGetOrSetFindAllKey<T, TResult>(IQueryOptions<T> options, Expression<Func<T, TResult>> selector)
        {
            var o = FormatQueryOptions<T>(options);

            return $"GetOrSetFindAll<{Name<T>()}>: [ \n\t{o},\n\tSelector = {ExpressionHelper.TranslateToString(selector)} ]";
        }

        private static string FormatGetOrSetFindKey<T, TResult>(IQueryOptions<T> options, Expression<Func<T, TResult>> selector)
        {
            var o = FormatQueryOptions<T>(options);

            return $"GetOrSetFind<{Name<T>()}>: [ \n\t{o},\n\tSelector = {ExpressionHelper.TranslateToString(selector)} ]";
        }

        private static string FormatGetOrSetCountKey<T>(IQueryOptions<T> options)
        {
            var o = FormatQueryOptions<T>(options);

            return $"GetOrSetCount<{Name<T>()}>: [ \n\t{o} ]";
        }

        private static string FormatGetOrSetDictionaryKey<T, TDictionaryKey, TElement>(IQueryOptions<T> options, Expression<Func<T, TDictionaryKey>> keySelector, Expression<Func<T, TElement>> elementSelector)
        {
            var o = FormatQueryOptions<T>(options);

            return $"GetOrSetDictionary<{Name<T>()}, {typeof(TDictionaryKey).Name}, {typeof(TElement).Name}>: [ \n\t{o},\n\tKeySelector = {ExpressionHelper.TranslateToString(keySelector)},\n\tElementSelector = {ExpressionHelper.TranslateToString(elementSelector)} ]";
        }

        private static string FormatGetOrSetGroupKey<T, TGroupKey, TResult>(IQueryOptions<T> options, Expression<Func<T, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<T>, TResult>> resultSelector)
        {
            var o = FormatQueryOptions<T>(options);

            return $"GetOrSetGroup<{Name<T>()}, {typeof(TGroupKey).Name}, {typeof(TResult).Name}>: [ \n\t{o},\n\tKeySelector = {ExpressionHelper.TranslateToString(keySelector)},\n\tResultSelector = {ExpressionHelper.TranslateToString(resultSelector)} ]";
        }
    }
}