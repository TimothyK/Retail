using Dapper;
using Dapper.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Retail.Data.Abstractions.OrderCreation;
using Retail.Data.SqlDb.EfModels;
using Retail.Data.SqlDb.EfModels.Models;
using Retail.Data.SqlDb.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

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
            var entity = AutoMap.Mapper.Map<Order>(order);
            _dbRetail.Orders.Add(entity);
            _dbRetail.SaveChanges();
        }

        public void DecrementProductInventory(IProductIdentifier product, IStoreIdentifier store, int quantity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ProductDto> GetAvailableProducts(IStoreIdentifier store)
        {
            var transaction = _dbRetail.Database.CurrentTransaction.GetDbTransaction();
            var parameters = new { store.StoreId };
            var products = transaction
                .Query<ProductDto>("Select product.ProductId" +
                ", product.ProductName" +
                ", product.Price" +
                ", product.SalesPrice" +
                ", inventory.Quantity" +
                " From dbo.Products product" +
                " Inner Join dbo.Inventory inventory On (product.ProductId = inventory.ProductId)" +
                " Where (inventory.StoreId = @StoreId)" +
                " And (product.Active = 1)" +
                " And (inventory.Quantity > 0)"
                , parameters);

            return products;
        }

        public double GetCustomerDiscount(ICustomerIdentifier customer)
        {
            return _dbRetail.Customers.AsNoTracking()
                .SingleOrDefault(cust => cust.CustomerId == customer.CustomerId)
                ?.Discount 
                ?? throw new CustomerNotFoundException(customer);
        }
    }
}
