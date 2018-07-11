namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using InMemory;
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Xunit;

    public class InMemoryRepositoryFileContextBaseTests : TestBase
    {
        [Fact]
        public void CreatesFileOnSaveChanges()
        {
            GetRepositoryContextFactories().OfType<InMemoryRepositoryFileContextBase>().ToList().ForEach(x => TestCreatesFileOnSaveChanges(x.GetType()));
        }

        [Fact]
        public void ThrowsIfPathIsFileName()
        {
            GetRepositoryContextFactories().OfType<InMemoryRepositoryFileContextBase>().ToList().ForEach(x => TestThrowsIfPathIsFileName(x.GetType()));
        }

        private static string GetFileExtension(InMemoryRepositoryFileContextBase context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var protectedFileExtensionPropertyInfo = context.GetType().GetProperty("FileExtension", BindingFlags.NonPublic | BindingFlags.Instance);
            if (protectedFileExtensionPropertyInfo == null)
                throw new InvalidOperationException($"Unable to find a 'FileExtension' property for the specified '{context.GetType()}' context type.");

            return (string)protectedFileExtensionPropertyInfo.GetValue(context);
        }

        private static void TestCreatesFileOnSaveChanges(Type repoContextType)
        {
            var dir = Path.GetTempPath() + Guid.NewGuid().ToString("N");
            var context = (InMemoryRepositoryFileContextBase)Activator.CreateInstance(repoContextType, dir);
            var repo = new Repository<Customer>(context);
            var defaultGeneratedPathName = "Customers" + GetFileExtension(context);
            var path = dir + "\\" + defaultGeneratedPathName;

            Assert.True(!File.Exists(path));

            repo.Add(new Customer());

            Assert.True(File.Exists(path));

            Directory.Delete(dir, true);
        }

        private static void TestThrowsIfPathIsFileName(Type repoContextType)
        {
            var path = Path.GetTempPath() + Guid.NewGuid().ToString("N") + "\\" + "TestData.txt";

            var ex = Assert.Throws<ArgumentException>(() =>
            {
                try
                {
                    return (IRepositoryContext)Activator.CreateInstance(repoContextType, path);
                }
                catch (Exception e)
                {
                    throw e?.InnerException ?? e;
                }
            });
            Assert.Equal($"The specified '{path}' path cannot be a file name.", ex.Message);
        }
    }
}
