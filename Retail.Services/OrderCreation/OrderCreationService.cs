using AutoMapper;
using Retail.Data.Abstractions.OrderCreation;
using Retail.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Customer = Retail.Services.CustomerServices.Customer;
using Store = Retail.Services.StoreLocator.Store;

namespace Retail.Services.OrderCreation
{
    public class OrderCreationService
    {
        private readonly IOrderRepository _repo;
        private IMapper _mapper;

        public OrderCreationService(IOrderRepository repo)
        {
            _repo = repo;
            _mapper = AutoMap.Mapper;
        }

        public Order CreateOrder(Customer customer, Store store)
        {
            return new Order(customer, store);
        }

        public IReadOnlyCollection<Product> GetAvailableProducts(Order order)
        {
            var discount = _repo.GetCustomerDiscount(order.Customer);

            return _repo.GetAvailableProducts(order.Customer, order.Store)
                .Select(_mapper.Map<Product>)
                .Select(product => ApplyDiscount(product, discount))
                .ToList();
        }

        private Product ApplyDiscount(Product product, double discount)
        {
            var discountPrice = product.OriginalPrice - (product.OriginalPrice * Convert.ToDecimal(discount));

            product.DiscountPrice = Math.Min(product.DiscountPrice, discountPrice);

            return product;
        }

        public void SubmitOrder(Order order)
        {

        }
    }
}
