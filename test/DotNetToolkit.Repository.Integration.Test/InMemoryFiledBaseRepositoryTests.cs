namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using Helpers;
    using System;
    using System.IO;
    using System.Reflection;
    using Xunit;

    public class InMemoryFiledBaseRepositoryTests : TestBase
    {
        [Fact]
        public void CreatesTempFileOnConstruction()
        {
            ForAllRepositoryFactoriesInMemoryFileBased(TestCreatesTempFileOnConstruction);
        }

        [Fact]
        public void GeneratesTempFileNameWhenOnlyDirectoryIsProvided()
        {
            ForAllRepositoryFactoriesInMemoryFileBased(TestGeneratesTempFileNameWhenOnlyDirectoryIsProvided);
        }

        [Fact]
        public void ThrowsIfFilePathIsInvalid()
        {
            ForAllRepositoryFactoriesInMemoryFileBased(TestThrowsIfFilePathIsInvalid);
        }

        [Fact]
        public void ThrowsIfFileExtensionIsNotValid()
        {
            ForAllRepositoryFactoriesInMemoryFileBased(TestThrowsIfFileExtensionIsNotValid);
        }

        private static string GetFileExtension(IRepository<Customer, int> repo)
        {
            var protectedFileExtensionPropertyInfo = repo.GetType().BaseType?.GetProperty("FileExtension", BindingFlags.NonPublic | BindingFlags.Instance);
            if (protectedFileExtensionPropertyInfo == null)
                throw new InvalidOperationException($"Unable to find a 'FileExtension' property for the specified '{repo.GetType()}' repository type.");

            return (string)protectedFileExtensionPropertyInfo.GetValue(repo);
        }

        private static void TestCreatesTempFileOnConstruction(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();
            var path = GetTempFileName(Guid.NewGuid().ToString("N") + GetFileExtension(repo));

            Assert.True(!File.Exists(path));

            repo = CreateRepositoryInstanceOfType(repo.GetType(), path);

            Assert.True(File.Exists(path));

            File.Delete(path);
        }

        private static void TestGeneratesTempFileNameWhenOnlyDirectoryIsProvided(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();
            var dir = GetTempFileName(string.Empty);
            var defaultGeneratedPathName = typeof(Customer).GetTableName() + GetFileExtension(repo);
            var path = dir + defaultGeneratedPathName;

            Assert.True(!File.Exists(path));

            repo = CreateRepositoryInstanceOfType(repo.GetType(), dir);

            Assert.True(File.Exists(path));

            File.Delete(path);
        }

        private static void TestThrowsIfFilePathIsInvalid(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();
            var path = "TestData";
            var ex = Assert.Throws<InvalidOperationException>(() => CreateRepositoryInstanceOfType(repo.GetType(), path));

            Assert.Equal($"The specified '{path}{GetFileExtension(repo)}' file is not a valid path.", ex.Message);
        }

        private static void TestThrowsIfFileExtensionIsNotValid(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();
            var path = GetTempFileName("TestData.tmp");
            var ex = Assert.Throws<InvalidOperationException>(() => CreateRepositoryInstanceOfType(repo.GetType(), path));

            Assert.Equal($"The specified '{path}' file has an invalid extension. Please consider using '{GetFileExtension(repo)}'.", ex.Message);
        }
    }
}
