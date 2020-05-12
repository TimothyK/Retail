using AutoMapper;
using Retail.Data.Abstractions.CustomerServices;
using Retail.Data.Abstractions.OrderCreation;
using System;

namespace Retail.Services.CustomerServices
{
    public class Customer : ICustomerIdentifier
    {
        public int CustomerId { get; internal set; }

        public Guid MembershipNumber { get; internal set; }

        public string FirstName { get; internal set; }
        public string LastName { get; internal set; }

        public MailingAddress Address { get; internal set; }

        public string PhoneNumber { get; internal set; }

        public double Discount { get; internal set; }
    }

    internal class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<CustomerDto, Customer>()
                .ForMember(cust => cust.Address, member => member.MapFrom(source => source));
                
            CreateMap<CustomerDto, MailingAddress>();
        }
    }
}
