using System;
using System.Collections.Generic;
using System.Text;

namespace Retail.Data.Abstractions.CustomerService
{
    public interface ICustomerSearch
    {
        CustomerDto GetCustomerByMembershipNumber(Guid membershipNumber);
    }
}
