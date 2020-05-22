using Microsoft.EntityFrameworkCore;
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
using TimothyK.Data;
using TimothyK.Data.UnitOfWork;

namespace Retail.Data.SqlDb.Tests.OrderCreation
{
    [TestClass]
    public class CreateOrderTests : BaseTests
    {
        #region Setup
        public override TestContext TestContext { get; set; }
        private static LocalDbAttacher _attachedDatabase;
        private UnitOfWork _unitOfWork;


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

            CreateTestData();
        }

        private Store _store;
        private Customer _customer;
        private Product _product;

        private void CreateTestData()
        {
            var db = _unitOfWork.CreateDbContext<RetailDbContext>();
            _store = db.CreateStore();
            _customer = db.CreateCustomer();
            _product = db.CreateProduct();
            db.SaveChanges();
        }

        [TestCleanup]
        public void TearDown()
        {
            _unitOfWork?.Dispose();  //implicit Rollback
            TestCleanup();
        }

        #endregion

        [TestMethod]
        public void SimpleOrder_Created()
        {
            //Arrange
            var dto = CreateOrderDto();

            //Act
            _repo.CreateOrder(dto);

            //Assert
            var db = _unitOfWork.CreateDbContext<RetailDbContext>();
            var order = db.Orders
                .Include(o => o.OrderLineItems)
                .Where(o => o.CustomerId == dto.Customer.CustomerId)
                .OrderByDescending(o => o.OrderId)
                .LastOrDefault();
            order.ShouldNotBeNull();

            order.CustomerId.ShouldBe(dto.Customer.CustomerId);
            order.StoreId.ShouldBe(dto.Store.StoreId);
            order.ShippingAddress.ShouldBe(dto.ShippingAddress);
            order.ShippingCity.ShouldBe(dto.ShippingCity);
            order.ShippingProvince.ShouldBe(dto.ShippingProvince);
            order.ShippingCountry.ShouldBe(dto.ShippingCountry);
            order.ShippingPostalCode.ShouldBe(dto.ShippingPostalCode);
            order.CustomerPhoneNumber.ShouldBe(dto.CustomerPhoneNumber);
            order.TotalPrice.ShouldBe(dto.TotalPrice);

            var expectedLineItem = dto.LineItems.Single(); 
            var actualLineItem = order.OrderLineItems.Single();
            actualLineItem.ProductId.ShouldBe(expectedLineItem.Product.ProductId);
            actualLineItem.UnitPrice.ShouldBe(expectedLineItem.UnitPrice);
            actualLineItem.Quantity.ShouldBe(expectedLineItem.Quantity);
            actualLineItem.TotalPrice.ShouldBe(expectedLineItem.TotalPrice);
        }

        private OrderDto CreateOrderDto()
        {
            var id = IdFactory.Next();
            decimal unitPrice = id + 0.99m;
            var order = new OrderDto
            {
                Customer = (CustomerIdentifier)_customer,
                Store = (StoreIdentifier)_store,
                ShippingAddress = $"{id} Main Street",
                ShippingCity = $"{id}ville",
                ShippingProvince = $"Distinct {id}",
                ShippingCountry = $"{id}land",
                ShippingPostalCode = $"zip{id}",
                CustomerPhoneNumber = $"({id}) {id}-{id}",
                TotalPrice = 3 * unitPrice,
                LineItems = new List<OrderLineItemDto>() { 
                    new OrderLineItemDto {
                        Product = (ProductIdentifier) _product,
                        UnitPrice = unitPrice,
                        Quantity = 2,
                        TotalPrice = 2 * unitPrice
                    } 
                }
            };

            return order;
        }
    }
}
