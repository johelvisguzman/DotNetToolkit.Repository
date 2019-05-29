namespace DotNetToolkit.Repository.Extensions.Internal
{
    using JetBrains.Annotations;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using Utility;

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
        public static void AddParameter([NotNull] this DbCommand command, [NotNull] string name, [CanBeNull] object value)
        {
            Guard.NotNull(command, nameof(command));
            Guard.NotEmpty(name, nameof(name));

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
        public static void AddParameter([NotNull] this DbCommand command, [NotNull] string name, [CanBeNull] object[] values)
        {
            Guard.NotNull(command, nameof(command));
            Guard.NotEmpty(name, nameof(name));

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
        public static void AddParameters([NotNull] this DbCommand command, [CanBeNull] Dictionary<string, object> parameters)
        {
            Guard.NotNull(command, nameof(command));

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
