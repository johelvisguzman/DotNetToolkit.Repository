namespace DotNetToolkit.Repository.AdoNet.Internal
{
    using Configuration.Conventions;
    using Extensions;
    using Extensions.Internal;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Utility;

    /// <summary>
    /// Contains various extensions for <see cref="DbCommand" />.
    /// </summary>
    internal static class DbCommandExtensions
    {
        internal static bool ExecuteObjectExist(this DbHelper dbHelper, IRepositoryConventions conventions, DbCommand command, object obj)
        {
            Guard.NotNull(dbHelper, nameof(dbHelper));
            Guard.NotNull(conventions, nameof(conventions));
            Guard.NotNull(command, nameof(command));
            Guard.NotNull(obj, nameof(obj));

            var entityType = obj.GetType();
            var tableName = conventions.GetTableName(entityType);
            var primaryKeyPropertyInfo = conventions.GetPrimaryKeyPropertyInfos(entityType).First();
            var primaryKeyColumnName = conventions.GetColumnName(primaryKeyPropertyInfo);

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

        internal static async Task<bool> ExecuteObjectExistAsync(this DbHelper dbHelper, IRepositoryConventions conventions, DbCommand command, object obj, CancellationToken cancellationToken = new CancellationToken())
        {
            Guard.NotNull(dbHelper, nameof(dbHelper));
            Guard.NotNull(conventions, nameof(conventions));
            Guard.NotNull(command, nameof(command));
            Guard.NotNull(obj, nameof(obj));

            var entityType = obj.GetType();
            var tableName = conventions.GetTableName(entityType);
            var primaryKeyPropertyInfo = conventions.GetPrimaryKeyPropertyInfos(entityType).First();
            var primaryKeyColumnName = conventions.GetColumnName(primaryKeyPropertyInfo);

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
