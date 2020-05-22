using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Data.Abstractions.OrderCreation;
using Retail.Data.SqlDb.Database;
using Retail.Data.SqlDb.EfModels;
using Retail.Data.SqlDb.OrderCreation;
using Retail.Data.SqlDb.TestRecordFactory;
using Shouldly;
using System;

namespace Retail.Data.SqlDb.Tests.OrderCreation
{
    [TestClass]
    public partial class GetCustomerDiscountTests : BaseTests
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

