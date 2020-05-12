using AutoMapper;
using Retail.Data.Abstractions.OrderCreation;
using Retail.Services.Utilities;
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
            return _repo.GetAvailableProducts(order.Customer, order.Store)
                .Select(_mapper.Map<Product>)
                .ToList();
        }

        public void SubmitOrder(Order order)
        {

        }
    }
}
