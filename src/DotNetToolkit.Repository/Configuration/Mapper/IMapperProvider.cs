namespace DotNetToolkit.Repository.Configuration.Mapper
{
    /// <summary>
    /// Represents a mapping provider.
    /// </summary>
    public interface IMapperProvider
    {
        /// <summary>
        /// Creates a mapper for the specified entity type.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <returns>The <see cref="IMapper{T}"/>.</returns>
        IMapper<T> Create<T>() where T : class;
    }
}
