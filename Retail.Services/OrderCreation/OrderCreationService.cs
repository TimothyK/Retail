﻿using Retail.Data.Abstractions.OrderCreation;
using System.Collections.Generic;
using System.Linq;
using Customer = Retail.Services.CustomerService.Customer;
using Store = Retail.Services.StoreLocator.Store;

namespace Retail.Services.OrderCreation
{
    public class OrderCreationService
    {
        private readonly IOrderSearch _repo;

        public OrderCreationService(IOrderSearch repo)
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
