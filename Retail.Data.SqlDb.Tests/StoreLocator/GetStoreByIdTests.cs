using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Data.SqlDb.EfModels;
using Retail.Data.SqlDb.StoreLocator;
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


    }
}
