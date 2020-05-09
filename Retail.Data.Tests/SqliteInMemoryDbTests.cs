using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Data.Models;
using Retail.Data.Tests.Helpers;
using System;
using System.Linq;

namespace Retail.Data.Tests
{
    [TestClass]
    public class SqliteInMemoryDbTests
    {
        private static RetailDbContext _dbRetail;
        private static SqliteDbContextFactory<RetailDbContext> _factory;

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            _factory = new SqliteDbContextFactory<RetailDbContext>(Type.GetType(context.FullyQualifiedTestClassName));
            _dbRetail = _factory.CreateContext();
        }

        [ClassCleanup]
        public static void ClassTeardown()
        {
            _dbRetail?.Dispose();
            _factory?.Dispose();
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
