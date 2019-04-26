namespace DotNetToolkit.Repository.Configuration.Mapper.Internal
{
    using Conventions.Internal;
    using Extensions;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// An implementation of <see cref="IMapper{T}" />.
    /// </summary>
    internal class DefaultMapper<T> : IMapper<T> where T : class
    {
        #region Fields

        private readonly Dictionary<string, PropertyInfo> _propertiesMapping;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMapper{T}"/> class.
        /// </summary>
        public DefaultMapper()
        {
            _propertiesMapping = typeof(T).GetRuntimeProperties()
                .Where(x => x.IsPrimitive() && x.IsColumnMapped())
                .OrderBy(x => x.GetColumnOrder())
                .ToDictionary(x => x.GetColumnName(), x => x);
        }

        #endregion

        #region Implementation of IMapper<out T>

        /// <summary>
        /// Maps each element to a new form.
        /// </summary>
        /// <param name="reader">The data reader used for transforming each element.</param>
        /// <returns>The new projected element form.</returns>
        public T Map(IDataReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var entity = Activator.CreateInstance<T>();

            for (var i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                var value = reader[name];

                if (value == DBNull.Value)
                    value = null;

                if (_propertiesMapping.ContainsKey(name))
                {
                    if (!reader.IsDBNull(reader.GetOrdinal(name)))
                    {
                        var pi = _propertiesMapping[name];

                        pi.SetValue(entity, value);
                    }
                }
            }

            return entity;
        }

        #endregion
    }
}
