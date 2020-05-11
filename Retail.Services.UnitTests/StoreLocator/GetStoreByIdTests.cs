using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Services.StoreLocator;
using Retail.Services.UnitTests.StoreLocator.TestDoubles;
using Shouldly;

namespace Retail.Services.UnitTests.StoreLocator
{
    [TestClass]
    public class GetStoreByIdTests
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
        public void StoreNotFound_ReturnsNull()
        {
            //Act
            var store = _service.GetStoreById(1);

            //Assert
            store.ShouldBeNull();
        }

        [TestMethod]
        [DataRow(1, "Edmonton")]
        [DataRow(2, "Calgary")]
        public void StoreFound_PropertiesMatch(int storeId, string storeName)
        {
            //Arrange
            _repo.AddStore(storeId, storeName);

            //Act
            var store = _service.GetStoreById(storeId);

            //Assert
            store.StoreId.ShouldBe(storeId);
            store.StoreName.ShouldBe(storeName);
        }
    }
}
