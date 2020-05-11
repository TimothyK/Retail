using System;
using System.Collections.Generic;
using System.Text;

namespace Retail.Data.Abstractions.OrderCreation
{
    public class ProductDto : IProductIdentifier
    {
        public int ProductId { get; }
        public string ProductName { get; }
        
        public decimal Price { get; }
        public decimal SalesPrice { get; }

        public int Quantity { get; }        
    }
}
