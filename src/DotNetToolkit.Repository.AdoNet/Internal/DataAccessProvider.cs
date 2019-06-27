namespace DotNetToolkit.Repository.AdoNet.Internal
{
    using System;

    internal static class DataAccessProvider
    {
        public static DataAccessProviderType GetProviderType(string providerName)
        {
            switch (providerName.ToLower())
            {
#if NETFULL
                case "system.data.sqlserverce.4.0":
                    return DataAccessProviderType.SqlServerCompact;
                case "microsoft.jet.oledb.4.0":
                    return DataAccessProviderType.OleDb;
#endif
                case "system.data.sqlclient":
                    return DataAccessProviderType.SqlServer;
                case "system.data.sqlite":
                case "microsoft.data.sqlite":
                    return DataAccessProviderType.SqLite;
                case "mysql.data.mysqlclient":
                case "mysql.data":
                    return DataAccessProviderType.MySql;
                case "npgsql":
                    return DataAccessProviderType.PostgreSql;
                default:
                    throw new NotSupportedException($"Unsupported Provider Factory specified: {providerName}");
            }
        }
    }
}
