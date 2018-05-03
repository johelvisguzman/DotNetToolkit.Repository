namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public abstract class TestBase
    {
        protected static void ForAllRepositories(Action<IRepository<Customer, int>> action)
        {
            GetRepositories().ForEach(action);
        }

        protected static void ForAllRepositoriesAsync(Func<IRepositoryAsync<Customer, int>, Task> action)
        {
            foreach (var repo in GetRepositories())
            {
                if (repo.GetType().IsSubClassOfGeneric(typeof(IRepositoryAsync<,>)))
                {
                    action((IRepositoryAsync<Customer, int>)repo);
                }
            }
        }

        protected static void ForAllRepositoriesInMemory(Action<InMemory.InMemoryRepository<Customer, int>> action)
        {
            GetRepositories().OfType<InMemory.InMemoryRepository<Customer, int>>().ToList().ForEach(action);
        }

        protected static void ForAllRepositoriesInMemoryFileBased(Action<IRepository<Customer, int>> action)
        {
            GetInMemoryFileBasedRepositories().ForEach(action);
        }

        protected static IRepository<Customer, int> CreateRepositoryInstanceOfType(Type type, object arg)
        {
            try
            {
                return (IRepository<Customer, int>)Activator.CreateInstance(type, arg);
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

        private static List<IRepository<Customer, int>> GetInMemoryFileBasedRepositories()
        {
            return new List<IRepository<Customer, int>>
            {
                new Json.JsonRepository<Customer>(GetTempFileName(Guid.NewGuid().ToString("N") + ".json")),
                new Xml.XmlRepository<Customer>(GetTempFileName(Guid.NewGuid().ToString("N") + ".xml")),
                new Csv.CsvRepository<Customer>(GetTempFileName(Guid.NewGuid().ToString("N") + ".csv"))
            };
        }

        private static List<IRepository<Customer, int>> GetRepositories()
        {
            TestAdoNetConnectionStringFactory.Create(out string providerName, out string connectionString);

            var repos = new List<IRepository<Customer, int>>
            {
                new InMemory.InMemoryRepository<Customer>(Guid.NewGuid().ToString()),
                new EntityFramework.EfRepository<Customer>(TestEfDbContextFactory.Create()),
                new EntityFrameworkCore.EfCoreRepository<Customer>(new TestEfCoreDbContext(Guid.NewGuid().ToString())),
                new AdoNet.AdoNetRepository<Customer>(providerName, connectionString)
            };

            repos.AddRange(GetInMemoryFileBasedRepositories());

            return repos;
        }
    }
}
