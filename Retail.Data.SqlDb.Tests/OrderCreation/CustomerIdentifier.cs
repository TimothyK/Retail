using Retail.Data.Abstractions.OrderCreation;
using Retail.Data.SqlDb.EfModels.Models;

namespace Retail.Data.SqlDb.Tests.OrderCreation
{
    internal class CustomerIdentifier : ICustomerIdentifier
    {
        public CustomerIdentifier(int customerId)
        {
            CustomerId = customerId;
        }

        public int CustomerId { get; }

        public static implicit operator CustomerIdentifier(Customer customer) =>
            new CustomerIdentifier(customer.CustomerId);
    }
}

