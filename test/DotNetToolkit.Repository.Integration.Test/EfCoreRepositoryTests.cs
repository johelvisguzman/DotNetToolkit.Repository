namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using EntityFrameworkCore;
    using FetchStrategies;
    using Queries;
    using Specifications;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class EfCoreRepositoryTests
    {
        [Fact]
        public void Add()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                const string name = "Random Name";

                var entity = new Customer { Name = name };
                var repo = new EfCoreRepository<Customer>(context);

                Assert.False(repo.Exists(x => x.Name.Equals(name)));

                repo.Add(entity);

                Assert.True(repo.Exists(x => x.Name.Equals(name)));
            }
        }

        [Fact]
        public async Task Add_Async()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                const string name = "Random Name";

                var entity = new Customer { Name = name };
                var repo = new EfCoreRepository<Customer>(context);

                Assert.False(await repo.ExistsAsync(x => x.Name.Equals(name)));

                await repo.AddAsync(entity);

                Assert.True(await repo.ExistsAsync(x => x.Name.Equals(name)));
            }
        }

        [Fact]
        public void Add_Range()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                const string name = "Random Name";

                var entities = new List<Customer>
                {
                    new Customer { Name = name },
                    new Customer { Name = name }
                };

                var repo = new EfCoreRepository<Customer>(context);

                Assert.Equal(0, repo.Count());

                repo.Add(entities);

                Assert.Equal(2, repo.Count());
            }
        }

        [Fact]
        public async Task Add_Range_Async()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                const string name = "Random Name";

                var entities = new List<Customer>
                {
                    new Customer { Name = name },
                    new Customer { Name = name }
                };

                var repo = new EfCoreRepository<Customer>(context);

                Assert.Equal(0, await repo.CountAsync());

                await repo.AddAsync(entities);

                Assert.Equal(2, await repo.CountAsync());
            }
        }

        [Fact]
        public void Delete()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                const string name = "Random Name";

                var entity = new Customer { Name = name };
                var repo = new EfCoreRepository<Customer>(context);

                repo.Add(entity);

                Assert.True(repo.Exists(x => x.Name.Equals(name)));

                var entityInDb = repo.Find(x => x.Name.Equals(name));

                repo.Delete(entityInDb);

                Assert.False(repo.Exists(x => x.Name.Equals(name)));
            }
        }

        [Fact]
        public async Task Delete_Async()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                const string name = "Random Name";

                var entity = new Customer { Name = name };
                var repo = new EfCoreRepository<Customer>(context);

                await repo.AddAsync(entity);

                Assert.True(await repo.ExistsAsync(x => x.Name.Equals(name)));

                var entityInDb = await repo.FindAsync(x => x.Name.Equals(name));

                await repo.DeleteAsync(entityInDb);

                Assert.False(await repo.ExistsAsync(x => x.Name.Equals(name)));
            }
        }

        [Fact]
        public void Delete_Range()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                const string name = "Random Name";

                var entities = new List<Customer>
                {
                    new Customer { Name = name },
                    new Customer { Name = name }
                };

                var repo = new EfCoreRepository<Customer>(context);

                repo.Add(entities);

                Assert.Equal(2, repo.Count());

                var entitiesInDb = repo.FindAll(x => x.Name.Equals(name));

                repo.Delete(entitiesInDb);

                Assert.Equal(0, repo.Count());
            }
        }

        [Fact]
        public async Task Delete_Range_Async()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                const string name = "Random Name";

                var entities = new List<Customer>
                {
                    new Customer { Name = name },
                    new Customer { Name = name }
                };

                var repo = new EfCoreRepository<Customer>(context);

                await repo.AddAsync(entities);

                Assert.Equal(2, await repo.CountAsync());

                var entitiesInDb = await repo.FindAllAsync(x => x.Name.Equals(name));

                await repo.DeleteAsync(entitiesInDb);

                Assert.Equal(0, await repo.CountAsync());
            }
        }

        [Fact]
        public void Update()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                const string expectedName = "New Random Name";
                const string name = "Random Name";

                var entity = new Customer { Name = name };
                var repo = new EfCoreRepository<Customer>(context);

                repo.Add(entity);

                var entityInDb = repo.Find(x => x.Name.Equals(name));

                entityInDb.Name = expectedName;

                repo.Update(entityInDb);

                Assert.True(repo.Exists(x => x.Name.Equals(expectedName)));
            }
        }

        [Fact]
        public async Task Update_Async()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                const string expectedName = "New Random Name";
                const string name = "Random Name";

                var entity = new Customer { Name = name };
                var repo = new EfCoreRepository<Customer>(context);

                await repo.AddAsync(entity);

                var entityInDb = await repo.FindAsync(x => x.Name.Equals(name));

                entityInDb.Name = expectedName;

                await repo.UpdateAsync(entityInDb);

                Assert.True(await repo.ExistsAsync(x => x.Name.Equals(expectedName)));
            }
        }

        [Fact]
        public void Update_Range()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                const string expectedName = "New Random Name";
                const string name = "Random Name";

                var entities = new List<Customer>
                {
                    new Customer { Name = name },
                    new Customer { Name = name }
                };

                var repo = new EfCoreRepository<Customer>(context);

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
        public async Task Update_Range_Async()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                const string expectedName = "New Random Name";
                const string name = "Random Name";

                var entities = new List<Customer>
                {
                    new Customer { Name = name },
                    new Customer { Name = name }
                };

                var repo = new EfCoreRepository<Customer>(context);

                await repo.AddAsync(entities);

                var entitiesInDb = await repo.FindAllAsync(x => x.Name.Equals(name));

                foreach (var entityInDb in entitiesInDb)
                {
                    entityInDb.Name = expectedName;
                }

                await repo.UpdateAsync(entitiesInDb);

                Assert.Equal(2, await repo.CountAsync(x => x.Name.Equals(expectedName)));
            }
        }

        [Fact]
        public void Find()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                const string name = "Random Name";

                var spec = new Specification<Customer>(x => x.Name.Equals(name));
                var entity = new Customer { Name = name };
                var repo = new EfCoreRepository<Customer>(context);

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
        public async Task Find_Async()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                const string name = "Random Name";

                var spec = new Specification<Customer>(x => x.Name.Equals(name));
                var entity = new Customer { Name = name };
                var repo = new EfCoreRepository<Customer>(context);

                Assert.Null(await repo.FindAsync(x => x.Name.Equals(name)));
                Assert.Null(await repo.FindAsync(spec));
                Assert.Null(await repo.FindAsync<string>(x => x.Name.Equals(name), x => x.Name));
                Assert.Null(await repo.FindAsync<string>(spec, x => x.Name));

                await repo.AddAsync(entity);

                Assert.NotNull(await repo.FindAsync(x => x.Name.Equals(name)));
                Assert.NotNull(await repo.FindAsync(spec));
                Assert.Equal(name, await repo.FindAsync<string>(x => x.Name.Equals(name), x => x.Name));
                Assert.Equal(name, await repo.FindAsync<string>(spec, x => x.Name));
            }
        }

        [Fact]
        public void Find_With_Sorting_Options_Ascending()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                var entities = new List<Customer>
                {
                    new Customer { Name = "Random Name 2" },
                    new Customer { Name = "Random Name 1" }
                };

                var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
                var queryOptions = new SortingOptions<Customer, string>(x => x.Name, true);
                var repo = new EfCoreRepository<Customer>(context);

                Assert.Null(repo.Find(x => x.Name.Contains("Random Name"), queryOptions)?.Name);
                Assert.Null(repo.Find(spec, queryOptions)?.Name);
                Assert.Null(repo.Find<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions));
                Assert.Null(repo.Find<string>(spec, x => x.Name, queryOptions));

                repo.Add(entities);

                Assert.Equal("Random Name 2", repo.Find(x => x.Name.Contains("Random Name"), queryOptions).Name);
                Assert.Equal("Random Name 2", repo.Find(spec, queryOptions).Name);
                Assert.Equal("Random Name 2", repo.Find<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions));
                Assert.Equal("Random Name 2", repo.Find<string>(spec, x => x.Name, queryOptions));
            }
        }

        [Fact]
        public async Task Find_With_Sorting_Options_Ascending_Async()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                var entities = new List<Customer>
                {
                    new Customer { Name = "Random Name 2" },
                    new Customer { Name = "Random Name 1" }
                };

                var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
                var queryOptions = new SortingOptions<Customer, string>(x => x.Name, true);
                var repo = new EfCoreRepository<Customer>(context);

                Assert.Null((await repo.FindAsync(x => x.Name.Contains("Random Name"), queryOptions))?.Name);
                Assert.Null((await repo.FindAsync(spec, queryOptions))?.Name);
                Assert.Null(await repo.FindAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions));
                Assert.Null(await repo.FindAsync<string>(spec, x => x.Name, queryOptions));

                await repo.AddAsync(entities);

                Assert.Equal("Random Name 2", (await repo.FindAsync(x => x.Name.Contains("Random Name"), queryOptions)).Name);
                Assert.Equal("Random Name 2", (await repo.FindAsync(spec, queryOptions)).Name);
                Assert.Equal("Random Name 2", await repo.FindAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions));
                Assert.Equal("Random Name 2", await repo.FindAsync<string>(spec, x => x.Name, queryOptions));
            }
        }

        [Fact]
        public void Find_With_Sorting_Options_Descending()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                var entities = new List<Customer>
                {
                    new Customer { Name = "Random Name 2" },
                    new Customer { Name = "Random Name 1" }
                };

                var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
                var queryOptions = new SortingOptions<Customer, string>(x => x.Name);
                var repo = new EfCoreRepository<Customer>(context);

                Assert.Null(repo.Find(x => x.Name.Contains("Random Name"), queryOptions)?.Name);
                Assert.Null(repo.Find(spec, queryOptions)?.Name);
                Assert.Null(repo.Find<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions));
                Assert.Null(repo.Find<string>(spec, x => x.Name, queryOptions));

                repo.Add(entities);

                Assert.Equal("Random Name 1", repo.Find(x => x.Name.Contains("Random Name"), queryOptions).Name);
                Assert.Equal("Random Name 1", repo.Find(spec, queryOptions).Name);
                Assert.Equal("Random Name 1", repo.Find<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions));
                Assert.Equal("Random Name 1", repo.Find<string>(spec, x => x.Name, queryOptions));
            }
        }

        [Fact]
        public async Task Find_With_Sorting_Options_Descending_Async()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                var entities = new List<Customer>
                {
                    new Customer { Name = "Random Name 2" },
                    new Customer { Name = "Random Name 1" }
                };

                var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
                var queryOptions = new SortingOptions<Customer, string>(x => x.Name);
                var repo = new EfCoreRepository<Customer>(context);

                Assert.Null((await repo.FindAsync(x => x.Name.Contains("Random Name"), queryOptions))?.Name);
                Assert.Null((await repo.FindAsync(spec, queryOptions))?.Name);
                Assert.Null(await repo.FindAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions));
                Assert.Null(await repo.FindAsync<string>(spec, x => x.Name, queryOptions));

                await repo.AddAsync(entities);

                Assert.Equal("Random Name 1", (await repo.FindAsync(x => x.Name.Contains("Random Name"), queryOptions)).Name);
                Assert.Equal("Random Name 1", (await repo.FindAsync(spec, queryOptions)).Name);
                Assert.Equal("Random Name 1", await repo.FindAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions));
                Assert.Equal("Random Name 1", await repo.FindAsync<string>(spec, x => x.Name, queryOptions));
            }
        }

        [Fact]
        public void FindAll()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                const string name = "Random Name";

                var spec = new Specification<Customer>(x => x.Name.Equals(name));
                var entity = new Customer { Name = name };
                var repo = new EfCoreRepository<Customer>(context);

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
        public async Task FindAll_Async()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                const string name = "Random Name";

                var spec = new Specification<Customer>(x => x.Name.Equals(name));
                var entity = new Customer { Name = name };
                var repo = new EfCoreRepository<Customer>(context);

                Assert.Empty(await repo.FindAllAsync(x => x.Name.Equals(name)));
                Assert.Empty(await repo.FindAllAsync(spec));
                Assert.Empty(await repo.FindAllAsync<string>(x => x.Name.Equals(name), x => x.Name));
                Assert.Empty(await repo.FindAllAsync<string>(spec, x => x.Name));

                await repo.AddAsync(entity);

                Assert.Single(await repo.FindAllAsync(x => x.Name.Equals(name)));
                Assert.Single(await repo.FindAllAsync(spec));
                Assert.Single(await repo.FindAllAsync<string>(x => x.Name.Equals(name), x => x.Name));
                Assert.Single(await repo.FindAllAsync<string>(spec, x => x.Name));
            }
        }

        [Fact]
        public void FindAll_With_Sorting_Options_Ascending()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                var entities = new List<Customer>
                {
                    new Customer { Name = "Random Name 2" },
                    new Customer { Name = "Random Name 1" }
                };

                var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
                var queryOptions = new SortingOptions<Customer, string>(x => x.Name, true);
                var repo = new EfCoreRepository<Customer>(context);

                Assert.Null(repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions).FirstOrDefault()?.Name);
                Assert.Null(repo.FindAll(spec, queryOptions).FirstOrDefault()?.Name);
                Assert.Null(repo.FindAll<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions).FirstOrDefault());
                Assert.Null(repo.FindAll<string>(spec, x => x.Name, queryOptions).FirstOrDefault());

                repo.Add(entities);

                Assert.Equal("Random Name 2", repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions).First().Name);
                Assert.Equal("Random Name 2", repo.FindAll(spec, queryOptions).First().Name);
                Assert.Equal("Random Name 2", repo.FindAll<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions).First());
                Assert.Equal("Random Name 2", repo.FindAll<string>(spec, x => x.Name, queryOptions).First());
            }
        }

        [Fact]
        public async Task FindAll_With_Sorting_Options_Ascending_Async()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                var entities = new List<Customer>
                {
                    new Customer { Name = "Random Name 2" },
                    new Customer { Name = "Random Name 1" }
                };

                var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
                var queryOptions = new SortingOptions<Customer, string>(x => x.Name, true);
                var repo = new EfCoreRepository<Customer>(context);

                Assert.Null((await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions)).FirstOrDefault()?.Name);
                Assert.Null((await repo.FindAllAsync(spec, queryOptions)).FirstOrDefault()?.Name);
                Assert.Null((await repo.FindAllAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions)).FirstOrDefault());
                Assert.Null((await repo.FindAllAsync<string>(spec, x => x.Name, queryOptions)).FirstOrDefault());

                await repo.AddAsync(entities);

                Assert.Equal("Random Name 2", (await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions)).First().Name);
                Assert.Equal("Random Name 2", (await repo.FindAllAsync(spec, queryOptions)).First().Name);
                Assert.Equal("Random Name 2", (await repo.FindAllAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions)).First());
                Assert.Equal("Random Name 2", (await repo.FindAllAsync<string>(spec, x => x.Name, queryOptions)).First());
            }
        }

        [Fact]
        public void FindAll_With_Sorting_Options_Descending()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                var entities = new List<Customer>
                {
                    new Customer { Name = "Random Name 2" },
                    new Customer { Name = "Random Name 1" }
                };

                var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
                var queryOptions = new SortingOptions<Customer, string>(x => x.Name);
                var repo = new EfCoreRepository<Customer>(context);

                Assert.Null(repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions).FirstOrDefault()?.Name);
                Assert.Null(repo.FindAll(spec, queryOptions).FirstOrDefault()?.Name);
                Assert.Null(repo.FindAll<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions).FirstOrDefault());
                Assert.Null(repo.FindAll<string>(spec, x => x.Name, queryOptions).FirstOrDefault());

                repo.Add(entities);

                Assert.Equal("Random Name 1", repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions).First().Name);
                Assert.Equal("Random Name 1", repo.FindAll(spec, queryOptions).First().Name);
                Assert.Equal("Random Name 1", repo.FindAll<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions).First());
                Assert.Equal("Random Name 1", repo.FindAll<string>(spec, x => x.Name, queryOptions).First());
            }
        }

        [Fact]
        public async Task FindAll_With_Sorting_Options_Descending_Async()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                var entities = new List<Customer>
                {
                    new Customer { Name = "Random Name 2" },
                    new Customer { Name = "Random Name 1" }
                };

                var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
                var queryOptions = new SortingOptions<Customer, string>(x => x.Name);
                var repo = new EfCoreRepository<Customer>(context);

                Assert.Null((await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions)).FirstOrDefault()?.Name);
                Assert.Null((await repo.FindAllAsync(spec, queryOptions)).FirstOrDefault()?.Name);
                Assert.Null((await repo.FindAllAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions)).FirstOrDefault());
                Assert.Null((await repo.FindAllAsync<string>(spec, x => x.Name, queryOptions)).FirstOrDefault());

                await repo.AddAsync(entities);

                Assert.Equal("Random Name 1", (await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions)).First().Name);
                Assert.Equal("Random Name 1", (await repo.FindAllAsync(spec, queryOptions)).First().Name);
                Assert.Equal("Random Name 1", (await repo.FindAllAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions)).First());
                Assert.Equal("Random Name 1", (await repo.FindAllAsync<string>(spec, x => x.Name, queryOptions)).First());
            }
        }

        [Fact]
        public void FindAll_With_Paging_Options_Sort_Ascending()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                var entities = new List<Customer>();

                for (var i = 0; i < 21; i++)
                {
                    entities.Add(new Customer { Name = "Random Name " + i });
                }

                var repo = new EfCoreRepository<Customer>(context);

                repo.Add(entities);

                var queryOptions = new PagingOptions<Customer, int>(1, 5, x => x.Id);
                var entitiesInDb = repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions);

                Assert.Equal(5, entitiesInDb.Count());
                Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
                Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1).Name);
                Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2).Name);
                Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3).Name);
                Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4).Name);

                queryOptions.PageIndex = 2;

                entitiesInDb = repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions);

                Assert.Equal(5, entitiesInDb.Count());
                Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
                Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1).Name);
                Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2).Name);
                Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3).Name);
                Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4).Name);

                queryOptions.PageIndex = 3;

                entitiesInDb = repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions);

                Assert.Equal(5, entitiesInDb.Count());
                Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
                Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1).Name);
                Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2).Name);
                Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3).Name);
                Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4).Name);

                queryOptions.PageIndex = 4;

                entitiesInDb = repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions);

                Assert.Equal(5, entitiesInDb.Count());
                Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
                Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1).Name);
                Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2).Name);
                Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3).Name);
                Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4).Name);

                queryOptions.PageIndex = 5;

                entitiesInDb = repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions);

                Assert.Single(entitiesInDb);
                Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
            }
        }

        [Fact]
        public async Task FindAll_With_Paging_Options_Sort_Ascending_Async()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                var entities = new List<Customer>();

                for (var i = 0; i < 21; i++)
                {
                    entities.Add(new Customer { Name = "Random Name " + i });
                }

                var repo = new EfCoreRepository<Customer>(context);

                await repo.AddAsync(entities);

                var queryOptions = new PagingOptions<Customer, int>(1, 5, x => x.Id);
                var entitiesInDb = await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions);

                Assert.Equal(5, entitiesInDb.Count());
                Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
                Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1).Name);
                Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2).Name);
                Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3).Name);
                Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4).Name);

                queryOptions.PageIndex = 2;

                entitiesInDb = await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions);

                Assert.Equal(5, entitiesInDb.Count());
                Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
                Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1).Name);
                Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2).Name);
                Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3).Name);
                Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4).Name);

                queryOptions.PageIndex = 3;

                entitiesInDb = await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions);

                Assert.Equal(5, entitiesInDb.Count());
                Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
                Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1).Name);
                Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2).Name);
                Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3).Name);
                Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4).Name);

                queryOptions.PageIndex = 4;

                entitiesInDb = await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions);

                Assert.Equal(5, entitiesInDb.Count());
                Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
                Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1).Name);
                Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2).Name);
                Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3).Name);
                Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4).Name);

                queryOptions.PageIndex = 5;

                entitiesInDb = await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions);

                Assert.Single(entitiesInDb);
                Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
            }
        }

        [Fact]
        public void FindAll_With_Paging_Options_Sort_Descending()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                var entities = new List<Customer>();

                for (var i = 0; i < 21; i++)
                {
                    entities.Add(new Customer { Name = "Random Name " + i });
                }

                var repo = new EfCoreRepository<Customer>(context);

                repo.Add(entities);

                var queryOptions = new PagingOptions<Customer, int>(1, 5, x => x.Id, true);
                var entitiesInDb = repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions);

                Assert.Equal(5, entitiesInDb.Count());
                Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
                Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1).Name);
                Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2).Name);
                Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3).Name);
                Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4).Name);

                queryOptions.PageIndex = 2;

                entitiesInDb = repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions);

                Assert.Equal(5, entitiesInDb.Count());
                Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
                Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1).Name);
                Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2).Name);
                Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3).Name);
                Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4).Name);

                queryOptions.PageIndex = 3;

                entitiesInDb = repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions);

                Assert.Equal(5, entitiesInDb.Count());
                Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
                Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1).Name);
                Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2).Name);
                Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3).Name);
                Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4).Name);

                queryOptions.PageIndex = 4;

                entitiesInDb = repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions);

                Assert.Equal(5, entitiesInDb.Count());
                Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
                Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1).Name);
                Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2).Name);
                Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3).Name);
                Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4).Name);

                queryOptions.PageIndex = 5;

                entitiesInDb = repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions);

                Assert.Single(entitiesInDb);
                Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
            }
        }

        [Fact]
        public async Task FindAll_With_Paging_Options_Sort_Descending_Async()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                var entities = new List<Customer>();

                for (var i = 0; i < 21; i++)
                {
                    entities.Add(new Customer { Name = "Random Name " + i });
                }

                var repo = new EfCoreRepository<Customer>(context);

                await repo.AddAsync(entities);

                var queryOptions = new PagingOptions<Customer, int>(1, 5, x => x.Id, true);
                var entitiesInDb = await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions);

                Assert.Equal(5, entitiesInDb.Count());
                Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
                Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1).Name);
                Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2).Name);
                Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3).Name);
                Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4).Name);

                queryOptions.PageIndex = 2;

                entitiesInDb = await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions);

                Assert.Equal(5, entitiesInDb.Count());
                Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
                Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1).Name);
                Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2).Name);
                Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3).Name);
                Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4).Name);

                queryOptions.PageIndex = 3;

                entitiesInDb = await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions);

                Assert.Equal(5, entitiesInDb.Count());
                Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
                Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1).Name);
                Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2).Name);
                Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3).Name);
                Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4).Name);

                queryOptions.PageIndex = 4;

                entitiesInDb = await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions);

                Assert.Equal(5, entitiesInDb.Count());
                Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
                Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1).Name);
                Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2).Name);
                Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3).Name);
                Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4).Name);

                queryOptions.PageIndex = 5;

                entitiesInDb = await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions);

                Assert.Single(entitiesInDb);
                Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
            }
        }

        [Fact]
        public void Get()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                int key = 1;
                const string name = "Random Name";

                var fetchStrategy = new FetchStrategy<Customer>();
                fetchStrategy.Include(x => x.Address);

                var entity = new Customer { Id = key, Name = name };
                var repo = new EfCoreRepository<Customer>(context);

                Assert.Null(repo.Get(key));
                Assert.Null(repo.Get(key, fetchStrategy));

                repo.Add(entity);

                Assert.NotNull(repo.Get(key));
                Assert.NotNull(repo.Get(key, fetchStrategy));
            }
        }

        [Fact]
        public async Task Get_Async()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                int key = 1;
                const string name = "Random Name";

                var fetchStrategy = new FetchStrategy<Customer>();
                fetchStrategy.Include(x => x.Address);

                var entity = new Customer { Id = key, Name = name };
                var repo = new EfCoreRepository<Customer>(context);

                Assert.Null(await repo.GetAsync(key));
                Assert.Null(await repo.GetAsync(key, fetchStrategy));

                await repo.AddAsync(entity);

                Assert.NotNull(await repo.GetAsync(key));
                Assert.NotNull(await repo.GetAsync(key, fetchStrategy));
            }
        }

        [Fact]
        public void Count()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                const string name = "Random Name";

                var spec = new Specification<Customer>(x => x.Name.Equals(name));
                var entity = new Customer { Name = name };
                var repo = new EfCoreRepository<Customer>(context);

                Assert.Equal(0, repo.Count());
                Assert.Equal(0, repo.Count(x => x.Name.Equals(name)));
                Assert.Equal(0, repo.Count(spec));

                repo.Add(entity);

                Assert.Equal(1, repo.Count());
                Assert.Equal(1, repo.Count(x => x.Name.Equals(name)));
                Assert.Equal(1, repo.Count(spec));
            }
        }

        [Fact]
        public async Task Count_Async()
        {
            using (var context = new TestEfCoreDbContext(Guid.NewGuid().ToString()))
            {
                const string name = "Random Name";

                var spec = new Specification<Customer>(x => x.Name.Equals(name));
                var entity = new Customer { Name = name };
                var repo = new EfCoreRepository<Customer>(context);

                Assert.Equal(0, await repo.CountAsync());
                Assert.Equal(0, await repo.CountAsync(x => x.Name.Equals(name)));
                Assert.Equal(0, await repo.CountAsync(spec));

                await repo.AddAsync(entity);

                Assert.Equal(1, await repo.CountAsync());
                Assert.Equal(1, await repo.CountAsync(x => x.Name.Equals(name)));
                Assert.Equal(1, await repo.CountAsync(spec));
            }
        }
    }
}
