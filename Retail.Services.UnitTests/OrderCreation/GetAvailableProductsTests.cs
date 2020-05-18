using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Retail.Data.Abstractions.OrderCreation;
using Retail.Services.CustomerServices;
using Retail.Services.OrderCreation;
using Retail.Services.StoreLocator;
using Shouldly;
using System;
using System.Collections.Generic;
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

        private void SetupCustomerDiscount(ICustomerIdentifier customer, double discount)
        {
            _repo
                .Setup(repo => repo.GetCustomerDiscount(customer))
                .Returns(discount);
        }
        
        private void SetupGetAvailableProducts(params ProductDto[] products)
        {
            _repo.Setup(repo => repo.GetAvailableProducts(_store))
                .Returns(products);
        }


        [TestMethod]
        public void VerifyCalledWithStore()
        {
            //Act            
            var products = _service.GetAvailableProducts(_order);

            //Assert
            _repo.Verify(repo => repo.GetAvailableProducts(_store));
        }

        [TestMethod]
        public void EmptyProducts_ReturnsEmpty()
        {
            //Arrange
            SetupGetAvailableProducts();

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
            SetupGetAvailableProducts(dto);

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

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public void ProductId_Set(int productId)
        {
            //Arrange
            var dto = TypicalProduct();
            dto.ProductId = productId;
            SetupGetAvailableProducts(dto);

            //Act            
            var products = _service.GetAvailableProducts(_order);

            //Assert
            var product = products.Single();
            ((IProductIdentifier)product).ProductId.ShouldBe(productId);
        }

        [TestMethod]
        [DataRow("Widget")]
        [DataRow("Thingamabob")]
        public void ProductName_Set(string productName)
        {
            //Arrange
            var dto = TypicalProduct();
            dto.ProductName = productName;
            SetupGetAvailableProducts(dto);

            //Act            
            var products = _service.GetAvailableProducts(_order);

            //Assert
            var product = products.Single();
            product.ProductName.ShouldBe(productName);
        }

        [TestMethod]
        [DataRow(9.99)]
        [DataRow(19.99)]
        public void OriginalPrice_Set(double input)
        {
            //Arrange
            var price = Convert.ToDecimal(input);
            var dto = TypicalProduct();
            dto.Price = price;
            SetupGetAvailableProducts(dto);

            //Act            
            var products = _service.GetAvailableProducts(_order);

            //Assert
            var product = products.Single();
            product.OriginalPrice.ShouldBe(price);
        }

        [TestMethod]
        [DataRow(50)]
        [DataRow(100)]
        public void Quantity_Set(int quantity)
        {
            //Arrange
            var dto = TypicalProduct();
            dto.Quantity = quantity;
            SetupGetAvailableProducts(dto);

            //Act            
            var products = _service.GetAvailableProducts(_order);

            //Assert
            var product = products.Single();
            product.Quantity.ShouldBe(quantity);
        }

        [TestMethod]
        public void NoSaleOrDiscount_DiscountPriceSameAsOriginal()
        {
            //Arrange
            var dto = TypicalProduct();
            dto.SalesPrice = dto.Price;
            SetupCustomerDiscount(_customer, 0.0);
            SetupGetAvailableProducts(dto);

            //Act            
            var products = _service.GetAvailableProducts(_order);

            //Assert
            var product = products.Single();
            product.DiscountPrice.ShouldBe(dto.Price);
        }

        [TestMethod]
        public void SaleAndNoDiscount_DiscountPriceSameAsSale()
        {
            //Arrange
            var dto = TypicalProduct();
            dto.Price = 19.99m;
            dto.SalesPrice = 15.99m;
            SetupCustomerDiscount(_customer, 0.0);
            SetupGetAvailableProducts(dto);

            //Act            
            var products = _service.GetAvailableProducts(_order);

            //Assert
            var product = products.Single();
            product.DiscountPrice.ShouldBe(dto.SalesPrice);
        }

        [TestMethod]
        public void NoSaleAndDiscount_DiscountPriceFromCustomer()
        {
            //Arrange
            var dto = TypicalProduct();
            dto.Price = 10.00m;
            dto.SalesPrice = dto.Price;
            SetupCustomerDiscount(_customer, 0.1);  //10%
            SetupGetAvailableProducts(dto);

            //Act            
            var products = _service.GetAvailableProducts(_order);

            //Assert
            var product = products.Single();
            product.DiscountPrice.ShouldBe(9.00m);  //10% off $10 is $9
        }

    }
}
