using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Data.Models;
using System;
using System.Linq;

namespace Retail.Data.Tests
{
    [TestClass]
    public class InMemoryDbTests
    {
        public static RetailContext _dbRetail;

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            var optionBuilder = new DbContextOptionsBuilder<RetailContext>()
                .UseInMemoryDatabase(context.FullyQualifiedTestClassName);
            _dbRetail = new RetailContext(optionBuilder.Options);
        }

        [ClassCleanup]
        public static void ClassTeardown()
        {
            _dbRetail.Dispose();
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
