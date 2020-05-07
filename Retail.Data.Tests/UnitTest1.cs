using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Data.Models;
using Retail.Data.Tests.Extensions;
using Shouldly;
using System;
using System.Linq;

namespace Retail.Data.Tests
{
    [TestClass]
    public class UnitTest1
    {
        public static RetailContext _dbRetail;

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            var localDb = new LocalDbContext(typeof(UnitTest1));
            
            localDb.AttachDatabase(@"Databases\Retail.mdf", @"Databases\Retail_log.ldf");

            _dbRetail = new RetailContext(localDb.GetDbConnectionOptions<RetailContext>());
        }

        [ClassCleanup]
        public static void ClassTeardown()
        {
            var localDb = new LocalDbContext(typeof(UnitTest1));

            localDb.DropDatabase();
        }


        [TestMethod]
        public void InitializationTest()
        {

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
