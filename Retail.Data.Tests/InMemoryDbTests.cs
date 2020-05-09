﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Data.Models;
using Retail.Data.Tests.Helpers;
using System;
using System.Linq;

namespace Retail.Data.Tests
{
    [TestClass]
    public class InMemoryDbTests : BaseTests
    {

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            ContextFactory = new InMemoryDbContextFactory<RetailDbContext>(Type.GetType(context.FullyQualifiedTestClassName));
            _dbRetail = ContextFactory.CreateContext();
        }

        [ClassCleanup]
        public static void ClassTeardown()
        {
            _dbRetail.Dispose();
            ContextFactory.Dispose();
        }
    }


}
