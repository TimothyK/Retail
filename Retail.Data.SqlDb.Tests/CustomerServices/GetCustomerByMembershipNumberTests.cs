using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Data.Abstractions.CustomerServices;
using Retail.Data.SqlDb.CustomerServices;
using Retail.Data.SqlDb.Database;
using Retail.Data.SqlDb.EfModels;
using Retail.Data.SqlDb.TestRecordFactory;
using Shouldly;
using System;

namespace Retail.Data.SqlDb.Tests.CustomerServices
{
    [TestClass]
    public class GetCustomerByMembershipNumberTests : BaseTests
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

        private ICustomerRepository _repo;

        [TestInitialize]
        public void Setup()
        {
            TestInitialize();
            _unitOfWork = new RetailUnitOfWork(_attachedDatabase);
            _repo = new CustomerRepository(_unitOfWork.CreateDbContext<RetailDbContext>());
        }

        [TestCleanup]
        public void TearDown()
        {
            _unitOfWork?.Dispose();  //implicit Rollback
            TestCleanup();
        }

        #endregion

        [TestMethod]
        public void NotFound_ReturnsNull()
        {
            //Act
            var customer = _repo.GetCustomerByMembershipNumber(Guid.NewGuid());

            //Assert
            customer.ShouldBeNull();
        }

        [TestMethod]
        public void Found_ReturnsCustomer()
        {
            //Arrange
            var db = _unitOfWork.CreateDbContext<RetailDbContext>();
            var expected = db.CreateCustomer();
            db.SaveChanges();

            //Act
            var actual = _repo.GetCustomerByMembershipNumber(expected.MembershipNumber);

            //Assert
            actual.ShouldNotBeNull();
        }

        [TestMethod]
        public void TwoCustomers_ReturnsCorrectCustomer()
        {
            //Arrange
            var db = _unitOfWork.CreateDbContext<RetailDbContext>();
            var expected = db.CreateCustomer();
            db.CreateCustomer();    //second customer            
            db.SaveChanges();

            //Act
            var actual = _repo.GetCustomerByMembershipNumber(expected.MembershipNumber);

            //Assert
            actual.MembershipNumber.ShouldBe(expected.MembershipNumber);
        }

        [TestMethod]
        public void DeactivatedCustomer_ReturnsNull()
        {
            //Arrange
            var db = _unitOfWork.CreateDbContext<RetailDbContext>();
            var expected = db.CreateCustomer();
            expected.Active = false;
            db.SaveChanges();

            //Act
            var actual = _repo.GetCustomerByMembershipNumber(expected.MembershipNumber);

            //Assert
            actual.ShouldBeNull();
        }
    }
}
