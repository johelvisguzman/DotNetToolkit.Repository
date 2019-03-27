namespace DotNetToolkit.Repository.Wpf.Demo.Infrastructure.Logging
{
    using Configuration.Logging;

    public class Log4NetLoggingProvider : ILoggerProvider
    {
        public ILogger Create(string categoryName)
        {
            return new Log4NetLogger(log4net.LogManager.GetLogger(categoryName));
        }
    }
}
