namespace DotNetToolkit.Repository.Transactions
{
    internal class NullTransactionManager : ITransactionManager
    {
        private static volatile NullTransactionManager _instance;
        private static readonly object _syncRoot = new object();

        public static NullTransactionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new NullTransactionManager();
                    }
                }

                return _instance;
            }
        }

        private NullTransactionManager() { }

        public void Dispose() { }

        public TransactionStatus Status { get; private set; }

        public void Commit()
        {
            Status = TransactionStatus.Committed;
        }

        public void Rollback()
        {
            Status = TransactionStatus.Aborted;
        }
    }
}
