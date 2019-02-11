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
        private static bool TryGetValue<T>(this ICacheProvider cacheProvider, string key, out T value)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (!cacheProvider.Cache.TryGetValue(key, out var obj))
            {
                value = default(T);

                return false;
            }

            value = (T)obj;

            return true;
        }

        private static void SetValue<T>(this ICacheProvider cacheProvider, string hashedKey, string key, QueryResult<T> value, CacheItemPriority priority, TimeSpan cacheExpiration, ILogger logger)
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

        private static QueryResult<T> GetOrSet<T>(this ICacheProvider cacheProvider, string key, Func<QueryResult<T>> getter, CacheItemPriority priority, TimeSpan cacheExpiration, ILogger logger)
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

            if (!cacheProvider.TryGetValue<QueryResult<T>>(hashedKey, out var value))
            {
                value = getter();

                cacheProvider.SetValue(hashedKey, key, value, priority, cacheExpiration, logger);
            }
            else
            {
                value.CacheUsed = true;
            }

            return value;
        }

        private static QueryResult<T> GetOrSet<T>(this ICacheProvider cacheProvider, string key, Func<QueryResult<T>> getter, TimeSpan cacheExpiration, ILogger logger)
        {
            return cacheProvider.GetOrSet<T>(key, getter, CacheItemPriority.Normal, cacheExpiration, logger);
        }

        private static QueryResult<T> GetOrSet<T>(this ICacheProvider cacheProvider, string key, Func<QueryResult<T>> getter, ILogger logger)
        {
            return cacheProvider.GetOrSet<T>(key, getter, cacheProvider.CacheExpiration ?? TimeSpan.Zero, logger);
        }

        public static QueryResult<IEnumerable<T>> GetOrSetExecuteSqlQuery<T>(this ICacheProvider cacheProvider, string sql, CommandType cmdType, object[] parameters, Func<IDataReader, T> projector, Func<QueryResult<IEnumerable<T>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (sql == null)
                throw new ArgumentNullException(nameof(sql));

            if (projector == null)
                throw new ArgumentNullException(nameof(projector));

            var key = FormatGetOrSetExecuteSqlQueryKey<T>(sql, cmdType, parameters);

            return cacheProvider.GetOrSet<IEnumerable<T>>(key, getter, logger);
        }

        public static QueryResult<int> GetOrSetExecuteSqlCommand<T>(this ICacheProvider cacheProvider, string sql, CommandType cmdType, object[] parameters, Func<QueryResult<int>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (sql == null)
                throw new ArgumentNullException(nameof(sql));

            var key = FormatGetOrSetExecuteSqlCommandKey<T>(sql, cmdType, parameters);

            return cacheProvider.GetOrSet<int>(key, getter, logger);
        }
        
        public static QueryResult<T> GetOrSet<T>(this ICacheProvider cacheProvider, object[] keys, IFetchQueryStrategy<T> fetchStrategy, Func<QueryResult<T>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            var key = FormatGetOrSetKey<T>(keys, fetchStrategy);

            return cacheProvider.GetOrSet<T>(key, getter, logger);
        }

        public static QueryResult<TResult> GetOrSet<T, TResult>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TResult>> selector, Func<QueryResult<TResult>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var key = FormatGetOrSetKey<T, TResult>(options, selector);

            return cacheProvider.GetOrSet<TResult>(key, getter, logger);
        }

        public static QueryResult<IEnumerable<TResult>> GetOrSetAll<T, TResult>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TResult>> selector, Func<QueryResult<IEnumerable<TResult>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var key = FormatGetOrSetAllKey<T, TResult>(options, selector);

            return cacheProvider.GetOrSet<IEnumerable<TResult>>(key, getter, logger);
        }

        public static QueryResult<int> GetOrSetCount<T>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Func<QueryResult<int>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            var key = FormatGetOrSetCountKey<T>(options);

            return cacheProvider.GetOrSet<int>(key, getter, logger);
        }

        public static QueryResult<Dictionary<TDictionaryKey, TElement>> GetOrSetDictionary<T, TDictionaryKey, TElement>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TDictionaryKey>> keySelector, Expression<Func<T, TElement>> elementSelector, Func<QueryResult<Dictionary<TDictionaryKey, TElement>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            var key = FormatGetOrSetDictionaryKey<T, TDictionaryKey, TElement>(options, keySelector, elementSelector);

            return cacheProvider.GetOrSet<Dictionary<TDictionaryKey, TElement>>(key, getter, logger);
        }
        
        public static QueryResult<IEnumerable<TResult>> GetOrSetGroup<T, TGroupKey, TResult>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<T>, TResult>> resultSelector, Func<QueryResult<IEnumerable<TResult>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            var key = FormatGetOrSetGroupKey<T, TGroupKey, TResult>(options, keySelector, resultSelector);

            return cacheProvider.GetOrSet<IEnumerable<TResult>>(key, getter, logger);
        }

        private static async Task<QueryResult<T>> GetOrSetAsync<T>(this ICacheProvider cacheProvider, string key, Func<Task<QueryResult<T>>> getter, CacheItemPriority priority, TimeSpan cacheExpiration, ILogger logger)
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

            if (!cacheProvider.TryGetValue<QueryResult<T>>(hashedKey, out var value))
            {
                value = await getter();

                cacheProvider.SetValue(hashedKey, key, value, priority, cacheExpiration, logger);
            }
            else
            {
                value.CacheUsed = true;
            }

            return value;
        }

        private static Task<QueryResult<T>> GetOrSetAsync<T>(this ICacheProvider cacheProvider, string key, Func<Task<QueryResult<T>>> getter, TimeSpan cacheExpiration, ILogger logger)
        {
            return cacheProvider.GetOrSetAsync<T>(key, getter, CacheItemPriority.Normal, cacheExpiration, logger);
        }

        private static Task<QueryResult<T>> GetOrSetAsync<T>(this ICacheProvider cacheProvider, string key, Func<Task<QueryResult<T>>> getter, ILogger logger)
        {
            return cacheProvider.GetOrSetAsync<T>(key, getter, cacheProvider.CacheExpiration ?? TimeSpan.Zero, logger);
        }

        public static Task<QueryResult<IEnumerable<T>>> GetOrSetExecuteSqlQueryAsync<T>(this ICacheProvider cacheProvider, string sql, CommandType cmdType, object[] parameters, Func<IDataReader, T> projector, Func<Task<QueryResult<IEnumerable<T>>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (sql == null)
                throw new ArgumentNullException(nameof(sql));

            if (projector == null)
                throw new ArgumentNullException(nameof(projector));

            var key = FormatGetOrSetExecuteSqlQueryKey<T>(sql, cmdType, parameters);

            return cacheProvider.GetOrSetAsync<IEnumerable<T>>(key, getter, logger);
        }

        public static Task<QueryResult<int>> GetOrSetExecuteSqlCommandAsync<T>(this ICacheProvider cacheProvider, string sql, CommandType cmdType, object[] parameters, Func<Task<QueryResult<int>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (sql == null)
                throw new ArgumentNullException(nameof(sql));

            var key = FormatGetOrSetExecuteSqlCommandKey<T>(sql, cmdType, parameters);

            return cacheProvider.GetOrSetAsync<int>(key, getter, logger);
        }

        public static Task<QueryResult<T>> GetOrSetAsync<T>(this ICacheProvider cacheProvider, object[] keys, IFetchQueryStrategy<T> fetchStrategy, Func<Task<QueryResult<T>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            var key = FormatGetOrSetKey<T>(keys, fetchStrategy);

            return cacheProvider.GetOrSetAsync<T>(key, getter, logger);
        }

        public static Task<QueryResult<TResult>> GetOrSetAsync<T, TResult>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TResult>> selector, Func<Task<QueryResult<TResult>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var key = FormatGetOrSetKey<T, TResult>(options, selector);

            return cacheProvider.GetOrSetAsync<TResult>(key, getter, logger);
        }

        public static Task<QueryResult<IEnumerable<TResult>>> GetOrSetAllAsync<T, TResult>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TResult>> selector, Func<Task<QueryResult<IEnumerable<TResult>>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var key = FormatGetOrSetAllKey<T, TResult>(options, selector);

            return cacheProvider.GetOrSetAsync<IEnumerable<TResult>>(key, getter, logger);
        }

        public static Task<QueryResult<int>> GetOrSetCountAsync<T>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Func<Task<QueryResult<int>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            var key = FormatGetOrSetCountKey<T>(options);

            return cacheProvider.GetOrSetAsync<int>(key, getter, logger);
        }

        public static Task<QueryResult<Dictionary<TDictionaryKey, TElement>>> GetOrSetDictionaryAsync<T, TDictionaryKey, TElement>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TDictionaryKey>> keySelector, Expression<Func<T, TElement>> elementSelector, Func<Task<QueryResult<Dictionary<TDictionaryKey, TElement>>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            var key = FormatGetOrSetDictionaryKey<T, TDictionaryKey, TElement>(options, keySelector, elementSelector);

            return cacheProvider.GetOrSetAsync<Dictionary<TDictionaryKey, TElement>>(key, getter, logger);
        }

        public static Task<QueryResult<IEnumerable<TResult>>> GetOrSetGroupAsync<T, TGroupKey, TResult>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<T>, TResult>> resultSelector, Func<Task<QueryResult<IEnumerable<TResult>>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            var key = FormatGetOrSetGroupKey<T, TGroupKey, TResult>(options, keySelector, resultSelector);

            return cacheProvider.GetOrSetAsync<IEnumerable<TResult>>(key, getter, logger);
        }

        private static string FormatHashKey(string key)
        {
            return $"{CacheProviderManager.CachePrefix}_{CacheProviderManager.GlobalCachingPrefixCounter}_{key.ToSHA256()}";
        }

        private static string FormatGetOrSetExecuteSqlQueryKey<T>(string sql, CommandType cmdType, object[] parameters)
        {
            var sb = new StringBuilder();

            sb.Append($"GetOrSetExecuteSqlQuery<{typeof(T).Name}>: [ \n\tSql = {sql},");

            if (parameters != null && parameters.Any())
            {
                sb.Append($"\n\tParameters = {string.Join(", ", parameters.Select(x => x.ToString()).ToArray())},");
            }

            sb.Append($"\n\tCommandType = {cmdType} ]");

            return sb.ToString();
        }

        private static string FormatGetOrSetExecuteSqlCommandKey<T>(string sql, CommandType cmdType, object[] parameters)
        {
            var sb = new StringBuilder();

            sb.Append($"GetOrSetExecuteSqlCommand<{typeof(T).Name}>: [ \n\tSql = {sql},");

            if (parameters != null && parameters.Any())
            {
                sb.Append($"\n\tParameters = {string.Join(", ", parameters.Select(x => x.ToString()).ToArray())},");
            }

            sb.Append($"\n\tCommandType = {cmdType} ]");

            return sb.ToString();
        }

        private static string FormatGetOrSetKey<T>(object[] keys, IFetchQueryStrategy<T> fetchStrategy)
        {
            var f = fetchStrategy != null ? fetchStrategy.ToString() : $"FetchQueryStrategy<{typeof(T).Name}>: [ null ]";

            return $"GetOrSet<{typeof(T).Name}>: [ \n\tKeys = {string.Join(", ", keys.Select(x => x.ToString()).ToArray())},\n\t{f} ]";
        }

        private static string FormatGetOrSetAllKey<T, TResult>(IQueryOptions<T> options, Expression<Func<T, TResult>> selector)
        {
            var o = options != null ? options.ToString() : $"QueryOptions<{typeof(T).Name}>: [ null ]";

            return $"GetOrSetAll<{typeof(T).Name}>: [ \n\t{o},\n\tSelector = {ExpressionHelper.TranslateToString(selector)} ]";
        }

        private static string FormatGetOrSetKey<T, TResult>(IQueryOptions<T> options, Expression<Func<T, TResult>> selector)
        {
            return $"GetOrSet<{typeof(T).Name}>: [ \n\t{options},\n\tSelector = {ExpressionHelper.TranslateToString(selector)} ]";
        }

        private static string FormatGetOrSetCountKey<T>(IQueryOptions<T> options)
        {
            var o = options != null ? options.ToString() : $"QueryOptions<{typeof(T).Name}>: [ null ]";

            return $"GetOrSetCount<{typeof(T).Name}>: [ \n\t{o} ]";
        }

        private static string FormatGetOrSetDictionaryKey<T, TDictionaryKey, TElement>(IQueryOptions<T> options, Expression<Func<T, TDictionaryKey>> keySelector, Expression<Func<T, TElement>> elementSelector)
        {
            var o = options != null ? options.ToString() : $"QueryOptions<{typeof(T).Name}>: [ null ]";

            return $"GetOrSetDictionary<{typeof(T).Name}, {typeof(TDictionaryKey).Name}, {typeof(TElement).Name}>: [ \n\t{o},\n\tKeySelector = {ExpressionHelper.TranslateToString(keySelector)},\n\tElementSelector = {ExpressionHelper.TranslateToString(elementSelector)} ]";
        }

        private static string FormatGetOrSetGroupKey<T, TGroupKey, TResult>(IQueryOptions<T> options, Expression<Func<T, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<T>, TResult>> resultSelector)
        {
            var o = options != null ? options.ToString() : $"QueryOptions<{typeof(T).Name}>: [ null ]";

            return $"GetOrSetGroup<{typeof(T).Name}, {typeof(TGroupKey).Name}, {typeof(TResult).Name}>: [ \n\t{o},\n\tKeySelector = {ExpressionHelper.TranslateToString(keySelector)},\n\tResultSelector = {ExpressionHelper.TranslateToString(resultSelector)} ]";
        }
    }
}