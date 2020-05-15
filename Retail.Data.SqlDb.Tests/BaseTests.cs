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

        public static void BaseClassInitialize(TestContext testContext)
        {
            _attchedDatabase = new LocalDbAttacher(Type.GetType(testContext.FullyQualifiedTestClassName))
                .AttachDatabase(@"Databases\Retail.mdf", @"Databases\Retail_log.ldf");
        }

        public static void BaseClassCleanup()
        {
            _attchedDatabase.DropDatabase();
        }
    }
}
