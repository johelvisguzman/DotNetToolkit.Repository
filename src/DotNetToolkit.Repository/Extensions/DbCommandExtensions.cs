namespace DotNetToolkit.Repository.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;

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
        public static void AddParameters(this DbCommand command, Dictionary<string, object> parameters)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (parameters == null || parameters.Count == 0)
                return;

            foreach (var item in parameters)
            {
                if (item.Value is object[] values)
                    command.AddParameter(item.Key, values);
                else
                    command.AddParameter(item.Key, item.Value);
            }
        }
    }
}
