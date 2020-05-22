using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Data.SqlDb.Database;
using Retail.Data.SqlDb.EfModels;
using Retail.Data.SqlDb.StoreLocator;
using Retail.Data.SqlDb.TestRecordFactory;
using Shouldly;
using System;
using TimothyK.Data.UnitOfWork;

namespace Retail.Data.SqlDb.Tests.StoreLocator
{
    [TestClass]
    public class GetStoreByIdTests : BaseTests
    {
        #region Setup
        public override TestContext TestContext { get; set; }
        private UnitOfWork _unitOfWork;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            BaseClassInitialize(testContext);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            BaseClassCleanup();
        }

        private StoreRepository _repo;

        [TestInitialize]
        public void Setup()
        {
            TestInitialize();
            _unitOfWork = new InMemoryUnitOfWork();
            _repo = new StoreRepository(_unitOfWork);
        }

        [TestCleanup]
        public void TearDown()
        {
            _unitOfWork?.Dispose();  //implicit Rollback
            TestCleanup();
        }
        #endregion

        [TestMethod]
        public void NoStores_ReturnsNull()
        {
            //Act
            var store = _repo.GetStoreById(1);

            //Assert
            store.ShouldBeNull();
        }

        [TestMethod]
        public void StoreExists_StoreReturned()
        {
            //Arrange
            var db = _unitOfWork.CreateDbContext<RetailDbContext>();
            var expected = db.CreateStore();
            db.SaveChanges();

            //Act
            var actual = _repo.GetStoreById(expected.StoreId);

            //Assert
            actual.ShouldNotBeNull();
        }

        [TestMethod]
        public void WrongStoreId_Returns()
        {
            //Arrange
            var db = _unitOfWork.CreateDbContext<RetailDbContext>();
            var expected = db.CreateStore();
            db.SaveChanges();

            //Act
            var actual = _repo.GetStoreById(expected.StoreId + 1);

            //Assert
            actual.ShouldBeNull();
        }

        [TestMethod]
        public void DeactivatedStore_ReturnsNull()
        {
            //Arrange
            var db = _unitOfWork.CreateDbContext<RetailDbContext>();
            var expected = db.CreateStore();
            expected.Active = false;    //deactivated
            db.SaveChanges();

            //Act
            var actual = _repo.GetStoreById(expected.StoreId);

            //Assert
            actual.ShouldBeNull();
        }

        [TestMethod]
        public void Store_PropertiesSet()
        {
            //Arrange
            var db = _unitOfWork.CreateDbContext<RetailDbContext>();
            var expected = db.CreateStore();
            db.SaveChanges();

            //Act
            var actual = _repo.GetStoreById(expected.StoreId);

            //Assert
            actual.StoreId.ShouldBe(expected.StoreId);
            actual.StoreName.ShouldBe(expected.StoreName);
        }

    }
}
