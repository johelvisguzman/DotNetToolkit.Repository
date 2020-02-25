namespace DotNetToolkit.Repository.Test
{
    using Configuration.Logging;
    using Configuration.Options;
    using Data;
    using InMemory;
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

            optionsBuilder.UseConventions(c =>
            {
                c.TableNameCallback = (type) => type.Name;
            });

            Assert.True(optionsBuilder.IsConfigured);

            Assert.NotNull(optionsBuilder.Options.Conventions);
            Assert.NotNull(optionsBuilder.Options.Conventions.TableNameCallback);
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

            optionsBuilder.UseCachingProvider(new TestCacheProvider());

            Assert.True(optionsBuilder.IsConfigured);

            Assert.NotNull(optionsBuilder.Options.CachingProvider);
        }

#if NETFULL
        [Fact]
        public void ConfigureFromXml()
        {
            var optionsBuilder = new RepositoryOptionsBuilder();

            Assert.False(optionsBuilder.IsConfigured);

            optionsBuilder.UseConfiguration();

            Assert.True(optionsBuilder.IsConfigured);

            TestConfiguration(optionsBuilder);
        }

        [Fact]
        public void ConfigureFromFile()
        {
            var optionsBuilder = new RepositoryOptionsBuilder();

            Assert.False(optionsBuilder.IsConfigured);

            optionsBuilder.UseConfiguration("repository.config");

            Assert.True(optionsBuilder.IsConfigured);

            TestConfiguration(optionsBuilder);
        }
#else
        [Fact]
        public void ConfigureFromJson()
        {
            var optionsBuilder = new RepositoryOptionsBuilder();

            Assert.False(optionsBuilder.IsConfigured);

            optionsBuilder.UseConfiguration(TestConfigurationHelper.GetConfiguration());

            Assert.True(optionsBuilder.IsConfigured);

            TestConfiguration(optionsBuilder);
        }
#endif

#if NETFULL
        [Fact]
        public void ThrowsIfConfigureFromRepositoryConfigNotFound()
        {
            var ex = Assert.Throws<FileNotFoundException>(
                () => new RepositoryOptionsBuilder()
                    .UseConfiguration("random.config"));

            Assert.Equal("The file is not found.", ex.Message);
            Assert.Equal("random.config", ex.FileName);
        }
#endif

        [Fact]
        public void ThrowsIfConfigurationSectionNotFound()
        {
            Exception ex;
#if NETFULL
            ex = Assert.Throws<InvalidOperationException>(
                () => new RepositoryOptionsBuilder()
                    .UseConfiguration("empty_repository.config"));

            Assert.Equal("Unable to find a 'repository' configuration section. For more information on DotNetToolkit.Repository configuration, visit the https://github.com/johelvisguzman/DotNetToolkit.Repository/wiki/Config-File-Setup.", ex.Message);
#else
            ex = Assert.Throws<InvalidOperationException>(
                    () => new RepositoryOptionsBuilder()
                        .UseConfiguration(TestConfigurationHelper.GetConfiguration("empty_repository.json")));

            Assert.Equal("Unable to find a 'repository' configuration section. For more information on DotNetToolkit.Repository configuration, visit the https://github.com/johelvisguzman/DotNetToolkit.Repository/wiki/Config-File-Setup.", ex.Message); 
#endif
        }

        private static void TestConfiguration(RepositoryOptionsBuilder optionsBuilder)
        {
            // is configured
            Assert.True(optionsBuilder.IsConfigured);

            // context factory
            Assert.NotNull(optionsBuilder.Options.ContextFactory);

            var context = optionsBuilder.Options.ContextFactory.Create() as IInMemoryRepositoryContext;

            Assert.NotNull(context);
            Assert.Equal("__InMemoryDatabaseName__", context.DatabaseName);

            // logging provider
            Assert.NotNull(optionsBuilder.Options.LoggerProvider);

            // caching provider
            Assert.NotNull(optionsBuilder.Options.CachingProvider);
            Assert.Equal(optionsBuilder.Options.CachingProvider.Expiry, TimeSpan.FromSeconds(30));

            // interceptor
            Assert.Single(optionsBuilder.Options.Interceptors);
            Assert.True(optionsBuilder.Options.Interceptors.ContainsKey(typeof(TestRepositoryInterceptor)));

            var interceptor = optionsBuilder.Options.Interceptors[typeof(TestRepositoryInterceptor)].Value as TestRepositoryInterceptor;

            Assert.Equal("random param", interceptor.P1);
            Assert.True(interceptor.P2);
        }
    }
}