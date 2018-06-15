namespace DotNetToolkit.Repository.Transactions
{
    using System;
    using Traits;

    /// <summary>
    /// Maintains business objects in-memory which have been changed (inserted, updated, or deleted) in a single transaction.
    /// Once the transaction is completed, the changes will be sent to the database in one single unit of work.
    /// </summary>
    /// <seealso cref="DotNetToolkit.Repository.Traits.ICanQuery" />
    /// <seealso cref="DotNetToolkit.Repository.Traits.ICanAggregate" />
    /// <seealso cref="DotNetToolkit.Repository.Traits.ICanAdd" />
    /// <seealso cref="DotNetToolkit.Repository.Traits.ICanUpdate" />
    /// <seealso cref="DotNetToolkit.Repository.Traits.ICanDelete" />
    /// <seealso cref="DotNetToolkit.Repository.Traits.ICanGet" />
    /// <seealso cref="DotNetToolkit.Repository.Traits.ICanFind" />
    /// <seealso cref="System.IDisposable" />
    public interface IUnitOfWork : ICanQuery, ICanAggregate, ICanAdd, ICanUpdate, ICanDelete, ICanGet, ICanFind, IDisposable
    {
        /// <summary>
        /// Commits all changes made in this unit of work.
        /// </summary>
        void Commit();
    }
}
