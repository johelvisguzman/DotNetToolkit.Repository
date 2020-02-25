namespace DotNetToolkit.Repository.Test.Data
{
    using Configuration.Caching;
    using System;

    public class TestCacheProvider : ICacheProvider
    {
        public ICacheKeyTransformer KeyTransformer { get; set; }

        public TimeSpan? Expiry { get; set; }

        public ICache Cache { get; set; }
    }
}
