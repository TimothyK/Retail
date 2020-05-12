using AutoMapper;
using Retail.Data.Abstractions.OrderCreation;

namespace Retail.Services.OrderCreation
{
    public class Product : IProductIdentifier
    {
        public int ProductId { get; internal set; }
        public string ProductName { get; internal set; }
        public decimal OriginalPrice { get; internal set; }
        public decimal DiscountPrice { get; internal set; }
        public int Quantity { get; internal set; }
    }

    internal class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductDto, Product>()
                .ForMember(product => product.OriginalPrice, cfg => cfg.MapFrom(dto => dto.Price))
                .ForMember(product => product.DiscountPrice, cfg => cfg.MapFrom(dto => dto.SalesPrice));
        }
    }

}
