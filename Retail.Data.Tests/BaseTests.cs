using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Data.Models;
using Retail.Data.Tests.Helpers;
using System;
using System.Linq;

namespace Retail.Data.Tests
{
    /// <summary>
    /// A base set of database tests that will be run under different contexts
    /// </summary>
    /// <remarks>
    /// <para>
    /// This project is meant to contain many different examples of how one would normally write automated tests against a database.
    /// However, using a base class here wouldn't be the normal way of writing tests.
    /// It is done this way in this example project just to prove that the same tests can run against multiple type of test databases.
    /// Each subclass creates the test database in a different way.
    /// </para>
    /// <para>
    /// In an actual project each test class would not be inherited and create its own test database.
    /// Initially the test class might use an In Memory database.
    /// If there is production code to test that requires SQL then the In Memory test database may need to be swap out a 
    /// Sqlite or LocalDb database.
    /// Automated tests should be written so that change would be easy.  See the sub classes to see how easy that is.
    /// </para>
    /// </remarks>
    public class BaseTests
    {
        protected static DbContextFactory<RetailDbContext> ContextFactory;
        protected static RetailDbContext _dbRetail;

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
