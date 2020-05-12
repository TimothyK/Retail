using System;

namespace Retail.Data.Abstractions.CustomerServices
{
    public interface ICustomerRepository
    {
        CustomerDto GetCustomerByMembershipNumber(Guid membershipNumber);
    }
}
