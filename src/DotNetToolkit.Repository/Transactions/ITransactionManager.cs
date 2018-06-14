namespace DotNetToolkit.Repository.Transactions
{
    using System;

    /// <summary>
    /// Provides transactional support for a unit of work.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface ITransactionManager : IDisposable
    {
        /// <summary>
        /// Commits all changes made to the database in the current transaction.
        /// </summary>
        void Commit();

        /// <summary>
        /// Discards all changes made to the database in the current transaction.
        /// </summary>
        void Rollback();
    }
}
