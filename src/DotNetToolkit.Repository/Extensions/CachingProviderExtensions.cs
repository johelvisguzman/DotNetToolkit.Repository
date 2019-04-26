namespace DotNetToolkit.Repository.Extensions
{
    using Configuration.Caching;
    using Configuration.Logging;
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
    internal static class CachingProviderExtensions
    {
        private static readonly object _syncRoot = new object();

        public static IQueryResult<IEnumerable<T>> GetOrSetExecuteSqlQuery<T>(
            [NotNull] this ICacheProvider cacheProvider, 
            [NotNull] string sql, CommandType cmdType, 
            [CanBeNull] Dictionary<string, object> parameters, 
            [NotNull] Func<IDataReader, T> projector, 
            [NotNull] Func<IQueryResult<IEnumerable<T>>> getter, 
            [NotNull] ILogger logger)
            => GetOrSet<IEnumerable<T>>(
                cacheProvider,
                FormatGetOrSetExecuteSqlQueryKey<T>(sql, cmdType, parameters),
                getter,
                logger);

        public static IQueryResult<int> GetOrSetExecuteSqlCommand<T>(
            [NotNull] this ICacheProvider cacheProvider,
            [NotNull] string sql, CommandType cmdType, 
            [CanBeNull] Dictionary<string, object> parameters, 
            [NotNull] Func<IQueryResult<int>> getter, 
            [NotNull] ILogger logger)
            => GetOrSet<int>(
                cacheProvider,
                FormatGetOrSetExecuteSqlCommandKey<T>(sql, cmdType, parameters),
                getter,
                logger);

        public static IQueryResult<T> GetOrSetFind<T>(
            [NotNull] this ICacheProvider cacheProvider, 
            [NotNull] object[] keys, 
            [CanBeNull] IFetchQueryStrategy<T> fetchStrategy, 
            [NotNull] Func<IQueryResult<T>> getter, 
            [NotNull] ILogger logger)
            => GetOrSet<T>(
                cacheProvider,
                FormatGetOrSetFindKey<T>(keys, fetchStrategy),
                getter,
                logger);

        public static IQueryResult<TResult> GetOrSetFind<T, TResult>(
            [NotNull] this ICacheProvider cacheProvider, 
            [CanBeNull] IQueryOptions<T> options, 
            [NotNull] Expression<Func<T, TResult>> selector, 
            [NotNull] Func<IQueryResult<TResult>> getter, 
            [NotNull] ILogger logger)
            => GetOrSet<TResult>(
                cacheProvider,
                FormatGetOrSetFindKey<T, TResult>(options, selector),
                getter,
                logger);

        public static IPagedQueryResult<IEnumerable<TResult>> GetOrSetFindAll<T, TResult>(
            [NotNull] this ICacheProvider cacheProvider, 
            [CanBeNull] IQueryOptions<T> options, 
            [NotNull] Expression<Func<T, TResult>> selector, 
            [NotNull] Func<IPagedQueryResult<IEnumerable<TResult>>> getter, 
            [NotNull] ILogger logger)
            => GetOrSet<IEnumerable<TResult>>(
                cacheProvider,
                FormatGetOrSetFindAllKey<T, TResult>(options, selector),
                getter,
                logger);

        public static IQueryResult<int> GetOrSetCount<T>(
            [NotNull] this ICacheProvider cacheProvider, 
            [CanBeNull] IQueryOptions<T> options, 
            [NotNull] Func<IQueryResult<int>> getter, 
            [NotNull] ILogger logger)
            => GetOrSet<int>(
                cacheProvider,
                FormatGetOrSetCountKey<T>(options),
                getter,
                logger);

        public static IPagedQueryResult<Dictionary<TDictionaryKey, TElement>> GetOrSetDictionary<T, TDictionaryKey, TElement>(
            [NotNull] this ICacheProvider cacheProvider, 
            [CanBeNull] IQueryOptions<T> options, 
            [NotNull] Expression<Func<T, TDictionaryKey>> keySelector, 
            [NotNull] Expression<Func<T, TElement>> elementSelector, 
            [NotNull] Func<IPagedQueryResult<Dictionary<TDictionaryKey, TElement>>> getter, 
            [NotNull] ILogger logger)
            => GetOrSet<Dictionary<TDictionaryKey, TElement>>(
                cacheProvider,
                FormatGetOrSetDictionaryKey<T, TDictionaryKey, TElement>(options, keySelector, elementSelector),
                getter,
                logger);

        public static IPagedQueryResult<IEnumerable<TResult>> GetOrSetGroup<T, TGroupKey, TResult>(
            [NotNull] this ICacheProvider cacheProvider,
            [CanBeNull] IQueryOptions<T> options, 
            [NotNull] Expression<Func<T, TGroupKey>> keySelector, 
            [NotNull] Expression<Func<TGroupKey, IEnumerable<T>, TResult>> resultSelector, 
            [NotNull] Func<IPagedQueryResult<IEnumerable<TResult>>> getter, 
            [NotNull] ILogger logger)
            => GetOrSet<IEnumerable<TResult>>(
                cacheProvider,
                FormatGetOrSetGroupKey<T, TGroupKey, TResult>(options, keySelector, resultSelector),
                getter,
                logger);

        public static Task<IQueryResult<IEnumerable<T>>> GetOrSetExecuteSqlQueryAsync<T>(
            [NotNull] this ICacheProvider cacheProvider, 
            [NotNull] string sql, 
            CommandType cmdType, 
            [CanBeNull] Dictionary<string, object> parameters, 
            [NotNull] Func<IDataReader, T> projector, 
            [NotNull] Func<Task<IQueryResult<IEnumerable<T>>>> getter, 
            [NotNull] ILogger logger)
            => GetOrSetAsync<IEnumerable<T>>(
                cacheProvider,
                FormatGetOrSetExecuteSqlQueryKey<T>(sql, cmdType, parameters),
                getter,
                logger);

        public static Task<IQueryResult<int>> GetOrSetExecuteSqlCommandAsync<T>(
            [NotNull] this ICacheProvider cacheProvider, 
            [NotNull] string sql, 
            CommandType cmdType, 
            [CanBeNull] Dictionary<string, object> parameters, 
            [NotNull] Func<Task<IQueryResult<int>>> getter, 
            [NotNull] ILogger logger)
            => GetOrSetAsync<int>(
                cacheProvider,
                FormatGetOrSetExecuteSqlCommandKey<T>(sql, cmdType, parameters),
                getter,
                logger);

        public static Task<IQueryResult<T>> GetOrSetFindAsync<T>(
            [NotNull] this ICacheProvider cacheProvider, 
            [NotNull] object[] keys, 
            [CanBeNull] IFetchQueryStrategy<T> fetchStrategy,
            [NotNull] Func<Task<IQueryResult<T>>> getter,
            [NotNull] ILogger logger)
            => GetOrSetAsync<T>(
                cacheProvider,
                FormatGetOrSetFindKey<T>(keys, fetchStrategy),
                getter,
                logger);

        public static Task<IQueryResult<TResult>> GetOrSetFindAsync<T, TResult>(
            [NotNull] this ICacheProvider cacheProvider, 
            [CanBeNull] IQueryOptions<T> options, 
            [NotNull] Expression<Func<T, TResult>> selector, 
            [NotNull] Func<Task<IQueryResult<TResult>>> getter, 
            [NotNull] ILogger logger)
            => GetOrSetAsync<TResult>(
                cacheProvider,
                FormatGetOrSetFindKey<T, TResult>(options, selector),
                getter,
                logger);

        public static Task<IPagedQueryResult<IEnumerable<TResult>>> GetOrSetFindAllAsync<T, TResult>(
            [NotNull] this ICacheProvider cacheProvider, 
            [CanBeNull] IQueryOptions<T> options, 
            [NotNull] Expression<Func<T, TResult>> selector, 
            [NotNull] Func<Task<IPagedQueryResult<IEnumerable<TResult>>>> getter, 
            [NotNull] ILogger logger)
            => GetOrSetAsync<IEnumerable<TResult>>(
                cacheProvider,
                FormatGetOrSetFindAllKey<T, TResult>(options, selector),
                getter,
                logger);

        public static Task<IQueryResult<int>> GetOrSetCountAsync<T>(
            [NotNull] this ICacheProvider cacheProvider, 
            [CanBeNull] IQueryOptions<T> options, 
            [NotNull] Func<Task<IQueryResult<int>>> getter, 
            [NotNull] ILogger logger)
            => GetOrSetAsync<int>(
                cacheProvider,
                FormatGetOrSetCountKey<T>(options),
                getter,
                logger);

        public static Task<IPagedQueryResult<Dictionary<TDictionaryKey, TElement>>> GetOrSetDictionaryAsync<T, TDictionaryKey, TElement>(
            [NotNull] this ICacheProvider cacheProvider, 
            [CanBeNull] IQueryOptions<T> options, 
            [NotNull] Expression<Func<T, TDictionaryKey>> keySelector, 
            [NotNull] Expression<Func<T, TElement>> elementSelector, 
            [NotNull] Func<Task<IPagedQueryResult<Dictionary<TDictionaryKey, TElement>>>> getter, 
            [NotNull] ILogger logger)
            => GetOrSetAsync<Dictionary<TDictionaryKey, TElement>>(
                cacheProvider,
                FormatGetOrSetDictionaryKey<T, TDictionaryKey, TElement>(options, keySelector, elementSelector),
                getter,
                logger);

        public static Task<IPagedQueryResult<IEnumerable<TResult>>> GetOrSetGroupAsync<T, TGroupKey, TResult>(
            [NotNull] this ICacheProvider cacheProvider, 
            [CanBeNull] IQueryOptions<T> options, 
            [NotNull] Expression<Func<T, TGroupKey>> keySelector, 
            [NotNull] Expression<Func<TGroupKey, IEnumerable<T>, TResult>> resultSelector, 
            [NotNull] Func<Task<IPagedQueryResult<IEnumerable<TResult>>>> getter, 
            [NotNull] ILogger logger)
            => GetOrSetAsync<IEnumerable<TResult>>(
                cacheProvider,
                FormatGetOrSetGroupKey<T, TGroupKey, TResult>(options, keySelector, resultSelector),
                getter,
                logger);

        private static Task<IQueryResult<T>> GetOrSetAsync<T>(
            [NotNull] this ICacheProvider cacheProvider, 
            [NotNull] string key, 
            [NotNull] Func<Task<IQueryResult<T>>> getter, 
            [NotNull] ILogger logger)
            => GetOrSetAsync<T>(
                cacheProvider,
                key,
                getter,
                cacheProvider.Expiry,
                logger);

        private static IQueryResult<T> GetOrSet<T>(
            [NotNull] this ICacheProvider cacheProvider, 
            [NotNull] string key, 
            [NotNull] Func<IQueryResult<T>> getter, 
            [NotNull] ILogger logger)
            => GetOrSet<T>(
                cacheProvider,
                key,
                getter,
                cacheProvider.Expiry,
                logger);

        private static Task<IPagedQueryResult<T>> GetOrSetAsync<T>(
            [NotNull] this ICacheProvider cacheProvider, 
            [NotNull] string key, 
            [NotNull] Func<Task<IPagedQueryResult<T>>> getter, 
            [NotNull] ILogger logger)
            => GetOrSetAsync<T>(
                cacheProvider,
                key,
                getter,
                cacheProvider.Expiry,
                logger);

        private static IPagedQueryResult<T> GetOrSet<T>(
            [NotNull] this ICacheProvider cacheProvider, 
            [NotNull] string key, 
            [NotNull] Func<IPagedQueryResult<T>> getter, 
            [NotNull] ILogger logger)
            => GetOrSet<T>(
                cacheProvider,
                key,
                getter,
                cacheProvider.Expiry,
                logger);

        private static PagedQueryResult<T> CreatePagedQueryResult<T>([NotNull] IPagedQueryResult<T> oldValue, bool cachedUsed = false) 
            => new PagedQueryResult<T>(oldValue.Result, oldValue.Total)
            {
                CacheUsed = cachedUsed
            };

        private static QueryResult<T> CreateQueryResult<T>([NotNull] IQueryResult<T> oldValue, bool cachedUsed = false) 
            => new QueryResult<T>(oldValue.Result)
            {
                CacheUsed = cachedUsed
            };

        private static void SetValue<T>([NotNull] this ICacheProvider cacheProvider, [NotNull] string hashedKey, [NotNull] string key, [NotNull] IQueryResult<T> value, [CanBeNull] TimeSpan? expiry, [NotNull] ILogger logger)
        {
            Guard.NotNull(cacheProvider);
            Guard.NotEmpty(hashedKey);
            Guard.NotEmpty(key);
            Guard.NotNull(logger);

            lock (_syncRoot)
            {
                logger.Debug(expiry.HasValue
                        ? $"Setting up cache for '{hashedKey}' expire handling in {expiry.Value.TotalSeconds} seconds"
                        : $"Setting up cache for '{hashedKey}'");

                cacheProvider.Cache.Set<IQueryResult<T>>(
                    hashedKey,
                    value,
                    expiry,
                    reason => logger.Debug($"Cache for '{hashedKey}' has expired. Evicting from cache for '{reason}'"));
            }
        }

        private static bool TryGetValue<T>([NotNull] this ICacheProvider cacheProvider, [NotNull] string key, out T value)
        {
            Guard.NotNull(cacheProvider);
            Guard.NotEmpty(key);

            lock (_syncRoot)
            {
                return cacheProvider.Cache.TryGetValue<T>(key, out value);
            }
        }

        private static QueryResult<T> GetOrSet<T>([NotNull] this ICacheProvider cacheProvider, [NotNull] string key, [NotNull] Func<IQueryResult<T>> getter, [CanBeNull] TimeSpan? expiry, [NotNull] ILogger logger)
        {
            Guard.NotNull(cacheProvider);
            Guard.NotEmpty(key);
            Guard.NotNull(logger);

            var hashedKey = FormatHashedKey<T>(cacheProvider, key);

            if (!cacheProvider.TryGetValue<QueryResult<T>>(hashedKey, out var value))
            {
                value = CreateQueryResult<T>(getter(), cachedUsed: false);

                cacheProvider.SetValue(hashedKey, key, value, expiry, logger);
            }
            else
            {
                value = CreateQueryResult<T>(value, cachedUsed: true);
            }

            return value;
        }

        private static async Task<IQueryResult<T>> GetOrSetAsync<T>([NotNull] this ICacheProvider cacheProvider, [NotNull] string key, [NotNull] Func<Task<IQueryResult<T>>> getter, [CanBeNull] TimeSpan? expiry, [NotNull] ILogger logger)
        {
            Guard.NotNull(cacheProvider);
            Guard.NotEmpty(key);
            Guard.NotNull(getter);
            Guard.NotNull(logger);

            var hashedKey = FormatHashedKey<T>(cacheProvider, key);

            if (!cacheProvider.TryGetValue<QueryResult<T>>(hashedKey, out var value))
            {
                value = CreateQueryResult<T>(await getter(), cachedUsed: false);

                cacheProvider.SetValue(hashedKey, key, value, expiry, logger);
            }
            else
            {
                value = CreateQueryResult<T>(value, cachedUsed: true);
            }

            return value;
        }

        private static IPagedQueryResult<T> GetOrSet<T>([NotNull] this ICacheProvider cacheProvider, [NotNull] string key, [NotNull] Func<IPagedQueryResult<T>> getter, [CanBeNull] TimeSpan? expiry, [NotNull] ILogger logger)
        {
            Guard.NotNull(cacheProvider);
            Guard.NotEmpty(key);
            Guard.NotNull(getter);
            Guard.NotNull(logger);

            var hashedKey = FormatHashedKey<T>(cacheProvider, key);

            if (!cacheProvider.TryGetValue<PagedQueryResult<T>>(hashedKey, out var value))
            {
                value = CreatePagedQueryResult<T>(getter(), cachedUsed: false);

                cacheProvider.SetValue(hashedKey, key, value, expiry, logger);
            }
            else
            {
                value = CreatePagedQueryResult<T>(value, cachedUsed: true);
            }

            return value;
        }

        private static async Task<IPagedQueryResult<T>> GetOrSetAsync<T>([NotNull] this ICacheProvider cacheProvider, [NotNull] string key, [NotNull] Func<Task<IPagedQueryResult<T>>> getter, [CanBeNull] TimeSpan? expiry, [NotNull] ILogger logger)
        {
            Guard.NotNull(cacheProvider);
            Guard.NotEmpty(key);
            Guard.NotNull(getter);
            Guard.NotNull(logger);

            var hashedKey = FormatHashedKey<T>(cacheProvider, key);

            if (!cacheProvider.TryGetValue<PagedQueryResult<T>>(hashedKey, out var value))
            {
                value = CreatePagedQueryResult<T>(await getter(), cachedUsed: false);

                cacheProvider.SetValue(hashedKey, key, value, expiry, logger);
            }
            else
            {
                value = CreatePagedQueryResult<T>(value, cachedUsed: true);
            }

            return value;
        }

        public static void IncrementCounter<T>([NotNull] this ICacheProvider cacheProvider)
        {
            Guard.NotNull(cacheProvider);

            lock (_syncRoot)
            {
                cacheProvider.Cache.Increment(FormatCachePrefixCounterKey<T>(), 1, 1);
            }
        }

        private static int GetCachingPrefixCounter<T>([NotNull] this ICacheProvider cacheProvider)
        {
            Guard.NotNull(cacheProvider);

            return !cacheProvider.TryGetValue<int>(FormatCachePrefixCounterKey<T>(), out var key) ? 1 : key;
        }

        private static string Name<T>() => typeof(T).FullName;
        private static string Glue => CacheProviderManager.CachePrefixGlue;

        private static string FormatCachePrefixCounterKey<T>()
        {
            return string.Format("{1}{0}{2}",
                Glue,
                Name<T>(),
                CacheProviderManager.CacheCounterPrefix);
        }

        private static string FormatHashedKey<T>([NotNull] this ICacheProvider cacheProvider, [NotNull] string key)
        {
            Guard.NotNull(cacheProvider);
            Guard.NotEmpty(key);
            
            return string.Format("{1}{0}{2}{0}{3}{0}{4}",
                Glue,
                CacheProviderManager.GlobalCachingPrefixCounter,
                cacheProvider.GetCachingPrefixCounter<T>(),
                key.ToSHA256(),
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
            Guard.NotEmpty(sql);

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
            Guard.NotEmpty(sql);

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
            Guard.NotEmpty(keys);

            var f = FormatFetchQueryStrategy(fetchStrategy);

            return $"GetOrSetFind<{Name<T>()}>: [ \n\tKeys = {string.Join(", ", keys.Select(x => x.ToString()).ToArray())},\n\t{f} ]";
        }

        private static string FormatGetOrSetFindAllKey<T, TResult>([CanBeNull] IQueryOptions<T> options, [NotNull] Expression<Func<T, TResult>> selector)
        {
            Guard.NotNull(selector);

            var o = FormatQueryOptions<T>(options);

            return $"GetOrSetFindAll<{Name<T>()}>: [ \n\t{o},\n\tSelector = {ExpressionHelper.TranslateToString(selector)} ]";
        }

        private static string FormatGetOrSetFindKey<T, TResult>([CanBeNull] IQueryOptions<T> options, [NotNull] Expression<Func<T, TResult>> selector)
        {
            Guard.NotNull(selector);

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
            Guard.NotNull(keySelector);
            Guard.NotNull(elementSelector);

            var o = FormatQueryOptions<T>(options);

            return $"GetOrSetDictionary<{Name<T>()}, {typeof(TDictionaryKey).Name}, {typeof(TElement).Name}>: [ \n\t{o},\n\tKeySelector = {ExpressionHelper.TranslateToString(keySelector)},\n\tElementSelector = {ExpressionHelper.TranslateToString(elementSelector)} ]";
        }

        private static string FormatGetOrSetGroupKey<T, TGroupKey, TResult>([CanBeNull] IQueryOptions<T> options, [NotNull] Expression<Func<T, TGroupKey>> keySelector, [NotNull] Expression<Func<TGroupKey, IEnumerable<T>, TResult>> resultSelector)
        {
            Guard.NotNull(keySelector);
            Guard.NotNull(resultSelector);

            var o = FormatQueryOptions<T>(options);

            return $"GetOrSetGroup<{Name<T>()}, {typeof(TGroupKey).Name}, {typeof(TResult).Name}>: [ \n\t{o},\n\tKeySelector = {ExpressionHelper.TranslateToString(keySelector)},\n\tResultSelector = {ExpressionHelper.TranslateToString(resultSelector)} ]";
        }
    }
}