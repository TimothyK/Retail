using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Retail.Services.CustomerService
{
    public class Customer
    {
        internal int CustomerId { get; }

        public Guid MembershipNumber { get; }

        public string FirstName { get; }
        public string LastName { get; }

        public MailingAddress Address { get; }

        public double Discount { get; }
    }
}
