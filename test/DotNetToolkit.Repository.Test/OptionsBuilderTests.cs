namespace DotNetToolkit.Repository.Test
{
    using Caching.InMemory;
    using Configuration.Conventions;
    using Configuration.Logging;
    using Configuration.Mapper;
    using Configuration.Options;
    using Data;
    using InMemory;
    using Integration.Test.Data;
    using System;
    using System.IO;
    using System.Linq;
    using Xunit;

    public class OptionsBuilderTests
    {
        [Fact]
        public void ConfigureInterceptor()
        {
            var optionsBuilder = new RepositoryOptionsBuilder();

            Assert.False(optionsBuilder.IsConfigured);

            optionsBuilder.UseInterceptor(new TestRepositoryInterceptor("Random Param", false));

            Assert.True(optionsBuilder.IsConfigured);

            Assert.Single(optionsBuilder.Options.Interceptors);

            Assert.True(optionsBuilder.Options.Interceptors.ContainsKey(typeof(TestRepositoryInterceptor)));
        }

        [Fact]
        public void ConfigureInterceptorOncePerType()
        {
            var optionsBuilder = new RepositoryOptionsBuilder();

            Assert.False(optionsBuilder.IsConfigured);

            optionsBuilder
                .UseInterceptor(new TestRepositoryInterceptor("Random Param", false))
                .UseInterceptor(new TestRepositoryInterceptor("Another Random Param", true));

            Assert.True(optionsBuilder.IsConfigured);

            Assert.Single(optionsBuilder.Options.Interceptors);

            Assert.True(optionsBuilder.Options.Interceptors.ContainsKey(typeof(TestRepositoryInterceptor)));
        }


        [Fact]
        public void ConfigureMultipleInterceptorsOfDifferentType()
        {
            var optionsBuilder = new RepositoryOptionsBuilder();

            Assert.False(optionsBuilder.IsConfigured);

            optionsBuilder
                .UseInterceptor(new TestRepositoryInterceptor("Random Param", false))
                .UseInterceptor(new TestRepositoryInterceptor2());

            Assert.True(optionsBuilder.IsConfigured);

            Assert.Equal(2, optionsBuilder.Options.Interceptors.Count());

            Assert.True(optionsBuilder.Options.Interceptors.ContainsKey(typeof(TestRepositoryInterceptor)));
            Assert.True(optionsBuilder.Options.Interceptors.ContainsKey(typeof(TestRepositoryInterceptor2)));
        }

        [Fact]
        public void ConfigureInternalContextFactory()
        {
            var optionsBuilder = new RepositoryOptionsBuilder();

            Assert.False(optionsBuilder.IsConfigured);

            optionsBuilder.UseInMemoryDatabase();

            Assert.True(optionsBuilder.IsConfigured);

            Assert.NotNull(optionsBuilder.Options.ContextFactory);
        }

        [Fact]
        public void ConfigureConventions()
        {
            var optionsBuilder = new RepositoryOptionsBuilder();

            Assert.False(optionsBuilder.IsConfigured);

            optionsBuilder.UseConventions(new RepositoryConventions());

            Assert.True(optionsBuilder.IsConfigured);

            Assert.NotNull(optionsBuilder.Options.Conventions);
        }

        [Fact]
        public void ConfigureMappingProvider()
        {
            var optionsBuilder = new RepositoryOptionsBuilder();

            Assert.False(optionsBuilder.IsConfigured);

            optionsBuilder.UseMapperProvider(new MapperProvider());

            Assert.True(optionsBuilder.IsConfigured);

            Assert.NotNull(optionsBuilder.Options.MapperProvider);
        }

        [Fact]
        public void ConfigureLoggingProvider()
        {
            var optionsBuilder = new RepositoryOptionsBuilder();

            Assert.False(optionsBuilder.IsConfigured);

            optionsBuilder.UseLoggerProvider(new ConsoleLoggerProvider(LogLevel.Debug));

            Assert.True(optionsBuilder.IsConfigured);

            Assert.NotNull(optionsBuilder.Options.LoggerProvider);
        }

        [Fact]
        public void ConfigureCachingProvider()
        {
            var optionsBuilder = new RepositoryOptionsBuilder();

            Assert.False(optionsBuilder.IsConfigured);

            optionsBuilder.UseCachingProvider(new InMemoryCacheProvider());

            Assert.True(optionsBuilder.IsConfigured);

            Assert.NotNull(optionsBuilder.Options.CachingProvider);
        }

        [Fact]
        public void ConfigureFromAppConfig()
        {
            var optionsBuilder = new RepositoryOptionsBuilder();

            Assert.False(optionsBuilder.IsConfigured);

            optionsBuilder.UseConfiguration();

            Assert.True(optionsBuilder.IsConfigured);

            TestConfiguration(optionsBuilder);
        }

        [Fact]
        public void ConfigureFromRepositoryConfig()
        {
            var optionsBuilder = new RepositoryOptionsBuilder();

            Assert.False(optionsBuilder.IsConfigured);

            optionsBuilder.UseConfiguration("repository.config");

            Assert.True(optionsBuilder.IsConfigured);

            TestConfiguration(optionsBuilder);
        }

        [Fact]
        public void ConfigureFromAppSetting()
        {
            var optionsBuilder = new RepositoryOptionsBuilder();

            Assert.False(optionsBuilder.IsConfigured);

            optionsBuilder.UseConfiguration(TestConfigurationHelper.GetConfiguration());

            Assert.True(optionsBuilder.IsConfigured);

            TestConfiguration(optionsBuilder);
        }


        [Fact]
        public void ThrowsIfConfigureFromRepositoryConfigNotFound()
        {
            var ex = Assert.Throws<FileNotFoundException>(
                () => new RepositoryOptionsBuilder()
                    .UseConfiguration("random.config"));

            Assert.Equal("The file is not found.", ex.Message);
            Assert.Equal("random.config", ex.FileName);
        }

        [Fact]
        public void ThrowsIfConfigurationSectionNotFound()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                () => new RepositoryOptionsBuilder()
                    .UseConfiguration("empty_repository.config"));

            Assert.Equal("Unable to find a 'repository' configuration section. For more information on DotNetToolkit.Repository configuration, visit the https://github.com/johelvisguzman/DotNetToolkit.Repository/wiki/Config-File-Setup.", ex.Message);

            ex = Assert.Throws<InvalidOperationException>(
                () => new RepositoryOptionsBuilder()
                    .UseConfiguration(TestConfigurationHelper.GetConfiguration("empty_repository.json")));

            Assert.Equal("Unable to find a 'repository' configuration section. For more information on DotNetToolkit.Repository configuration, visit the https://github.com/johelvisguzman/DotNetToolkit.Repository/wiki/Config-File-Setup.", ex.Message);
        }

        private static void TestConfiguration(RepositoryOptionsBuilder optionsBuilder)
        {
            Assert.True(optionsBuilder.IsConfigured);

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
