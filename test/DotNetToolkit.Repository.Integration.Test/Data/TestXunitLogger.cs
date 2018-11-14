﻿namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using Configuration.Logging;
    using System;
    using System.Exception;
    using Xunit.Abstractions;

    public class TestXUnitLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TestXUnitLoggerProvider(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public ILogger Create(string categoryName)
        {
            return new TestXunitLogger(_testOutputHelper, categoryName);
        }
    }

    class TestXunitLogger : ILogger
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly string _categoryName;

        public TestXunitLogger(ITestOutputHelper testOutputHelper, string categoryName)
        {
            _testOutputHelper = testOutputHelper;
            _categoryName = categoryName;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log(LogLevel logLevel, string message)
        {
            if (!IsEnabled(logLevel))
                return;

            try
            {
                _testOutputHelper.WriteLine("[{0} {1}] [{2}]   {3}",
                    DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"),
                    logLevel.ToString(),
                    _categoryName,
                    message);
            }
            catch (Exception) { } // ignore any errors
        }
    }
}
