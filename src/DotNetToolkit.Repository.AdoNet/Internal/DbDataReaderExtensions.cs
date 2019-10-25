namespace DotNetToolkit.Repository.AdoNet.Internal
{
    using System;
    using System.Data.Common;

    internal static class DbDataReaderExtensions
    {
        public static T GetValue<T>(this DbDataReader reader, int ordinal)
        {
            var value = reader.GetValue(ordinal);

            if (value == null || value == DBNull.Value)
            {
                return default(T);
            }
            else
            {
                return (T)value;
            }
        }
    }
}
