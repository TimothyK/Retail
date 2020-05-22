using AutoMapper.QueryableExtensions;
using Retail.Data.Abstractions.CustomerServices;
using Retail.Data.SqlDb.EfModels;
using Retail.Data.SqlDb.Utilities;
using System;
using System.Linq;
using TimothyK.Data.UnitOfWork;

namespace Retail.Data.SqlDb.CustomerServices
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public CustomerRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private RetailDbContext CreateDbContext() => _unitOfWork.CreateDbContext<RetailDbContext>();

        public CustomerDto GetCustomerByMembershipNumber(Guid membershipNumber)
        {
            return CreateDbContext().Customers
                .Where(customer => customer.MembershipNumber == membershipNumber)
                .ProjectTo<CustomerDto>(AutoMap.Configuration)
                .SingleOrDefault();
                
        }
    }
}
