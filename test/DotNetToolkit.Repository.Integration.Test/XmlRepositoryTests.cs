namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using FetchStrategies;
    using Queries;
    using Specifications;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Xml;
    using Xunit;

    public class XmlRepositoryTests
    {
        private static string GetTempFileName(string fileName = null)
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = "TestData-" + Guid.NewGuid().ToString("N") + ".xml";

            var path = Path.GetTempPath() + fileName;

            if (File.Exists(path))
                File.Delete(path);

            return path;
        }

        [Fact]
        public void Creates_Temp_File_On_Construction()
        {
            var path = GetTempFileName();

            if (File.Exists(path))
                File.Delete(path);

            Assert.True(!File.Exists(path));

            var repo = new XmlRepository<Customer>(path);

            Assert.True(File.Exists(path));
        }

        [Fact]
        public void Generates_Temp_File_Name_When_Only_Directory_Is_Provided()
        {
            var generatedName = $"{typeof(Customer).Name}.xml";
            var path = GetTempFileName(generatedName);

            Assert.True(!File.Exists(path));

            var repo = new XmlRepository<Customer>(path);

            Assert.True(File.Exists(path));
        }

        [Fact]
        public void Add()
        {
            const string name = "Random Name";

            var entity = new Customer { Name = name };
            var repo = new XmlRepository<Customer>(GetTempFileName());

            Assert.False(repo.Exists(x => x.Name.Equals(name)));

            repo.Add(entity);

            Assert.True(repo.Exists(x => x.Name.Equals(name)));
        }

        [Fact]
        public void Add_Range()
        {
            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Name = name },
                new Customer { Name = name }
            };

            var repo = new XmlRepository<Customer>(GetTempFileName());

            Assert.Equal(0, repo.Count());

            repo.Add(entities);

            Assert.Equal(2, repo.Count());
        }

        [Fact]
        public void Delete()
        {
            const string name = "Random Name";

            var entity = new Customer { Name = name };
            var repo = new XmlRepository<Customer>(GetTempFileName());

            repo.Add(entity);

            Assert.True(repo.Exists(x => x.Name.Equals(name)));

            var entityInDb = repo.Find(x => x.Name.Equals(name));

            repo.Delete(entityInDb);

            Assert.False(repo.Exists(x => x.Name.Equals(name)));
        }

        [Fact]
        public void Delete_Range()
        {
            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Name = name },
                new Customer { Name = name }
            };

            var repo = new XmlRepository<Customer>(GetTempFileName());

            repo.Add(entities);

            Assert.Equal(2, repo.Count());

            var entitiesInDb = repo.FindAll(x => x.Name.Equals(name));

            repo.Delete(entitiesInDb);

            Assert.Equal(0, repo.Count());
        }

        [Fact]
        public void Update()
        {
            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entity = new Customer { Name = name };
            var repo = new XmlRepository<Customer>(GetTempFileName());

            repo.Add(entity);

            var entityInDb = repo.Find(x => x.Name.Equals(name));

            entityInDb.Name = expectedName;

            repo.Update(entityInDb);

            Assert.True(repo.Exists(x => x.Name.Equals(expectedName)));
        }

        [Fact]
        public void Update_Range()
        {
            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Name = name },
                new Customer { Name = name }
            };

            var repo = new XmlRepository<Customer>(GetTempFileName());

            repo.Add(entities);

            var entitiesInDb = repo.FindAll(x => x.Name.Equals(name));

            foreach (var entityInDb in entitiesInDb)
            {
                entityInDb.Name = expectedName;
            }

            repo.Update(entitiesInDb);

            Assert.Equal(2, repo.Count(x => x.Name.Equals(expectedName)));
        }

        [Fact]
        public void Find()
        {
            const string name = "Random Name";

            var spec = new Specification<Customer>(x => x.Name.Equals(name));
            var entity = new Customer { Name = name };
            var repo = new XmlRepository<Customer>(GetTempFileName());

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

        [Fact]
        public void Find_With_Sorting_Options_Ascending()
        {
            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
            var queryOptions = new SortingOptions<Customer, string>(x => x.Name, true);
            var repo = new XmlRepository<Customer>(GetTempFileName());

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

        [Fact]
        public void Find_With_Sorting_Options_Descending()
        {
            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
            var queryOptions = new SortingOptions<Customer, string>(x => x.Name);
            var repo = new XmlRepository<Customer>(GetTempFileName());

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

        [Fact]
        public void FindAll()
        {
            const string name = "Random Name";

            var spec = new Specification<Customer>(x => x.Name.Equals(name));
            var entity = new Customer { Name = name };
            var repo = new XmlRepository<Customer>(GetTempFileName());

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

        [Fact]
        public void FindAll_With_Sorting_Options_Ascending()
        {
            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
            var queryOptions = new SortingOptions<Customer, string>(x => x.Name, true);
            var repo = new XmlRepository<Customer>(GetTempFileName());

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

        [Fact]
        public void FindAll_With_Sorting_Options_Descending()
        {
            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
            var queryOptions = new SortingOptions<Customer, string>(x => x.Name);
            var repo = new XmlRepository<Customer>(GetTempFileName());

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

        [Fact]
        public void FindAll_With_Paging_Options_Sort_Ascending()
        {
            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            var repo = new XmlRepository<Customer>(GetTempFileName());

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


        [Fact]
        public void FindAll_With_Paging_Options_Sort_Descending()
        {
            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            var repo = new XmlRepository<Customer>(GetTempFileName());

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

        [Fact]
        public void Get()
        {
            int key = 1;
            const string name = "Random Name";

            var fetchStrategy = new FetchStrategy<Customer>();
            fetchStrategy.Include(x => x.Address);

            var entity = new Customer { Id = key, Name = name };
            var repo = new XmlRepository<Customer>(GetTempFileName());

            Assert.Null(repo.Get(key));
            Assert.Null(repo.Get(key, fetchStrategy));

            repo.Add(entity);

            Assert.NotNull(repo.Get(key));
            Assert.NotNull(repo.Get(key, fetchStrategy));
        }

        [Fact]
        public void Count()
        {
            const string name = "Random Name";

            var spec = new Specification<Customer>(x => x.Name.Equals(name));
            var entity = new Customer { Name = name };
            var repo = new XmlRepository<Customer>(GetTempFileName());

            Assert.Equal(0, repo.Count());
            Assert.Equal(0, repo.Count(x => x.Name.Equals(name)));
            Assert.Equal(0, repo.Count(spec));

            repo.Add(entity);

            Assert.Equal(1, repo.Count());
            Assert.Equal(1, repo.Count(x => x.Name.Equals(name)));
            Assert.Equal(1, repo.Count(spec));
        }

        [Fact]
        public void Can_Scoped()
        {
            var path = GetTempFileName("TestData.xml");

            using (var repo = new XmlRepository<Customer>(path))
            {
                var entity = new Customer { Name = "Random Name" };

                repo.Add(entity);

                Assert.NotNull(repo.Find(x => x.Name.Equals("Random Name")));
                Assert.Equal(1, entity.Id);
            }

            using (var repo = new XmlRepository<Customer>(path))
            {
                var entity = new Customer { Name = "Random Name" };

                repo.Add(entity);

                Assert.Equal(2, entity.Id);
                Assert.Equal(2, repo.Count(x => x.Name.Equals("Random Name")));
            }

            using (var repo = new XmlRepository<Customer>(GetTempFileName()))
            {
                var entity = new Customer { Name = "Random Name" };

                repo.Add(entity);

                Assert.NotNull(repo.Find(x => x.Name.Equals("Random Name")));
                Assert.Equal(1, entity.Id);
            }
        }

        [Fact]
        public void ToDictionary()
        {
            const string name = "Random Name";

            var spec = new Specification<Customer>(x => x.Name.Equals(name));
            var entity = new Customer { Id = 1, Name = name };
            var expectedDictionary = new Dictionary<int, Customer>();
            var expectedDictionaryByElementSelector = new Dictionary<int, string>();

            expectedDictionary.Add(entity.Id, entity);
            expectedDictionaryByElementSelector.Add(entity.Id, entity.Name);

            var repo = new XmlRepository<Customer>(GetTempFileName());

            Assert.False(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));

            repo.Add(entity);

            Assert.True(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));
        }

        [Fact]
        public void ToDictionary_With_Paging_Options_Sort_Descending()
        {
            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i + 1, Name = "Random Name " + i });
            }

            var queryOptions = new PagingOptions<Customer, int>(1, 5, x => x.Id, true);
            var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
            var expectedDictionary = entities.ToDictionary(x => x.Id);
            var expectedDictionaryByElementSelector = entities.ToDictionary(x => x.Id, y => y.Name);
            var repo = new XmlRepository<Customer>(GetTempFileName());

            Assert.False(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));

            repo.Add(entities);

            var entitiesInDb = repo.ToDictionary(x => x.Id, queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Value.Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1).Value.Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2).Value.Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3).Value.Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);
            expectedDictionary.Add(entities[1].Id, entities[1]);
            expectedDictionary.Add(entities[2].Id, entities[2]);
            expectedDictionary.Add(entities[3].Id, entities[3]);
            expectedDictionary.Add(entities[4].Id, entities[4]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);
            expectedDictionaryByElementSelector.Add(entities[1].Id, entities[1].Name);
            expectedDictionaryByElementSelector.Add(entities[2].Id, entities[2].Name);
            expectedDictionaryByElementSelector.Add(entities[3].Id, entities[3].Name);
            expectedDictionaryByElementSelector.Add(entities[4].Id, entities[4].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));

            queryOptions.PageIndex = 2;

            entitiesInDb = repo.ToDictionary(x => x.Id, queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Value.Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1).Value.Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2).Value.Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3).Value.Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);
            expectedDictionary.Add(entities[1].Id, entities[1]);
            expectedDictionary.Add(entities[2].Id, entities[2]);
            expectedDictionary.Add(entities[3].Id, entities[3]);
            expectedDictionary.Add(entities[4].Id, entities[4]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);
            expectedDictionaryByElementSelector.Add(entities[1].Id, entities[1].Name);
            expectedDictionaryByElementSelector.Add(entities[2].Id, entities[2].Name);
            expectedDictionaryByElementSelector.Add(entities[3].Id, entities[3].Name);
            expectedDictionaryByElementSelector.Add(entities[4].Id, entities[4].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));

            queryOptions.PageIndex = 3;

            entitiesInDb = repo.ToDictionary(x => x.Id, queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Value.Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1).Value.Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2).Value.Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3).Value.Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);
            expectedDictionary.Add(entities[1].Id, entities[1]);
            expectedDictionary.Add(entities[2].Id, entities[2]);
            expectedDictionary.Add(entities[3].Id, entities[3]);
            expectedDictionary.Add(entities[4].Id, entities[4]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);
            expectedDictionaryByElementSelector.Add(entities[1].Id, entities[1].Name);
            expectedDictionaryByElementSelector.Add(entities[2].Id, entities[2].Name);
            expectedDictionaryByElementSelector.Add(entities[3].Id, entities[3].Name);
            expectedDictionaryByElementSelector.Add(entities[4].Id, entities[4].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));

            queryOptions.PageIndex = 4;

            entitiesInDb = repo.ToDictionary(x => x.Id, queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Value.Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1).Value.Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2).Value.Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3).Value.Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);
            expectedDictionary.Add(entities[1].Id, entities[1]);
            expectedDictionary.Add(entities[2].Id, entities[2]);
            expectedDictionary.Add(entities[3].Id, entities[3]);
            expectedDictionary.Add(entities[4].Id, entities[4]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);
            expectedDictionaryByElementSelector.Add(entities[1].Id, entities[1].Name);
            expectedDictionaryByElementSelector.Add(entities[2].Id, entities[2].Name);
            expectedDictionaryByElementSelector.Add(entities[3].Id, entities[3].Name);
            expectedDictionaryByElementSelector.Add(entities[4].Id, entities[4].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));

            queryOptions.PageIndex = 5;

            entitiesInDb = repo.ToDictionary(x => x.Id, queryOptions);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));
        }

        [Fact]
        public void Throws_If_File_Path_Is_Invalid()
        {
            var path = "TestData";
            var ex = Assert.Throws<InvalidOperationException>(() => new XmlRepository<Customer>(path));

            Assert.Equal($"The specified '{path}.xml' file is not a valid path.", ex.Message);
        }

        [Fact]
        public void Throws_If_File_Extension_Is_Not_Xml()
        {
            var path = GetTempFileName("TestData.txt");
            var ex = Assert.Throws<InvalidOperationException>(() => new XmlRepository<Customer>(path));

            Assert.Equal($"The specified '{path}' file has an invalid extension. Please consider using '.xml'.", ex.Message);
        }

        [Fact]
        public void Throws_If_Add_With_No_Id_Defined_Entity()
        {
            var repo = new XmlRepository<CustomerWithNoId>(GetTempFileName());
            var entity = new CustomerWithNoId { Name = "Random Name" };

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Add(entity));
            Assert.Equal($"The instance of entity type '{entity.GetType()}' requires a primary key to be defined.", ex.Message);
        }

        [Fact]
        public void Throws_If_Delete_With_No_Id_Defined_Entity()
        {
            var repo = new XmlRepository<CustomerWithNoId>(GetTempFileName());
            var entity = new CustomerWithNoId { Name = "Random Name" };

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Delete(entity));
            Assert.Equal($"The instance of entity type '{entity.GetType()}' requires a primary key to be defined.", ex.Message);
        }

        [Fact]
        public void Throws_If_Update_With_No_Id_Defined_Entity()
        {
            var repo = new XmlRepository<CustomerWithNoId>(GetTempFileName());
            var entity = new CustomerWithNoId { Name = "Random Name" };

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Update(entity));
            Assert.Equal($"The instance of entity type '{entity.GetType()}' requires a primary key to be defined.", ex.Message);
        }

        [Fact]
        public void Throws_If_Delete_When_Entity_No_In_Store()
        {
            var repo = new XmlRepository<Customer>(GetTempFileName());
            var entity = new Customer { Name = "Random Name" };

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Delete(entity));
            Assert.Equal("Attempted to update or delete an entity that does not exist in the in-memory store.", ex.Message);
        }

        [Fact]
        public void Throws_If_Update_When_Entity_No_In_Store()
        {
            var repo = new XmlRepository<Customer>(GetTempFileName());
            var entity = new Customer { Name = "Random Name" };

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Update(entity));
            Assert.Equal("Attempted to update or delete an entity that does not exist in the in-memory store.", ex.Message);
        }

        [Fact]
        public void Throws_If_Adding_Entity_Of_Same_Type_With_Same_Primary_Key_Value()
        {
            var repo = new XmlRepository<Customer>(GetTempFileName());
            var entity = new Customer { Name = "Random Name" };

            repo.Add(entity);

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Add(entity));
            Assert.Equal($"The instance of entity type '{typeof(Customer)}' cannot be added to the in-memory store because another instance of this type with the same key is already being tracked.", ex.Message);
        }
    }
}
