using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TimothyK.Testing;

namespace Retail.Data.SqlDb.Tests
{
    [TestClass]
    public abstract class BaseTests
    {
        private DbContextFactory<RetailDbContext> _factory;
        protected RetailDbContext _dbRetail;

        [ClassInitialize]
        public void BaseClassSetup(TestContext testContext)
        {
            _factory = new LocalDbContextFactory<RetailDbContext>(Type.GetType(testContext.FullyQualifiedTestClassName))
                .AttachDatabase(@"Databases\Retail.mdf", @"Databases\Retail_log.ldf");
            _dbRetail = _factory.CreateContext();
        }

        [ClassCleanup]
        public void BaseClassTearDown()
        {
            _dbRetail?.Dispose();
            _factory?.Dispose();
        }


    }
}
