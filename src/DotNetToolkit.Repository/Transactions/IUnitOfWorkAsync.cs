namespace DotNetToolkit.Repository.Transactions
{
    using Factories;
    using System;

    /// <summary>
    /// Maintains business objects in-memory which have been changed (inserted, updated, or deleted) in a single transaction.
    /// Once the transaction is completed, the changes will be sent to the database in one single asynchronous unit of work.
    /// </summary>
    /// <seealso cref="DotNetToolkit.Repository.Transactions.IUnitOfWork" />
    /// <seealso cref="DotNetToolkit.Repository.Factories.IRepositoryFactoryAsync" />
    /// <seealso cref="System.IDisposable" />
    public interface IUnitOfWorkAsync : IUnitOfWork, IRepositoryFactoryAsync, IDisposable
    {
    }
}
