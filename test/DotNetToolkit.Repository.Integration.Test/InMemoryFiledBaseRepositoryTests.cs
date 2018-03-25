namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Json;
    using System;
    using System.IO;
    using Xunit;

    public class InMemoryFiledBaseRepositoryTests : TestBase
    {
        [Fact]
        public void CreatesTempFileOnConstruction()
        {
            ForAllRepositoriesInMemoryFileBased(TestCreatesTempFileOnConstruction);
        }

        [Fact]
        public void GeneratesTempFileNameWhenOnlyDirectoryIsProvided()
        {
            ForAllRepositoriesInMemoryFileBased(TestGeneratesTempFileNameWhenOnlyDirectoryIsProvided);
        }

        [Fact]
        public void ThrowsIfFilePathIsInvalid()
        {
            ForAllRepositoriesInMemoryFileBased(TestThrowsIfFilePathIsInvalid);
        }

        [Fact]
        public void ThrowsIfFileExtensionIsNotValid()
        {
            ForAllRepositoriesInMemoryFileBased(TestThrowsIfFileExtensionIsNotValid);
        }

        private static string GetFileExtension(Type type)
        {
            if (type == typeof(JsonRepository<Customer>))
                return ".json";

            return ".xml";
        }

        private static void TestCreatesTempFileOnConstruction(IRepository<Customer> repo)
        {
            var path = GetTempFileName(Guid.NewGuid().ToString("N") + GetFileExtension(repo.GetType()));

            Assert.True(!File.Exists(path));

            repo = CreateRepositoryInstanceOfType(repo.GetType(), path);

            Assert.True(File.Exists(path));
        }

        private static void TestGeneratesTempFileNameWhenOnlyDirectoryIsProvided(IRepository<Customer> repo)
        {
            var path = GetTempFileName($"{typeof(Customer).Name}" + GetFileExtension(repo.GetType()));

            Assert.True(!File.Exists(path));

            repo = CreateRepositoryInstanceOfType(repo.GetType(), path);

            Assert.True(File.Exists(path));
        }

        private static void TestThrowsIfFilePathIsInvalid(IRepository<Customer> repo)
        {
            var path = "TestData";
            var ex = Assert.Throws<InvalidOperationException>(() => CreateRepositoryInstanceOfType(repo.GetType(), path));

            Assert.Equal($"The specified '{path}{GetFileExtension(repo.GetType())}' file is not a valid path.", ex.Message);
        }

        private static void TestThrowsIfFileExtensionIsNotValid(IRepository<Customer> repo)
        {
            var path = GetTempFileName("TestData.tmp");
            var ex = Assert.Throws<InvalidOperationException>(() => CreateRepositoryInstanceOfType(repo.GetType(), path));

            Assert.Equal($"The specified '{path}' file has an invalid extension. Please consider using '{GetFileExtension(repo.GetType())}'.", ex.Message);
        }
    }
}
