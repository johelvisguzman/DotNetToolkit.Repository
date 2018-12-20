namespace DotNetToolkit.Repository.Transactions
{
    /// <summary>
    /// Represents a status for the transaction manager.
    /// </summary>
    public enum TransactionStatus
    {
        /// <summary>
        /// The transaction is active/unknown.
        /// </summary>
        Active = 0,
        /// <summary>
        /// The transaction has been committed.
        /// </summary>
        Committed = 1,
        /// <summary>
        /// The transaction has been rolled back.
        /// </summary>
        Aborted = 2
    }
}
