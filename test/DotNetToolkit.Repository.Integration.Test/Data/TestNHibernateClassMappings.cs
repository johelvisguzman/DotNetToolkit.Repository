namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using global::NHibernate.Mapping.ByCode;
    using global::NHibernate.Mapping.ByCode.Conformist;

    public class CustomerMap : ClassMapping<Customer>
    {
        public CustomerMap()
        {
            Id(x => x.Id, map => map.Generator(new IdentityGeneratorDef()));
            Property(x => x.Name);
            OneToOne(x => x.Address, map =>
            {
                map.PropertyReference(typeof(CustomerAddress).GetPropertyOrFieldMatchingName("CustomerId"));
                map.Cascade(Cascade.All);
            });
            Lazy(false); // do not want nhibernate to do any lazy loading or any of it's magic
        }
    }

    public class CustomerAddressMap : ClassMapping<CustomerAddress>
    {
        public CustomerAddressMap()
        {
            Id(x => x.Id, map => map.Generator(new IdentityGeneratorDef()));
            Property(x => x.City);
            Property(x => x.State);
            Property(x => x.Street);
            Property(x => x.CustomerId);
            OneToOne(x => x.Customer, map => map.Constrained(false));
            Lazy(false); // do not want nhibernate to do any lazy loading or any of it's magic
        }
    }

    public class CustomerWithNoIdentityMap : ClassMapping<CustomerWithNoIdentity>
    {
        public CustomerWithNoIdentityMap()
        {
            Id(x => x.Id, map => map.Generator(new AssignedGeneratorDef()));
            Property(x => x.Name);
            Lazy(false); // do not want nhibernate to do any lazy loading or any of it's magic
        }
    }

    public class CustomerWithTwoCompositePrimaryKeyMap : ClassMapping<CustomerWithTwoCompositePrimaryKey>
    {
        public CustomerWithTwoCompositePrimaryKeyMap()
        {
            ComposedId(map =>
            {
                map.Property(x => x.Id1);
                map.Property(x => x.Id2);
            });
            Property(x => x.Name);
            Lazy(false); // do not want nhibernate to do any lazy loading or any of it's magic
        }
    }

    public class CustomerWithThreeCompositePrimaryKeyMap : ClassMapping<CustomerWithThreeCompositePrimaryKey>
    {
        public CustomerWithThreeCompositePrimaryKeyMap()
        {
            ComposedId(map =>
            {
                map.Property(x => x.Id1);
                map.Property(x => x.Id2);
                map.Property(x => x.Id3);
            });
            Property(x => x.Name);
            Lazy(false); // do not want nhibernate to do any lazy loading or any of it's magic
        }
    }
}
