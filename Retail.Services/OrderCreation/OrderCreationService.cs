using Retail.Data.Abstractions.OrderCreation;
using System.Collections.Generic;
using System.Linq;
using Customer = Retail.Services.CustomerServices.Customer;
using Store = Retail.Services.StoreLocator.Store;

namespace Retail.Services.OrderCreation
{
    public class OrderCreationService
    {
        private readonly IOrderRepository _repo;

        public OrderCreationService(IOrderRepository repo)
        {
            _repo = repo;
        }

        public Order CreateOrder(Customer customer, Store store)
        {
            return new Order(customer, store);
        }

        public IReadOnlyCollection<Product> GetAvailableProducts(Order order)
        {
            return Enumerable.Empty<Product>().ToList();
        }

        public void SubmitOrder(Order order)
        {

        }
    }
}
