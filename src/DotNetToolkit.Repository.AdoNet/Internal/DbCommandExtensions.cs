namespace DotNetToolkit.Repository.AdoNet.Internal
{
    using Configuration.Conventions;
    using Extensions;
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains various extensions for <see cref="DbCommand" />.
    /// </summary>
    internal static class DbCommandExtensions
    {
        internal static bool ExecuteObjectExist(this DbHelper dbHelper, DbCommand command, object obj)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var entityType = obj.GetType();
            var tableName = entityType.GetTableName();
            var primaryKeyPropertyInfo = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(entityType).First();
            var primaryKeyColumnName = primaryKeyPropertyInfo.GetColumnName();

            command.CommandText = $"SELECT * FROM [{tableName}]\nWHERE {primaryKeyColumnName} = @{primaryKeyColumnName}";
            command.CommandType = CommandType.Text;
            command.Parameters.Clear();
            command.AddParameter($"@{primaryKeyColumnName}", primaryKeyPropertyInfo.GetValue(obj, null));

            var existInDb = false;

            using (var reader = dbHelper.ExecuteReader(command))
            {
                while (reader.Read())
                {
                    existInDb = true;

                    break;
                }
            }

            return existInDb;
        }

        internal static async Task<bool> ExecuteObjectExistAsync(this DbHelper dbHelper, DbCommand command, object obj, CancellationToken cancellationToken = new CancellationToken())
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var entityType = obj.GetType();
            var tableName = entityType.GetTableName();
            var primaryKeyPropertyInfo = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(entityType).First();
            var primaryKeyColumnName = primaryKeyPropertyInfo.GetColumnName();

            command.CommandText = $"SELECT * FROM [{tableName}]\nWHERE {primaryKeyColumnName} = @{primaryKeyColumnName}";
            command.CommandType = CommandType.Text;
            command.Parameters.Clear();
            command.AddParameter($"@{primaryKeyColumnName}", primaryKeyPropertyInfo.GetValue(obj, null));

            var existInDb = false;

            using (var reader = await dbHelper.ExecuteReaderAsync(command, cancellationToken))
            {
                while (reader.Read())
                {
                    existInDb = true;

                    break;
                }
            }

            return existInDb;
        }
    }
}
