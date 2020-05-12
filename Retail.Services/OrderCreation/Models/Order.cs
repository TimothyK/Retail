using AutoMapper;
using Retail.Data.Abstractions.OrderCreation;
using System;
using System.Collections.Generic;
using System.Linq;
using Customer = Retail.Services.CustomerServices.Customer;
using Store = Retail.Services.StoreLocator.Store;

namespace Retail.Services.OrderCreation
{
    public class Order
    {
        public Order(Customer customer, Store store)
        {
            Customer = customer;
            Store = store;
        }

        public Customer Customer { get; }
        public Store Store { get; }

        public List<OrderLineItem> LineItems { get; set; } = new List<OrderLineItem>();

        public void AddLineItem(int quantity, Product product) => 
            LineItems.Add(new OrderLineItem(quantity, product));

        public decimal TotalPrice => LineItems.Select(lineItem => lineItem.TotalPrice).Sum();
    }

    public class OrderLineItem
    {
        public OrderLineItem(int quantity, Product product)
        {
            Quantity = quantity;
            Product = product;
        }

        public Product Product { get; }
        public int Quantity { get; }

        public decimal UnitPrice => Product.DiscountPrice;
        public decimal TotalPrice => UnitPrice * Quantity;
    }

    public class OrderProfiler : Profile
    {
        public OrderProfiler()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(dto => dto.ShippingAddress, cfg => cfg.MapFrom(order => order.Customer.Address.Address))
                .ForMember(dto => dto.ShippingCity, cfg => cfg.MapFrom(order => order.Customer.Address.City))
                .ForMember(dto => dto.ShippingProvince, cfg => cfg.MapFrom(order => order.Customer.Address.Province))
                .ForMember(dto => dto.ShippingCountry, cfg => cfg.MapFrom(order => order.Customer.Address.Country))
                .ForMember(dto => dto.ShippingPostalCode, cfg => cfg.MapFrom(order => order.Customer.Address.PostalCode));

            CreateMap<OrderLineItem, OrderLineItemDto>();

        }
    }
}

