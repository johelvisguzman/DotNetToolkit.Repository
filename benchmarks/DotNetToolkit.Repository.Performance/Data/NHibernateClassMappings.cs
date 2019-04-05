namespace DotNetToolkit.Repository.Performance.Data
{
    using global::NHibernate.Mapping.ByCode;
    using global::NHibernate.Mapping.ByCode.Conformist;

    public class CustomerMap : ClassMapping<Customer>
    {
        public CustomerMap()
        {
            Id(x => x.Id, map => map.Generator(new IdentityGeneratorDef()));
            Property(x => x.Name);
            Lazy(false); // do not want nhibernate to do any lazy loading or any of it's magic
        }
    }
}
