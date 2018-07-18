namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using Configuration;
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
        protected static void ForAllRepositoryFactories(Action<IRepositoryFactory> action, params Type[] contextTypeExceptionList)
        {
            GetRepositoryContextFactories()
                .ToList()
                .ForEach(x =>
                {
                    var contextType = x().GetType();

                    if (contextTypeExceptionList != null && contextTypeExceptionList.Contains(contextType))
                        return;

                    action(new RepositoryFactory(x));
                });
        }

        protected static void ForAllRepositoryFactoriesAsync(Func<IRepositoryFactory, Task> action, params Type[] contextTypeExceptionList)
        {
            GetRepositoryContextFactories()
                .ToList()
                .ForEach(async x =>
                {
                    var contextType = x().GetType();

                    if (contextTypeExceptionList != null && contextTypeExceptionList.Contains(contextType))
                        return;

                    var task = Record.ExceptionAsync(() => action(new RepositoryFactory(x)));
                    if (task != null)
                    {
                        var ex = await task;

                        // the in-memory context will not support async operations for now (an exception should be thrown)
                        if (typeof(InMemoryRepositoryContext).IsAssignableFrom(contextType))
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
                    var contextType = x().GetType();

                    // the in-memory context will not support transactions currently
                    if (typeof(InMemoryRepositoryContext).IsAssignableFrom(contextType) || typeof(EfCoreRepositoryContext).IsAssignableFrom(contextType))
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
