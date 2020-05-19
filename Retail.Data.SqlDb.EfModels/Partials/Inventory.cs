using System.ComponentModel.DataAnnotations.Schema;

namespace Retail.Data.SqlDb.EfModels.Models
{
    public partial class Inventory
    {
        [ForeignKey(nameof(ProductId))]
        [InverseProperty("Inventory")]
        public virtual Product Product { get; set; }

        [ForeignKey(nameof(StoreId))]
        [InverseProperty("Inventory")]
        public virtual Store Store { get; set; }
    }
}
