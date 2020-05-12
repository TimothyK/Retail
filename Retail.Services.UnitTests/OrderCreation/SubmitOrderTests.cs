using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Retail.Data.Abstractions.OrderCreation;
using Retail.Services.CustomerServices;
using Retail.Services.OrderCreation;
using Retail.Services.StoreLocator;
using Shouldly;
using System;
using System.Linq;

namespace Retail.Services.UnitTests.OrderCreation
{
    [TestClass]
    public class SubmitOrderTests
    {
        private Mock<IOrderRepository> _repo;
        private Customer _customer;
        private Store _store;
        private Order _order;
        private OrderCreationService _service;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<IOrderRepository>();

            _customer = new Customer();
            _store = new Store();
            _order = new Order(_customer, _store);
            CreateProducts();

            _service = new OrderCreationService(_repo.Object);
        }

        private void CreateProducts()
        {
            var apple = new ProductDto
            {
                ProductId = 1,
                ProductName = "Apple",
                Price = 1.99m,
                SalesPrice = 0.88m,
                Quantity = 10
            };
            var orange = new ProductDto
            {
                ProductId = 2,
                ProductName = "Orange",
                Price = 2.99m,
                SalesPrice = 1.59m,
                Quantity = 50
            };

            _repo.Setup(repo => repo.GetAvailableProducts(_customer, _store))
                .Returns(new[] { apple, orange });
        }

        [TestMethod]
        public void OrderWithNoLineItems_ThrowsInvalidOp()
        {
            Should.Throw<InvalidOperationException>(() => _service.SubmitOrder(_order));
        }

        [TestMethod]
        public void AddLineItem_AddsLineItem()
        {
            //Arrange
            const int quantity = 1;
            var product = _service.GetAvailableProducts(_order).First();

            //Act
            _order.AddLineItem(quantity, product);

            //Assert
            _order.LineItems.Count.ShouldBe(quantity);
            var lineItem = _order.LineItems.Single();
            lineItem.Quantity.ShouldBe(quantity); 
            lineItem.Product.ShouldBe(product);            
        }

        [TestMethod]
        public void Order_SavesOrder()
        {
            //Arrange
            var product = _service.GetAvailableProducts(_order).First();
            const int quantity = 1;
            _order.AddLineItem(quantity, product);
            OrderDto dto = null;
            _repo.Setup(repo => repo.CreateOrder(It.IsAny<OrderDto>()))
                .Callback<OrderDto>(x => dto = x);

            //Act
            _service.SubmitOrder(_order);

            //Assert
            dto.ShouldNotBeNull();
            dto.Customer.ShouldBe(_customer);
            dto.TotalPrice.ShouldBe(_order.TotalPrice);
            dto.LineItems.Count.ShouldBe(1);
            var lineItem = dto.LineItems.Single();
            lineItem.Product.ShouldBe(product);
            lineItem.Quantity.ShouldBe(quantity);
        }

        [TestMethod]
        public void Order_DecrementsQuantity()
        {
            //Arrange
            var product = _service.GetAvailableProducts(_order).First();
            const int quantity = 1;
            _order.AddLineItem(quantity, product);

            //Act
            _service.SubmitOrder(_order);

            //Assert
            _repo.Verify(repo => repo.DecrementProductInventory(product, _store, quantity));
        }
    }
}
