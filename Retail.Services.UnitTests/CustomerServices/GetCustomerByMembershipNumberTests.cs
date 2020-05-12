using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Retail.Data.Abstractions.CustomerServices;
using Retail.Data.Abstractions.OrderCreation;
using Retail.Services.CustomerServices;
using Shouldly;
using System;

namespace Retail.Services.UnitTests.CustomerServices
{
    [TestClass]
    public class GetCustomerByMembershipNumberTests
    {
        private Mock<ICustomerRepository> _repo;
        private CustomerService _service;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<ICustomerRepository>();
            _service = new CustomerService(_repo.Object);
        }


        [TestMethod]
        public void NotFound_ReturnsNull()
        {
            //Act
            var customer = _service.GetCustomerByMembershipNumber(Guid.NewGuid());

            //Arrange
            customer.ShouldBeNull();
        }

        [TestMethod]
        public void Found_ReturnsCustomer()
        {
            //Arrange
            var dto = TypicalCustomer();
            _repo.Setup(r => r.GetCustomerByMembershipNumber(dto.MembershipNumber)).Returns(dto);

            //Act
            var customer = _service.GetCustomerByMembershipNumber(dto.MembershipNumber);

            //Arrange
            customer.ShouldNotBeNull();
            customer.MembershipNumber.ShouldBe(dto.MembershipNumber);
            ((ICustomerIdentifier)customer).CustomerId.ShouldBe(dto.CustomerId);

            customer.FirstName.ShouldBe(dto.FirstName);
            customer.LastName.ShouldBe(dto.LastName);

            customer.Address.Address.ShouldBe(dto.Address);
            customer.Address.City.ShouldBe(dto.City);
            customer.Address.Province.ShouldBe(dto.Province);
            customer.Address.Country.ShouldBe(dto.Country);
            customer.Address.PostalCode.ShouldBe(dto.PostalCode);

            customer.PhoneNumber.ShouldBe(dto.PhoneNumber);
            customer.Discount.ShouldBe(dto.Discount);
        }

        private CustomerDto TypicalCustomer()
        {
            return new CustomerDto
            {
                CustomerId = 1,
                MembershipNumber = Guid.NewGuid(),
                FirstName = "Bob",
                LastName = "Robertson",
                Address = "123 Roberson Street",
                City = "Robville",
                Province = "Robshire",
                Country = "ROB",
                PostalCode = "R0B 3R4",
                PhoneNumber = "18008088008",
                Discount = 0.15
            };
        }

    }
}
