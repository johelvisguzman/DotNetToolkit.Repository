namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using Csv;
    using EntityFrameworkCore;
    using Factories;
    using InMemory;
    using Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Xml;
    using Xunit;

    public abstract class TestBase
    {
        protected static void ForAllRepositoryFactories(Action<IRepositoryFactory> action)
        {
            GetRepositoryContextFactories()
                .ToList()
                .ForEach(x => action(new RepositoryFactory(x)));
        }

        protected static void ForAllRepositoryFactoriesAsync(Func<IRepositoryFactory, Task> action)
        {
            GetRepositoryContextFactories()
                .ToList()
                .ForEach(async x =>
                {
                    var task = Record.ExceptionAsync(() => action(new RepositoryFactory(x)));
                    if (task != null)
                    {
                        var ex = await task;

                        // the in-memory context will not support async operations for now (an exception should be thrown)
                        if (x() is InMemoryRepositoryContext)
                        {
                            Assert.Contains(Properties.Resources.IRepositoryContextNotAsync, ex.Message);
                        }
                        else
                        {
                            Assert.Null(ex);
                        }
                    }
                });
        }

        protected static void ForAllUnitOfWorkFactories(Action<IUnitOfWorkFactory> action)
        {
            GetRepositoryContextFactories()
                .ToList()
                .ForEach(x =>
                {
                    // the in-memory context will not support transactions currently
                    if (x() is InMemoryRepositoryContext || x() is EfCoreRepositoryContext)
                        return;

                    action(new UnitOfWorkFactory(x));
                });
        }

        protected static IEnumerable<Func<IRepositoryContext>> GetRepositoryContextFactories()
        {
            return new List<Func<IRepositoryContext>>
            {
                () => TestAdoNetContextFactory.Create(),
                () => TestEfCoreDbContextFactory.Create(),
                () => TestEfDbContextFactory.Create(),
                () => new InMemoryRepositoryContext(Guid.NewGuid().ToString()),
                () => new CsvRepositoryContext(Path.GetTempPath() + Guid.NewGuid().ToString("N")),
                () => new JsonRepositoryContext(Path.GetTempPath() + Guid.NewGuid().ToString("N")),
                () => new XmlRepositoryContext(Path.GetTempPath() + Guid.NewGuid().ToString("N"))
            };
        }
    }
}
