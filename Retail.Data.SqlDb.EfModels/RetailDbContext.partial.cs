using Microsoft.EntityFrameworkCore;
using Retail.Data.SqlDb.EfModels.Models;

namespace Retail.Data.SqlDb.EfModels
{
    public partial class RetailDbContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasQueryFilter(customer => customer.Active == null || customer.Active.Value);

            modelBuilder.Entity<Store>()
                .HasQueryFilter(store => store.Active == null || store.Active.Value);

            modelBuilder.Entity<Inventory>()
                .HasOne(inventory => inventory.Product)
                .WithMany(product => product.Inventory)
                .HasForeignKey(inventory => inventory.ProductId);
            modelBuilder.Entity<Inventory>()
                .HasOne(inventory => inventory.Store)
                .WithMany(store => store.Inventory)
                .HasForeignKey(inventory => inventory.StoreId);
        }
    }
}
