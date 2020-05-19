using Retail.Data.SqlDb.EfModels;
using Retail.Data.SqlDb.EfModels.Models;

namespace Retail.Data.SqlDb.Tests.TestRecordFactory
{
    internal static class ProductFactory
    {
        public static Product CreateProduct(this RetailDbContext db)
        {
            var id = IdFactory.Next();
            var product = new Product
            {
                ProductName = $"Product {id}",
                Active = true,
                Price = id * 2 + 0.99m,
                SalesPrice = id + 0.99m,
            };
            db.Add(product);

            return product;
        }

        public static Product AddInventory(this Product product, Store store, int quantity)
        {
            var inventory = new Inventory
            {
                Product = product,
                Store = store,
                Quantity = quantity
            };
            
            product.Inventory.Add(inventory);
            store.Inventory.Add(inventory);

            return product;
        }
    }
}
