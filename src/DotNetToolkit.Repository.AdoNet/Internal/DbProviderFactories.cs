namespace DotNetToolkit.Repository.AdoNet.Internal
{
    using Microsoft.DotNet.PlatformAbstractions;
    using Microsoft.Extensions.DependencyModel;
    using Properties;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    internal static class DbProviderFactories
    {
        #region Private Methods

        private static Type GetTypeFromName(string typeName)
        {
            return GetTypeFromName(typeName, null);
        }

        private static IEnumerable<Assembly> GetReferencingAssemblies(string assemblyName)
        {
            var assemblies = new List<Assembly>();
            var dependencies = DependencyContext.Default.RuntimeLibraries;
            foreach (var library in dependencies)
            {
                if (IsCandidateLibrary(library, assemblyName))
                {
                    var assembly = Assembly.Load(new AssemblyName(library.Name));
                    assemblies.Add(assembly);
                }
            }
            return assemblies;
        }

        private static bool IsCandidateLibrary(RuntimeLibrary library, string assemblyName)
        {
            return library.Name == (assemblyName)
                   || library.Dependencies.Any(d => d.Name.StartsWith(assemblyName));
        }

        private static Type GetTypeFromName(string typeName, string assemblyName)
        {
            var type = Type.GetType(typeName, false);
            if (type != null)
                return type;

            var assemblies = GetReferencingAssemblies(RuntimeEnvironment.GetRuntimeIdentifier());

            // try to find manually
            foreach (var asm in assemblies)
            {
                type = asm.GetType(typeName, false);

                if (type != null)
                    break;
            }
            if (type != null)
                return type;

            // see if we can load the assembly
            if (!string.IsNullOrEmpty(assemblyName))
            {
                var a = LoadAssembly(assemblyName);
                if (a != null)
                {
                    type = Type.GetType(typeName, false);
                    if (type != null)
                        return type;
                }
            }

            return null;
        }

        private static Assembly LoadAssembly(string assemblyName)
        {
            Assembly assembly = null;
            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch { }

            if (assembly != null)
                return assembly;

            if (File.Exists(assemblyName))
            {
                assembly = Assembly.LoadFrom(assemblyName);
                if (assembly != null)
                    return assembly;
            }
            return null;
        }

        private static object GetStaticProperty(string typeName, string property)
        {
            var type = GetTypeFromName(typeName);

            return type == null ? null : GetStaticProperty(type, property);
        }

        private static object GetStaticProperty(Type type, string property)
        {
            return type.GetRuntimeProperty(property).GetValue(null, null);
        }

        private static DbProviderFactory GetFactory(string dbProviderFactoryTypename, string assemblyName)
        {
            var instance = GetStaticProperty(dbProviderFactoryTypename, "Instance");
            if (instance == null)
            {
                var a = LoadAssembly(assemblyName);
                if (a != null)
                    instance = GetStaticProperty(dbProviderFactoryTypename, "Instance");
            }

            if (instance == null)
                throw new InvalidOperationException(string.Format(Resources.UnableToRetrieveDbProviderFactoryForm, dbProviderFactoryTypename));

            return instance as DbProviderFactory;
        }

        private static DbProviderFactory GetFactory(DataAccessProviderTypes type)
        {
            if (type == DataAccessProviderTypes.SqlServer)
                return SqlClientFactory.Instance; // this library has a ref to SqlClient so this works

            if (type == DataAccessProviderTypes.SqLite)
            {
#if NETFULL
                return GetFactory("System.Data.SQLite.SQLiteFactory", "System.Data.SQLite");
#else
                return GetFactory("Microsoft.Data.Sqlite.SqliteFactory", "Microsoft.Data.Sqlite");
#endif
            }
            if (type == DataAccessProviderTypes.MySql)
                return GetFactory("MySql.Data.MySqlClient.MySqlClientFactory", "MySql.Data");
            if (type == DataAccessProviderTypes.PostgreSql)
                return GetFactory("Npgsql.NpgsqlFactory", "Npgsql");
#if NETFULL
            if (type == DataAccessProviderTypes.OleDb)
                return System.Data.OleDb.OleDbFactory.Instance;
            if (type == DataAccessProviderTypes.SqlServerCompact)
                return System.Data.Common.DbProviderFactories.GetFactory("System.Data.SqlServerCe.4.0");
#endif

            throw new NotSupportedException(string.Format(Resources.UnsupportedProviderFactory, type.ToString()));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns an instance of a DbProviderFactory.
        /// </summary>
        /// <param name="providerName">Invariant name of a provider.</param>
        /// <returns>An instance of a DbProviderFactory for a specified provider name.</returns>
        public static DbProviderFactory GetFactory(string providerName)
        {
#if NETFULL
            return System.Data.Common.DbProviderFactories.GetFactory(providerName);
#else
            var providername = providerName.ToLower();

            if (providerName == "system.data.sqlclient")
                return GetFactory(DataAccessProviderTypes.SqlServer);
            if (providerName == "system.data.sqlite" || providerName == "microsoft.data.sqlite")
                return GetFactory(DataAccessProviderTypes.SqLite);
            if (providerName == "mysql.data.mysqlclient" || providername == "mysql.data")
                return GetFactory(DataAccessProviderTypes.MySql);
            if (providerName == "npgsql")
                return GetFactory(DataAccessProviderTypes.PostgreSql);

            throw new NotSupportedException(string.Format(Resources.UnsupportedProviderFactory, providerName));
#endif
        }

        #endregion
    }

    internal enum DataAccessProviderTypes
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