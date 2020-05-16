﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Data.SqlDb.EfModels;
using Retail.Data.SqlDb.StoreLocator;
using Retail.Data.SqlDb.Tests.TestRecordFactory;
using Shouldly;
using System.Linq;
using TimothyK.Data.UnitOfWork;

namespace Retail.Data.SqlDb.Tests.StoreLocator
{
    [TestClass]
    public class GetStoresTests : BaseTests
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
        public void EmptyDatabase_NoStores()
        {
            //Act
            var stores = _repo.GetStores();

            //Assert
            stores.ShouldBeEmpty();
        }

        [TestMethod]
        public void SingleStore_Returned()
        {
            //Arrange            
            var db = _unitOfWork.CreateDbContext<RetailDbContext>();
            var expected = db.CreateStore();
            db.SaveChanges();

            //Act
            var stores = _repo.GetStores().ToList();

            //Assert
            stores.Count.ShouldBe(1);
            var actual = stores.Single();
            actual.StoreId.ShouldBe(expected.StoreId);
            actual.StoreName.ShouldBe(expected.StoreName);                
        }

        [TestMethod]
        public void DeactiveStore_NotReturned()
        {
            //Arrange
            var db = _unitOfWork.CreateDbContext<RetailDbContext>();
            var store = db.CreateStore();
            store.Active = false;
            db.SaveChanges();

            //Act
            var stores = _repo.GetStores().ToList();

            //Assert
            stores.ShouldBeEmpty();
        }
    }
}