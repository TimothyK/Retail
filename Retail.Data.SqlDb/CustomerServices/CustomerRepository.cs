using AutoMapper.QueryableExtensions;
using Retail.Data.Abstractions.CustomerServices;
using Retail.Data.SqlDb.EfModels;
using Retail.Data.SqlDb.Utilities;
using System;
using System.Linq;

namespace Retail.Data.SqlDb.CustomerServices
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly RetailDbContext _dbRetail;

        public CustomerRepository(RetailDbContext dbRetail)
        {
            _dbRetail = dbRetail;
        }

        public CustomerDto GetCustomerByMembershipNumber(Guid membershipNumber)
        {
            return _dbRetail.Customers
                .Where(customer => customer.MembershipNumber == membershipNumber)
                .ProjectTo<CustomerDto>(AutoMap.Configuration)
                .SingleOrDefault();
                
        }
    }
}
