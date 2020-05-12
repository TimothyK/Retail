using System;

namespace Retail.Data.Abstractions.CustomerServices
{
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public Guid MembershipNumber { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }

        public string PhoneNumber { get; set; }
    }
}
