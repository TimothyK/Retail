﻿using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Retail.Data.Abstractions.OrderCreation;
using Retail.Data.SqlDb.EfModels;
using Retail.Data.SqlDb.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Retail.Data.SqlDb.OrderCreation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly RetailDbContext _dbRetail;

        public OrderRepository(RetailDbContext dbRetail)
        {
            _dbRetail = dbRetail;
        }

        public void CreateOrder(OrderDto order)
        {
            throw new NotImplementedException();
        }

        public void DecrementProductInventory(IProductIdentifier product, IStoreIdentifier store, int quantity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ProductDto> GetAvailableProducts(IStoreIdentifier store)
        {
            return _dbRetail.Products
                .Include(product => product.Inventory)
                .ProjectTo<ProductDto>(AutoMap.Configuration);
        }

        public double GetCustomerDiscount(ICustomerIdentifier customer)
        {
            return _dbRetail.Customers
                .SingleOrDefault(cust => cust.CustomerId == customer.CustomerId)
                ?.Discount 
                ?? throw new CustomerNotFoundException(customer);
        }
    }
}
