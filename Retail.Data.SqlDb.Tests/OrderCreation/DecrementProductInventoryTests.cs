using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Data.Abstractions.OrderCreation;
using Retail.Data.SqlDb.Database;
using Retail.Data.SqlDb.EfModels;
using Retail.Data.SqlDb.EfModels.Models;
using Retail.Data.SqlDb.OrderCreation;
using Retail.Data.SqlDb.TestRecordFactory;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Retail.Data.SqlDb.Tests.OrderCreation
{
    [TestClass]
    public class DecrementProductInventoryTests : BaseTests
    {
        #region Setup
        public override TestContext TestContext { get; set; }
        private static RetailLocalDbAttacher _attachedDatabase;
        private RetailUnitOfWork _unitOfWork;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            BaseClassInitialize(testContext);
            _attachedDatabase = new RetailLocalDbAttacher(Type.GetType(testContext.FullyQualifiedTestClassName));
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _attachedDatabase?.DropDatabase();
            BaseClassCleanup();
        }

        private IOrderRepository _repo;

        [TestInitialize]
        public void Setup()
        {
            TestInitialize();
            _unitOfWork = new RetailUnitOfWork(_attachedDatabase);
            _repo = new OrderRepository(_unitOfWork);
        }

        [TestCleanup]
        public void TearDown()
        {
            _unitOfWork?.Dispose();  //implicit Rollback
            TestCleanup();
        }

        #endregion

        [TestMethod]
        public void DecrementProduct_QuantityIsDecremented()
        {
            //Arrange
            Store store;
            Product product;
            {
                var db = _unitOfWork.CreateDbContext<RetailDbContext>();            
                store = db.CreateStore();
                product = db.CreateProduct()
                    .AddInventory(store, 100);
                db.SaveChanges();
            }

            //Act
            _repo.DecrementProductInventory((ProductIdentifier)product, (StoreIdentifier)store, 20);

            //Assert
            {
                var db = _unitOfWork.CreateDbContext<RetailDbContext>();
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
            {
                var db = _unitOfWork.CreateDbContext<RetailDbContext>();
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
            {
                var db = _unitOfWork.CreateDbContext<RetailDbContext>();
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
