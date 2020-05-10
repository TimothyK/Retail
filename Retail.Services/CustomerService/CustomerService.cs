using Retail.Data.Abstractions.CustomerService;
using System;

namespace Retail.Services.CustomerService
{
    public class CustomerService
    {
        private readonly ICustomerSearch _repo;

        public CustomerService(ICustomerSearch repo)
        {
            _repo = repo;
        }

        public Customer GetCustomerByMembershipNumber(Guid membershipNumber)
        {
            return null;
        }
    }
}
