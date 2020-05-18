using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TimothyK.Data.UnitOfWork;

[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.ClassLevel)]

namespace Retail.Data.SqlDb.Tests
{
    [TestClass]
    public abstract class BaseTests
    {
        protected static LocalDbAttacher _attchedDatabase;

        protected static void BaseClassInitialize(TestContext testContext)
        {
            _attchedDatabase = new LocalDbAttacher(Type.GetType(testContext.FullyQualifiedTestClassName))
                .AttachDatabase(@"Databases\Retail.mdf", @"Databases\Retail_log.ldf");
        }

        protected static void BaseClassCleanup()
        {
            _attchedDatabase.DropDatabase();
        }

        protected UnitOfWork _unitOfWork;

        protected void TestInitialize()
        {
            _unitOfWork = new SqlServerUnitOfWork(_attchedDatabase.ConnectionString);
        }

        protected void TestCleanup()
        {
            _unitOfWork.Dispose(); //implicit Rollback
        }
    }
}
