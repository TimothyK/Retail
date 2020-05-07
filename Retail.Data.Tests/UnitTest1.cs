using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Data.Models;
using System;
using System.Linq;

namespace Retail.Data.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = @"(localdb)\mssqllocaldb",
                IntegratedSecurity = true,
                InitialCatalog = "Retail"
            };
            var contextBuilder = new DbContextOptionsBuilder<RetailContext>();           ;            
            contextBuilder
                .UseSqlServer(connectionStringBuilder.ConnectionString);

            var dbRetail = new RetailContext(contextBuilder.Options);

            //Add a product
            var product = new Product() 
            { 
                ProductName = $"Product {Guid.NewGuid()}"
            };
            dbRetail.Add(product);

            dbRetail.SaveChanges();


            //Purge all products
            var products = dbRetail.Products.ToList();
            foreach (var p in products)
                dbRetail.Remove(p);

            dbRetail.SaveChanges();

        }
    }
}
