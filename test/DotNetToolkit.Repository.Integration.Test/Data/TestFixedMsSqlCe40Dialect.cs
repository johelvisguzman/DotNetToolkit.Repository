namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using global::NHibernate.Dialect;

    public class TestFixedMsSqlCe40Dialect : MsSqlCe40Dialect
    {
        public override bool SupportsVariableLimit => true;
    }
}
