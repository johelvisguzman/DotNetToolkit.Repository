namespace DotNetToolkit.Repository.AdoNet
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;

    /// <summary>
    /// Contains various extensions for <see cref="DbCommand" />.
    /// </summary>
    public static class DbCommandExtensions
    {
        /// <summary>
        /// Adds a new parameter with the specified name and value to the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public static void AddParmeter(this DbCommand command, string name, object value)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var parameter = command.CreateParameter();

            parameter.Value = value ?? DBNull.Value;
            parameter.ParameterName = name;

            command.Parameters.Add(parameter);
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
                command.AddParmeter(item.Key, item.Value);
            }
        }
    }
}
