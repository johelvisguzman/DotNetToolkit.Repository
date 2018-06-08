namespace DotNetToolkit.Repository.Integration.Test
{
    using AdoNet;
    using FetchStrategies;
    using Queries;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class AdoNetRepositoryTests
    {
        [Fact]
        public void GetWithFetchStrategyWithNavigationProperty()
        {
            Data.TestAdoNetConnectionStringFactory.Create(out string providerName, out string connectionString);

            var customerKey = 1;
            var addressKey = 1;

            var addressRepo = new AdoNetRepository<CustomerAddress>(providerName, connectionString);
            var customerRepo = new AdoNetRepository<Customer>(providerName, connectionString);
            var customerFetchStrategy = new FetchStrategy<Customer>();

            var entity = new Customer
            {
                Id = customerKey,
                Name = "Random Name",
                AddressId = addressKey
            };

            customerRepo.Add(entity);

            var address = new CustomerAddress
            {
                Id = addressKey,
                Street = "Street",
                City = "New City",
                State = "ST",
                CustomerId = customerKey,
                Customer = entity
            };

            addressRepo.Add(address);

            TestCustomerAddress(address, customerRepo.Get(customerKey, customerFetchStrategy).Address);

            // for one to one, the navigation properties will be included automatically (no need to fetch)
            TestCustomerAddress(address, customerRepo.Get(customerKey).Address);
        }

        [Fact]
        public void GetWithFetchStrategyAndForeignKeyAnnotationChanged()
        {
            Data.TestAdoNetConnectionStringFactory.Create(out string providerName, out string connectionString);

            var customerKey = 1;
            var addressKey = 1;

            var addressWithForeignKeyAnnotationOnForeignKeyRepo = new AdoNetRepository<CustomerAddressWithForeignKeyAnnotationOnForeignKey>(providerName, connectionString);
            var customerWithForeignKeyAnnotationOnForeignKeyRepo = new AdoNetRepository<CustomerWithForeignKeyAnnotationOnForeignKey>(providerName, connectionString);
            var customerWithForeignKeyAnnotationOnForeignKeyFetchStrategy = new FetchStrategy<CustomerWithForeignKeyAnnotationOnForeignKey>();

            var customerWithForeignKeyAnnotationOnForeignKey = new CustomerWithForeignKeyAnnotationOnForeignKey
            {
                Id = customerKey,
                Name = "Random Name",
                AddressKey = addressKey
            };

            customerWithForeignKeyAnnotationOnForeignKeyRepo.Add(customerWithForeignKeyAnnotationOnForeignKey);

            var addressWithForeignKeyAnnotationOnForeignKey = new CustomerAddressWithForeignKeyAnnotationOnForeignKey
            {
                Id = addressKey,
                Street = "Street",
                City = "New City",
                State = "ST",
                CustomerKey = customerKey,
                Customer = customerWithForeignKeyAnnotationOnForeignKey
            };

            addressWithForeignKeyAnnotationOnForeignKeyRepo.Add(addressWithForeignKeyAnnotationOnForeignKey);

            TestCustomerAddress(addressWithForeignKeyAnnotationOnForeignKey, customerWithForeignKeyAnnotationOnForeignKeyRepo.Get(customerKey, customerWithForeignKeyAnnotationOnForeignKeyFetchStrategy).Address);

            // for one to one, the navigation properties will be included automatically (no need to fetch)
            TestCustomerAddress(addressWithForeignKeyAnnotationOnForeignKey, customerWithForeignKeyAnnotationOnForeignKeyRepo.Get(customerKey).Address);

            var addressWithForeignKeyAnnotationOnNavigationPropertyRepo = new AdoNetRepository<CustomerAddressWithForeignKeyAnnotationOnNavigationProperty>(providerName, connectionString);
            var customerWithForeignKeyAnnotationOnNavigationPropertyRepo = new AdoNetRepository<CustomerWithForeignKeyAnnotationOnNavigationProperty>(providerName, connectionString);
            var customerWithForeignKeyAnnotationOnNavigationPropertyFetchStrategy = new FetchStrategy<CustomerWithForeignKeyAnnotationOnNavigationProperty>();

            var customerWithForeignKeyAnnotationOnNavigationProperty = new CustomerWithForeignKeyAnnotationOnNavigationProperty
            {
                Id = customerKey,
                Name = "Random Name",
                AddressKey = addressKey
            };

            customerWithForeignKeyAnnotationOnNavigationPropertyRepo.Add(customerWithForeignKeyAnnotationOnNavigationProperty);

            var addressWithForeignKeyAnnotationOnNavigationProperty = new CustomerAddressWithForeignKeyAnnotationOnNavigationProperty
            {
                Id = addressKey,
                Street = "Street",
                City = "New City",
                State = "ST",
                CustomerKey = customerKey,
                Customer = customerWithForeignKeyAnnotationOnNavigationProperty
            };

            addressWithForeignKeyAnnotationOnNavigationPropertyRepo.Add(addressWithForeignKeyAnnotationOnNavigationProperty);

            TestCustomerAddress(addressWithForeignKeyAnnotationOnNavigationProperty, customerWithForeignKeyAnnotationOnNavigationPropertyRepo.Get(customerKey, customerWithForeignKeyAnnotationOnNavigationPropertyFetchStrategy).Address);

            // for one to one, the navigation properties will be included automatically (no need to fetch)
            TestCustomerAddress(addressWithForeignKeyAnnotationOnNavigationProperty, customerWithForeignKeyAnnotationOnNavigationPropertyRepo.Get(customerKey).Address);
        }

        [Fact]
        public async Task GetWithFetchStrategyWithNavigationPropertyAsync()
        {
            Data.TestAdoNetConnectionStringFactory.Create(out string providerName, out string connectionString);

            var addressRepo = new AdoNetRepository<CustomerAddress>(providerName, connectionString);
            var customerRepo = new AdoNetRepository<Customer>(providerName, connectionString);
            var customerKey = 1;
            var addressKey = 1;
            var fetchStrategy = new FetchStrategy<Customer>();

            var entity = new Customer
            {
                Id = customerKey,
                Name = "Random Name",
                AddressId = addressKey
            };

            await customerRepo.AddAsync(entity);

            var address = new CustomerAddress
            {
                Id = addressKey,
                Street = "Street",
                City = "New City",
                State = "ST",
                CustomerId = customerKey,
                Customer = entity
            };

            addressRepo.Add(address);

            TestCustomerAddress(address, (await customerRepo.GetAsync(customerKey, fetchStrategy)).Address);

            // for one to one, the navigation properties will be included automatically (no need to fetch)
            TestCustomerAddress(address, (await customerRepo.GetAsync(customerKey)).Address);
        }

        [Fact]
        public void FindWithFetchStrategyWithNavigationProperty()
        {
            Data.TestAdoNetConnectionStringFactory.Create(out string providerName, out string connectionString);

            var addressRepo = new AdoNetRepository<CustomerAddress>(providerName, connectionString);
            var customerRepo = new AdoNetRepository<Customer>(providerName, connectionString);
            var customerKey = 1;
            var addressKey = 1;

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();

            var entity = new Customer
            {
                Id = customerKey,
                Name = name,
                AddressId = addressKey
            };

            customerRepo.Add(entity);

            var address = new CustomerAddress
            {
                Id = addressKey,
                Street = "Street",
                City = "New City",
                State = "ST",
                CustomerId = customerKey,
                Customer = entity
            };

            addressRepo.Add(address);

            // for one to one, the navigation properties will be included automatically (no need to fetch)
            TestCustomerAddress(address, customerRepo.Find(x => x.Name.Equals(name)).Address);
            TestCustomerAddress(address, customerRepo.Find(options).Address);
            TestCustomerAddress(address, customerRepo.Find<CustomerAddress>(x => x.Name.Equals(name), x => x.Address));
            TestCustomerAddress(address, customerRepo.Find<CustomerAddress>(options, x => x.Address));
        }

        [Fact]
        public async Task FindWithFetchStrategyWithNavigationPropertyAsync()
        {
            Data.TestAdoNetConnectionStringFactory.Create(out string providerName, out string connectionString);

            var addressRepo = new AdoNetRepository<CustomerAddress>(providerName, connectionString);
            var customerRepo = new AdoNetRepository<Customer>(providerName, connectionString);
            var customerKey = 1;
            var addressKey = 1;

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();

            var entity = new Customer
            {
                Id = customerKey,
                Name = name,
                AddressId = addressKey
            };

            await customerRepo.AddAsync(entity);

            var address = new CustomerAddress
            {
                Id = addressKey,
                Street = "Street",
                City = "New City",
                State = "ST",
                CustomerId = customerKey,
                Customer = entity
            };

            await addressRepo.AddAsync(address);

            // for one to one, the navigation properties will be included automatically (no need to fetch)
            TestCustomerAddress(address, (await customerRepo.FindAsync(x => x.Name.Equals(name))).Address);
            TestCustomerAddress(address, (await customerRepo.FindAsync(options)).Address);
            TestCustomerAddress(address, await customerRepo.FindAsync<CustomerAddress>(x => x.Name.Equals(name), x => x.Address));
            TestCustomerAddress(address, await customerRepo.FindAsync<CustomerAddress>(options, x => x.Address));
        }

        [Fact]
        public void FindAlldWithFetchStrategyWithNavigationProperty()
        {
            Data.TestAdoNetConnectionStringFactory.Create(out string providerName, out string connectionString);

            var addressRepo = new AdoNetRepository<CustomerAddress>(providerName, connectionString);
            var customerRepo = new AdoNetRepository<Customer>(providerName, connectionString);
            var customerKey = 1;
            var addressKey = 1;

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();

            var entity = new Customer
            {
                Id = customerKey,
                Name = name,
                AddressId = addressKey
            };

            customerRepo.Add(entity);

            var address = new CustomerAddress
            {
                Id = addressKey,
                Street = "Street",
                City = "New City",
                State = "ST",
                CustomerId = customerKey,
                Customer = entity
            };

            addressRepo.Add(address);

            // for one to one, the navigation properties will be included automatically (no need to fetch)
            TestCustomerAddress(address, customerRepo.FindAll()?.FirstOrDefault()?.Address);
            TestCustomerAddress(address, customerRepo.FindAll(x => x.Name.Equals(name))?.FirstOrDefault()?.Address);
            TestCustomerAddress(address, customerRepo.FindAll(options)?.FirstOrDefault()?.Address);
            TestCustomerAddress(address, customerRepo.FindAll<CustomerAddress>(x => x.Name.Equals(name), x => x.Address)?.FirstOrDefault());
            TestCustomerAddress(address, customerRepo.FindAll<CustomerAddress>(options, x => x.Address)?.FirstOrDefault());
        }

        [Fact]
        public async Task FindAlldWithFetchStrategyWithNavigationPropertyAsync()
        {
            Data.TestAdoNetConnectionStringFactory.Create(out string providerName, out string connectionString);

            var addressRepo = new AdoNetRepository<CustomerAddress>(providerName, connectionString);
            var customerRepo = new AdoNetRepository<Customer>(providerName, connectionString);
            var customerKey = 1;
            var addressKey = 1;

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();

            var entity = new Customer
            {
                Id = customerKey,
                Name = name,
                AddressId = addressKey
            };

            await customerRepo.AddAsync(entity);

            var address = new CustomerAddress
            {
                Id = addressKey,
                Street = "Street",
                City = "New City",
                State = "ST",
                CustomerId = customerKey,
                Customer = entity
            };

            await addressRepo.AddAsync(address);

            // for one to one, the navigation properties will be included automatically (no need to fetch)
            TestCustomerAddress(address, (await customerRepo.FindAllAsync())?.FirstOrDefault()?.Address);
            TestCustomerAddress(address, (await customerRepo.FindAllAsync(x => x.Name.Equals(name)))?.FirstOrDefault()?.Address);
            TestCustomerAddress(address, (await customerRepo.FindAllAsync(options))?.FirstOrDefault()?.Address);
            TestCustomerAddress(address, (await customerRepo.FindAllAsync<CustomerAddress>(x => x.Name.Equals(name), x => x.Address))?.FirstOrDefault());
            TestCustomerAddress(address, (await customerRepo.FindAllAsync<CustomerAddress>(options, x => x.Address))?.FirstOrDefault());
        }

        [Fact]
        public void ExistWithFetchStrategyWithNavigationProperty()
        {
            Data.TestAdoNetConnectionStringFactory.Create(out string providerName, out string connectionString);

            var addressRepo = new AdoNetRepository<CustomerAddress>(providerName, connectionString);
            var customerRepo = new AdoNetRepository<Customer>(providerName, connectionString);
            var customerKey = 1;
            var addressKey = 1;

            const string street = "Street";

            var options = new QueryOptions<Customer>();

            var entity = new Customer
            {
                Id = customerKey,
                Name = "Random Name",
                AddressId = addressKey
            };

            customerRepo.Add(entity);

            var address = new CustomerAddress
            {
                Id = addressKey,
                Street = street,
                City = "New City",
                State = "ST",
                CustomerId = customerKey,
                Customer = entity
            };

            addressRepo.Add(address);

            Assert.True(customerRepo.Exists(options));

            // for one to one, the navigation properties will be included automatically (no need to fetch)
            Assert.True(customerRepo.Exists(x => x.Address.Street.Equals(street)));
        }

        [Fact]
        public async Task ExistWithFetchStrategyWithNavigationPropertyAsync()
        {
            Data.TestAdoNetConnectionStringFactory.Create(out string providerName, out string connectionString);

            var addressRepo = new AdoNetRepository<CustomerAddress>(providerName, connectionString);
            var customerRepo = new AdoNetRepository<Customer>(providerName, connectionString);
            var customerKey = 1;
            var addressKey = 1;

            const string street = "Street";

            var options = new QueryOptions<Customer>();

            var entity = new Customer
            {
                Id = customerKey,
                Name = "Random Name",
                AddressId = addressKey
            };

            await customerRepo.AddAsync(entity);

            var address = new CustomerAddress
            {
                Id = addressKey,
                Street = street,
                City = "New City",
                State = "ST",
                CustomerId = customerKey,
                Customer = entity
            };

            await addressRepo.AddAsync(address);

            Assert.True(await customerRepo.ExistsAsync(options));

            // for one to one, the navigation properties will be included automatically (no need to fetch)
            Assert.True(await customerRepo.ExistsAsync(x => x.Address.Street.Equals(street)));
        }

        [Fact]
        public void DeleteWithKeyDataAttributeChanged()
        {
            Data.TestAdoNetConnectionStringFactory.Create(out string providerName, out string connectionString);

            var repo = new AdoNetRepository<CustomerWithKeyAnnotation>(providerName, connectionString);

            const string name = "Random Name";

            var entity = new CustomerWithKeyAnnotation { Name = name };

            repo.Add(entity);

            Assert.True(repo.Exists(entity.Key));

            repo.Delete(entity.Key);

            Assert.False(repo.Exists(entity.Key));
        }

        private static void TestCustomerAddress(CustomerAddress expected, CustomerAddress actual)
        {
            Assert.NotNull(expected);
            Assert.NotNull(actual);
            Assert.NotEqual(expected, actual);

            // The navigation property should have all the values mapped correctly
            Assert.Equal(expected.Street, actual.Street);
            Assert.Equal(expected.City, actual.City);
            Assert.Equal(expected.State, actual.State);

            // The navigation property should have a key linking back to the main class (customer)
            Assert.NotEqual(0, actual.CustomerId);
            Assert.NotEqual(0, expected.CustomerId);
            Assert.Equal(expected.CustomerId, actual.CustomerId);

            // If the navigation property has also a navigation property linking back to the main class (customer),
            // then that navigation property should also be mapped correctly
            Assert.NotNull(expected.Customer);
            Assert.NotNull(actual.Customer);
            Assert.Equal(expected.Customer.Id, expected.Customer.Id);
            Assert.Equal(expected.Customer.Name, expected.Customer.Name);
            Assert.Equal(expected.Customer.AddressId, expected.Customer.AddressId);
        }

        private static void TestCustomerAddress(CustomerAddressWithForeignKeyAnnotationOnForeignKey expected, CustomerAddressWithForeignKeyAnnotationOnForeignKey actual)
        {
            Assert.NotNull(expected);
            Assert.NotNull(actual);
            Assert.NotEqual(expected, actual);

            // The navigation property should have all the values mapped correctly
            Assert.Equal(expected.Street, actual.Street);
            Assert.Equal(expected.City, actual.City);
            Assert.Equal(expected.State, actual.State);

            // The navigation property should have a key linking back to the main class (customer)
            Assert.NotEqual(0, actual.CustomerKey);
            Assert.NotEqual(0, expected.CustomerKey);
            Assert.Equal(expected.CustomerKey, actual.CustomerKey);

            // If the navigation property has also a navigation property linking back to the main class (customer),
            // then that navigation property should also be mapped correctly
            Assert.NotNull(expected.Customer);
            Assert.NotNull(actual.Customer);
            Assert.Equal(expected.Customer.Id, expected.Customer.Id);
            Assert.Equal(expected.Customer.Name, expected.Customer.Name);
            Assert.Equal(expected.Customer.AddressKey, expected.Customer.AddressKey);
        }

        private static void TestCustomerAddress(CustomerAddressWithForeignKeyAnnotationOnNavigationProperty expected, CustomerAddressWithForeignKeyAnnotationOnNavigationProperty actual)
        {
            Assert.NotNull(expected);
            Assert.NotNull(actual);
            Assert.NotEqual(expected, actual);

            // The navigation property should have all the values mapped correctly
            Assert.Equal(expected.Street, actual.Street);
            Assert.Equal(expected.City, actual.City);
            Assert.Equal(expected.State, actual.State);

            // The navigation property should have a key linking back to the main class (customer)
            Assert.NotEqual(0, actual.CustomerKey);
            Assert.NotEqual(0, expected.CustomerKey);
            Assert.Equal(expected.CustomerKey, actual.CustomerKey);

            // If the navigation property has also a navigation property linking back to the main class (customer),
            // then that navigation property should also be mapped correctly
            Assert.NotNull(expected.Customer);
            Assert.NotNull(actual.Customer);
            Assert.Equal(expected.Customer.Id, expected.Customer.Id);
            Assert.Equal(expected.Customer.Name, expected.Customer.Name);
            Assert.Equal(expected.Customer.AddressKey, expected.Customer.AddressKey);
        }

        class Customer
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int AddressId { get; set; }
            public CustomerAddress Address { get; set; }
        }

        [Table("Customers")]
        class CustomerWithKeyAnnotation
        {
            [Key]
            [Column("Id")]
            public int Key { get; set; }
            public string Name { get; set; }
            public int AddressId { get; set; }
            public CustomerAddress Address { get; set; }
        }

        [Table("Customers")]
        class CustomerWithForeignKeyAnnotationOnForeignKey
        {
            public int Id { get; set; }
            public string Name { get; set; }
            [Column("AddressId")]
            public int AddressKey { get; set; }
            public CustomerAddressWithForeignKeyAnnotationOnForeignKey Address { get; set; }
        }

        [Table("Customers")]
        class CustomerWithForeignKeyAnnotationOnNavigationProperty
        {
            public int Id { get; set; }
            public string Name { get; set; }
            [Column("AddressId")]
            public int AddressKey { get; set; }
            [ForeignKey("AddressId")]
            public CustomerAddressWithForeignKeyAnnotationOnNavigationProperty Address { get; set; }
        }

        class CustomerAddress
        {
            public int Id { get; set; }
            public string Street { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public int CustomerId { get; set; }
            public Customer Customer { get; set; }
        }

        [Table("CustomerAddresses")]
        class CustomerAddressWithForeignKeyAnnotationOnForeignKey
        {
            public int Id { get; set; }
            public string Street { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            [ForeignKey("Customer")]
            [Column("CustomerId")]
            public int CustomerKey { get; set; }
            public CustomerWithForeignKeyAnnotationOnForeignKey Customer { get; set; }
        }

        [Table("CustomerAddresses")]
        class CustomerAddressWithForeignKeyAnnotationOnNavigationProperty
        {
            public int Id { get; set; }
            public string Street { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            [Column("CustomerId")]
            public int CustomerKey { get; set; }
            [ForeignKey("CustomerId")]
            public CustomerWithForeignKeyAnnotationOnNavigationProperty Customer { get; set; }
        }
    }
}