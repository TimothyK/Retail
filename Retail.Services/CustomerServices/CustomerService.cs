using AutoMapper;
using Retail.Data.Abstractions.CustomerServices;
using Retail.Services.Utilities;
using System;

namespace Retail.Services.CustomerServices
{
    public class CustomerService
    {
        private readonly ICustomerRepository _repo;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository repo)
        {
            _repo = repo;
            _mapper = AutoMap.Mapper;
        }

        public Customer GetCustomerByMembershipNumber(Guid membershipNumber)
        {
            var dto = _repo.GetCustomerByMembershipNumber(membershipNumber);
            return (dto == null) ? null : _mapper.Map<Customer>(dto);
        }
    }
}
