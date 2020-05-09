using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Data.Tests.Extensions;
using System;

namespace Retail.Data.Tests
{
    [TestClass]
    public class ClassLocalDbTests : BaseTests
    {

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            ContextFactory = new LocalDbContextFactory<RetailDbContext>(Type.GetType(context.FullyQualifiedTestClassName))           
                .AttachDatabase(@"Databases\Retail.mdf", @"Databases\Retail_log.ldf");
            _dbRetail = ContextFactory.CreateContext();
        }

        [ClassCleanup]
        public static void ClassTeardown()
        {
            _dbRetail?.Dispose();
            ContextFactory?.Dispose();
        }

    }


}
