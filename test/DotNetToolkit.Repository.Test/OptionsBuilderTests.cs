namespace DotNetToolkit.Repository.Test
{
    using Configuration.Options;
    using Data;
    using InMemory;
    using Internal;
    using System.Linq;
    using Xunit;

    public class OptionsBuilderTests
    {
        [Fact]
        public void ConfigureInterceptor()
        {
            var optionsBuilder = new RepositoryOptionsBuilder()
                .UseInterceptor(new TestRepositoryInterceptor("Random Param", false));

            Assert.Single(optionsBuilder.Options.Extensions);

            var coreExtension = optionsBuilder.Options.FindExtension<RepositoryOptionsCoreExtension>();

            Assert.Single(coreExtension.Interceptors);

            Assert.True(coreExtension.ContainsInterceptorOfType(typeof(TestRepositoryInterceptor)));
        }

        [Fact]
        public void ConfigureInterceptorOncePerType()
        {
            var optionsBuilder = new RepositoryOptionsBuilder()
                .UseInterceptor(new TestRepositoryInterceptor("Random Param", false))
                .UseInterceptor(new TestRepositoryInterceptor("Another Random Param", true));

            Assert.Single(optionsBuilder.Options.Extensions);

            var coreExtension = optionsBuilder.Options.FindExtension<RepositoryOptionsCoreExtension>();

            Assert.Single(coreExtension.Interceptors);

            Assert.True(coreExtension.ContainsInterceptorOfType(typeof(TestRepositoryInterceptor)));
        }


        [Fact]
        public void ConfigureMultipleInterceptorsOfDifferentType()
        {
            var optionsBuilder = new RepositoryOptionsBuilder()
                .UseInterceptor(new TestRepositoryInterceptor("Random Param", false))
                .UseInterceptor(new TestRepositoryInterceptor2());

            Assert.Single(optionsBuilder.Options.Extensions);

            var coreExtension = optionsBuilder.Options.FindExtension<RepositoryOptionsCoreExtension>();

            Assert.Equal(2, coreExtension.Interceptors.Count());

            Assert.True(coreExtension.ContainsInterceptorOfType(typeof(TestRepositoryInterceptor)));
            Assert.True(coreExtension.ContainsInterceptorOfType(typeof(TestRepositoryInterceptor2)));
        }

        [Fact]
        public void ConfigureInternalContextFactory()
        {
            var optionsBuilder = new RepositoryOptionsBuilder()
                .UseInMemoryDatabase();

            Assert.Single(optionsBuilder.Options.Extensions);
        }
    }
}
