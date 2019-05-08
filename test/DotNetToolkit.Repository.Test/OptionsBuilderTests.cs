namespace DotNetToolkit.Repository.Test
{
    using Configuration.Conventions;
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
        public void ConfigureConventions()
        {
            var conventions = new RepositoryConventions
            {
                PrimaryKeysCallback = (type) => null
            };

            var optionsBuilder = new RepositoryOptionsBuilder()
                .UseInMemoryDatabase()
                .UseConventions(conventions);

            Assert.NotNull(optionsBuilder.Options.Conventions);

            // will fail since it cannot find a primary key due to the conventions returning null
            var ex = Assert.Throws<InvalidOperationException>(() => new Repository<Customer>(optionsBuilder.Options));

            Assert.Equal($"The instance of entity type '{typeof(Customer).FullName}' requires a primary key to be defined.", ex.Message);
        }

        [Fact]
        public void ConfigureFromAppConfig()
        {
            var optionsBuilder = new RepositoryOptionsBuilder()
                .UseConfiguration();

            TestConfiguration(optionsBuilder);
        }
        
        [Fact]
        public void ConfigureFromRepositoryConfig()
        {
            var optionsBuilder = new RepositoryOptionsBuilder()
                .UseConfiguration("repository.config");

            TestConfiguration(optionsBuilder);
        }

        [Fact]
        public void ConfigureFromAppSetting()
        {
            var optionsBuilder = new RepositoryOptionsBuilder()
                .UseConfiguration(TestConfigurationHelper.GetConfiguration());

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
