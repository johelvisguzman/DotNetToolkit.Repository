namespace DotNetToolkit.Repository.Factories
{
    using Transactions;

    /// <summary>
    /// Represents a unit of work factory.
    /// </summary>
    public interface IUnitOfWorkFactory
    {
        /// <summary>
        /// Creates a new unit of work.
        /// </summary>
        /// <returns>The new unit of work.</returns>
        IUnitOfWork Create();

        /// <summary>
        /// Creates a new repository for the specified type.
        /// </summary>
        /// <returns>The new repository.</returns>
        T CreateInstance<T>() where T : class;
    }
}
