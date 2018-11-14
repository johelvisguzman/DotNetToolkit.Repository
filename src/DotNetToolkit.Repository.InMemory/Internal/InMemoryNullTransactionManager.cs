namespace DotNetToolkit.Repository.InMemory.Internal
{
    using Transactions;

    /// <summary>
    /// An implementation of <see cref="ITransactionManager" />.
    /// </summary>
    /// <seealso cref="ITransactionManager" />
    internal class InMemoryNullTransactionManager : ITransactionManager
    {
        internal static InMemoryNullTransactionManager Instance { get; } = new InMemoryNullTransactionManager();

        private InMemoryNullTransactionManager() { }

        public void Dispose() { }

        public void Commit() { }

        public void Rollback() { }
    }
}
