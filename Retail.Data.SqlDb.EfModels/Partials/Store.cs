using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Retail.Data.SqlDb.EfModels.Models
{
    public partial class Store
    {
        [InverseProperty(nameof(Models.Inventory.Store))]
        public virtual ICollection<Inventory> Inventory { get; set; } = new HashSet<Inventory>();
    }
}
