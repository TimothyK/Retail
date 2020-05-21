using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Data.SqlDb.CustomerServices;
using Retail.Data.SqlDb.EfModels;
using Retail.Data.SqlDb.OrderCreation;
using Retail.Data.SqlDb.StoreLocator;
using Retail.Data.SqlDb.TestRecordFactory;
using Retail.Services.CustomerServices;
using Retail.Services.OrderCreation;
using Retail.Services.StoreLocator;
using Shouldly;
using System;
using System.Linq;
using TimothyK.Data.UnitOfWork;

namespace Retail.Services.IntegrationTests
{
    [TestClass]
    public class IntegrationTests
    {
        private static LocalDbAttacher _attachedDatabase;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            _attachedDatabase = new LocalDbAttacher(Type.GetType(testContext.FullyQualifiedTestClassName))
                .AttachDatabase(@"Databases\Retail.mdf", @"Databases\Retail_log.ldf");
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _attachedDatabase?.DropDatabase();
        }

        private UnitOfWork _unitOfWork;

        [TestInitialize]
        public void Setup()
        {
            _unitOfWork = new SqlServerUnitOfWork(_attachedDatabase.ConnectionString);
        }

        [TestCleanup]
        public void Teardown()
        {
            _unitOfWork?.Dispose();
        }

        [TestMethod]
        public void FullWalkthrough()
        {
            const decimal price = 100.00m;
            const double customerDiscount = 0.1;
            const int startQuantity = 100;
            const int purchaseQuantity = 20;
            const int expectedFinalQuantity = startQuantity - purchaseQuantity;
            decimal expectedTotalPrice = purchaseQuantity * (price - (price * Convert.ToDecimal(customerDiscount)));

            int storeId;
            Guid membershipNumber;
            int productId;
            {//Arrange
                var db = _unitOfWork.CreateDbContext<RetailDbContext>();
            
                var customer = db.CreateCustomer()
                    .With(customer => customer.Discount = customerDiscount);
                var store = db.CreateStore();
                var product = db.CreateProduct()
                    .With(product => product.Price = price)
                    .With(product => product.SalesPrice = null)
                    .AddInventory(store, startQuantity);
                db.SaveChanges();
                membershipNumber = customer.MembershipNumber;
                storeId = store.StoreId;
                productId = product.ProductId;
            }

            {//Act
                var db = _unitOfWork.CreateDbContext<RetailDbContext>();

                var storeLocator = new StoreLocatorService(new StoreRepository(db));
                var customerService = new CustomerService(new CustomerRepository(db));
                var orderService = new OrderCreationService(new OrderRepository(db));

                var store = storeLocator.GetStoreById(storeId);
                var customer = customerService.GetCustomerByMembershipNumber(membershipNumber);

                var order = orderService.CreateOrder(customer, store);
                var products = orderService.GetAvailableProducts(order);
                foreach (var product in products)
                    order.AddLineItem(purchaseQuantity, product);

                orderService.SubmitOrder(order);
            }

            {//Assert
                var db = _unitOfWork.CreateDbContext<RetailDbContext>();

                var inventory = db.Inventories
                    .Single(inventory => inventory.ProductId == productId && inventory.StoreId == storeId);
                inventory.Quantity.ShouldBe(expectedFinalQuantity);

                db.Orders
                    .Single(order => order.StoreId == storeId)
                    .TotalPrice.ShouldBe(expectedTotalPrice);
            }
        }
    }
}
