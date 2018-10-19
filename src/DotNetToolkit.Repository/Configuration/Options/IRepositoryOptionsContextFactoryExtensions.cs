namespace DotNetToolkit.Repository.Configuration.Options
{
    using Factories;
    using System;

    /// <summary>
    /// An implementation of <see cref="IRepositoryOptionsExtensions" />.
    /// </summary>
    public interface IRepositoryOptionsContextFactoryExtensions : IRepositoryOptionsExtensions
    {
        /// <summary>
        /// Gets the configured repository context factory.
        /// </summary>
        IRepositoryContextFactory ContextFactory { get; }

        /// <summary>
        /// Gets the type of the repository context factory.
        /// </summary>
        Type ContextFactoryType { get; }
    }
}
