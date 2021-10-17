namespace DotNetToolkit.Repository.Caching.InMemory
{
    using DotNetToolkit.Repository.Utility;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Internal;
    using System;

    /// <summary>
    /// The options to be used by the in-memory caching provider.
    /// </summary>
    public class InMemoryCacheOptions
    {
        private ISystemClock _clock;
        private TimeSpan? _expirationScanFrequency;
        private TimeSpan? _expiry;

        /// <summary>
        /// Gets the system clock.
        /// </summary>
        public ISystemClock Clock { get { return _clock; } }

        /// <summary>
        /// Gets the expiration scan frequency.
        /// </summary>
        public TimeSpan? ExpirationScanFrequency { get { return _expirationScanFrequency; } }

        /// <summary>
        /// Gets the expiration time.
        /// </summary>
        public TimeSpan? Expiry { get { return _expiry; } }

        /// <summary>
        /// Adds the giving system clock vaue to the options.
        /// </summary>
        /// <param name="clock">The system clock to be added.</param>
        public InMemoryCacheOptions WithClock([NotNull] ISystemClock clock)
        {
            _clock = Guard.NotNull(clock, nameof(clock));

            return this;
        }

        /// <summary>
        /// Adds the giving system clock vaue to the options.
        /// </summary>
        /// <param name="expirationScanFrequency">The minimum length of time between successive scans for expired items to be added.</param>
        public InMemoryCacheOptions WithExpirationScanFrequency([NotNull] TimeSpan expirationScanFrequency)
        {
            _expirationScanFrequency = Guard.NotNull(expirationScanFrequency, nameof(expirationScanFrequency));

            return this;
        }

        /// <summary>
        /// Adds the giving caching expiration time to the options.
        /// </summary>
        /// <param name="expiry">The caching expiration time to be added.</param>
        public InMemoryCacheOptions WithExpiry([NotNull] TimeSpan expiry)
        {
            _expiry = Guard.NotNull(expiry, nameof(expiry));

            return this;
        }
    }
}
