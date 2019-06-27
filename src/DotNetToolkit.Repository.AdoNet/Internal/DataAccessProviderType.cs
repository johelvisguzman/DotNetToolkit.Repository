namespace DotNetToolkit.Repository.AdoNet.Internal
{
    internal enum DataAccessProviderType
    {
        SqlServer,
        SqLite,
        MySql,
        PostgreSql,

#if NETFULL
        OleDb,
        SqlServerCompact
#endif
    }
}
