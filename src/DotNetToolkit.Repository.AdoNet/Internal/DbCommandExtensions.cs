namespace DotNetToolkit.Repository.AdoNet.Internal
{
    using Configuration.Conventions;
    using System;
    using System.Collections.Generic;
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
        /// <summary>
        /// Adds a new parameter with the specified name and value to the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public static void AddParameter(this DbCommand command, string name, object value)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            name = name.StartsWith("@") ? name : "@" + name;

            var parameter = command.CreateParameter();

            parameter.Value = value ?? DBNull.Value;
            parameter.ParameterName = name;

            command.Parameters.Add(parameter);
        }

        /// <summary>
        /// Adds a new parameter with the specified name and value to the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="name">The name.</param>
        /// <param name="values">The collection of values.</param>
        public static void AddParameter(this DbCommand command, string name, object[] values)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            name = name.StartsWith("@") ? name : "@" + name;

            var names = string.Join(", ", values.Select((value, i) =>
            {
                var parameterName = name + i;

                command.AddParameter(parameterName, value);

                return parameterName;
            }));

            command.CommandText = command.CommandText.Replace(name, names);

        }

        /// <summary>
        /// Adds a new collection of parameters to the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <exception cref="System.ArgumentNullException">command</exception>
        public static void AddParmeters(this DbCommand command, Dictionary<string, object> parameters)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (parameters == null || parameters.Count == 0)
                return;

            command.Parameters.Clear();

            foreach (var item in parameters)
            {
                if (item.Value is object[] values)
                    command.AddParameter(item.Key, values);
                else
                    command.AddParameter(item.Key, item.Value);
            }
        }

        internal static bool ExecuteObjectExist(this DbCommand command, object obj)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var entityType = obj.GetType();
            var tableName = entityType.GetTableName();
            var primeryKeyPropertyInfo = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(entityType).First();
            var primeryKeyColumnName = primeryKeyPropertyInfo.GetColumnName();

            command.CommandText = $"SELECT * FROM [{tableName}]\nWHERE {primeryKeyColumnName} = @{primeryKeyColumnName}";
            command.CommandType = CommandType.Text;
            command.Parameters.Clear();
            command.AddParameter($"@{primeryKeyColumnName}", primeryKeyPropertyInfo.GetValue(obj, null));

            var existInDb = false;

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    existInDb = true;

                    break;
                }
            }

            return existInDb;
        }

        internal static async Task<bool> ExecuteObjectExistAsync(this DbCommand command, object obj, CancellationToken cancellationToken = new CancellationToken())
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var entityType = obj.GetType();
            var tableName = entityType.GetTableName();
            var primeryKeyPropertyInfo = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(entityType).First();
            var primeryKeyColumnName = primeryKeyPropertyInfo.GetColumnName();

            command.CommandText = $"SELECT * FROM [{tableName}]\nWHERE {primeryKeyColumnName} = @{primeryKeyColumnName}";
            command.CommandType = CommandType.Text;
            command.Parameters.Clear();
            command.AddParameter($"@{primeryKeyColumnName}", primeryKeyPropertyInfo.GetValue(obj, null));

            var existInDb = false;

            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
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
