﻿namespace DotNetToolkit.Repository.Configuration.Caching.Internal
{
    using System;

    /// <summary>
    /// An implementation of <see cref="ICacheProvider" />.
    /// </summary>
    internal class NullCacheProvider : ICacheProvider
    {
        internal static NullCacheProvider Instance { get; } = new NullCacheProvider();

        private NullCacheProvider() { }

        public TimeSpan? Expiry { get; set; }

        public ICache Cache { get; } = NullCache.Instance;
    }
}