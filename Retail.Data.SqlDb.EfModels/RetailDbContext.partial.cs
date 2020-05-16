using Microsoft.EntityFrameworkCore;
using Retail.Data.SqlDb.EfModels.Models;

namespace Retail.Data.SqlDb.EfModels
{
    public partial class RetailDbContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Store>()
                .HasQueryFilter(store => store.Active == null || store.Active.Value);
                
        }
    }
}
