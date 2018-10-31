namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using Configuration.Logging;
    using Configuration.Options;
    using EntityFrameworkCore;
    using Factories;
    using InMemory;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;

    public abstract class TestBase
    {
        protected TestBase(ITestOutputHelper testOutputHelper)
        {
            TestXUnitLoggerProvider = new TestXUnitLoggerProvider(testOutputHelper);
        }

        protected ILoggerProvider TestXUnitLoggerProvider { get; }

        protected void ForAllRepositoryFactories(Action<IRepositoryFactory> action, params Type[] contextConfigurationTypeExceptionList)
        {
            GetRepositoryContextFactories()
                .ToList()
                .ForEach(x =>
                {
                    var type = x.GetType();

                    if (contextConfigurationTypeExceptionList != null && contextConfigurationTypeExceptionList.Contains(type))
                        return;

                    var options = new RepositoryOptionsBuilder()
                        .UseInternalContextFactory(x)
                        .UseLoggerProvider(TestXUnitLoggerProvider)
                        .Options;

                    action(new RepositoryFactory(options));
                });
        }

        protected void ForAllRepositoryFactoriesAsync(Func<IRepositoryFactory, Task> action, params Type[] contextTypeExceptionList)
        {
            GetRepositoryContextFactories()
                .ToList()
                .ForEach(async x =>
                {
                    var type = x.GetType();

                    if (contextTypeExceptionList != null && contextTypeExceptionList.Contains(type))
                        return;

                    var options = new RepositoryOptionsBuilder()
                        .UseInternalContextFactory(x)
                        .UseLoggerProvider(TestXUnitLoggerProvider)
                        .Options;

                    // Perform test
                    var task = Record.ExceptionAsync(() => action(new RepositoryFactory(options)));

                    // Checks to see if we have any un-handled exception
                    if (task != null)
                    {
                        var ex = await task;

                        Assert.Null(ex);
                    }
                });
        }

        protected void ForAllUnitOfWorkFactories(Action<IUnitOfWorkFactory> action)
        {
            GetRepositoryContextFactories()
                .ToList()
                .ForEach(x =>
                {
                    var type = x.GetType();

                    var options = new RepositoryOptionsBuilder()
                        .UseInternalContextFactory(x)
                        .UseLoggerProvider(TestXUnitLoggerProvider)
                        .Options;

                    // the in-memory context will not support transactions currently
                    if (typeof(InMemoryRepositoryContextFactory).IsAssignableFrom(type) ||
                        typeof(EfCoreRepositoryContextFactory<TestEfCoreDbContext>).IsAssignableFrom(type))
                        return;

                    action(new UnitOfWorkFactory(options));
                });
        }

        protected void ForAllUnitOfWorkFactoriesAsync(Func<IUnitOfWorkFactory, Task> action)
        {
            GetRepositoryContextFactories()
                .ToList()
                .ForEach(async x =>
                {
                    var type = x.GetType();

                    var options = new RepositoryOptionsBuilder()
                        .UseInternalContextFactory(x)
                        .UseLoggerProvider(TestXUnitLoggerProvider)
                        .Options;

                    // the in-memory context will not support transactions currently
                    if (typeof(InMemoryRepositoryContextFactory).IsAssignableFrom(type) ||
                        typeof(EfCoreRepositoryContextFactory<TestEfCoreDbContext>).IsAssignableFrom(type))
                        return;

                    // Perform test
                    var task = Record.ExceptionAsync(() => action(new UnitOfWorkFactory(options)));

                    // Checks to see if we have any un-handled exception
                    if (task != null)
                    {
                        var ex = await task;

                        Assert.Null(ex);
                    }
                });
        }

        protected static IEnumerable<IRepositoryContextFactory> GetRepositoryContextFactories()
        {
            return new List<IRepositoryContextFactory>
            {
                TestAdoNetContextFactory.Create(),
                TestEfCoreDbContextFactory.Create(),
                TestEfDbContextFactory.Create(),
                new InMemoryRepositoryContextFactory(Guid.NewGuid().ToString())
            };
        }
    }
}
