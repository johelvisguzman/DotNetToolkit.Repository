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
        protected static void ForAllRepositoryFactories(Action<IRepositoryFactory> action, params Type[] contextConfigurationTypeExceptionList)
        {
            GetRepositoryContextFactories()
                .ToList()
                .ForEach(x =>
                {
                    var type = x.GetType();

                    if (contextConfigurationTypeExceptionList != null && contextConfigurationTypeExceptionList.Contains(type))
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
                    var type = x.GetType();

                    if (contextTypeExceptionList != null && contextTypeExceptionList.Contains(type))
                        return;

                    // Perform test
                    var task = Record.ExceptionAsync(() => action(new RepositoryFactory(x)));

                    // Checks to see if we have any un-handled exception
                    if (task != null)
                    {
                        var ex = await task;

                        // the in-memory context will not support async operations for now (an exception should be thrown)
                        if (typeof(InMemoryRepositoryContextFactory).IsAssignableFrom(type) ||
                            typeof(CsvRepositoryContextFactory).IsAssignableFrom(type) ||
                            typeof(JsonRepositoryContextFactory).IsAssignableFrom(type) ||
                            typeof(XmlRepositoryContextFactory).IsAssignableFrom(type))
                        {
                            var err = ex.InnerException?.Message ?? ex.Message;

                            Assert.Contains(Properties.Resources.IRepositoryContextNotAsync, err);
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
                    var type = x.GetType();

                    // the in-memory context will not support transactions currently
                    if (typeof(InMemoryRepositoryContextFactory).IsAssignableFrom(type) ||
                        typeof(CsvRepositoryContextFactory).IsAssignableFrom(type) ||
                        typeof(JsonRepositoryContextFactory).IsAssignableFrom(type) ||
                        typeof(XmlRepositoryContextFactory).IsAssignableFrom(type) ||
                        typeof(EfCoreRepositoryContextFactory<TestEfCoreDbContext>).IsAssignableFrom(type))
                        return;

                    action(new UnitOfWorkFactory(x));
                });
        }

        protected static IEnumerable<IRepositoryContextFactory> GetRepositoryContextFactories()
        {
            return new List<IRepositoryContextFactory>
            {
                TestAdoNetContextFactory.Create(),
                TestEfCoreDbContextFactory.Create(),
                TestEfDbContextFactory.Create(),
                new InMemoryRepositoryContextFactory(Guid.NewGuid().ToString()),
                new CsvRepositoryContextFactory(Path.GetTempPath() + Guid.NewGuid().ToString("N")),
                new JsonRepositoryContextFactory(Path.GetTempPath() + Guid.NewGuid().ToString("N")),
                new XmlRepositoryContextFactory(Path.GetTempPath() + Guid.NewGuid().ToString("N"))
            };
        }
    }
}
