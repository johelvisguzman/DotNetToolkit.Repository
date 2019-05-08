namespace DotNetToolkit.Repository.NHibernate.Internal
{
    using global::NHibernate;
    using global::NHibernate.SqlCommand;
    using System;
    using Utility;

    internal class LoggerInterceptor : EmptyInterceptor, IInterceptor
    {
        private readonly Action<string> _logCallback;

        public LoggerInterceptor(Action<string> logCallback)
        {
            _logCallback = Guard.NotNull(logCallback, nameof(logCallback));
        }

        public override SqlString OnPrepareStatement(SqlString sql)
        {
            _logCallback(sql.ToString());

            return base.OnPrepareStatement(sql);
        }
    }
}
