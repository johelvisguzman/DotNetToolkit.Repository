namespace DotNetToolkit.Repository.AdoNet.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;

    internal static class DbExtensions
    {
        public static void AddParmeter(this DbCommand command, string name, object value)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var parameter = command.CreateParameter();

            parameter.Value = value ?? DBNull.Value;
            parameter.ParameterName = name;

            command.Parameters.Add(parameter);
        }

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
