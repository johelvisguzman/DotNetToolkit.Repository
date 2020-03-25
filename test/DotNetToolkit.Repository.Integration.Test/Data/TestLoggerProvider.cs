namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using Configuration.Logging;

    public class TestLoggerProvider : ILoggerProvider
    {
        public ILogger Create(string categoryName) => new TestLogger();
    }

    public class TestLogger : ILogger
    {
        public bool IsEnabled(LogLevel logLevel) => false;
        public void Log(LogLevel logLevel, string message) { }
    }
}
