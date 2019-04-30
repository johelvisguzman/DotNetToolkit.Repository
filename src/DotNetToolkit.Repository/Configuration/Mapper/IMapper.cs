namespace DotNetToolkit.Repository.Configuration.Mapper
{
    using Conventions;
    using System.Data;

    /// <summary>
    /// Represents a mapper which takes a data reader returned by an executed SQL query and maps it into new entity form.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public interface IMapper<out T> where T : class
    {
        /// <summary>
        /// Maps each element to a new form.
        /// </summary>
        /// <param name="reader">The data reader used for transforming each element.</param>
        /// <param name="conventions">The configurable conventions.</param>
        /// <returns>The new projected element form.</returns>
        T Map(IDataReader reader, IRepositoryConventions conventions);
    }
}
