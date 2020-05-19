using AutoMapper;
using Retail.Data.Abstractions.OrderCreation;
using Retail.Data.SqlDb.EfModels.Models;

namespace Retail.Data.SqlDb.OrderCreation
{
    internal class OrderMapProfile : Profile
    {
        public OrderMapProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dto => dto.Quantity, cfg => cfg.MapFrom(product => 100));
        }
    }
}
