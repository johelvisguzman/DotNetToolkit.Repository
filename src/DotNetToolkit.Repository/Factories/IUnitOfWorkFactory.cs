namespace DotNetToolkit.Repository.Factories
{
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
    }
}
