namespace DotNetToolkit.Repository.Test
{
    using Configuration.Options;
    using Data;
    using InMemory;
    using Integration.Test.Data;
    using System.Linq;
    using Xunit;

    public class OptionsBuilderTests
    {
        [Fact]
        public void ConfigureInterceptor()
        {
            var optionsBuilder = new RepositoryOptionsBuilder()
                .UseInterceptor(new TestRepositoryInterceptor("Random Param", false));

            Assert.Single(optionsBuilder.Options.Interceptors);

            Assert.True(optionsBuilder.Options.Interceptors.ContainsKey(typeof(TestRepositoryInterceptor)));
        }

        [Fact]
        public void ConfigureInterceptorOncePerType()
        {
            var optionsBuilder = new RepositoryOptionsBuilder()
                .UseInterceptor(new TestRepositoryInterceptor("Random Param", false))
                .UseInterceptor(new TestRepositoryInterceptor("Another Random Param", true));

            Assert.Single(optionsBuilder.Options.Interceptors);

            Assert.True(optionsBuilder.Options.Interceptors.ContainsKey(typeof(TestRepositoryInterceptor)));
        }


        [Fact]
        public void ConfigureMultipleInterceptorsOfDifferentType()
        {
            var optionsBuilder = new RepositoryOptionsBuilder()
                .UseInterceptor(new TestRepositoryInterceptor("Random Param", false))
                .UseInterceptor(new TestRepositoryInterceptor2());

            Assert.Equal(2, optionsBuilder.Options.Interceptors.Count());

            Assert.True(optionsBuilder.Options.Interceptors.ContainsKey(typeof(TestRepositoryInterceptor)));
            Assert.True(optionsBuilder.Options.Interceptors.ContainsKey(typeof(TestRepositoryInterceptor2)));
        }

        [Fact]
        public void ConfigureInternalContextFactory()
        {
            var optionsBuilder = new RepositoryOptionsBuilder()
                .UseInMemoryDatabase();

            Assert.NotNull(optionsBuilder.Options.ContextFactory);
        }

        [Fact]
        public void ConfigureFromAppConfig()
        {
            var optionsBuilder = new RepositoryOptionsBuilder()
                .UseConfiguration();

            Assert.NotNull(optionsBuilder.Options.ContextFactory);
            Assert.NotNull(optionsBuilder.Options.LoggerProvider);
            Assert.NotNull(optionsBuilder.Options.CachingProvider);
            Assert.NotNull(optionsBuilder.Options.CachingProvider.Expiry);
            Assert.NotNull(optionsBuilder.Options.MapperProvider);

            Assert.Equal(1, optionsBuilder.Options.Interceptors.Count());
            Assert.True(optionsBuilder.Options.Interceptors.ContainsKey(typeof(TestRepositoryInterceptor)));
        }

        [Fact]
        public void ConfigureFromAppSetting()
        {
            var optionsBuilder = new RepositoryOptionsBuilder()
                .UseConfiguration(TestConfigurationHelper.GetConfiguration());

            Assert.NotNull(optionsBuilder.Options.ContextFactory);
            Assert.NotNull(optionsBuilder.Options.LoggerProvider);
            Assert.NotNull(optionsBuilder.Options.CachingProvider);
            Assert.NotNull(optionsBuilder.Options.CachingProvider.Expiry);
            Assert.NotNull(optionsBuilder.Options.MapperProvider);

            Assert.Equal(1, optionsBuilder.Options.Interceptors.Count());
            Assert.True(optionsBuilder.Options.Interceptors.ContainsKey(typeof(TestRepositoryInterceptor)));
        }
    }
}
