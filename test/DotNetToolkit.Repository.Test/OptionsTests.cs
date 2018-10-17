namespace DotNetToolkit.Repository.Test
{
    using Configuration.Options;
    using InMemory;
    using Internal;
    using System.Linq;
    using Xunit;

    public class OptionsTests
    {
        [Fact]
        public void AddExtensions()
        {
            var options = new RepositoryOptions();

            Assert.Empty(options.Extensions);

            options.AddOrUpdateExtension(new RepositoryOptionsCoreExtension());

            Assert.Single(options.Extensions);
        }

        [Fact]
        public void RemoveExtensions()
        {
            var options = new RepositoryOptions();

            options.AddOrUpdateExtension(new RepositoryOptionsCoreExtension());

            Assert.Single(options.Extensions);

            options.TryRemoveExtensions<RepositoryOptionsCoreExtension>();

            Assert.Empty(options.Extensions);
        }

        [Fact]
        public void RemoveExtensionsOfSameType()
        {
            var options = new RepositoryOptions();

            options.AddOrUpdateExtension(new RepositoryOptionsCoreExtension());
            options.AddOrUpdateExtension(new InMemoryRepositoryOptionsExtension());

            Assert.Equal(2, options.Extensions.Count());

            options.TryRemoveExtensions<IRepositoryOptionsExtensions>();

            Assert.Empty(options.Extensions);
        }

        [Fact]
        public void FindExtension()
        {
            var options = new RepositoryOptions();

            Assert.Null(options.FindExtension<RepositoryOptionsCoreExtension>());
            Assert.Null(options.FindExtension<IRepositoryOptionsExtensions>());

            options.AddOrUpdateExtension(new RepositoryOptionsCoreExtension());

            Assert.NotNull(options.FindExtension<RepositoryOptionsCoreExtension>());
            Assert.NotNull(options.FindExtension<IRepositoryOptionsExtensions>());
        }

        [Fact]
        public void Clone()
        {
            var options1 = new RepositoryOptions();
            options1.AddOrUpdateExtension(new RepositoryOptionsCoreExtension());

            var options2 = options1.Clone();

            Assert.Single(options2.Extensions);
            Assert.False(options2 == options1);
        }
    }
}
