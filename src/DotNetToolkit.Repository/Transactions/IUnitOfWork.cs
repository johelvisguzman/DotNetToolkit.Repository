namespace DotNetToolkit.Repository.Transactions
{
    using Factories;
    using System;

    /// <summary>
    /// Maintains business objects in-memory which have been changed (inserted, updated, or deleted) in a single transaction.
    /// Once the transaction is completed, the changes will be sent to the database in one single unit of work.
    /// </summary>
    /// <seealso cref="DotNetToolkit.Repository.Factories.IRepositoryFactory" />
    /// <seealso cref="System.IDisposable" />
    public interface IUnitOfWork : IRepositoryFactory, IDisposable
    {
        /// <summary>
        /// Commits all changes made in this unit of work.
        /// </summary>
        void Commit();
    }
}
