using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Data.SqlDb.Database;
using Serilog;
using Serilog.Context;
using System;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using TimothyK.Data.UnitOfWork;

//[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.ClassLevel)]

namespace Retail.Data.SqlDb.Tests
{
    [TestClass]
    public abstract class BaseTests
    {
        internal static ILoggerFactory LoggerFactory { get; }
        private static readonly Guid _testRunId;
        private static readonly string _seqAddress;

        static BaseTests()
        {
            _seqAddress = ServiceController.GetServices().Any(service => service.ServiceName == "Seq")
                ? "http://localhost:5341"
                : string.Empty;

            _testRunId = Guid.NewGuid();
            
            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.WithProperty("TestRunId", _testRunId)
                .Enrich.WithThreadId()
                .Enrich.FromLogContext()
                .WriteTo.Console();
            if (!string.IsNullOrEmpty(_seqAddress))
                loggerConfig.WriteTo.Seq("http://localhost:5341");
            Log.Logger = loggerConfig.CreateLogger();

            LoggerFactory = new LoggerFactory()
                .AddSerilog();            
        }

        public abstract TestContext TestContext { get; set; }
        private static IDisposable _testClassContext;
        private IDisposable _testMethodContext;

        protected static void BaseClassInitialize(TestContext testContext)
        {
            _testClassContext = LogContext.PushProperty("TestClass", testContext.FullyQualifiedTestClassName);
        }

        protected static void BaseClassCleanup()
        {
            _testClassContext?.Dispose();
        }

        protected void TestInitialize()
        {
            _testMethodContext = LogContext.PushProperty("TestMethod", TestContext.TestName);
            using (LogContext.PushProperty("StackTrace", new System.Diagnostics.StackTrace()))
            {
                if (string.IsNullOrEmpty(_seqAddress))
                    Log.Information("Install Seq for enhanced logging.  https://datalust.co/download");
                else
                    Log.Information("TestInitialize - Details at {Url}", $"{_seqAddress}/#/events?filter=" + WebUtility.UrlEncode($"TestRunId = '{_testRunId}' && TestClass = '{TestContext.FullyQualifiedTestClassName}' && TestMethod = '{TestContext.TestName}'"));
            }
        }

        protected void TestCleanup()
        {
            _testMethodContext?.Dispose();
        }
    }
}
