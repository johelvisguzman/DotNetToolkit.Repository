namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using Configuration.Options;
    using EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using System;

    public static class TestEfCoreOptionsBuilderFactory
    {
        public static RepositoryOptionsBuilder Create()
        {
            return new RepositoryOptionsBuilder().UseEntityFrameworkCore<TestEfCoreDbContext>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });
        }
    }
}