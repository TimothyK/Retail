using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Data.SqlDb.EfModels;
using System;
using TimothyK.Testing;

namespace Retail.Data.SqlDb.Tests
{
    [TestClass]
    public abstract class BaseTests
    {
        private static DbContextFactory<RetailDbContext> _factory;
        protected static RetailDbContext _dbRetail;

        public static void BaseClassInitialize(TestContext testContext)
        {
            _factory = new LocalDbContextFactory<RetailDbContext>(Type.GetType(testContext.FullyQualifiedTestClassName))
                .AttachDatabase(@"Databases\Retail.mdf", @"Databases\Retail_log.ldf");
            _dbRetail = _factory.CreateContext();
        }

        public static void BaseClassCleanup()
        {
            _dbRetail?.Dispose();
            _factory?.Dispose();
        }


    }
}
