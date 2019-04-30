namespace DotNetToolkit.Repository.Configuration.Mapper.Internal
{
    using Conventions;
    using Extensions;
    using JetBrains.Annotations;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="IMapper{T}" />.
    /// </summary>
    internal class DefaultMapper<T> : IMapper<T> where T : class
    {
        #region Fields

        private Dictionary<string, PropertyInfo> _propertiesMapping;

        #endregion

        #region Private Methods

        private Dictionary<string, PropertyInfo> GetProperties([NotNull] IRepositoryConventions conventions)
        {
            Guard.NotNull(conventions);

            if (_propertiesMapping == null || _propertiesMapping.Count == 0)
            {
                _propertiesMapping = typeof(T).GetRuntimeProperties()
                    .Where(x => x.IsPrimitive() && conventions.IsColumnMapped(x))
                    .ToDictionary(conventions.GetColumnName, x => x);
            }

            return _propertiesMapping;
        }

        #endregion

        #region Implementation of IMapper<out T>

        /// <summary>
        /// Maps each element to a new form.
        /// </summary>
        /// <param name="reader">The data reader used for transforming each element.</param>
        /// <param name="conventions">The configurable conventions.</param>
        /// <returns>The new projected element form.</returns>
        public T Map([NotNull] IDataReader reader, [NotNull] IRepositoryConventions conventions)
        {
            Guard.NotNull(reader);
            Guard.NotNull(conventions);

            var properties = GetProperties(conventions);
            var entity = Activator.CreateInstance<T>();

            for (var i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                var value = reader[name];

                if (value == DBNull.Value)
                    value = null;

                if (properties.ContainsKey(name))
                {
                    if (!reader.IsDBNull(reader.GetOrdinal(name)))
                    {
                        var pi = properties[name];

                        pi.SetValue(entity, value);
                    }
                }
            }

            return entity;
        }

        #endregion
    }
}
