namespace DotNetToolkit.Repository.Caching.InMemory.Internal
{
    using Configuration.Caching;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Internal;
    using System;
    using Utility;

    internal class InMemoryCacheProvider : ICacheProvider
    {
        #region Properties

        public IMemoryCache Cache { get; }
        public TimeSpan? Expiry { get; set; }

        #endregion

        #region Constructors

        public InMemoryCacheProvider() : this(null, null, null) { }
        
        public InMemoryCacheProvider(ISystemClock clock, TimeSpan? expirationScanFrequency, TimeSpan? expiry) 
            : this (GetMemoryCacheOptions(clock, expirationScanFrequency), expiry) { }

        private InMemoryCacheProvider(MemoryCacheOptions options, TimeSpan? expiry)
        {
            Cache = new MemoryCache(Guard.NotNull(options, nameof(options)));
            Expiry = expiry;
        }

        #endregion

        #region Private Methods

        private static MemoryCacheOptions GetMemoryCacheOptions(ISystemClock clock, TimeSpan? expirationScanFrequency)
        {
            var options = new MemoryCacheOptions();

            options.Clock = clock;

            if (expirationScanFrequency.HasValue)
            {
                options.ExpirationScanFrequency = expirationScanFrequency.Value;
            }

            return options;
        }

        private void Set<T>(string key, T value, CacheItemPriority priority, TimeSpan? expiry, Action<string> cacheRemovedCallback = null)
        {
            Guard.NotEmpty(key, nameof(key));

            var policy = new MemoryCacheEntryOptions();

            if (cacheRemovedCallback != null)
            {
                policy.PostEvictionCallbacks.Add(new PostEvictionCallbackRegistration
                {
                    EvictionCallback = (o, val, reason, state) =>
                    {
                        cacheRemovedCallback(reason.ToString());
                    }
                });
            }

            if (expiry.HasValue && expiry.Value != TimeSpan.Zero)
                policy.AbsoluteExpiration = DateTimeOffset.Now.Add(expiry.Value);

            if (expiry.HasValue && expiry.Value == TimeSpan.Zero && priority != CacheItemPriority.NeverRemove)
                policy.Priority = CacheItemPriority.NeverRemove;
            else
                policy.Priority = priority;

            Cache.Set<T>(key, value, policy);
        }

        #endregion

        #region Implementation of ICacheProvider

        public void Set<T>([NotNull] string key, T value, TimeSpan? expiry = null, Action<string> cacheRemovedCallback = null)
        {
            Set<T>(key, value, CacheItemPriority.Normal, expiry ?? Expiry, cacheRemovedCallback);
        }

        public void Remove([NotNull] string key)
        {
            Cache.Remove(Guard.NotEmpty(key, nameof(key)));
        }

        public bool TryGetValue<T>([NotNull] string key, out T value)
        {
            return Cache.TryGetValue<T>(Guard.NotEmpty(key, nameof(key)), out value);
        }

        public int Increment([NotNull] string key, int defaultValue, int incrementValue)
        {
            Guard.NotEmpty(key, nameof(key));

            if (!TryGetValue<int>(key, out var current))
                current = defaultValue;

            var value = current + incrementValue;

            Set<int>(key, value, CacheItemPriority.NeverRemove, expiry: null);

            return value;
        }

        #endregion
    }
}
