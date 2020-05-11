using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Services.StoreLocator;
using Retail.Services.UnitTests.StoreLocator.TestDoubles;
using Shouldly;
using System.Linq;

namespace Retail.Services.UnitTests.StoreLocator
{
    [TestClass]
    public class GetStoresTests
    {
        private StoreRepository _repo;
        private StoreLocatorService _service;

        [TestInitialize]
        public void Setup()
        {
            _repo = new StoreRepository();
            _service = new StoreLocatorService(_repo);
        }

        [TestMethod]
        public void NoStores_ReturnsEmpty()
        {
            _service.GetStores().ShouldBeEmpty();
        }

        [TestMethod]
        public void OneStore_ReturnsStore()
        {
            //Arrange
            _repo.AddStore(1, "Edmonton");

            //Act
            var stores = _service.GetStores().ToList();

            //Assert
            stores.Count.ShouldBe(1);
        }

        [TestMethod]
        [DataRow(1, "Edmonton")]
        [DataRow(2, "Calgary")]
        public void OneStore_PropertiesAreSet(int storeId, string storeName)
        {
            //Arrange
            _repo.AddStore(storeId, storeName);

            //Act
            var stores = _service.GetStores().ToList();

            //Assert
            var store = stores.Single();
            store.StoreId.ShouldBe(storeId);
            store.StoreName.ShouldBe(storeName);
        }

        [TestMethod]
        public void TwoStores_BothReturned()
        {
            //Arrange
            _repo.AddStore(1, "Edmonton");
            _repo.AddStore(2, "Calgary");

            //Act
            var stores = _service.GetStores().ToList();

            //Assert
            stores.Count.ShouldBe(2);
            stores.Any(store => store.StoreId == 1).ShouldBeTrue();
            stores.Any(store => store.StoreId == 2).ShouldBeTrue();
        }

    }
}
