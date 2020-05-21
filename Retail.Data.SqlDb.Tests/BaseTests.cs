using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        private static readonly ILoggerFactory _loggerFactory;
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

            _loggerFactory = new LoggerFactory()
                .AddSerilog();            
        }

        protected static LocalDbAttacher _attchedDatabase;

        public abstract TestContext TestContext { get; set; }
        private static IDisposable _testClassContext;
        private static IDisposable _testMethodContext;

        protected static void BaseClassInitialize(TestContext testContext)
        {
            _attchedDatabase = new LocalDbAttacher(Type.GetType(testContext.FullyQualifiedTestClassName))
                .AttachDatabase(@"Databases\Retail.mdf", @"Databases\Retail_log.ldf");

            _testClassContext = LogContext.PushProperty("TestClass", testContext.FullyQualifiedTestClassName);
        }

        protected static void BaseClassCleanup()
        {
            _testClassContext?.Dispose();
            _attchedDatabase?.DropDatabase();
        }

        protected UnitOfWork _unitOfWork;

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

            _unitOfWork = new SqlServerUnitOfWork(_attchedDatabase.ConnectionString)
                .AddBuilderOptions(builder => builder.UseLoggerFactory(_loggerFactory))
                .AddBuilderOptions(builder => builder.EnableSensitiveDataLogging());
        }

        protected void TestCleanup()
        {
            _testMethodContext?.Dispose();
            _unitOfWork.Dispose(); //implicit Rollback
        }
    }
}
