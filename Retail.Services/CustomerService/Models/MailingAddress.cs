using System;
using System.Collections.Generic;
using System.Text;

namespace Retail.Services.CustomerService
{
    public class MailingAddress
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
    }
}
