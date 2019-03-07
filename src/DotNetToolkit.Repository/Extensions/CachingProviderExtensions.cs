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

        public static IQueryResult<T> GetOrSet<T>(this ICacheProvider cacheProvider, object[] keys, IFetchQueryStrategy<T> fetchStrategy, Func<IQueryResult<T>> getter, ILogger logger)
            => GetOrSet<T>(
                cacheProvider,
                FormatGetOrSetKey<T>(keys, fetchStrategy),
                getter,
                logger);

        public static IQueryResult<TResult> GetOrSet<T, TResult>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TResult>> selector, Func<IQueryResult<TResult>> getter, ILogger logger)
            => GetOrSet<TResult>(
                cacheProvider,
                FormatGetOrSetKey<T, TResult>(options, selector),
                getter,
                logger);

        public static IQueryResult<IEnumerable<TResult>> GetOrSetAll<T, TResult>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TResult>> selector, Func<IQueryResult<IEnumerable<TResult>>> getter, ILogger logger)
            => GetOrSet<IEnumerable<TResult>>(
                cacheProvider,
                FormatGetOrSetAllKey<T, TResult>(options, selector),
                getter,
                logger);

        public static IQueryResult<int> GetOrSetCount<T>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Func<IQueryResult<int>> getter, ILogger logger)
            => GetOrSet<int>(
                cacheProvider,
                FormatGetOrSetCountKey<T>(options),
                getter,
                logger);

        public static IQueryResult<Dictionary<TDictionaryKey, TElement>> GetOrSetDictionary<T, TDictionaryKey, TElement>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TDictionaryKey>> keySelector, Expression<Func<T, TElement>> elementSelector, Func<IQueryResult<Dictionary<TDictionaryKey, TElement>>> getter, ILogger logger)
            => GetOrSet<Dictionary<TDictionaryKey, TElement>>(
                cacheProvider,
                FormatGetOrSetDictionaryKey<T, TDictionaryKey, TElement>(options, keySelector, elementSelector),
                getter,
                logger);

        public static IQueryResult<IEnumerable<TResult>> GetOrSetGroup<T, TGroupKey, TResult>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<T>, TResult>> resultSelector, Func<IQueryResult<IEnumerable<TResult>>> getter, ILogger logger)
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

        public static Task<IQueryResult<T>> GetOrSetAsync<T>(this ICacheProvider cacheProvider, object[] keys, IFetchQueryStrategy<T> fetchStrategy, Func<Task<IQueryResult<T>>> getter, ILogger logger)
            => GetOrSetAsync<T>(
                cacheProvider,
                FormatGetOrSetKey<T>(keys, fetchStrategy),
                getter,
                logger);

        public static Task<IQueryResult<TResult>> GetOrSetAsync<T, TResult>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TResult>> selector, Func<Task<IQueryResult<TResult>>> getter, ILogger logger)
            => GetOrSetAsync<TResult>(
                cacheProvider,
                FormatGetOrSetKey<T, TResult>(options, selector),
                getter,
                logger);

        public static Task<IQueryResult<IEnumerable<TResult>>> GetOrSetAllAsync<T, TResult>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TResult>> selector, Func<Task<IQueryResult<IEnumerable<TResult>>>> getter, ILogger logger)
            => GetOrSetAsync<IEnumerable<TResult>>(
                cacheProvider,
                FormatGetOrSetAllKey<T, TResult>(options, selector),
                getter,
                logger);

        public static Task<IQueryResult<int>> GetOrSetCountAsync<T>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Func<Task<IQueryResult<int>>> getter, ILogger logger)
            => GetOrSetAsync<int>(
                cacheProvider,
                FormatGetOrSetCountKey<T>(options),
                getter,
                logger);

        public static Task<IQueryResult<Dictionary<TDictionaryKey, TElement>>> GetOrSetDictionaryAsync<T, TDictionaryKey, TElement>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TDictionaryKey>> keySelector, Expression<Func<T, TElement>> elementSelector, Func<Task<IQueryResult<Dictionary<TDictionaryKey, TElement>>>> getter, ILogger logger)
            => GetOrSetAsync<Dictionary<TDictionaryKey, TElement>>(
                cacheProvider,
                FormatGetOrSetDictionaryKey<T, TDictionaryKey, TElement>(options, keySelector, elementSelector),
                getter,
                logger);

        public static Task<IQueryResult<IEnumerable<TResult>>> GetOrSetGroupAsync<T, TGroupKey, TResult>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<T>, TResult>> resultSelector, Func<Task<IQueryResult<IEnumerable<TResult>>>> getter, ILogger logger)
            => GetOrSetAsync<IEnumerable<TResult>>(
                cacheProvider,
                FormatGetOrSetGroupKey<T, TGroupKey, TResult>(options, keySelector, resultSelector),
                getter,
                logger);

        private static Task<IQueryResult<T>> GetOrSetAsync<T>(ICacheProvider cacheProvider, string key, Func<Task<IQueryResult<T>>> getter, TimeSpan cacheExpiration, ILogger logger)
            => GetOrSetAsync<T>(
                cacheProvider,
                key,
                getter,
                CacheItemPriority.Normal,
                cacheExpiration,
                logger);

        private static Task<IQueryResult<T>> GetOrSetAsync<T>(ICacheProvider cacheProvider, string key, Func<Task<IQueryResult<T>>> getter, ILogger logger)
            => GetOrSetAsync<T>(
                cacheProvider,
                key,
                getter,
                cacheProvider.CacheExpiration ?? TimeSpan.Zero,
                logger);

        private static IQueryResult<T> GetOrSet<T>(ICacheProvider cacheProvider, string key, Func<IQueryResult<T>> getter, TimeSpan cacheExpiration, ILogger logger)
            => GetOrSet<T>(
                cacheProvider,
                key,
                getter,
                CacheItemPriority.Normal,
                cacheExpiration,
                logger);

        private static IQueryResult<T> GetOrSet<T>(ICacheProvider cacheProvider, string key, Func<IQueryResult<T>> getter, ILogger logger)
            => GetOrSet<T>(
                cacheProvider,
                key,
                getter,
                cacheProvider.CacheExpiration ?? TimeSpan.Zero,
                logger);

        private static bool TryGetValue<T>(this ICacheProvider cacheProvider, string key, out IQueryResult<T> value)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            lock (_syncRoot)
            {
                if (cacheProvider.Cache.TryGetValue(key, out var obj))
                {
                    var oldValue = (IQueryResult<T>)obj;

                    value = new QueryResult<T>(oldValue.Result, oldValue.Total)
                    {
                        CacheUsed = true
                    };

                    return true;
                }
            }

            value = default(IQueryResult<T>);

            return false;
        }

        private static void SetValue<T>(this ICacheProvider cacheProvider, string hashedKey, string key, IQueryResult<T> value, CacheItemPriority priority, TimeSpan cacheExpiration, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (hashedKey == null)
                throw new ArgumentNullException(nameof(hashedKey));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (cacheExpiration == null)
                throw new ArgumentNullException(nameof(cacheExpiration));

            lock (_syncRoot)
            {
                logger.Debug(cacheExpiration != TimeSpan.Zero
                        ? $"Setting up cache for '{hashedKey}' expire handling in {cacheExpiration.TotalSeconds} seconds"
                        : $"Setting up cache for '{hashedKey}'");

                cacheProvider.Cache.Set(
                    hashedKey,
                    value,
                    priority,
                    cacheExpiration,
                    reason => logger.Debug($"Cache for '{hashedKey}' has expired. Evicting from cache for '{reason}'"));
            }
        }

        private static IQueryResult<T> GetOrSet<T>(ICacheProvider cacheProvider, string key, Func<IQueryResult<T>> getter, CacheItemPriority priority, TimeSpan cacheExpiration, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (getter == null)
                throw new ArgumentNullException(nameof(getter));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (cacheExpiration == null)
                throw new ArgumentNullException(nameof(cacheExpiration));

            var hashedKey = FormatHashKey(key);

            if (!cacheProvider.TryGetValue<T>(hashedKey, out var value))
            {
                value = getter();

                cacheProvider.SetValue(hashedKey, key, value, priority, cacheExpiration, logger);
            }

            return value;
        }

        private static async Task<IQueryResult<T>> GetOrSetAsync<T>(ICacheProvider cacheProvider, string key, Func<Task<IQueryResult<T>>> getter, CacheItemPriority priority, TimeSpan cacheExpiration, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (getter == null)
                throw new ArgumentNullException(nameof(getter));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (cacheExpiration == null)
                throw new ArgumentNullException(nameof(cacheExpiration));

            var hashedKey = FormatHashKey(key);

            if (!cacheProvider.TryGetValue<T>(hashedKey, out var value))
            {
                value = await getter();

                cacheProvider.SetValue(hashedKey, key, value, priority, cacheExpiration, logger);
            }

            return value;
        }

        private static string FormatHashKey(string key)
        {
            return $"{CacheProviderManager.CachePrefix}_{CacheProviderManager.GlobalCachingPrefixCounter}_{key.ToSHA256()}";
        }

        private static string FormatQueryOptions<T>(IQueryOptions<T> options)
        {
            return options != null ? options.ToString() : $"QueryOptions<{typeof(T).Name}>: [ null ]";
        }

        private static string FormatFetchQueryStrategy<T>(IFetchQueryStrategy<T> fetchStrategy)
        {
            return fetchStrategy != null ? fetchStrategy.ToString() : $"FetchQueryStrategy<{typeof(T).Name}>: [ null ]";
        }

        private static string FormatGetOrSetExecuteSqlQueryKey<T>(string sql, CommandType cmdType, Dictionary<string, object> parameters)
        {
            var sb = new StringBuilder();

            sb.Append($"GetOrSetExecuteSqlQuery<{typeof(T).Name}>: [ \n\tSql = {sql},");

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

            sb.Append($"GetOrSetExecuteSqlCommand<{typeof(T).Name}>: [ \n\tSql = {sql},");

            if (parameters != null && parameters.Any())
            {
                sb.Append($"\n\tParameters = {parameters.ToDebugString()},");
            }

            sb.Append($"\n\tCommandType = {cmdType} ]");

            return sb.ToString();
        }

        private static string FormatGetOrSetKey<T>(object[] keys, IFetchQueryStrategy<T> fetchStrategy)
        {
            var f = FormatFetchQueryStrategy(fetchStrategy);

            return $"GetOrSet<{typeof(T).Name}>: [ \n\tKeys = {string.Join(", ", keys.Select(x => x.ToString()).ToArray())},\n\t{f} ]";
        }

        private static string FormatGetOrSetAllKey<T, TResult>(IQueryOptions<T> options, Expression<Func<T, TResult>> selector)
        {
            var o = FormatQueryOptions<T>(options);

            return $"GetOrSetAll<{typeof(T).Name}>: [ \n\t{o},\n\tSelector = {ExpressionHelper.TranslateToString(selector)} ]";
        }

        private static string FormatGetOrSetKey<T, TResult>(IQueryOptions<T> options, Expression<Func<T, TResult>> selector)
        {
            return $"GetOrSet<{typeof(T).Name}>: [ \n\t{options},\n\tSelector = {ExpressionHelper.TranslateToString(selector)} ]";
        }

        private static string FormatGetOrSetCountKey<T>(IQueryOptions<T> options)
        {
            var o = FormatQueryOptions<T>(options);

            return $"GetOrSetCount<{typeof(T).Name}>: [ \n\t{o} ]";
        }

        private static string FormatGetOrSetDictionaryKey<T, TDictionaryKey, TElement>(IQueryOptions<T> options, Expression<Func<T, TDictionaryKey>> keySelector, Expression<Func<T, TElement>> elementSelector)
        {
            var o = FormatQueryOptions<T>(options);

            return $"GetOrSetDictionary<{typeof(T).Name}, {typeof(TDictionaryKey).Name}, {typeof(TElement).Name}>: [ \n\t{o},\n\tKeySelector = {ExpressionHelper.TranslateToString(keySelector)},\n\tElementSelector = {ExpressionHelper.TranslateToString(elementSelector)} ]";
        }

        private static string FormatGetOrSetGroupKey<T, TGroupKey, TResult>(IQueryOptions<T> options, Expression<Func<T, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<T>, TResult>> resultSelector)
        {
            var o = FormatQueryOptions<T>(options);

            return $"GetOrSetGroup<{typeof(T).Name}, {typeof(TGroupKey).Name}, {typeof(TResult).Name}>: [ \n\t{o},\n\tKeySelector = {ExpressionHelper.TranslateToString(keySelector)},\n\tResultSelector = {ExpressionHelper.TranslateToString(resultSelector)} ]";
        }
    }
}