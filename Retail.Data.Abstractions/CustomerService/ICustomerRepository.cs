using System;
using System.Collections.Generic;
using System.Text;

namespace Retail.Data.Abstractions.CustomerService
{
    public interface ICustomerRepository
    {
        CustomerDto GetCustomerByMembershipNumber(Guid membershipNumber);
    }
}
