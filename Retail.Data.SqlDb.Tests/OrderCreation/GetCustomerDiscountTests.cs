﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Data.Abstractions.OrderCreation;
using Retail.Data.SqlDb.EfModels;
using Retail.Data.SqlDb.OrderCreation;
using Retail.Data.SqlDb.Tests.TestRecordFactory;
using Shouldly;

namespace Retail.Data.SqlDb.Tests.OrderCreation
{
    [TestClass]
    public partial class GetCustomerDiscountTests : BaseTests
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
        [DataRow(0.05)]
        [DataRow(0.10)]
        public void CustomerFound_ReturnsDiscount(double expectedDiscount)
        {
            //Arrange
            var db = _unitOfWork.CreateDbContext<RetailDbContext>();
            var customer = db.CreateCustomer();
            customer.Discount = expectedDiscount;
            db.SaveChanges();

            //Act
            var actualDiscount = _repo.GetCustomerDiscount((CustomerIdentifier)customer);

            //Assert
            actualDiscount.ShouldBe(expectedDiscount);
        }

        [TestMethod]
        public void NoCustomer_ThrowsCustomerNotFound()
        {
            //Arrange
            var customer = new CustomerIdentifier(-1);

            //Act
            var ex = Should.Throw<CustomerNotFoundException>(() => _repo.GetCustomerDiscount(customer));

            //Assert
            ex.Customer.CustomerId.ShouldBe(customer.CustomerId);
        }

        [TestMethod]
        public void TwoCustomers_ReturnsCorrectCustomer()
        {
            //Arrange
            var db = _unitOfWork.CreateDbContext<RetailDbContext>();
            var expectedCustomer = db.CreateCustomer();
            expectedCustomer.Discount = 0.05;

            var secondCustomer = db.CreateCustomer();
            secondCustomer.Discount = 0.10;

            db.SaveChanges();

            //Act
            var actualDiscount = _repo.GetCustomerDiscount((CustomerIdentifier) expectedCustomer);

            //Assert
            actualDiscount.ShouldBe(expectedCustomer.Discount);
        }

    }
}

