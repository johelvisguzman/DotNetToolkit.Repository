namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using Factories;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public abstract class TestBase
    {
        protected static void ForAllRepositories(Action<IRepositoryFactory> action)
        {
            GetRepositoryFactories().ForEach(action);
        }

        protected static void ForAllRepositoriesAsync(Func<IRepositoryFactoryAsync, Task> action)
        {
            foreach (var repo in GetRepositoryFactories())
            {
                var repoAsync = repo as IRepositoryFactoryAsync;

                if (repoAsync != null)
                {
                    action(repoAsync);
                }
            }
        }

        protected static void ForAllRepositoriesInMemory(Action<InMemory.InMemoryRepository<Customer, int>> action)
        {
            GetRepositoryFactories().OfType<InMemory.InMemoryRepository<Customer, int>>().ToList().ForEach(action);
        }

        protected static void ForAllRepositoriesInMemoryFileBased(Action<IRepositoryFactory> action)
        {
            GetInMemoryFileBasedRepositoryFactories().ForEach(action);
        }

        protected static void ForAllUnitOfWorkFactories(Action<IUnitOfWorkFactory> action)
        {
            GetUnitOfWorkFactories().ForEach(action);
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

        private static List<IRepositoryFactory> GetInMemoryFileBasedRepositoryFactories()
        {
            return new List<IRepositoryFactory>
            {
                new Json.JsonRepositoryFactory(GetTempFileName(Guid.NewGuid().ToString("N") + ".json")),
                new Xml.XmlRepositoryFactory(GetTempFileName(Guid.NewGuid().ToString("N") + ".xml")),
                new Csv.CsvRepositoryFactory(GetTempFileName(Guid.NewGuid().ToString("N") + ".csv"))
            };
        }

        private static List<IRepositoryFactory> GetRepositoryFactories()
        {
            TestAdoNetConnectionStringFactory.Create(out string providerName, out string connectionString);

            var efCoreContext = new TestEfCoreDbContext(Guid.NewGuid().ToString());
            var efContext = TestEfDbContextFactory.Create();

            var repos = new List<IRepositoryFactory>
            {
                new InMemory.InMemoryRepositoryFactory(Guid.NewGuid().ToString()),
                new EntityFramework.EfRepositoryFactory(() => efContext),
                new EntityFrameworkCore.EfCoreRepositoryFactory(() => efCoreContext),
                new AdoNet.AdoNetRepositoryFactory(providerName, connectionString)
            };

            repos.AddRange(GetInMemoryFileBasedRepositoryFactories());

            return repos;
        }

        private static List<IUnitOfWorkFactory> GetUnitOfWorkFactories()
        {
            TestAdoNetConnectionStringFactory.Create(out string providerName, out string connectionString);

            var uows = new List<IUnitOfWorkFactory>
            {
                new EntityFramework.EfUnitOfWorkFactory(TestEfDbContextFactory.Create),
                new AdoNet.AdoNetUnitOfWorkFactory(providerName, connectionString)
            };

            return uows;
        }
    }
}
