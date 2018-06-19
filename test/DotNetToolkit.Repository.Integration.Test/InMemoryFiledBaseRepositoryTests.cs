namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
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
        public void ThrowsIfPathIsFileName()
        {
            ForAllRepositoryFactoriesInMemoryFileBased(TestThrowsIfPathIsFileName);
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
            var path = Path.GetTempPath() + Guid.NewGuid().ToString("N");

            Assert.True(!Directory.Exists(path));

            repo = CreateRepositoryInstanceOfType(repo.GetType(), path);

            Assert.True(Directory.Exists(path));

            Directory.Delete(path, true);
        }

        private static void TestGeneratesTempFileNameWhenOnlyDirectoryIsProvided(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();
            var dir = Path.GetTempPath() + Guid.NewGuid().ToString("N");
            var defaultGeneratedPathName = "Customers" + GetFileExtension(repo);
            var path = dir + "\\" + defaultGeneratedPathName;

            Assert.True(!File.Exists(path));

            repo = CreateRepositoryInstanceOfType(repo.GetType(), dir);

            Assert.True(File.Exists(path));

            Directory.Delete(dir, true);
        }

        private static void TestThrowsIfPathIsFileName(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();
            var path = Path.GetTempPath() + Guid.NewGuid().ToString("N") + "\\" + "TestData.txt";

            var ex = Assert.Throws<InvalidOperationException>(() => CreateRepositoryInstanceOfType(repo.GetType(), path));
            Assert.Equal($"The specified '{path}' path cannot be a file name.", ex.Message);
        }
    }
}
