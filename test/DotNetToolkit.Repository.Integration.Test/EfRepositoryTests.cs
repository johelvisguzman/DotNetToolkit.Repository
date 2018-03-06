namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using EntityFramework;
    using Specifications;
    using System.Collections.Generic;
    using Xunit;

    public class EfRepositoryTests
    {
        [Fact]
        public void Add()
        {
            using (var context = TestDbContextFactory.Create())
            {
                const string name = "Random Name";

                var entity = new Customer { Name = name };
                var repo = new EfRepository<Customer>(context);

                Assert.False(repo.Exists(x => x.Name.Equals(name)));

                repo.Add(entity);

                Assert.True(repo.Exists(x => x.Name.Equals(name)));
            }
        }

        [Fact]
        public void Add_Range()
        {
            using (var context = TestDbContextFactory.Create())
            {
                const string name = "Random Name";

                var entities = new List<Customer>
                {
                    new Customer { Name = name },
                    new Customer { Name = name }
                };

                var repo = new EfRepository<Customer>(context);

                Assert.Equal(0, repo.Count());

                repo.Add(entities);

                Assert.Equal(2, repo.Count());
            }
        }

        [Fact]
        public void Delete()
        {
            using (var context = TestDbContextFactory.Create())
            {
                const string name = "Random Name";

                var entity = new Customer { Name = name };
                var repo = new EfRepository<Customer>(context);

                repo.Add(entity);

                Assert.True(repo.Exists(x => x.Name.Equals(name)));

                var entityInDb = repo.Find(x => x.Name.Equals(name));

                repo.Delete(entityInDb);

                Assert.False(repo.Exists(x => x.Name.Equals(name)));
            }
        }

        [Fact]
        public void Delete_Range()
        {
            using (var context = TestDbContextFactory.Create())
            {
                const string name = "Random Name";

                var entities = new List<Customer>
                {
                    new Customer { Name = name },
                    new Customer { Name = name }
                };

                var repo = new EfRepository<Customer>(context);

                repo.Add(entities);

                Assert.Equal(2, repo.Count());

                var entitiesInDb = repo.FindAll(x => x.Name.Equals(name));

                repo.Delete(entitiesInDb);

                Assert.Equal(0, repo.Count());
            }
        }

        [Fact]
        public void Update()
        {
            using (var context = TestDbContextFactory.Create())
            {
                const string expectedName = "New Random Name";
                const string name = "Random Name";

                var entity = new Customer { Name = name };
                var repo = new EfRepository<Customer>(context);

                repo.Add(entity);

                var entityInDb = repo.Find(x => x.Name.Equals(name));

                entityInDb.Name = expectedName;

                repo.Update(entityInDb);

                Assert.True(repo.Exists(x => x.Name.Equals(expectedName)));
            }
        }

        [Fact]
        public void Update_Range()
        {
            using (var context = TestDbContextFactory.Create())
            {
                const string expectedName = "New Random Name";
                const string name = "Random Name";

                var entities = new List<Customer>
                {
                    new Customer { Name = name },
                    new Customer { Name = name }
                };

                var repo = new EfRepository<Customer>(context);

                repo.Add(entities);

                var entitiesInDb = repo.FindAll(x => x.Name.Equals(name));

                foreach (var entityInDb in entitiesInDb)
                {
                    entityInDb.Name = expectedName;
                }

                repo.Update(entitiesInDb);

                Assert.Equal(2, repo.Count(x => x.Name.Equals(expectedName)));
            }
        }

        [Fact]
        public void Find()
        {
            using (var context = TestDbContextFactory.Create())
            {
                const string name = "Random Name";

                var spec = new Specification<Customer>(x => x.Name.Equals(name));
                var entity = new Customer { Name = name };
                var repo = new EfRepository<Customer>(context);

                Assert.Null(repo.Find(x => x.Name.Equals(name)));
                Assert.Null(repo.Find(spec));
                Assert.Null(repo.Find<string>(x => x.Name.Equals(name), x => x.Name));
                Assert.Null(repo.Find<string>(spec, x => x.Name));

                repo.Add(entity);

                Assert.NotNull(repo.Find(x => x.Name.Equals(name)));
                Assert.NotNull(repo.Find(spec));
                Assert.Equal(name, repo.Find<string>(x => x.Name.Equals(name), x => x.Name));
                Assert.Equal(name, repo.Find<string>(spec, x => x.Name));
            }
        }

        [Fact]
        public void FindAll()
        {
            using (var context = TestDbContextFactory.Create())
            {
                const string name = "Random Name";

                var spec = new Specification<Customer>(x => x.Name.Equals(name));
                var entity = new Customer { Name = name };
                var repo = new EfRepository<Customer>(context);

                Assert.Empty(repo.FindAll(x => x.Name.Equals(name)));
                Assert.Empty(repo.FindAll(spec));
                Assert.Empty(repo.FindAll<string>(x => x.Name.Equals(name), x => x.Name));
                Assert.Empty(repo.FindAll<string>(spec, x => x.Name));

                repo.Add(entity);

                Assert.Single(repo.FindAll(x => x.Name.Equals(name)));
                Assert.Single(repo.FindAll(spec));
                Assert.Single(repo.FindAll<string>(x => x.Name.Equals(name), x => x.Name));
                Assert.Single(repo.FindAll<string>(spec, x => x.Name));
            }
        }

        [Fact]
        public void Get()
        {
            using (var context = TestDbContextFactory.Create())
            {
                int key = 1;
                const string name = "Random Name";

                var entity = new Customer { Id = key, Name = name };
                var repo = new EfRepository<Customer>(context);

                Assert.Null(repo.Get(key));

                repo.Add(entity);

                Assert.NotNull(repo.Get(key));
            }
        }

        [Fact]
        public void Count()
        {
            using (var context = TestDbContextFactory.Create())
            {
                const string name = "Random Name";

                var spec = new Specification<Customer>(x => x.Name.Equals(name));
                var entity = new Customer { Name = name };
                var repo = new EfRepository<Customer>(context);

                Assert.Equal(0, repo.Count());
                Assert.Equal(0, repo.Count(x => x.Name.Equals(name)));
                Assert.Equal(0, repo.Count(spec));

                repo.Add(entity);

                Assert.Equal(1, repo.Count());
                Assert.Equal(1, repo.Count(x => x.Name.Equals(name)));
                Assert.Equal(1, repo.Count(spec));
            }
        }
    }
}
