namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using EntityFramework;
    using EntityFrameworkCore;
    using InMemory;
    using Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Xml;

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

        protected static void ForAllRepositoriesInMemory(Action<InMemoryRepository<Customer>> action)
        {
            GetRepositories().OfType<InMemoryRepository<Customer>>().ToList().ForEach(action);
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
                new JsonRepository<Customer>(GetTempFileName(Guid.NewGuid().ToString("N") + ".json")),
                new XmlRepository<Customer>(GetTempFileName(Guid.NewGuid().ToString("N") + ".xml"))
            };
        }

        private static List<IRepository<Customer>> GetEfRepositories()
        {
            return new List<IRepository<Customer>>
            {
                new EfRepository<Customer>(TestEfDbContextFactory.Create()),
                new EfCoreRepository<Customer>(new TestEfCoreDbContext(Guid.NewGuid().ToString())),
            };
        }

        private static List<IRepository<Customer>> GetRepositories()
        {
            var repos = new List<IRepository<Customer>>
            {
                new InMemoryRepository<Customer>(Guid.NewGuid().ToString()),
            };

            repos.AddRange(GetEfRepositories());
            repos.AddRange(GetInMemoryFileBasedRepositories());

            return repos;
        }
    }
}
