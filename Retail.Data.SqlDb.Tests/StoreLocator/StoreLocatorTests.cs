using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Data.SqlDb.StoreLocator;
using Shouldly;

namespace Retail.Data.SqlDb.Tests.StoreLocator
{
    [TestClass]
    public class StoreLocatorTests : BaseTests
    {
        private StoreRepository _repo;

        [TestInitialize]
        public void Setup()
        {
            _repo = new StoreRepository(_dbRetail);
        }

        [TestMethod]
        public void EmptyDatabase_NoStores()
        {
            //Act
            var stores = _repo.GetStores();

            //Assert
            stores.ShouldBeEmpty();
        }
    }
}
