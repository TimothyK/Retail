using Retail.Data.Abstractions.CustomerService;
using System;

namespace Retail.Services.CustomerService
{
    public class CustomerService
    {
        private readonly ICustomerRepository _repo;

        public CustomerService(ICustomerRepository repo)
        {
            _repo = repo;
        }

        public Customer GetCustomerByMembershipNumber(Guid membershipNumber)
        {
            return null;
        }
    }
}
