using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Retail.Data.Abstractions.OrderCreation;
using Retail.Services.CustomerServices;
using Retail.Services.OrderCreation;
using Retail.Services.StoreLocator;
using Shouldly;
using System.Linq;

namespace Retail.Services.UnitTests.OrderCreation
{
    [TestClass]
    public class GetAvailableProductsTests
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

            _service = new OrderCreationService(_repo.Object);
        }

        [TestMethod]
        public void VerifyCalledWithCustomerAndStore()
        {
            //Act            
            var products = _service.GetAvailableProducts(_order);

            //Assert
            _repo.Verify(repo => repo.GetAvailableProducts(_customer, _store));
        }

        [TestMethod]
        public void EmptyProducts_ReturnsEmpty()
        {
            //Arrange
            _repo
                .Setup(repo => repo.GetAvailableProducts(_customer, _store))
                .Returns(Enumerable.Empty<ProductDto>());

            //Act            
            var products = _service.GetAvailableProducts(_order);

            //Assert
            products.ShouldBeEmpty();
        }

        [TestMethod]
        public void OneProduct_ReturnsProduct()
        {
            //Arrange
            var dto = TypicalProduct();
            _repo
                .Setup(repo => repo.GetAvailableProducts(_customer, _store))
                .Returns(new[] { dto });

            //Act            
            var products = _service.GetAvailableProducts(_order);

            //Assert
            products.ShouldNotBeEmpty();
        }

        private ProductDto TypicalProduct() =>
             new ProductDto
             {
                 ProductId = 1,
                 ProductName = "Widget",
                 Price = 19.99m,
                 SalesPrice = 9.99m,
                 Quantity = 5
             };

    }
}
