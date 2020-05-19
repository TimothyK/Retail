using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Data.Abstractions.OrderCreation;
using Retail.Data.SqlDb.EfModels;
using Retail.Data.SqlDb.EfModels.Models;
using Retail.Data.SqlDb.OrderCreation;
using Retail.Data.SqlDb.Tests.TestRecordFactory;
using Shouldly;
using System.Linq;

namespace Retail.Data.SqlDb.Tests.OrderCreation
{
    [TestClass]
    public class GetAvailableProductsTests : BaseTests
    {
        #region Setup
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

        private IOrderRepository _repo;

        [TestInitialize]
        public void Setup()
        {
            TestInitialize();
            _repo = new OrderRepository(_unitOfWork.CreateDbContext<RetailDbContext>());

            CreateStore();
        }

        private Store _store;
        private void CreateStore()
        {
            var db = _unitOfWork.CreateDbContext<RetailDbContext>();
            _store = db.CreateStore();            
            db.SaveChanges();
        }

        [TestCleanup]
        public void TearDown()
        {
            TestCleanup();
        }

        #endregion

        #region Act

        private System.Collections.Generic.IEnumerable<ProductDto> GetAvailableProducts()
        {
            return _repo.GetAvailableProducts((StoreIdentifier)_store);
        }
        
        #endregion

        [TestMethod]
        public void NoProducts_ReturnsEmpty()
        {
            //Act
            var products = GetAvailableProducts();

            //Assert
            products.ShouldBeEmpty();
        }

        [TestMethod]
        public void SingleProduct_ReturnsProduct()
        {
            //Arrange
            var db = _unitOfWork.CreateDbContext<RetailDbContext>();
            db.CreateProduct()
                .AddInventory(_store, 100);
            db.SaveChanges();

            //Act
            var products = GetAvailableProducts();

            //Assert
            products.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void SingleProduct_ProductPropertiesSet()
        {
            //Arrange
            var db = _unitOfWork.CreateDbContext<RetailDbContext>();
            const int quantity = 100;
            var expectedProduct = db.CreateProduct()
                .AddInventory(_store, quantity);
            db.SaveChanges();

            //Act
            var products = GetAvailableProducts();

            //Assert
            var actualProduct = products.Single();
            actualProduct.ProductId.ShouldBe(expectedProduct.ProductId);
            actualProduct.ProductName.ShouldBe(expectedProduct.ProductName);
            actualProduct.Price.ShouldBe(expectedProduct.Price.Value);
            actualProduct.SalesPrice.ShouldBe(expectedProduct.SalesPrice.Value);
            actualProduct.Quantity.ShouldBe(quantity);
        }

        [TestMethod]
        public void MultipleProducts_OnlyProductsAtStoreAreReturned()
        {
            //Arrange
            var db = _unitOfWork.CreateDbContext<RetailDbContext>();
            var store2 = db.CreateStore();
            var product1 = db.CreateProduct()
                .AddInventory(_store, 10)
                .AddInventory(store2, 99);
            var product2 = db.CreateProduct()
                .AddInventory(_store, 20);
            db.CreateProduct()
                .AddInventory(store2, 30);
            db.SaveChanges();

            //Act
            var products = GetAvailableProducts().ToList();

            //Assert
            products.Count.ShouldBe(2);
            products.Single(product => product.ProductId == product1.ProductId).Quantity.ShouldBe(10);
            products.Single(product => product.ProductId == product2.ProductId).Quantity.ShouldBe(20);
        }

        [TestMethod]
        public void DeactivatedProduct_NotReturned()
        {
            //Arrange
            var db = _unitOfWork.CreateDbContext<RetailDbContext>();
            const int quantity = 100;
            var product = db.CreateProduct()
                .AddInventory(_store, quantity);
            product.Active = false;     //Deactivate
            db.SaveChanges();

            //Act
            var products = GetAvailableProducts();

            //Assert
            products.ShouldBeEmpty();
        }

        [TestMethod]
        public void ZeroQuantity_NotReturned()
        {
            //Arrange
            var db = _unitOfWork.CreateDbContext<RetailDbContext>();
            const int quantity = 0;     //No quantity
            var product = db.CreateProduct()
                .AddInventory(_store, quantity);
            db.SaveChanges();

            //Act
            var products = GetAvailableProducts();

            //Assert
            products.ShouldBeEmpty();
        }

    }
}
