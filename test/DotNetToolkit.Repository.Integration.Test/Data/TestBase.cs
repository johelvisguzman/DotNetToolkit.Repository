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

        protected static void ForAllRepositoryFactoriesAsync(Func<IRepositoryFactoryAsync, Task> action)
        {
            GetRepositoryContextFactoriesAsync()
                .ToList()
                .ForEach(x =>
                {
                    var ex = Record.ExceptionAsync(async () => await action(new RepositoryFactoryAsync(x)));

                    Assert.Null(ex?.Result);
                });
        }

        protected static void ForAllUnitOfWorkFactories(Action<IUnitOfWorkFactory> action)
        {
            GetRepositoryContextFactories()
                .ToList()
                .ForEach(x =>
                {
                    // the in memory context will not support transactions currently
                    if (x() is InMemoryRepositoryContext || x() is EfCoreRepositoryContext)
                        return;

                    action(new UnitOfWorkFactory(x));
                });
        }

        protected static IEnumerable<Func<IRepositoryContextAsync>> GetRepositoryContextFactoriesAsync()
        {
            return new List<Func<IRepositoryContextAsync>>
            {
                () => TestAdoNetContextFactory.Create(),
                () => TestEfCoreDbContextFactory.Create(),
                () => TestEfDbContextFactory.Create()
            };
        }

        protected static IEnumerable<Func<IRepositoryContext>> GetRepositoryContextFactories()
        {
            var contexts = new List<Func<IRepositoryContext>>
            {
                () => new InMemoryRepositoryContext(Guid.NewGuid().ToString()),
                () => new CsvRepositoryContext(Path.GetTempPath() + Guid.NewGuid().ToString("N")),
                () => new JsonRepositoryContext(Path.GetTempPath() + Guid.NewGuid().ToString("N")),
                () => new XmlRepositoryContext(Path.GetTempPath() + Guid.NewGuid().ToString("N"))
            };

            contexts.AddRange(GetRepositoryContextFactoriesAsync());

            return contexts;
        }
    }
}
