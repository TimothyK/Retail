using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Data.Abstractions.OrderCreation;
using Retail.Data.SqlDb.EfModels;
using Retail.Data.SqlDb.EfModels.Models;
using Retail.Data.SqlDb.OrderCreation;
using Retail.Data.SqlDb.Tests.TestRecordFactory;
using Shouldly;
using System.Collections.Generic;
using System.Linq;

namespace Retail.Data.SqlDb.Tests.OrderCreation
{
    [TestClass]
    public class DecrementProductInventoryTests : BaseTests
    {
        #region Setup
        public override TestContext TestContext { get; set; }

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
        }

        [TestCleanup]
        public void TearDown()
        {
            TestCleanup();
        }

        #endregion

        [TestMethod]
        public void DecrementProduct_QuantityIsDecremented()
        {
            //Arrange
            Store store;
            Product product;
            using (var db = _unitOfWork.CreateDbContext<RetailDbContext>()) 
            { 
                store = db.CreateStore();
                product = db.CreateProduct()
                    .AddInventory(store, 100);
                db.SaveChanges();
            }

            //Act
            _repo.DecrementProductInventory((ProductIdentifier)product, (StoreIdentifier)store, 20);

            //Assert
            using (var db = _unitOfWork.CreateDbContext<RetailDbContext>())
            {
                var inventory = db.Inventories
                    .Single(inventory => inventory.ProductId == product.ProductId && inventory.StoreId == store.StoreId);
                inventory.Quantity.ShouldBe(80);
            }

        }

        [TestMethod]
        public void MultipleInventories_CorrectInventoryIsDecremented()
        {
            //Arrange
            Store store1;
            Store store2;
            Product product1;
            Product product2;
            using (var db = _unitOfWork.CreateDbContext<RetailDbContext>())
            {
                store1 = db.CreateStore();
                store2 = db.CreateStore();
                product1 = db.CreateProduct()
                    .AddInventory(store1, 100)
                    .AddInventory(store2, 200);
                product2 = db.CreateProduct()
                    .AddInventory(store1, 300)
                    .AddInventory(store2, 400); 
                db.SaveChanges();
            }

            //Act
            _repo.DecrementProductInventory((ProductIdentifier)product2, (StoreIdentifier)store2, 20);

            //Assert
            using (var db = _unitOfWork.CreateDbContext<RetailDbContext>())
            {
                var storeIds = new[] { store1, store2 }.Select(store => store.StoreId);
                var inventories = db.Inventories
                    .Where(inventory => storeIds.Contains(inventory.StoreId))
                    .ToList();
                inventories
                    .QuantityShouldBe(product1, store1, 100)
                    .QuantityShouldBe(product1, store2, 200)
                    .QuantityShouldBe(product2, store1, 300)
                    .QuantityShouldBe(product2, store2, 380);

            }

        }

    }

    internal static class ProductInventoryAssertions
    {
        public static List<Inventory> QuantityShouldBe(this List<Inventory> inventories, Product product, Store store, int expectedQuantity)
        {
            var inventory = inventories
                .SingleOrDefault(inventory => 
                    inventory.ProductId == product.ProductId 
                    && inventory.StoreId == store.StoreId);

            if (inventory == null)
                Assert.Fail($"Inventory record for ProductId {product.ProductId} and StoreId {store.StoreId} was not found");

            inventory.Quantity.ShouldBe(expectedQuantity, $"Quantity for ProductId {product.ProductId} and StoreId {store.StoreId} is incorrect");

            return inventories;
        }
    }
}
