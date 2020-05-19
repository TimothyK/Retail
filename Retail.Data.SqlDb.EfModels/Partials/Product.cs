using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Retail.Data.SqlDb.EfModels.Models
{
    public partial class Product
    {
        [InverseProperty(nameof(Models.Inventory.Product))]
        public virtual ICollection<Inventory> Inventory { get; set; } = new HashSet<Inventory>();
    }
}
