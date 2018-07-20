namespace DotNetToolkit.Repository.Integration.Test
{
    using AdoNet.Internal;
    using AdoNet.Internal.Schema;
    using Data;
    using FetchStrategies;
    using Queries;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class AdoNetRepositoryTests
    {
        [Fact]
        public void FindWithNavigationPropertyByKey_OneToOneRelationship()
        {
            var contextFactory = Data.TestAdoNetContextFactory.Create();
            var context = contextFactory.Create();

            var customerKey = 1;
            var addressKey = 1;

            var addressRepo = new Repository<CustomerAddress>(context);
            var customerRepo = new Repository<Customer>(context);
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

            TestCustomerAddress(address, customerRepo.Find(customerKey, customerFetchStrategy).Address);

            // for one to one, the navigation properties will be included automatically (no need to fetch)
            TestCustomerAddress(address, customerRepo.Find(customerKey).Address);
        }

        [Fact]
        public void FindWithCompositeNavigationPropertyByKey_OneToOneRelationship()
        {
            var contextFactory = Data.TestAdoNetContextFactory.Create();
            var context = contextFactory.Create();

            var customerKey = 1;
            var addressKey1 = 1;
            var addressKey2 = 2;

            var addressRepo = new Repository<CustomerAddressWithTwoCompositePrimaryKey>(context);
            var customerRepo = new Repository<CustomerWithTwoCompositePrimaryKey>(context);
            var customerFetchStrategy = new FetchStrategy<CustomerWithTwoCompositePrimaryKey>();

            var entity = new CustomerWithTwoCompositePrimaryKey
            {
                Id = customerKey,
                Name = "Random Name",
                AddressId1 = addressKey1,
                AddressId2 = addressKey2
            };

            var address = new CustomerAddressWithTwoCompositePrimaryKey
            {
                Id1 = addressKey1,
                Id2 = addressKey2,
                Street = "Street",
                City = "New City",
                State = "ST",
                CustomerId = customerKey,
                Customer = entity
            };

            addressRepo.Add(address);
            customerRepo.Add(entity);

            TestCustomerAddress(address, customerRepo.Find(customerKey, customerFetchStrategy).Address);

            // for one to one, the navigation properties will be included automatically (no need to fetch)
            TestCustomerAddress(address, customerRepo.Find(customerKey).Address);
        }

        [Fact]
        public void FindWithForeignKeyAnnotationChangedByKey_OneToOneRelationship()
        {
            var contextFactory = Data.TestAdoNetContextFactory.Create();
            var context = contextFactory.Create();

            var customerKey = 1;
            var addressKey = 1;

            var addressWithForeignKeyAnnotationOnForeignKeyRepo = new Repository<CustomerAddressWithForeignKeyAnnotationOnForeignKey>(context);
            var customerWithForeignKeyAnnotationOnForeignKeyRepo = new Repository<CustomerWithForeignKeyAnnotationOnForeignKey>(context);
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

            TestCustomerAddress(addressWithForeignKeyAnnotationOnForeignKey, customerWithForeignKeyAnnotationOnForeignKeyRepo.Find(customerKey, customerWithForeignKeyAnnotationOnForeignKeyFetchStrategy).Address);

            // for one to one, the navigation properties will be included automatically (no need to fetch)
            TestCustomerAddress(addressWithForeignKeyAnnotationOnForeignKey, customerWithForeignKeyAnnotationOnForeignKeyRepo.Find(customerKey).Address);

            var addressWithForeignKeyAnnotationOnNavigationPropertyRepo = new Repository<CustomerAddressWithForeignKeyAnnotationOnNavigationProperty>(context);
            var customerWithForeignKeyAnnotationOnNavigationPropertyRepo = new Repository<CustomerWithForeignKeyAnnotationOnNavigationProperty>(context);
            var customerWithForeignKeyAnnotationOnNavigationPropertyFetchStrategy = new FetchStrategy<CustomerWithForeignKeyAnnotationOnNavigationProperty>();

            customerKey = 2;
            addressKey = 2;

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

            TestCustomerAddress(addressWithForeignKeyAnnotationOnNavigationProperty, customerWithForeignKeyAnnotationOnNavigationPropertyRepo.Find(customerKey, customerWithForeignKeyAnnotationOnNavigationPropertyFetchStrategy).Address);

            // for one to one, the navigation properties will be included automatically (no need to fetch)
            TestCustomerAddress(addressWithForeignKeyAnnotationOnNavigationProperty, customerWithForeignKeyAnnotationOnNavigationPropertyRepo.Find(customerKey).Address);
        }

        [Fact]
        public void FindWithNavigationPropertyByCompositeKey_OneToOneRelationship()
        {
            var contextFactory = Data.TestAdoNetContextFactory.Create();
            var context = contextFactory.Create();

            var customerKey = 1;
            var addressKey = 1;

            var addressRepo = new Repository<CustomerCompositeAddress>(context);
            var customerRepo = new Repository<CustomerWithCompositeAddress>(context);
            var customerFetchStrategy = new FetchStrategy<CustomerWithCompositeAddress>();

            var entity = new CustomerWithCompositeAddress
            {
                Id = customerKey,
                Name = "Random Name",
                AddressId = addressKey
            };

            customerRepo.Add(entity);

            var address = new CustomerCompositeAddress
            {
                Id = addressKey,
                Street = "Street",
                City = "New City",
                State = "ST",
                CustomerId = customerKey,
                Customer = entity
            };

            addressRepo.Add(address);

            TestCustomerAddress(address, customerRepo.Find(customerKey, customerFetchStrategy).Address);

            // for one to one, the navigation properties will be included automatically (no need to fetch)
            TestCustomerAddress(address, customerRepo.Find(customerKey).Address);
        }

        [Fact]
        public async Task FindWithNavigationPropertyByKeyAsync_OneToOneRelationship()
        {
            var contextFactory = Data.TestAdoNetContextFactory.Create();
            var context = contextFactory.Create();

            var addressRepo = new Repository<CustomerAddress>(context);
            var customerRepo = new Repository<Customer>(context);
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

            TestCustomerAddress(address, (await customerRepo.FindAsync(customerKey, fetchStrategy)).Address);

            // for one to one, the navigation properties will be included automatically (no need to fetch)
            TestCustomerAddress(address, (await customerRepo.FindAsync(customerKey)).Address);
        }

        [Fact]
        public void FindWithNavigationProperty_OneToOneRelationship()
        {
            var contextFactory = Data.TestAdoNetContextFactory.Create();
            var context = contextFactory.Create();

            var addressRepo = new Repository<CustomerAddress>(context);
            var customerRepo = new Repository<Customer>(context);
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
        public async Task FindWithNavigationPropertyAsync_OneToOneRelationship()
        {
            var contextFactory = Data.TestAdoNetContextFactory.Create();
            var context = contextFactory.Create();

            var addressRepo = new Repository<CustomerAddress>(context);
            var customerRepo = new Repository<Customer>(context);
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
        public void FindAlldWithNavigationProperty_OneToOneRelationship()
        {
            var contextFactory = Data.TestAdoNetContextFactory.Create();
            var context = contextFactory.Create();

            var addressRepo = new Repository<CustomerAddress>(context);
            var customerRepo = new Repository<Customer>(context);
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
        public async Task FindAlldWithNavigationPropertyAsync_OneToOneRelationship()
        {
            var contextFactory = Data.TestAdoNetContextFactory.Create();
            var context = contextFactory.Create();

            var addressRepo = new Repository<CustomerAddress>(context);
            var customerRepo = new Repository<Customer>(context);
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
        public void ExistWithNavigationProperty_OneToOneRelationship()
        {
            var contextFactory = Data.TestAdoNetContextFactory.Create();
            var context = contextFactory.Create();

            var addressRepo = new Repository<CustomerAddress>(context);
            var customerRepo = new Repository<Customer>(context);
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
        public async Task ExistWithNavigationPropertyAsync_OneToOneRelationship()
        {
            var contextFactory = Data.TestAdoNetContextFactory.Create();
            var context = contextFactory.Create();

            var addressRepo = new Repository<CustomerAddress>(context);
            var customerRepo = new Repository<Customer>(context);
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
        public void DeleteWithKeyDataAttribute()
        {
            var contextFactory = Data.TestAdoNetContextFactory.Create();
            var context = contextFactory.Create();

            var repo = new Repository<CustomerWithKeyAnnotation>(context);

            const string name = "Random Name";

            var entity = new CustomerWithKeyAnnotation { Name = name };

            repo.Add(entity);

            Assert.True(repo.Exists(entity.Key));

            repo.Delete(entity.Key);

            Assert.False(repo.Exists(entity.Key));
        }

        [Fact]
        public async Task DeleteWithKeyDataAttributeAsync()
        {
            var contextFactory = Data.TestAdoNetContextFactory.Create();
            var context = contextFactory.Create();

            var repo = new Repository<CustomerWithKeyAnnotation>(context);

            const string name = "Random Name";

            var entity = new CustomerWithKeyAnnotation { Name = name };

            await repo.AddAsync(entity);

            Assert.True(await repo.ExistsAsync(entity.Key));

            await repo.DeleteAsync(entity.Key);

            Assert.False(await repo.ExistsAsync(entity.Key));
        }

        [Fact]
        public void FindWithComplexExpressions()
        {
            var contextFactory = Data.TestAdoNetContextFactory.Create();
            var context = contextFactory.Create();

            var repo = new Repository<Customer>(context);

            var entities = new List<Customer>
            {
                new Customer { Id = 1, Name = "Random Name 1" },
                new Customer { Id = 2, Name = "Random Name 2" },
                new Customer { Id = 3, Name = "Random Test Name 3" },
                new Customer { Id = 4, Name = "Random Name 4" }
            };

            repo.Add(entities);

            // The ado.net repository should be able translate the expression into a valid sql query string to execute
            // things like parentheses and operators should be tested

            // property variables on the left and constants on the right (and vice verse)
            Assert.Equal(1, repo.Find(x => 1 == x.Id).Id);
            Assert.Equal(1, repo.Find(x => x.Id == 1).Id);
            Assert.Equal(1, repo.Find<int>(x => 1 == x.Id, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id == 1, x => x.Id));

            // boolean
            Assert.Equal(1, repo.Find(x => true).Id);
            Assert.Null(repo.Find(x => false));
            Assert.Equal(1, repo.Find(x => true, x => x.Id));
            Assert.Equal(0, repo.Find(x => false, x => x.Id));

            // method calls
            Assert.Equal(1, repo.Find(x => x.Name.StartsWith("Random")).Id);
            Assert.Equal(2, repo.Find(x => x.Name.Equals("Random Name 2")).Id);
            Assert.Equal(3, repo.Find(x => x.Name.Contains("Test")).Id);
            Assert.Equal(4, repo.Find(x => x.Name.EndsWith("4")).Id);
            Assert.Equal(1, repo.Find<int>(x => x.Name.StartsWith("Random"), x => x.Id));
            Assert.Equal(2, repo.Find<int>(x => x.Name.Equals("Random Name 2"), x => x.Id));
            Assert.Equal(3, repo.Find<int>(x => x.Name.Contains("Test"), x => x.Id));
            Assert.Equal(4, repo.Find<int>(x => x.Name.EndsWith("4"), x => x.Id));

            // relational and equality operators - property variables on the left and constants on the right (and vice verse)
            Assert.Equal(1, repo.Find(x => x.Id == 1).Id);
            Assert.Equal(1, repo.Find(x => x.Id != 2).Id);
            Assert.Equal(2, repo.Find(x => x.Id > 1).Id);
            Assert.Equal(3, repo.Find(x => x.Id >= 3).Id);
            Assert.Equal(1, repo.Find(x => x.Id < 2).Id);
            Assert.Equal(1, repo.Find(x => x.Id <= 2).Id);
            Assert.Equal(1, repo.Find<int>(x => x.Id == 1, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id != 2, x => x.Id));
            Assert.Equal(2, repo.Find<int>(x => x.Id > 1, x => x.Id));
            Assert.Equal(3, repo.Find<int>(x => x.Id >= 3, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id < 2, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id <= 2, x => x.Id));

            // conditional or operations - property variables on the left and constants on the right (and vice verse)
            // relational and equality operators
            Assert.Equal(1, repo.Find(x => (x.Id == 1) || (x.Id >= 1)).Id);
            Assert.Equal(1, repo.Find(x => (1 == x.Id) || (1 <= x.Id)).Id);
            Assert.Equal(1, repo.Find<int>(x => (x.Id == 1) || (x.Id >= 1), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => (1 == x.Id) || (1 <= x.Id), x => x.Id));

            // logical or operations - property variables on the left and constants on the right (and vice verse)
            Assert.Equal(1, repo.Find(x => (x.Id == 1) | (x.Id >= 1)).Id);
            Assert.Equal(1, repo.Find(x => (1 == x.Id) | (1 <= x.Id)).Id);
            Assert.Equal(1, repo.Find<int>(x => (x.Id == 1) | (x.Id >= 1), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => (1 == x.Id) | (1 <= x.Id), x => x.Id));

            // conditional and operations - property variables on the left and constants on the right (and vice verse)
            // relational and equality operators
            Assert.Equal(2, repo.Find(x => x.Id > 1 && x.Id < 3).Id);
            Assert.Equal(2, repo.Find(x => (x.Id > 1) && (x.Id < 3)).Id);
            Assert.Equal(2, repo.Find<int>(x => x.Id > 1 && x.Id < 3, x => x.Id));
            Assert.Equal(2, repo.Find<int>(x => (x.Id > 1) && (x.Id < 3), x => x.Id));

            // logical and operations - property variables on the left and constants on the right (and vice verse)
            Assert.Equal(1, repo.Find(x => (x.Id == 1) & (x.Id >= 1)).Id);
            Assert.Equal(1, repo.Find(x => (1 == x.Id) & (1 <= x.Id)).Id);
            Assert.Equal(1, repo.Find<int>(x => (x.Id == 1) & (x.Id >= 1), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => (1 == x.Id) & (1 <= x.Id), x => x.Id));
        }

        [Fact]
        public async void FindWithComplexExpressionsAsync()
        {
            var contextFactory = Data.TestAdoNetContextFactory.Create();
            var context = contextFactory.Create();

            var repo = new Repository<Customer>(context);

            var entities = new List<Customer>
            {
                new Customer { Id = 1, Name = "Random Name 1" },
                new Customer { Id = 2, Name = "Random Name 2" },
                new Customer { Id = 3, Name = "Random Test Name 3" },
                new Customer { Id = 4, Name = "Random Name 4" }
            };

            await repo.AddAsync(entities);

            // The ado.net repository should be able translate the expression into a valid sql query string to execute
            // things like parentheses and operators should be tested

            // property variables on the left and constants on the right (and vice verse)
            Assert.Equal(1, (await repo.FindAsync(x => 1 == x.Id)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id == 1)).Id);
            Assert.Equal(1, repo.Find<int>(x => 1 == x.Id, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id == 1, x => x.Id));

            // boolean
            Assert.Equal(1, (await repo.FindAsync(x => true)).Id);
            Assert.Null(await repo.FindAsync(x => false));
            Assert.Equal(1, await repo.FindAsync(x => true, x => x.Id));
            Assert.Equal(0, await repo.FindAsync(x => false, x => x.Id));

            // method calls
            Assert.Equal(1, (await repo.FindAsync(x => x.Name.StartsWith("Random"))).Id);
            Assert.Equal(2, (await repo.FindAsync(x => x.Name.Equals("Random Name 2"))).Id);
            Assert.Equal(3, (await repo.FindAsync(x => x.Name.Contains("Test"))).Id);
            Assert.Equal(4, (await repo.FindAsync(x => x.Name.EndsWith("4"))).Id);
            Assert.Equal(1, repo.Find<int>(x => x.Name.StartsWith("Random"), x => x.Id));
            Assert.Equal(2, repo.Find<int>(x => x.Name.Equals("Random Name 2"), x => x.Id));
            Assert.Equal(3, repo.Find<int>(x => x.Name.Contains("Test"), x => x.Id));
            Assert.Equal(4, repo.Find<int>(x => x.Name.EndsWith("4"), x => x.Id));

            // relational and equality operators - property variables on the left and constants on the right (and vice verse)
            Assert.Equal(1, (await repo.FindAsync(x => x.Id == 1)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id != 2)).Id);
            Assert.Equal(2, (await repo.FindAsync(x => x.Id > 1)).Id);
            Assert.Equal(3, (await repo.FindAsync(x => x.Id >= 3)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id < 2)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id <= 2)).Id);
            Assert.Equal(1, repo.Find<int>(x => x.Id == 1, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id != 2, x => x.Id));
            Assert.Equal(2, repo.Find<int>(x => x.Id > 1, x => x.Id));
            Assert.Equal(3, repo.Find<int>(x => x.Id >= 3, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id < 2, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id <= 2, x => x.Id));

            // conditional or operations - property variables on the left and constants on the right (and vice verse)
            // relational and equality operators
            Assert.Equal(1, (await repo.FindAsync(x => (x.Id == 1) || (x.Id >= 1))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => (1 == x.Id) || (1 <= x.Id))).Id);
            Assert.Equal(1, repo.Find<int>(x => (x.Id == 1) || (x.Id >= 1), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => (1 == x.Id) || (1 <= x.Id), x => x.Id));

            // logical or operations - property variables on the left and constants on the right (and vice verse)
            Assert.Equal(1, (await repo.FindAsync(x => (x.Id == 1) | (x.Id >= 1))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => (1 == x.Id) | (1 <= x.Id))).Id);
            Assert.Equal(1, repo.Find<int>(x => (x.Id == 1) | (x.Id >= 1), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => (1 == x.Id) | (1 <= x.Id), x => x.Id));

            // conditional and operations - property variables on the left and constants on the right (and vice verse)
            // relational and equality operators
            Assert.Equal(2, (await repo.FindAsync(x => x.Id > 1 && x.Id < 3)).Id);
            Assert.Equal(2, (await repo.FindAsync(x => (x.Id > 1) && (x.Id < 3))).Id);
            Assert.Equal(2, repo.Find<int>(x => x.Id > 1 && x.Id < 3, x => x.Id));
            Assert.Equal(2, repo.Find<int>(x => (x.Id > 1) && (x.Id < 3), x => x.Id));

            // logical and operations - property variables on the left and constants on the right (and vice verse)
            Assert.Equal(1, (await repo.FindAsync(x => (x.Id == 1) & (x.Id >= 1))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => (1 == x.Id) & (1 <= x.Id))).Id);
            Assert.Equal(1, repo.Find<int>(x => (x.Id == 1) & (x.Id >= 1), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => (1 == x.Id) & (1 <= x.Id), x => x.Id));
        }

        [Fact]
        public void FindAllWithComplexExpressions()
        {
            var contextFactory = Data.TestAdoNetContextFactory.Create();
            var context = contextFactory.Create();

            var repo = new Repository<Customer>(context);

            var entities = new List<Customer>
            {
                new Customer { Id = 1, Name = "Random Name 1" },
                new Customer { Id = 2, Name = "Random Name 2" },
                new Customer { Id = 3, Name = "Random Test Name 3" },
                new Customer { Id = 4, Name = "Random Name 4" }
            };

            repo.Add(entities);

            // The ado.net repository should be able translate the expression into a valid sql query string to execute
            // things like parentheses and operators should be tested

            // property variables on the left and constants on the right (and vice verse)
            Assert.Equal(1, repo.FindAll(x => 1 == x.Id).First().Id);
            Assert.Equal(1, repo.FindAll(x => x.Id == 1).First().Id);
            Assert.Equal(1, repo.FindAll<int>(x => 1 == x.Id, x => x.Id).First());
            Assert.Equal(1, repo.FindAll<int>(x => x.Id == 1, x => x.Id).First());

            // boolean
            Assert.Equal(1, repo.FindAll(x => true).First().Id);
            Assert.Empty(repo.FindAll(x => false));
            Assert.Equal(1, repo.FindAll(x => true, x => x.Id).First());
            Assert.Empty(repo.FindAll(x => false, x => x.Id));

            // method calls
            Assert.Equal(4, repo.FindAll(x => x.Name.StartsWith("Random")).Count());
            Assert.Single(repo.FindAll(x => x.Name.Equals("Random Name 2")));
            Assert.Single(repo.FindAll(x => x.Name.Contains("Test")));
            Assert.Single(repo.FindAll(x => x.Name.EndsWith("4")));
            Assert.Equal(4, repo.FindAll<int>(x => x.Name.StartsWith("Random"), x => x.Id).Count());
            Assert.Single(repo.FindAll<int>(x => x.Name.Equals("Random Name 2"), x => x.Id));
            Assert.Single(repo.FindAll<int>(x => x.Name.Contains("Test"), x => x.Id));
            Assert.Single(repo.FindAll<int>(x => x.Name.EndsWith("4"), x => x.Id));

            // relational and equality operators - property variables on the left and constants on the right (and vice verse)
            Assert.Single(repo.FindAll(x => x.Id == 1));
            Assert.Equal(3, repo.FindAll(x => x.Id != 2).Count());
            Assert.Equal(3, repo.FindAll(x => x.Id > 1).Count());
            Assert.Equal(2, repo.FindAll(x => x.Id >= 3).Count());
            Assert.Single(repo.FindAll(x => x.Id < 2));
            Assert.Equal(2, repo.FindAll(x => x.Id <= 2).Count());
            Assert.Single(repo.FindAll<int>(x => x.Id == 1, x => x.Id));
            Assert.Equal(3, repo.FindAll<int>(x => x.Id != 2, x => x.Id).Count());
            Assert.Equal(3, repo.FindAll<int>(x => x.Id > 1, x => x.Id).Count());
            Assert.Equal(2, repo.FindAll<int>(x => x.Id >= 3, x => x.Id).Count());
            Assert.Single(repo.FindAll<int>(x => x.Id < 2, x => x.Id));
            Assert.Equal(2, repo.FindAll<int>(x => x.Id <= 2, x => x.Id).Count());

            // conditional or operations - property variables on the left and constants on the right (and vice verse)
            // relational and equality operators
            Assert.Equal(4, repo.FindAll(x => (x.Id == 1) || (x.Id >= 1)).Count());
            Assert.Single(repo.FindAll(x => (1 == x.Id) || (1 <= x.Id)));
            Assert.Equal(4, repo.FindAll<int>(x => (x.Id == 1) || (x.Id >= 1), x => x.Id).Count());
            Assert.Single(repo.FindAll<int>(x => (1 == x.Id) || (1 <= x.Id), x => x.Id));

            // logical or operations - property variables on the left and constants on the right (and vice verse)
            Assert.Equal(4, repo.FindAll(x => (x.Id == 1) | (x.Id >= 1)).Count());
            Assert.Single(repo.FindAll(x => (1 == x.Id) | (1 <= x.Id)));
            Assert.Equal(4, repo.FindAll<int>(x => (x.Id == 1) | (x.Id >= 1), x => x.Id).Count());
            Assert.Single(repo.FindAll<int>(x => (1 == x.Id) | (1 <= x.Id), x => x.Id));

            // conditional and operations - property variables on the left and constants on the right (and vice verse)
            // relational and equality operators
            Assert.Single(repo.FindAll(x => x.Id > 1 && x.Id < 3));
            Assert.Single(repo.FindAll(x => (x.Id > 1) && (x.Id < 3)));
            Assert.Single(repo.FindAll<int>(x => x.Id > 1 && x.Id < 3, x => x.Id));
            Assert.Single(repo.FindAll<int>(x => (x.Id > 1) && (x.Id < 3), x => x.Id));

            // logical and operations - property variables on the left and constants on the right (and vice verse)
            Assert.Single(repo.FindAll(x => (x.Id == 1) & (x.Id >= 1)));
            Assert.Single(repo.FindAll(x => (1 == x.Id) & (1 <= x.Id)));
            Assert.Single(repo.FindAll<int>(x => (x.Id == 1) & (x.Id >= 1), x => x.Id));
            Assert.Single(repo.FindAll<int>(x => (1 == x.Id) & (1 <= x.Id), x => x.Id));
        }

        [Fact]
        public async void FindAllWithComplexExpressionsAsync()
        {
            var contextFactory = Data.TestAdoNetContextFactory.Create();
            var context = contextFactory.Create();

            var repo = new Repository<Customer>(context);

            var entities = new List<Customer>
            {
                new Customer { Id = 1, Name = "Random Name 1" },
                new Customer { Id = 2, Name = "Random Name 2" },
                new Customer { Id = 3, Name = "Random Test Name 3" },
                new Customer { Id = 4, Name = "Random Name 4" }
            };

            await repo.AddAsync(entities);

            // The ado.net repository should be able translate the expression into a valid sql query string to execute
            // things like parentheses and operators should be tested

            // property variables on the left and constants on the right (and vice verse)
            Assert.Equal(1, (await repo.FindAllAsync(x => 1 == x.Id)).First().Id);
            Assert.Equal(1, (await repo.FindAllAsync(x => x.Id == 1)).First().Id);
            Assert.Equal(1, (await repo.FindAllAsync<int>(x => 1 == x.Id, x => x.Id)).First());
            Assert.Equal(1, (await repo.FindAllAsync<int>(x => x.Id == 1, x => x.Id)).First());

            // boolean
            Assert.Equal(1, (await repo.FindAllAsync(x => true)).First().Id);
            Assert.Empty(await repo.FindAllAsync(x => false));
            Assert.Equal(1, (await repo.FindAllAsync(x => true, x => x.Id)).First());
            Assert.Empty(await repo.FindAllAsync(x => false, x => x.Id));

            // method calls
            Assert.Equal(4, (await repo.FindAllAsync(x => x.Name.StartsWith("Random"))).Count());
            Assert.Single(await repo.FindAllAsync(x => x.Name.Equals("Random Name 2")));
            Assert.Single(await repo.FindAllAsync(x => x.Name.Contains("Test")));
            Assert.Single(await repo.FindAllAsync(x => x.Name.EndsWith("4")));
            Assert.Equal(4, (await repo.FindAllAsync<int>(x => x.Name.StartsWith("Random"), x => x.Id)).Count());
            Assert.Single(await repo.FindAllAsync<int>(x => x.Name.Equals("Random Name 2"), x => x.Id));
            Assert.Single(await repo.FindAllAsync<int>(x => x.Name.Contains("Test"), x => x.Id));
            Assert.Single(await repo.FindAllAsync<int>(x => x.Name.EndsWith("4"), x => x.Id));

            // relational and equality operators - property variables on the left and constants on the right (and vice verse)
            Assert.Single(await repo.FindAllAsync(x => x.Id == 1));
            Assert.Equal(3, (await repo.FindAllAsync(x => x.Id != 2)).Count());
            Assert.Equal(3, (await repo.FindAllAsync(x => x.Id > 1)).Count());
            Assert.Equal(2, (await repo.FindAllAsync(x => x.Id >= 3)).Count());
            Assert.Single(await repo.FindAllAsync(x => x.Id < 2));
            Assert.Equal(2, (await repo.FindAllAsync(x => x.Id <= 2)).Count());
            Assert.Single(await repo.FindAllAsync<int>(x => x.Id == 1, x => x.Id));
            Assert.Equal(3, (await repo.FindAllAsync<int>(x => x.Id != 2, x => x.Id)).Count());
            Assert.Equal(3, (await repo.FindAllAsync<int>(x => x.Id > 1, x => x.Id)).Count());
            Assert.Equal(2, (await repo.FindAllAsync<int>(x => x.Id >= 3, x => x.Id)).Count());
            Assert.Single(await repo.FindAllAsync<int>(x => x.Id < 2, x => x.Id));
            Assert.Equal(2, (await repo.FindAllAsync<int>(x => x.Id <= 2, x => x.Id)).Count());

            // conditional or operations - property variables on the left and constants on the right (and vice verse)
            // relational and equality operators
            Assert.Equal(4, (await repo.FindAllAsync(x => (x.Id == 1) || (x.Id >= 1))).Count());
            Assert.Single(await repo.FindAllAsync(x => (1 == x.Id) || (1 <= x.Id)));
            Assert.Equal(4, (await repo.FindAllAsync<int>(x => (x.Id == 1) || (x.Id >= 1), x => x.Id)).Count());
            Assert.Single(await repo.FindAllAsync<int>(x => (1 == x.Id) || (1 <= x.Id), x => x.Id));

            // logical or operations - property variables on the left and constants on the right (and vice verse)
            Assert.Equal(4, (await repo.FindAllAsync(x => (x.Id == 1) | (x.Id >= 1))).Count());
            Assert.Single(await repo.FindAllAsync(x => (1 == x.Id) | (1 <= x.Id)));
            Assert.Equal(4, (await repo.FindAllAsync<int>(x => (x.Id == 1) | (x.Id >= 1), x => x.Id)).Count());
            Assert.Single(await repo.FindAllAsync<int>(x => (1 == x.Id) | (1 <= x.Id), x => x.Id));

            // conditional and operations - property variables on the left and constants on the right (and vice verse)
            // relational and equality operators
            Assert.Single(await repo.FindAllAsync(x => x.Id > 1 && x.Id < 3));
            Assert.Single(await repo.FindAllAsync(x => (x.Id > 1) && (x.Id < 3)));
            Assert.Single(await repo.FindAllAsync<int>(x => x.Id > 1 && x.Id < 3, x => x.Id));
            Assert.Single(await repo.FindAllAsync<int>(x => (x.Id > 1) && (x.Id < 3), x => x.Id));

            // logical and operations - property variables on the left and constants on the right (and vice verse)
            Assert.Single(await repo.FindAllAsync(x => (x.Id == 1) & (x.Id >= 1)));
            Assert.Single(await repo.FindAllAsync(x => (1 == x.Id) & (1 <= x.Id)));
            Assert.Single(await repo.FindAllAsync<int>(x => (x.Id == 1) & (x.Id >= 1), x => x.Id));
            Assert.Single(await repo.FindAllAsync<int>(x => (1 == x.Id) & (1 <= x.Id), x => x.Id));
        }

        [Fact]
        public void ThrowsIfSchemaTableColumnsMismatchOnConstruction()
        {
            var contextFactory = Data.TestAdoNetContextFactory.Create();
            var context = contextFactory.Create();

            var ex = Assert.Throws<InvalidOperationException>(() => new Repository<CustomerColumnNameMismatch>(context).Add(new CustomerColumnNameMismatch()));
            Assert.Equal($"The model '{typeof(CustomerColumnNameMismatch).Name}' has changed since the database was created. Consider updating the database.", ex.Message);

            ex = Assert.Throws<InvalidOperationException>(() => new Repository<CustomerColumnNameMissing>(context).Add(new CustomerColumnNameMissing()));
            Assert.Equal($"The model '{typeof(CustomerColumnNameMissing).Name}' has changed since the database was created. Consider updating the database.", ex.Message);

            ex = Assert.Throws<InvalidOperationException>(() => new Repository<CustomerKeyMismatch>(context).Add(new CustomerKeyMismatch()));
            Assert.Equal($"The model '{typeof(CustomerKeyMismatch).Name}' has changed since the database was created. Consider updating the database.", ex.Message);

            ex = Assert.Throws<InvalidOperationException>(() => new Repository<CustomerColumnRequiredMissing>(context).Add(new CustomerColumnRequiredMissing()));
            Assert.Equal($"The model '{typeof(CustomerColumnRequiredMissing).Name}' has changed since the database was created. Consider updating the database.", ex.Message);
        }

        [Fact]
        public void CreateTableOnSaveChanges()
        {
            var contextFactory = TestAdoNetContextFactory.Create();
            var context = (AdoNetRepositoryContext)contextFactory.Create();
            var schemaHelper = new SchemaTableHelper(context);

            Assert.False(schemaHelper.ExexuteTableExists<CustomerNotCreated>());
            Assert.False(schemaHelper.ExexuteTableExists<CustomerAddressNotCreated>());

            var repo = new Repository<CustomerNotCreated>(context);

            // Needs to create foreign table since the CustomerNotCreated table needs it
            new Repository<CustomerAddressNotCreated>(context).Add(new CustomerAddressNotCreated
            {
                Id1 = 1,
                Id2 = 1,
                State = "ST",
                Street1 = "Street 1",
                City = "City"
            });

            repo.Add(new CustomerNotCreated { Name = "Random Name", AddressId1 = 1, AddressId2 = 1 });

            Assert.True(schemaHelper.ExexuteTableExists<CustomerNotCreated>());

            // the customer address is a navigation property of the customer table, and so, it will be created as well
            Assert.True(schemaHelper.ExexuteTableExists<CustomerAddressNotCreated>());

            schemaHelper.ExecuteTableValidate<CustomerNotCreated>();
            schemaHelper.ExecuteTableValidate<CustomerAddressNotCreated>();
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

        private static void TestCustomerAddress(CustomerAddressWithTwoCompositePrimaryKey expected, CustomerAddressWithTwoCompositePrimaryKey actual)
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
            Assert.Equal(expected.Customer.AddressId1, expected.Customer.AddressId1);
            Assert.Equal(expected.Customer.AddressId2, expected.Customer.AddressId2);
        }

        private static void TestCustomerAddress(CustomerCompositeAddress expected, CustomerCompositeAddress actual)
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

        [Table("Customers")]
        class CustomerWithCompositeAddress
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int AddressId { get; set; }
            public CustomerCompositeAddress Address { get; set; }
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

        [Table("CustomerCompositeAddresses")]
        class CustomerCompositeAddress
        {
            [Key]
            public int Id { get; set; }
            [Key]
            public int CustomerId { get; set; }
            public string Street { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public CustomerWithCompositeAddress Customer { get; set; }
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

        [Table("CustomersColumnNameMismatch")]
        class CustomerColumnNameMismatch
        {
            public int Id { get; set; }
            public string MismatchName { get; set; }
        }

        [Table("CustomersColumnNameMissing")]
        class CustomerColumnNameMissing
        {
            public int Id { get; set; }
        }

        [Table("CustomersColumnNameMissing")]
        class CustomerKeyMismatch
        {
            public int Id { get; set; }
            [Key]
            public int Id1 { get; set; }
        }

        [Table("CustomersColumnRequiredMissing")]
        class CustomerColumnRequiredMissing
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        [Table("CustomersWithTwoCompositePrimaryKey")]
        class CustomerWithTwoCompositePrimaryKey
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int AddressId1 { get; set; }
            public int AddressId2 { get; set; }
            public CustomerAddressWithTwoCompositePrimaryKey Address { get; set; }
        }

        [Table("CustomersAddressWithTwoCompositePrimaryKey")]
        class CustomerAddressWithTwoCompositePrimaryKey
        {
            [Key]
            public int Id1 { get; set; }
            [Key]
            public int Id2 { get; set; }
            public string Street { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public int CustomerId { get; set; }
            public CustomerWithTwoCompositePrimaryKey Customer { get; set; }
        }

        [Table("CustomersNotCreated")]
        class CustomerNotCreated
        {
            public int Id { get; set; }
            [Required]
            public string Name { get; set; }
            [ForeignKey("Address")]
            public int AddressId1 { get; set; }
            [ForeignKey("Address")]
            public int AddressId2 { get; set; }
            public CustomerAddressNotCreated Address { get; set; }
        }

        [Table("CustomersAddressNotCreated")]
        class CustomerAddressNotCreated
        {
            [Key]
            public int Id1 { get; set; }
            [Key]
            public int Id2 { get; set; }
            [Required]
            public string Street1 { get; set; }
            [Required]
            public string City { get; set; }
            [Required]
            [Column("ST")]
            [StringLength(2)]
            public string State { get; set; }
            public CustomerNotCreated Customer { get; set; }
        }
    }
}