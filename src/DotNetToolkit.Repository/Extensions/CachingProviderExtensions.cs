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

        private static T Get<T>(this ICacheProvider cacheProvider, string key)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            cacheProvider.TryGetValue<T>(key, out var value);

            return value;
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

            var hashedKey = $"{CacheProviderManager.CachePrefix}{CacheProviderManager.GlobalCachingPrefixCounter}_{key.ToSHA256()}";

            if (!cacheProvider.TryGetValue<QueryResult<T>>(hashedKey, out var value))
            {
                value = getter();

                if (value == null)
                    return default(QueryResult<T>);

                logger.Debug(cacheExpiration != TimeSpan.Zero
                    ? $"Setting up cache for {hashedKey} expire handling in {cacheExpiration.TotalSeconds} seconds"
                    : $"Setting up cache for {hashedKey}");

                cacheProvider.Cache.Set(
                    hashedKey,
                    value,
                    priority,
                    cacheExpiration,
                    reason => logger.Debug($"Cache for {hashedKey} has expired. Evicting from cache for {reason}"));
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

        public static QueryResult<IEnumerable<T>> GetOrSetExecuteQuery<T>(this ICacheProvider cacheProvider, string sql, CommandType cmdType, object[] parameters, Func<QueryResult<IEnumerable<T>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (sql == null)
                throw new ArgumentNullException(nameof(sql));

            var sb = new StringBuilder();

            sb.Append($"GetOrSetExecuteQuery<{typeof(T).Name}>: [ \n\tSql = {sql},");

            if (parameters != null && parameters.Any())
            {
                sb.Append($"\n\tParameters = {string.Join(", ", parameters.Select(x => x.ToString()).ToArray())},");
            }

            sb.Append("\n\tCommandType = {cmdType} ]");

            var key = sb.ToString();

            return cacheProvider.GetOrSet<IEnumerable<T>>(key, getter, logger);
        }

        public static QueryResult<int> GetOrSetExecuteQuery<T>(this ICacheProvider cacheProvider, string sql, CommandType cmdType, object[] parameters, Func<QueryResult<int>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (sql == null)
                throw new ArgumentNullException(nameof(sql));

            var sb = new StringBuilder();

            sb.Append($"GetOrSetExecuteQuery<{typeof(T).Name}>: [ \n\tSql = {sql},");

            if (parameters != null && parameters.Any())
            {
                sb.Append($"\n\tParameters = {string.Join(", ", parameters.Select(x => x.ToString()).ToArray())},");
            }

            sb.Append("\n\tCommandType = {cmdType} ]");

            var key = sb.ToString();

            return cacheProvider.GetOrSet<int>(key, getter, logger);
        }

        public static QueryResult<T> GetOrSet<T>(this ICacheProvider cacheProvider, object[] keys, Func<QueryResult<T>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            var key = $"GetOrSet<{typeof(T).Name}>: [ Keys = {string.Join(", ", keys.Select(x => x.ToString()).ToArray())} ]";

            return cacheProvider.GetOrSet<T>(key, getter, logger);
        }

        public static QueryResult<T> GetOrSet<T>(this ICacheProvider cacheProvider, object[] keys, IFetchQueryStrategy<T> fetchStrategy, Func<QueryResult<T>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            var f = fetchStrategy != null ? fetchStrategy.ToString() : $"FetchQueryStrategy<{typeof(T).Name}>: [ null ]";
            var key = $"GetOrSet<{typeof(T).Name}>: [ \n\tKeys = {string.Join(", ", keys.Select(x => x.ToString()).ToArray())},\n\t{f} ]";

            return cacheProvider.GetOrSet<T>(key, getter, logger);
        }

        public static QueryResult<T> GetOrSet<T>(this ICacheProvider cacheProvider, Expression<Func<T, bool>> predicate, Func<QueryResult<T>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var key = $"GetOrSet<{typeof(T).Name}>: [ Predicate = {ExpressionHelper.TranslateToString(predicate)} ]";

            return cacheProvider.GetOrSet<T>(key, getter, logger);
        }

        public static QueryResult<T> GetOrSet<T>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Func<QueryResult<T>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var key = $"GetOrSet<{typeof(T).Name}>: [ \n\t{options} ]";

            return cacheProvider.GetOrSet<T>(key, getter, logger);
        }

        public static QueryResult<TResult> GetOrSet<T, TResult>(this ICacheProvider cacheProvider, Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, Func<QueryResult<TResult>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var key = $"GetOrSet<{typeof(T).Name}>: [ Predicate = {ExpressionHelper.TranslateToString(predicate)}, Selector = {ExpressionHelper.TranslateToString(selector)} ]";

            return cacheProvider.GetOrSet<TResult>(key, getter, logger);
        }

        public static QueryResult<TResult> GetOrSet<T, TResult>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TResult>> selector, Func<QueryResult<TResult>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var key = $"GetOrSet<{typeof(T).Name}>: [ \n\t{options},\n\tSelector = {ExpressionHelper.TranslateToString(selector)} ]";

            return cacheProvider.GetOrSet<TResult>(key, getter, logger);
        }

        public static QueryResult<IEnumerable<T>> GetOrSetAll<T>(this ICacheProvider cacheProvider, Func<QueryResult<IEnumerable<T>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            var key = $"GetOrSetAll<{typeof(T).Name}>";

            return cacheProvider.GetOrSet<IEnumerable<T>>(key, getter, logger);
        }

        public static QueryResult<IEnumerable<T>> GetOrSetAll<T>(this ICacheProvider cacheProvider, Expression<Func<T, bool>> predicate, Func<QueryResult<IEnumerable<T>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var key = $"GetOrSetAll<{typeof(T).Name}>: [ Predicate = {ExpressionHelper.TranslateToString(predicate)} ]";

            return cacheProvider.GetOrSet<IEnumerable<T>>(key, getter, logger);
        }

        public static QueryResult<IEnumerable<T>> GetOrSetAll<T>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Func<QueryResult<IEnumerable<T>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            var o = options != null ? options.ToString() : $"QueryOptions<{typeof(T).Name}>: [ null ]";
            var key = $"GetOrSetAll<{typeof(T).Name}>: [ \n\t{o} ]";

            return cacheProvider.GetOrSet<IEnumerable<T>>(key, getter, logger);
        }

        public static QueryResult<IEnumerable<TResult>> GetOrSetAll<T, TResult>(this ICacheProvider cacheProvider, Expression<Func<T, TResult>> selector, Func<QueryResult<IEnumerable<TResult>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var key = $"GetOrSetAll<{typeof(T).Name}>: [ Selector = {ExpressionHelper.TranslateToString(selector)} ]";

            return cacheProvider.GetOrSet<IEnumerable<TResult>>(key, getter, logger);
        }

        public static QueryResult<IEnumerable<TResult>> GetOrSetAll<T, TResult>(this ICacheProvider cacheProvider, Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, Func<QueryResult<IEnumerable<TResult>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var key = $"GetOrSetAll<{typeof(T).Name}>: [ Predicate = {ExpressionHelper.TranslateToString(predicate)}, Selector = {ExpressionHelper.TranslateToString(selector)} ]";

            return cacheProvider.GetOrSet<IEnumerable<TResult>>(key, getter, logger);
        }

        public static QueryResult<IEnumerable<TResult>> GetOrSetAll<T, TResult>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TResult>> selector, Func<QueryResult<IEnumerable<TResult>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var o = options != null ? options.ToString() : $"QueryOptions<{typeof(T).Name}>: [ null ]";
            var key = $"GetOrSetAll<{typeof(T).Name}>: [ \n\t{o},\n\tSelector = {ExpressionHelper.TranslateToString(selector)} ]";

            return cacheProvider.GetOrSet<IEnumerable<TResult>>(key, getter, logger);
        }

        public static QueryResult<bool> GetOrSetExists<T>(this ICacheProvider cacheProvider, object[] keys, Func<QueryResult<bool>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            var key = $"GetOrSetExists<{typeof(T).Name}>: [ Keys = {string.Join(", ", keys.Select(x => x.ToString()).ToArray())} ]";

            return cacheProvider.GetOrSet<bool>(key, getter, logger);
        }

        public static QueryResult<bool> GetOrSetExists<T>(this ICacheProvider cacheProvider, Expression<Func<T, bool>> predicate, Func<QueryResult<bool>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var key = $"GetOrSetExists<{typeof(T).Name}>: [ Predicate = {ExpressionHelper.TranslateToString(predicate)} ]";

            return cacheProvider.GetOrSet<bool>(key, getter, logger);
        }

        public static QueryResult<bool> GetOrSetExists<T>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Func<QueryResult<bool>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var key = $"GetOrSetExists<{typeof(T).Name}>: [ \n\t{options} ]";

            return cacheProvider.GetOrSet<bool>(key, getter, logger);
        }

        public static QueryResult<int> GetOrSetCount<T>(this ICacheProvider cacheProvider, Func<QueryResult<int>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            var key = $"GetOrSetCount<{typeof(T).Name}>";

            return cacheProvider.GetOrSet<int>(key, getter, logger);
        }

        public static QueryResult<int> GetOrSetCount<T>(this ICacheProvider cacheProvider, Expression<Func<T, bool>> predicate, Func<QueryResult<int>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var key = $"GetOrSetCount<{typeof(T).Name}>: [ Predicate = {ExpressionHelper.TranslateToString(predicate)} ]";

            return cacheProvider.GetOrSet<int>(key, getter, logger);
        }

        public static QueryResult<int> GetOrSetCount<T>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Func<QueryResult<int>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            var o = options != null ? options.ToString() : $"QueryOptions<{typeof(T).Name}>: [ null ]";
            var key = $"GetOrSetCount<{typeof(T).Name}>: [ \n\t{o} ]";

            return cacheProvider.GetOrSet<int>(key, getter, logger);
        }

        public static QueryResult<Dictionary<TDictionaryKey, T>> GetOrSetDictionary<T, TDictionaryKey>(this ICacheProvider cacheProvider, Expression<Func<T, TDictionaryKey>> keySelector, Func<QueryResult<Dictionary<TDictionaryKey, T>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            var key = $"GetOrSetDictionary<{typeof(T).Name}, {typeof(TDictionaryKey).Name}>: [ KeySelector = {ExpressionHelper.TranslateToString(keySelector)} ]";

            return cacheProvider.GetOrSet<Dictionary<TDictionaryKey, T>>(key, getter, logger);
        }

        public static QueryResult<Dictionary<TDictionaryKey, T>> GetOrSetDictionary<T, TDictionaryKey>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TDictionaryKey>> keySelector, Func<QueryResult<Dictionary<TDictionaryKey, T>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            var o = options != null ? options.ToString() : $"QueryOptions<{typeof(T).Name}>: [ null ]";
            var key = $"GetOrSetDictionary<{typeof(T).Name}, {typeof(TDictionaryKey).Name}>: [ \n\t{o},\n\tKeySelector = {ExpressionHelper.TranslateToString(keySelector)} ]";

            return cacheProvider.GetOrSet<Dictionary<TDictionaryKey, T>>(key, getter, logger);
        }

        public static QueryResult<Dictionary<TDictionaryKey, TElement>> GetOrSetDictionary<T, TDictionaryKey, TElement>(this ICacheProvider cacheProvider, Expression<Func<T, TDictionaryKey>> keySelector, Expression<Func<T, TElement>> elementSelector, Func<QueryResult<Dictionary<TDictionaryKey, TElement>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            var key = $"GetOrSetDictionary<{typeof(T).Name}, {typeof(TDictionaryKey).Name}, {typeof(TElement).Name}>: [ KeySelector = {ExpressionHelper.TranslateToString(keySelector)}, ElementSelector = {ExpressionHelper.TranslateToString(elementSelector)} ]";

            return cacheProvider.GetOrSet<Dictionary<TDictionaryKey, TElement>>(key, getter, logger);
        }

        public static QueryResult<Dictionary<TDictionaryKey, TElement>> GetOrSetDictionary<T, TDictionaryKey, TElement>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TDictionaryKey>> keySelector, Expression<Func<T, TElement>> elementSelector, Func<QueryResult<Dictionary<TDictionaryKey, TElement>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            var o = options != null ? options.ToString() : $"QueryOptions<{typeof(T).Name}>: [ null ]";
            var key = $"GetOrSetDictionary<{typeof(T).Name}, {typeof(TDictionaryKey).Name}, {typeof(TElement).Name}>: [ \n\t{o},\n\tKeySelector = {ExpressionHelper.TranslateToString(keySelector)},\n\tElementSelector = {ExpressionHelper.TranslateToString(elementSelector)} ]";

            return cacheProvider.GetOrSet<Dictionary<TDictionaryKey, TElement>>(key, getter, logger);
        }

        public static QueryResult<IEnumerable<TResult>> GetOrSetGroup<T, TGroupKey, TResult>(this ICacheProvider cacheProvider, Expression<Func<T, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<T>, TResult>> resultSelector, Func<QueryResult<IEnumerable<TResult>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            var key = $"GetOrSetGroup<{typeof(T).Name}, {typeof(TGroupKey).Name}, {typeof(TResult).Name}>: [ KeySelector = {ExpressionHelper.TranslateToString(keySelector)}, ResultSelector = {ExpressionHelper.TranslateToString(resultSelector)} ]";

            return cacheProvider.GetOrSet<IEnumerable<TResult>>(key, getter, logger);
        }

        public static QueryResult<IEnumerable<TResult>> GetOrSetGroup<T, TGroupKey, TResult>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<T>, TResult>> resultSelector, Func<QueryResult<IEnumerable<TResult>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            var o = options != null ? options.ToString() : $"QueryOptions<{typeof(T).Name}>: [ null ]";
            var key = $"GetOrSetGroup<{typeof(T).Name}, {typeof(TGroupKey).Name}, {typeof(TResult).Name}>: [ \n\t{o},\n\tKeySelector = {ExpressionHelper.TranslateToString(keySelector)},\n\tResultSelector = {ExpressionHelper.TranslateToString(resultSelector)} ]";

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

            var hashedKey = $"{CacheProviderManager.CachePrefix}{CacheProviderManager.GlobalCachingPrefixCounter}_{key.ToSHA256()}";

            if (!cacheProvider.TryGetValue<QueryResult<T>>(hashedKey, out var value))
            {
                value = await getter();

                if (value == null)
                    return default(QueryResult<T>);

                logger.Debug(cacheExpiration != TimeSpan.Zero
                    ? $"Setting up cache for {hashedKey} expire handling in {cacheExpiration.TotalSeconds} seconds"
                    : $"Setting up cache for {hashedKey}");

                cacheProvider.Cache.Set(
                    hashedKey,
                    value,
                    priority,
                    cacheExpiration,
                    reason => logger.Debug($"Cache for {hashedKey} has expired. Evicting from cache for {reason}"));
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

        public static Task<QueryResult<IEnumerable<T>>> GetOrSetExecuteQueryAsync<T>(this ICacheProvider cacheProvider, string sql, CommandType cmdType, object[] parameters, Func<Task<QueryResult<IEnumerable<T>>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (sql == null)
                throw new ArgumentNullException(nameof(sql));

            var sb = new StringBuilder();

            sb.Append($"GetOrSetExecuteQueryAsync<{typeof(T).Name}>: [ \n\tSql = {sql},");

            if (parameters != null && parameters.Any())
            {
                sb.Append($"\n\tParameters = {string.Join(", ", parameters.Select(x => x.ToString()).ToArray())},");
            }

            sb.Append("\n\tCommandType = {cmdType} ]");

            var key = sb.ToString();

            return cacheProvider.GetOrSetAsync<IEnumerable<T>>(key, getter, logger);
        }

        public static Task<QueryResult<int>> GetOrSetExecuteQueryAsync<T>(this ICacheProvider cacheProvider, string sql, CommandType cmdType, object[] parameters, Func<Task<QueryResult<int>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (sql == null)
                throw new ArgumentNullException(nameof(sql));

            var sb = new StringBuilder();

            sb.Append($"GetOrSetExecuteQueryAsync<{typeof(T).Name}>: [ \n\tSql = {sql},");

            if (parameters != null && parameters.Any())
            {
                sb.Append($"\n\tParameters = {string.Join(", ", parameters.Select(x => x.ToString()).ToArray())},");
            }

            sb.Append("\n\tCommandType = {cmdType} ]");

            var key = sb.ToString();

            return cacheProvider.GetOrSetAsync<int>(key, getter, logger);
        }

        public static Task<QueryResult<T>> GetOrSetAsync<T>(this ICacheProvider cacheProvider, object[] keys, Func<Task<QueryResult<T>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            var key = $"GetOrSetAsync<{typeof(T).Name}>: [ Keys = {string.Join(", ", keys.Select(x => x.ToString()).ToArray())} ]";

            return cacheProvider.GetOrSetAsync<T>(key, getter, logger);
        }

        public static Task<QueryResult<T>> GetOrSetAsync<T>(this ICacheProvider cacheProvider, object[] keys, IFetchQueryStrategy<T> fetchStrategy, Func<Task<QueryResult<T>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            var f = fetchStrategy != null ? fetchStrategy.ToString() : $"FetchQueryStrategy<{typeof(T).Name}>: [ null ]";
            var key = $"GetOrSetAsync<{typeof(T).Name}>: [ \n\tKeys = {string.Join(", ", keys.Select(x => x.ToString()).ToArray())},\n\t{f} ]";

            return cacheProvider.GetOrSetAsync<T>(key, getter, logger);
        }

        public static Task<QueryResult<T>> GetOrSetAsync<T>(this ICacheProvider cacheProvider, Expression<Func<T, bool>> predicate, Func<Task<QueryResult<T>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var key = $"GetOrSetAsync<{typeof(T).Name}>: [ Predicate = {ExpressionHelper.TranslateToString(predicate)} ]";

            return cacheProvider.GetOrSetAsync<T>(key, getter, logger);
        }

        public static Task<QueryResult<T>> GetOrSetAsync<T>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Func<Task<QueryResult<T>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var key = $"GetOrSetAsync<{typeof(T).Name}>: [ \n\t{options} ]";

            return cacheProvider.GetOrSetAsync<T>(key, getter, logger);
        }

        public static Task<QueryResult<TResult>> GetOrSetAsync<T, TResult>(this ICacheProvider cacheProvider, Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, Func<Task<QueryResult<TResult>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var key = $"GetOrSetAsync<{typeof(T).Name}>: [ Predicate = {ExpressionHelper.TranslateToString(predicate)}, Selector = {ExpressionHelper.TranslateToString(selector)} ]";

            return cacheProvider.GetOrSetAsync<TResult>(key, getter, logger);
        }

        public static Task<QueryResult<TResult>> GetOrSetAsync<T, TResult>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TResult>> selector, Func<Task<QueryResult<TResult>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var key = $"GetOrSetAsync<{typeof(T).Name}>: [ \n\t{options},\n\tSelector = {ExpressionHelper.TranslateToString(selector)} ]";

            return cacheProvider.GetOrSetAsync<TResult>(key, getter, logger);
        }

        public static Task<QueryResult<IEnumerable<T>>> GetOrSetAllAsync<T>(this ICacheProvider cacheProvider, Func<Task<QueryResult<IEnumerable<T>>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            var key = $"GetOrSetAllAsync<{typeof(T).Name}>";

            return cacheProvider.GetOrSetAsync<IEnumerable<T>>(key, getter, logger);
        }

        public static Task<QueryResult<IEnumerable<T>>> GetOrSetAllAsync<T>(this ICacheProvider cacheProvider, Expression<Func<T, bool>> predicate, Func<Task<QueryResult<IEnumerable<T>>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var key = $"GetOrSetAllAsync<{typeof(T).Name}>: [ Predicate = {ExpressionHelper.TranslateToString(predicate)} ]";

            return cacheProvider.GetOrSetAsync<IEnumerable<T>>(key, getter, logger);
        }

        public static Task<QueryResult<IEnumerable<T>>> GetOrSetAllAsync<T>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Func<Task<QueryResult<IEnumerable<T>>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            var o = options != null ? options.ToString() : $"QueryOptions<{typeof(T).Name}>: [ null ]";
            var key = $"GetOrSetAllAsync<{typeof(T).Name}>: [ \n\t{o} ]";

            return cacheProvider.GetOrSetAsync<IEnumerable<T>>(key, getter, logger);
        }

        public static Task<QueryResult<IEnumerable<TResult>>> GetOrSetAllAsync<T, TResult>(this ICacheProvider cacheProvider, Expression<Func<T, TResult>> selector, Func<Task<QueryResult<IEnumerable<TResult>>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var key = $"GetOrSetAllAsync<{typeof(T).Name}>: [ Selector = {ExpressionHelper.TranslateToString(selector)} ]";

            return cacheProvider.GetOrSetAsync<IEnumerable<TResult>>(key, getter, logger);
        }

        public static Task<QueryResult<IEnumerable<TResult>>> GetOrSetAllAsync<T, TResult>(this ICacheProvider cacheProvider, Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, Func<Task<QueryResult<IEnumerable<TResult>>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var key = $"GetOrSetAllAsync<{typeof(T).Name}>: [ Predicate = {ExpressionHelper.TranslateToString(predicate)}, Selector = {ExpressionHelper.TranslateToString(selector)} ]";

            return cacheProvider.GetOrSetAsync<IEnumerable<TResult>>(key, getter, logger);
        }

        public static Task<QueryResult<IEnumerable<TResult>>> GetOrSetAllAsync<T, TResult>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TResult>> selector, Func<Task<QueryResult<IEnumerable<TResult>>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var o = options != null ? options.ToString() : $"QueryOptions<{typeof(T).Name}>: [ null ]";
            var key = $"GetOrSetAllAsync<{typeof(T).Name}>: [ \n\t{o},\n\tSelector = {ExpressionHelper.TranslateToString(selector)} ]";

            return cacheProvider.GetOrSetAsync<IEnumerable<TResult>>(key, getter, logger);
        }

        public static Task<QueryResult<bool>> GetOrSetExistsAsync<T>(this ICacheProvider cacheProvider, object[] keys, Func<Task<QueryResult<bool>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            var key = $"GetOrSetExistsAsync<{typeof(T).Name}>: [ Keys = {string.Join(", ", keys.Select(x => x.ToString()).ToArray())} ]";

            return cacheProvider.GetOrSetAsync<bool>(key, getter, logger);
        }

        public static Task<QueryResult<bool>> GetOrSetExistsAsync<T>(this ICacheProvider cacheProvider, Expression<Func<T, bool>> predicate, Func<Task<QueryResult<bool>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var key = $"GetOrSetExistsAsync<{typeof(T).Name}>: [ Predicate = {ExpressionHelper.TranslateToString(predicate)} ]";

            return cacheProvider.GetOrSetAsync<bool>(key, getter, logger);
        }

        public static Task<QueryResult<bool>> GetOrSetExistsAsync<T>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Func<Task<QueryResult<bool>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var key = $"GetOrSetExistsAsync<{typeof(T).Name}>: [ \n\t{options} ]";

            return cacheProvider.GetOrSetAsync<bool>(key, getter, logger);
        }

        public static Task<QueryResult<int>> GetOrSetCountAsync<T>(this ICacheProvider cacheProvider, Func<Task<QueryResult<int>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            var key = $"GetOrSetCountAsync<{typeof(T).Name}>";

            return cacheProvider.GetOrSetAsync<int>(key, getter, logger);
        }

        public static Task<QueryResult<int>> GetOrSetCountAsync<T>(this ICacheProvider cacheProvider, Expression<Func<T, bool>> predicate, Func<Task<QueryResult<int>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var key = $"GetOrSetCountAsync<{typeof(T).Name}>: [ Predicate = {ExpressionHelper.TranslateToString(predicate)} ]";

            return cacheProvider.GetOrSetAsync<int>(key, getter, logger);
        }

        public static Task<QueryResult<int>> GetOrSetCountAsync<T>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Func<Task<QueryResult<int>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            var o = options != null ? options.ToString() : $"QueryOptions<{typeof(T).Name}>: [ null ]";
            var key = $"GetOrSetCountAsync<{typeof(T).Name}>: [ \n\t{o} ]";

            return cacheProvider.GetOrSetAsync<int>(key, getter, logger);
        }

        public static Task<QueryResult<Dictionary<TDictionaryKey, T>>> GetOrSetDictionaryAsync<T, TDictionaryKey>(this ICacheProvider cacheProvider, Expression<Func<T, TDictionaryKey>> keySelector, Func<Task<QueryResult<Dictionary<TDictionaryKey, T>>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            var key = $"GetOrSetDictionaryAsync<{typeof(T).Name}, {typeof(TDictionaryKey).Name}>: [ KeySelector = {ExpressionHelper.TranslateToString(keySelector)} ]";

            return cacheProvider.GetOrSetAsync<Dictionary<TDictionaryKey, T>>(key, getter, logger);
        }

        public static Task<QueryResult<Dictionary<TDictionaryKey, T>>> GetOrSetDictionaryAsync<T, TDictionaryKey>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TDictionaryKey>> keySelector, Func<Task<QueryResult<Dictionary<TDictionaryKey, T>>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            var o = options != null ? options.ToString() : $"QueryOptions<{typeof(T).Name}>: [ null ]";
            var key = $"GetOrSetDictionaryAsync<{typeof(T).Name}, {typeof(TDictionaryKey).Name}>: [ \n\t{o},\n\tKeySelector = {ExpressionHelper.TranslateToString(keySelector)} ]";

            return cacheProvider.GetOrSetAsync<Dictionary<TDictionaryKey, T>>(key, getter, logger);
        }

        public static Task<QueryResult<Dictionary<TDictionaryKey, TElement>>> GetOrSetDictionaryAsync<T, TDictionaryKey, TElement>(this ICacheProvider cacheProvider, Expression<Func<T, TDictionaryKey>> keySelector, Expression<Func<T, TElement>> elementSelector, Func<Task<QueryResult<Dictionary<TDictionaryKey, TElement>>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            var key = $"GetOrSetDictionaryAsync<{typeof(T).Name}, {typeof(TDictionaryKey).Name}, {typeof(TElement).Name}>: [ KeySelector = {ExpressionHelper.TranslateToString(keySelector)}, ElementSelector = {ExpressionHelper.TranslateToString(elementSelector)} ]";

            return cacheProvider.GetOrSetAsync<Dictionary<TDictionaryKey, TElement>>(key, getter, logger);
        }

        public static Task<QueryResult<Dictionary<TDictionaryKey, TElement>>> GetOrSetDictionaryAsync<T, TDictionaryKey, TElement>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TDictionaryKey>> keySelector, Expression<Func<T, TElement>> elementSelector, Func<Task<QueryResult<Dictionary<TDictionaryKey, TElement>>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            var o = options != null ? options.ToString() : $"QueryOptions<{typeof(T).Name}>: [ null ]";
            var key = $"GetOrSetDictionaryAsync<{typeof(T).Name}, {typeof(TDictionaryKey).Name}, {typeof(TElement).Name}>: [ \n\t{o},\n\tKeySelector = {ExpressionHelper.TranslateToString(keySelector)},\n\tElementSelector = {ExpressionHelper.TranslateToString(elementSelector)} ]";

            return cacheProvider.GetOrSetAsync<Dictionary<TDictionaryKey, TElement>>(key, getter, logger);
        }

        public static Task<QueryResult<IEnumerable<TResult>>> GetOrSetGroupAsync<T, TGroupKey, TResult>(this ICacheProvider cacheProvider, Expression<Func<T, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<T>, TResult>> resultSelector, Func<Task<QueryResult<IEnumerable<TResult>>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            var key = $"GetOrSetGroupAsync<{typeof(T).Name}, {typeof(TGroupKey).Name}, {typeof(TResult).Name}>: [ KeySelector = {ExpressionHelper.TranslateToString(keySelector)}, ResultSelector = {ExpressionHelper.TranslateToString(resultSelector)} ]";

            return cacheProvider.GetOrSetAsync<IEnumerable<TResult>>(key, getter, logger);
        }

        public static Task<QueryResult<IEnumerable<TResult>>> GetOrSetGroupAsync<T, TGroupKey, TResult>(this ICacheProvider cacheProvider, IQueryOptions<T> options, Expression<Func<T, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<T>, TResult>> resultSelector, Func<Task<QueryResult<IEnumerable<TResult>>>> getter, ILogger logger)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            var o = options != null ? options.ToString() : $"QueryOptions<{typeof(T).Name}>: [ null ]";
            var key = $"GetOrSetGroupAsync<{typeof(T).Name}, {typeof(TGroupKey).Name}, {typeof(TResult).Name}>: [ \n\t{o},\n\tKeySelector = {ExpressionHelper.TranslateToString(keySelector)},\n\tResultSelector = {ExpressionHelper.TranslateToString(resultSelector)} ]";

            return cacheProvider.GetOrSetAsync<IEnumerable<TResult>>(key, getter, logger);
        }
    }
}