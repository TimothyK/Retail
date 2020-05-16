using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Data.SqlDb.EfModels;
using Retail.Data.SqlDb.StoreLocator;
using Retail.Data.SqlDb.Tests.TestRecordFactory;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using TimothyK.Data.UnitOfWork;

namespace Retail.Data.SqlDb.Tests.StoreLocator
{
    [TestClass]
    public class GetStoreByIdTests : BaseTests
    {
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

        private SqlServerUnitOfWork _unitOfWork;
        private StoreRepository _repo;

        [TestInitialize]
        public void Setup()
        {
            _unitOfWork = new SqlServerUnitOfWork(_attchedDatabase.ConnectionString);
            _repo = new StoreRepository(_unitOfWork.CreateDbContext<RetailDbContext>());
        }

        [TestCleanup]
        public void TearDown()
        {
            _unitOfWork.Dispose();  //implicit Rollback
        }

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
