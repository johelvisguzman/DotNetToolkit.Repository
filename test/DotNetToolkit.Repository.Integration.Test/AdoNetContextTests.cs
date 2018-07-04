namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class AdoNetContextTests : TestBase
    {
        [Fact]
        public void ExecuteNonQuery()
        {
            var context = TestAdoNetContextFactory.Create();

            var sql = "INSERT INTO Customers(Name) VALUES('Random Name')";

            Assert.Equal(1, context.ExecuteNonQuery(sql));
            Assert.Equal(1, context.ExecuteNonQuery(sql, CommandType.Text));

            var sqlWithParameter = "INSERT INTO Customers(Name) VALUES(@name)";
            var parameters = new Dictionary<string, object> { { "name", "Random Name" } };

            Assert.Equal(1, context.ExecuteNonQuery(sqlWithParameter, parameters));
            Assert.Equal(1, context.ExecuteNonQuery(sqlWithParameter, CommandType.Text, parameters));
        }

        [Fact]
        public void ExecuteReader()
        {
            var context = TestAdoNetContextFactory.Create();

            var sql = "INSERT INTO Customers(Name) VALUES('Random Name')";

            Assert.Equal(1, context.ExecuteReader(sql).RecordsAffected);
            Assert.Equal(1, context.ExecuteReader(sql, CommandType.Text).RecordsAffected);

            var sqlWithParameter = "INSERT INTO Customers(Name) VALUES(@name)";
            var parameters = new Dictionary<string, object> { { "name", "Random Name" } };

            Assert.Equal(1, context.ExecuteReader(sqlWithParameter, parameters).RecordsAffected);
            Assert.Equal(1, context.ExecuteReader(sqlWithParameter, CommandType.Text, parameters).RecordsAffected);
        }

        [Fact]
        public void ExecuteScalar()
        {
            var context = TestAdoNetContextFactory.Create();

            context.ExecuteNonQuery("INSERT INTO Customers(Name) VALUES('Random Name')");

            var sql = "SELECT Name FROM Customers WHERE Name = 'Random Name'";

            Assert.Equal("Random Name", context.ExecuteScalar<string>(sql));
            Assert.Equal("Random Name", context.ExecuteScalar<string>(sql, CommandType.Text));

            var sqlWithParameter = "SELECT Name FROM Customers WHERE Name = @name";
            var parameters = new Dictionary<string, object> { { "name", "Random Name" } };

            Assert.Equal("Random Name", context.ExecuteScalar<string>(sqlWithParameter, parameters));
            Assert.Equal("Random Name", context.ExecuteScalar<string>(sqlWithParameter, CommandType.Text, parameters));
        }

        [Fact]
        public void ExecuteObject()
        {
            var expectedEntity = new Customer { Id = 1, Name = "Random Name" };

            var context = TestAdoNetContextFactory.Create();

            context.ExecuteNonQuery("INSERT INTO Customers(Name) VALUES('Random Name')");

            var sql = "SELECT * FROM Customers WHERE Name = 'Random Name'";

            TestExecuteObject(expectedEntity, context.ExecuteObject<Customer>(sql, reader => new Customer
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            }));

            TestExecuteObject(expectedEntity, context.ExecuteObject<Customer>(sql, reader => new Customer
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            }));

            TestExecuteObject(expectedEntity, context.ExecuteObject<Customer>(sql));
            TestExecuteObject(expectedEntity, context.ExecuteObject<Customer>(sql, CommandType.Text));

            var sqlWithParameter = "SELECT * FROM Customers WHERE Name = @name";
            var parameters = new Dictionary<string, object> { { "name", "Random Name" } };

            TestExecuteObject(expectedEntity, context.ExecuteObject<Customer>(sqlWithParameter, parameters, reader => new Customer
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            }));

            TestExecuteObject(expectedEntity, context.ExecuteObject<Customer>(sqlWithParameter, CommandType.Text, parameters, reader => new Customer
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            }));

            TestExecuteObject(expectedEntity, context.ExecuteObject<Customer>(sqlWithParameter, parameters));
            TestExecuteObject(expectedEntity, context.ExecuteObject<Customer>(sqlWithParameter, CommandType.Text, parameters));
        }

        [Fact]
        public void ExecuteList()
        {
            var expectedEntities = new List<Customer>
            {
                new Customer { Id = 1, Name = "Random Name 1"},
                new Customer { Id = 2, Name = "Random Name 2"}
            };

            var context = TestAdoNetContextFactory.Create();

            context.ExecuteNonQuery("INSERT INTO Customers(Name) VALUES('Random Name 1')");
            context.ExecuteNonQuery("INSERT INTO Customers(Name) VALUES('Random Name 2')");

            var sql = "SELECT * FROM Customers WHERE Id > 0";

            TestExecuteList(expectedEntities, context.ExecuteList<Customer>(sql, reader => new Customer
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            }));

            TestExecuteList(expectedEntities, context.ExecuteList<Customer>(sql, reader => new Customer
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            }));

            TestExecuteList(expectedEntities, context.ExecuteList<Customer>(sql));
            TestExecuteList(expectedEntities, context.ExecuteList<Customer>(sql, CommandType.Text));

            var sqlWithParameter = "SELECT * FROM Customers WHERE Id > @minumId";
            var parameters = new Dictionary<string, object> { { "minumId", 0 } };

            TestExecuteList(expectedEntities, context.ExecuteList<Customer>(sqlWithParameter, parameters, reader => new Customer
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            }));

            TestExecuteList(expectedEntities, context.ExecuteList<Customer>(sqlWithParameter, CommandType.Text, parameters, reader => new Customer
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            }));

            TestExecuteList(expectedEntities, context.ExecuteList<Customer>(sqlWithParameter, parameters));
            TestExecuteList(expectedEntities, context.ExecuteList<Customer>(sqlWithParameter, CommandType.Text, parameters));
        }

        [Fact]
        public void ExecuteDataTable()
        {
            var context = TestAdoNetContextFactory.Create();

            context.ExecuteNonQuery("INSERT INTO Customers(Name) VALUES('Random Name')");

            var sql = "SELECT * FROM Customers WHERE Name = 'Random Name'";

            TestExecuteDataTable(context.ExecuteDataTable(sql));
            TestExecuteDataTable(context.ExecuteDataTable(sql, CommandType.Text));

            var sqlWithParameter = "SELECT * FROM Customers WHERE Name = @name";
            var parameters = new Dictionary<string, object> { { "name", "Random Name" } };

            TestExecuteDataTable(context.ExecuteDataTable(sqlWithParameter, parameters));
            TestExecuteDataTable(context.ExecuteDataTable(sqlWithParameter, CommandType.Text, parameters));
        }

        [Fact]
        public void ExecuteDictionary()
        {
            var entity = new Customer { Id = 1, Name = "Random Name" };
            var expectedDictionary = new Dictionary<int, string>
            {
                { entity.Id, entity.Name }
            };

            var context = TestAdoNetContextFactory.Create();

            context.ExecuteNonQuery("INSERT INTO Customers(Name) VALUES('Random Name')");

            var sql = "SELECT Id, Name FROM Customers WHERE Name = 'Random Name'";

            TestExecuteDictionary(expectedDictionary, context.ExecuteDictionary<int, string>(sql, o => (int)o, o => (string)o));
            TestExecuteDictionary(expectedDictionary, context.ExecuteDictionary<int, string>(sql, CommandType.Text, o => (int)o, o => (string)o));
            TestExecuteDictionary(expectedDictionary, context.ExecuteDictionary<int, string>(sql));
            TestExecuteDictionary(expectedDictionary, context.ExecuteDictionary<int, string>(sql, CommandType.Text));

            var sqlWithParameter = "SELECT Id, Name FROM Customers WHERE Name = @name";
            var parameters = new Dictionary<string, object> { { "name", "Random Name" } };

            TestExecuteDictionary(expectedDictionary, context.ExecuteDictionary<int, string>(sqlWithParameter, parameters, o => (int)o, o => (string)o));
            TestExecuteDictionary(expectedDictionary, context.ExecuteDictionary<int, string>(sqlWithParameter, CommandType.Text, parameters, o => (int)o, o => (string)o));
            TestExecuteDictionary(expectedDictionary, context.ExecuteDictionary<int, string>(sqlWithParameter, parameters));
            TestExecuteDictionary(expectedDictionary, context.ExecuteDictionary<int, string>(sqlWithParameter, CommandType.Text, parameters));
        }

        [Fact]
        public async Task ExecuteNonQueryAsync()
        {
            var context = TestAdoNetContextFactory.Create();

            var sql = "INSERT INTO Customers(Name) VALUES('Random Name')";

            Assert.Equal(1, await context.ExecuteNonQueryAsync(sql));
            Assert.Equal(1, await context.ExecuteNonQueryAsync(sql, CommandType.Text));

            var sqlWithParameter = "INSERT INTO Customers(Name) VALUES(@name)";
            var parameters = new Dictionary<string, object> { { "name", "Random Name" } };

            Assert.Equal(1, await context.ExecuteNonQueryAsync(sqlWithParameter, parameters));
            Assert.Equal(1, await context.ExecuteNonQueryAsync(sqlWithParameter, CommandType.Text, parameters));
        }

        [Fact]
        public async Task ExecuteReaderAsync()
        {
            var context = TestAdoNetContextFactory.Create();

            var sql = "INSERT INTO Customers(Name) VALUES('Random Name')";

            Assert.Equal(1, (await context.ExecuteReaderAsync(sql)).RecordsAffected);
            Assert.Equal(1, (await context.ExecuteReaderAsync(sql, CommandType.Text)).RecordsAffected);

            var sqlWithParameter = "INSERT INTO Customers(Name) VALUES(@name)";
            var parameters = new Dictionary<string, object> { { "name", "Random Name" } };

            Assert.Equal(1, (await context.ExecuteReaderAsync(sqlWithParameter, parameters)).RecordsAffected);
            Assert.Equal(1, (await context.ExecuteReaderAsync(sqlWithParameter, CommandType.Text, parameters)).RecordsAffected);
        }

        [Fact]
        public async Task ExecuteScalarAsync()
        {
            var context = TestAdoNetContextFactory.Create();

            await context.ExecuteNonQueryAsync("INSERT INTO Customers(Name) VALUES('Random Name')");

            var sql = "SELECT Name FROM Customers WHERE Name = 'Random Name'";

            Assert.Equal("Random Name", await context.ExecuteScalarAsync<string>(sql));
            Assert.Equal("Random Name", await context.ExecuteScalarAsync<string>(sql, CommandType.Text));

            var sqlWithParameter = "SELECT Name FROM Customers WHERE Name = @name";
            var parameters = new Dictionary<string, object> { { "name", "Random Name" } };

            Assert.Equal("Random Name", await context.ExecuteScalarAsync<string>(sqlWithParameter, parameters));
            Assert.Equal("Random Name", await context.ExecuteScalarAsync<string>(sqlWithParameter, CommandType.Text, parameters));
        }

        [Fact]
        public async Task ExecuteObjectAsync()
        {
            var expectedEntity = new Customer { Id = 1, Name = "Random Name" };

            var context = TestAdoNetContextFactory.Create();

            await context.ExecuteNonQueryAsync("INSERT INTO Customers(Name) VALUES('Random Name')");

            var sql = "SELECT * FROM Customers WHERE Name = 'Random Name'";

            TestExecuteObject(expectedEntity, await context.ExecuteObjectAsync<Customer>(sql, reader => new Customer
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            }));

            TestExecuteObject(expectedEntity, await context.ExecuteObjectAsync<Customer>(sql, reader => new Customer
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            }));

            TestExecuteObject(expectedEntity, await context.ExecuteObjectAsync<Customer>(sql));
            TestExecuteObject(expectedEntity, await context.ExecuteObjectAsync<Customer>(sql, CommandType.Text));

            var sqlWithParameter = "SELECT * FROM Customers WHERE Name = @name";
            var parameters = new Dictionary<string, object> { { "name", "Random Name" } };

            TestExecuteObject(expectedEntity, await context.ExecuteObjectAsync<Customer>(sqlWithParameter, parameters, reader => new Customer
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            }));

            TestExecuteObject(expectedEntity, await context.ExecuteObjectAsync<Customer>(sqlWithParameter, CommandType.Text, parameters, reader => new Customer
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            }));

            TestExecuteObject(expectedEntity, await context.ExecuteObjectAsync<Customer>(sqlWithParameter, parameters));
            TestExecuteObject(expectedEntity, await context.ExecuteObjectAsync<Customer>(sqlWithParameter, CommandType.Text, parameters));
        }

        [Fact]
        public async Task ExecuteListAsync()
        {
            var expectedEntities = new List<Customer>
            {
                new Customer { Id = 1, Name = "Random Name 1"},
                new Customer { Id = 2, Name = "Random Name 2"}
            };

            var context = TestAdoNetContextFactory.Create();

            await context.ExecuteNonQueryAsync("INSERT INTO Customers(Name) VALUES('Random Name 1')");
            await context.ExecuteNonQueryAsync("INSERT INTO Customers(Name) VALUES('Random Name 2')");

            var sql = "SELECT * FROM Customers WHERE Id > 0";

            TestExecuteList(expectedEntities, await context.ExecuteListAsync<Customer>(sql, reader => new Customer
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            }));

            TestExecuteList(expectedEntities, await context.ExecuteListAsync<Customer>(sql, reader => new Customer
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            }));

            TestExecuteList(expectedEntities, await context.ExecuteListAsync<Customer>(sql));
            TestExecuteList(expectedEntities, await context.ExecuteListAsync<Customer>(sql, CommandType.Text));

            var sqlWithParameter = "SELECT * FROM Customers WHERE Id > @minumId";
            var parameters = new Dictionary<string, object> { { "minumId", 0 } };

            TestExecuteList(expectedEntities, await context.ExecuteListAsync<Customer>(sqlWithParameter, parameters, reader => new Customer
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            }));

            TestExecuteList(expectedEntities, await context.ExecuteListAsync<Customer>(sqlWithParameter, CommandType.Text, parameters, reader => new Customer
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            }));

            TestExecuteList(expectedEntities, await context.ExecuteListAsync<Customer>(sqlWithParameter, parameters));
            TestExecuteList(expectedEntities, await context.ExecuteListAsync<Customer>(sqlWithParameter, CommandType.Text, parameters));
        }

        [Fact]
        public void Add()
        {
            var context = TestAdoNetContextFactory.Create();
            var entity = new Customer { Name = "Random Name" };
            var sql = "SELECT * FROM Customers WHERE Name = 'Random Name'";

            context.Add<Customer>(entity);

            Assert.Null(context.ExecuteObject<Customer>(sql));

            Assert.Equal(1, context.SaveChanges());

            TestExecuteObject(entity, context.ExecuteObject<Customer>(sql));
        }

        [Fact]
        public async void AddAsync()
        {
            var context = TestAdoNetContextFactory.Create();
            var entity = new Customer { Name = "Random Name" };
            var sql = "SELECT * FROM Customers WHERE Name = 'Random Name'";

            context.Add<Customer>(entity);

            Assert.Null(context.ExecuteObject<Customer>(sql));

            Assert.Equal(1, await context.SaveChangesAsync());

            TestExecuteObject(entity, await context.ExecuteObjectAsync<Customer>(sql));
        }

        [Fact]
        public void Remove()
        {
            var context = TestAdoNetContextFactory.Create();
            var entity = new Customer { Name = "Random Name" };
            var sql = "SELECT * FROM Customers WHERE Name = 'Random Name'";

            context.Add<Customer>(entity);
            Assert.Equal(1, context.SaveChanges());
            Assert.NotNull(context.ExecuteObject<Customer>(sql));

            context.Remove<Customer>(entity);
            Assert.Equal(1, context.SaveChanges());
            Assert.Null(context.ExecuteObject<Customer>(sql));
        }

        [Fact]
        public async void RemoveAsync()
        {
            var context = TestAdoNetContextFactory.Create();
            var entity = new Customer { Name = "Random Name" };
            var sql = "SELECT * FROM Customers WHERE Name = 'Random Name'";

            context.Add<Customer>(entity);
            Assert.Equal(1, await context.SaveChangesAsync());
            Assert.NotNull(await context.ExecuteObjectAsync<Customer>(sql));

            context.Remove<Customer>(entity);
            Assert.Equal(1, await context.SaveChangesAsync());
            Assert.Null(await context.ExecuteObjectAsync<Customer>(sql));
        }

        [Fact]
        public void Update()
        {
            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var context = TestAdoNetContextFactory.Create();
            var entity = new Customer { Name = name };

            context.Add<Customer>(entity);
            Assert.Equal(1, context.SaveChanges());

            var entityInDb = context.ExecuteObject<Customer>($"SELECT * FROM Customers WHERE Name = '{name}'");

            entityInDb.Name = expectedName;

            context.Update<Customer>(entityInDb);
            Assert.Equal(1, context.SaveChanges());

            Assert.NotNull(context.ExecuteObject<Customer>($"SELECT * FROM Customers WHERE Name = '{expectedName}'"));
        }

        [Fact]
        public async void UpdateAsync()
        {
            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var context = TestAdoNetContextFactory.Create();
            var entity = new Customer { Name = name };

            context.Add<Customer>(entity);
            Assert.Equal(1, await context.SaveChangesAsync());

            var entityInDb = await context.ExecuteObjectAsync<Customer>($"SELECT * FROM Customers WHERE Name = '{name}'");

            entityInDb.Name = expectedName;

            context.Update<Customer>(entityInDb);
            Assert.Equal(1, await context.SaveChangesAsync());

            Assert.NotNull(await context.ExecuteObjectAsync<Customer>($"SELECT * FROM Customers WHERE Name = '{expectedName}'"));
        }

        [Fact]
        public void ThrowsIfDeleteWhenEntityNoInStore()
        {
            var context = TestAdoNetContextFactory.Create();
            var entity = new Customer { Name = "Random Name" };

            context.Remove<Customer>(entity);

            var ex = Assert.Throws<InvalidOperationException>(() => context.SaveChanges());
            Assert.Equal("Attempted to update or delete an entity that does not exist in the database.", ex.Message);
        }

        [Fact]
        public void ThrowsIfUpdateWhenEntityNoInStore()
        {
            var context = TestAdoNetContextFactory.Create();
            var entity = new Customer { Name = "Random Name" };

            context.Update<Customer>(entity);

            var ex = Assert.Throws<InvalidOperationException>(() => context.SaveChanges());
            Assert.Equal("Attempted to update or delete an entity that does not exist in the database.", ex.Message);
        }

        [Fact]
        public void ThrowsIfAddingEntityOfSameTypeWithSamePrimaryKeyValue()
        {
            var context = TestAdoNetContextFactory.Create();
            var entity = new Customer { Name = "Random Name" };

            context.Add<Customer>(entity);
            context.Add<Customer>(entity);

            var ex = Assert.Throws<InvalidOperationException>(() => context.SaveChanges());
            Assert.Equal($"The instance of entity type '{typeof(Customer)}' cannot be added to the database because another instance of this type with the same key is already being tracked.", ex.Message);
        }

        [Fact]
        public async Task ExecuteDictionaryAsync()
        {
            var entity = new Customer { Id = 1, Name = "Random Name" };
            var expectedDictionary = new Dictionary<int, string>
            {
                { entity.Id, entity.Name }
            };

            var context = TestAdoNetContextFactory.Create();

            await context.ExecuteNonQueryAsync("INSERT INTO Customers(Name) VALUES('Random Name')");

            var sql = "SELECT Id, Name FROM Customers WHERE Name = 'Random Name'";

            TestExecuteDictionary(expectedDictionary, await context.ExecuteDictionaryAsync<int, string>(sql, o => (int)o, o => (string)o));
            TestExecuteDictionary(expectedDictionary, await context.ExecuteDictionaryAsync<int, string>(sql, CommandType.Text, o => (int)o, o => (string)o));
            TestExecuteDictionary(expectedDictionary, await context.ExecuteDictionaryAsync<int, string>(sql));
            TestExecuteDictionary(expectedDictionary, await context.ExecuteDictionaryAsync<int, string>(sql, CommandType.Text));

            var sqlWithParameter = "SELECT Id, Name FROM Customers WHERE Name = @name";
            var parameters = new Dictionary<string, object> { { "name", "Random Name" } };

            TestExecuteDictionary(expectedDictionary, await context.ExecuteDictionaryAsync<int, string>(sqlWithParameter, parameters, o => (int)o, o => (string)o));
            TestExecuteDictionary(expectedDictionary, await context.ExecuteDictionaryAsync<int, string>(sqlWithParameter, CommandType.Text, parameters, o => (int)o, o => (string)o));
            TestExecuteDictionary(expectedDictionary, await context.ExecuteDictionaryAsync<int, string>(sqlWithParameter, parameters));
            TestExecuteDictionary(expectedDictionary, await context.ExecuteDictionaryAsync<int, string>(sqlWithParameter, CommandType.Text, parameters));
        }

        private static void TestExecuteList(IEnumerable<Customer> expectedList, IEnumerable<Customer> actualList)
        {
            Assert.NotNull(expectedList);
            Assert.NotNull(actualList);
            Assert.NotSame(expectedList, actualList);
            Assert.Equal(expectedList.Count(), actualList.Count());

            for (var i = 0; i < expectedList.Count(); i++)
            {
                TestExecuteObject(expectedList.ElementAt(i), actualList.ElementAt(i));
            }
        }

        private static void TestExecuteObject(Customer expected, Customer actual)
        {
            Assert.NotNull(expected);
            Assert.NotNull(actual);
            Assert.NotSame(expected, actual);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.AddressId, actual.AddressId);
            Assert.Equal(expected.Name, actual.Name);
        }

        private static void TestExecuteDataTable(DataTable actual)
        {
            Assert.NotNull(actual);
            Assert.Equal(3, actual.Columns.Count);
            Assert.Equal(1, actual.Rows.Count);
        }

        private static void TestExecuteDictionary(Dictionary<int, string> expected, Dictionary<int, string> actual)
        {
            Assert.NotNull(expected);
            Assert.NotNull(actual);
            Assert.NotSame(expected, actual);
            Assert.True(expected.All(x => actual.ContainsKey(x.Key)));
        }
    }
}
