using Retail.Data.Abstractions.OrderCreation;
using System;

namespace Retail.Data.SqlDb.OrderCreation
{
    public class CustomerNotFoundException : Exception
    {
        public CustomerNotFoundException(ICustomerIdentifier customer) : base($"Customer {customer.CustomerId} was not found")
        {
            Customer = customer;
        }

        public ICustomerIdentifier Customer { get; }
    }
}
