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
using TimothyK.Data.UnitOfWork;

namespace Retail.Data.SqlDb.OrderCreation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public OrderRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private RetailDbContext CreateDbContext() => _unitOfWork.CreateDbContext<RetailDbContext>();

        public void CreateOrder(OrderDto order)
        {
            var db = CreateDbContext();
            var entity = AutoMap.Mapper.Map<Order>(order);
            db.Orders.Add(entity);
            db.SaveChanges();
        }

        public void DecrementProductInventory(IProductIdentifier product, IStoreIdentifier store, int quantity)
        {
            CreateDbContext().Database.ExecuteSqlInterpolated($"Update dbo.Inventory Set Quantity = Quantity - {quantity} Where (ProductId = {product.ProductId}) And (StoreId = {store.StoreId})");
        }

        public IEnumerable<ProductDto> GetAvailableProducts(IStoreIdentifier store)
        {
            var transaction = _unitOfWork.DbTransaction;
            var parameters = new { store.StoreId };
            var products = transaction
                .Query<ProductDto>("Select product.ProductId" +
                ", product.ProductName" +
                ", product.Price" +
                ", IsNull(product.SalesPrice, product.Price) As SalesPrice" +
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
            return CreateDbContext().Customers.AsNoTracking()
                .SingleOrDefault(cust => cust.CustomerId == customer.CustomerId)
                ?.Discount 
                ?? throw new CustomerNotFoundException(customer);
        }
    }
}
