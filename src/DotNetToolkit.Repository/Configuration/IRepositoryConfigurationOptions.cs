namespace DotNetToolkit.Repository.Configuration
{
    using Factories;
    using Interceptors;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a repository configuration options.
    /// </summary>
    public interface IRepositoryConfigurationOptions
    {
        /// <summary>
        /// Gets a repository context factory.
        /// </summary>
        IRepositoryContextFactory GetContextFactory();

        /// <summary>
        /// Gets the repository interceptors.
        /// </summary>
        IEnumerable<IRepositoryInterceptor> GetInterceptors();
    }
}
