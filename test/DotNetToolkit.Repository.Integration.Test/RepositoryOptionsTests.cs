namespace DotNetToolkit.Repository.Integration.Test
{
    using Configuration.Options;
    using Data;
    using Xunit;
    using Xunit.Abstractions;

    public class RepositoryOptionsTests : TestBase
    {
        public RepositoryOptionsTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void CanConfigureRepositoriesWithDefaultContextFactoryFromAppConfig()
        {
            var options = new RepositoryOptionsBuilder()
                .UseConfiguration()
                .UseLoggerProvider(TestXUnitLoggerProvider)
                .Options;

            Assert.NotNull(options.ContextFactory);
        }

        [Fact]
        public void CanConfigureRepositoriesWithInterceptorsFromAppConfig()
        {
            var options = new RepositoryOptionsBuilder()
                .UseConfiguration()
                .UseLoggerProvider(TestXUnitLoggerProvider)
                .Options;

            Assert.NotEmpty(options.Interceptors);
        }
    }
}
