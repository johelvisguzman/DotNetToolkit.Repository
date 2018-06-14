namespace DotNetToolkit.Repository.Factories
{
    using Transactions;

    /// <summary>
    /// Represents an asynchronous unit of work factory.
    /// </summary>
    /// <seealso cref="DotNetToolkit.Repository.Factories.IUnitOfWorkFactory" />
    public interface IUnitOfWorkFactoryAsync : IUnitOfWorkFactory
    {
        /// <summary>
        /// Creates a new asynchronous unit of work.
        /// </summary>
        /// <returns>The new asynchronous unit of work.</returns>
        IUnitOfWorkAsync CreateAsync();
    }
}
