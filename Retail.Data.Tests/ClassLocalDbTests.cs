using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Data.Models;
using Retail.Data.Tests.Extensions;
using System;
using System.Linq;

namespace Retail.Data.Tests
{
    [TestClass]
    public class ClassLocalDbTests
    {
        public static LocalDbTestContext _localDbTestContext;
        public static RetailContext _dbRetail;

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            _localDbTestContext = new LocalDbTestContext(Type.GetType(context.FullyQualifiedTestClassName));            
            _localDbTestContext.AttachDatabase(@"Databases\Retail.mdf", @"Databases\Retail_log.ldf");
            _dbRetail = new RetailContext(_localDbTestContext.GetDbConnectionOptions<RetailContext>());
        }

        [ClassCleanup]
        public static void ClassTeardown()
        {
            _localDbTestContext.DropDatabase();
        }

        [TestMethod]
        public void SimpleDbTest()
        {
            //Add a product
            var product = new Product() 
            { 
                ProductName = $"Product {Guid.NewGuid()}"
            };
            _dbRetail.Add(product);
            _dbRetail.SaveChanges();

            //Purge all products
            var products = _dbRetail.Products.ToList();
            foreach (var p in products)
                _dbRetail.Remove(p);
            _dbRetail.SaveChanges();
        }
    }


}
