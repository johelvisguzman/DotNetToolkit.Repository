namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public abstract class TestBase
    {
        protected static void ForAllRepositories(Action<IRepository<Customer>> action)
        {
            GetRepositories().ForEach(action);
        }

        protected static void ForAllRepositoriesAsync(Func<IRepositoryAsync<Customer>, Task> action)
        {
            GetRepositories().OfType<IRepositoryAsync<Customer>>().ToList().ForEach(async repo => await action(repo));
        }

        protected static void ForAllRepositoriesInMemory(Action<InMemory.InMemoryRepository<Customer>> action)
        {
            GetRepositories().OfType<InMemory.InMemoryRepository<Customer>>().ToList().ForEach(action);
        }

        protected static void ForAllRepositoriesInMemoryFileBased(Action<IRepository<Customer>> action)
        {
            GetInMemoryFileBasedRepositories().ForEach(action);
        }

        protected static IRepository<Customer> CreateRepositoryInstanceOfType(Type type, object arg)
        {
            try
            {
                return (IRepository<Customer>)Activator.CreateInstance(type, arg);
            }
            catch (Exception ex)
            {
                throw ex?.InnerException ?? ex;
            }
        }

        protected static string GetTempFileName(string fileName)
        {
            var path = Path.GetTempPath() + fileName;

            if (File.Exists(path))
                File.Delete(path);

            return path;
        }

        private static List<IRepository<Customer>> GetInMemoryFileBasedRepositories()
        {
            return new List<IRepository<Customer>>
            {
                new Json.JsonRepository<Customer>(GetTempFileName(Guid.NewGuid().ToString("N") + ".json")),
                new Xml.XmlRepository<Customer>(GetTempFileName(Guid.NewGuid().ToString("N") + ".xml")),
                new Csv.CsvRepository<Customer>(GetTempFileName(Guid.NewGuid().ToString("N") + ".csv"))
            };
        }

        private static List<IRepository<Customer>> GetRepositories()
        {
            var adoNet = TestAdoNetConnectionStringFactory.Create();

            var repos = new List<IRepository<Customer>>
            {
                new InMemory.InMemoryRepository<Customer>(Guid.NewGuid().ToString()),
                new EntityFramework.EfRepository<Customer>(TestEfDbContextFactory.Create()),
                new EntityFrameworkCore.EfCoreRepository<Customer>(new TestEfCoreDbContext(Guid.NewGuid().ToString())),
                new AdoNet.AdoNetRepository<Customer>(adoNet.Item1, adoNet.Item2)
            };

            repos.AddRange(GetInMemoryFileBasedRepositories());

            return repos;
        }
    }
}
