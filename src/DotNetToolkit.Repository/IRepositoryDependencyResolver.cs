namespace DotNetToolkit.Repository
{
    using System;

    /// <summary>
    /// Represents a dependency resolver.
    /// </summary>
    public interface IRepositoryDependencyResolver
    {
        /// <summary>
        /// Gets an instance of the requested type with the given name from the container.
        /// </summary>
        /// <typeparam name="T">The type of the object to resolve.</typeparam>
        /// <returns>The resolved object.</returns>
        T Resolve<T>();

        /// <summary>
        /// Gets an instance of the requested type with the given name from the container.
        /// </summary>
        /// <param name="type">The type of the object to resolve.</param>
        /// <returns>The resolved object.</returns>
        object Resolve(Type type);
    }
}
