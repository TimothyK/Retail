using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Data.Models;
using System;
using System.Linq;

namespace Retail.Data.Tests
{
    [TestClass]
    public class AssemblyLocalDbTests : BaseTests
    {
        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            _dbRetail = Databases.AttachAssemblyDatabase.LocalDbFactory.CreateContext();
        }

    }


}
