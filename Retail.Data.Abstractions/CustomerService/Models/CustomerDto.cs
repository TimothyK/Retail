using System;

namespace Retail.Data.Abstractions.CustomerService
{
    public class CustomerDto
    {
        public int CustomerId { get; }
        public Guid MembershipNumber { get; }

        public string FirstName { get; }
        public string LastName { get; }

        public string Address { get; }
        public string City { get; }
        public string Province { get; }
        public string Country { get; }
        public string PostalCode { get; }

        public string PhoneNumber { get; set; }

        public double Discount { get; }
    }
}
