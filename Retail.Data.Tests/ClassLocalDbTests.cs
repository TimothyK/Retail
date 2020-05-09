using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Data.Models;
using Retail.Data.Tests.Extensions;
using Retail.Data.Tests.Helpers;
using System;
using System.Linq;

namespace Retail.Data.Tests
{
    [TestClass]
    public class ClassLocalDbTests
    {
        private static DbContextFactory<RetailDbContext> _dbFactory;
        private static RetailDbContext _dbRetail;

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            _dbFactory = new LocalDbContextFactory<RetailDbContext>(Type.GetType(context.FullyQualifiedTestClassName))           
                .AttachDatabase(@"Databases\Retail.mdf", @"Databases\Retail_log.ldf");
            _dbRetail = _dbFactory.CreateContext();
        }

        [ClassCleanup]
        public static void ClassTeardown()
        {
            _dbRetail?.Dispose();
            _dbFactory?.Dispose();
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
