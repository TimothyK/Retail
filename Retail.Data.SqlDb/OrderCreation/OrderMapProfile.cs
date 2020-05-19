using AutoMapper;
using Retail.Data.Abstractions.OrderCreation;
using Retail.Data.SqlDb.EfModels.Models;

namespace Retail.Data.SqlDb.OrderCreation
{
    internal class OrderMapProfile : Profile
    {
        public OrderMapProfile()
        {
            CreateMap<OrderDto, Order>()
                .ForMember(order => order.CustomerId, cfg => cfg.MapFrom(dto => dto.Customer.CustomerId))
                .ForMember(order => order.Customer, cfg => cfg.Ignore())
                .ForMember(order => order.StoreId, cfg => cfg.MapFrom(dto => dto.Store.StoreId))
                .ForMember(order => order.Store, cfg => cfg.Ignore())
                .ForMember(order => order.OrderId, cfg => cfg.Ignore())
                .ForMember(order => order.OrderLineItems, cfg => cfg.MapFrom(dto => dto.LineItems));
            CreateMap<OrderLineItemDto, OrderLineItem>()
                .ForMember(lineItem => lineItem.ProductId, cfg => cfg.MapFrom(dto => dto.Product.ProductId))
                .ForMember(lineItem => lineItem.Product, cfg => cfg.Ignore())
                .ForMember(lineItem => lineItem.OrderId, cfg => cfg.Ignore())
                .ForMember(lineItem => lineItem.Order, cfg => cfg.Ignore())
                .ForMember(lineItem => lineItem.LineItemId, cfg => cfg.Ignore());
        }
    }
}
