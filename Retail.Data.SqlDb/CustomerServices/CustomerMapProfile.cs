using AutoMapper;
using Retail.Data.Abstractions.CustomerServices;
using Retail.Data.SqlDb.EfModels.Models;

namespace Retail.Data.SqlDb.CustomerServices
{
    internal class CustomerMapProfile : Profile
    {
        public CustomerMapProfile()
        {
            CreateMap<Customer, CustomerDto>();
        }
    }
}
