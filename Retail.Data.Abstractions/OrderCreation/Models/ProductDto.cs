namespace Retail.Data.Abstractions.OrderCreation
{
    public class ProductDto : IProductIdentifier
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        
        public decimal Price { get; set; }
        public decimal SalesPrice { get; set; }

        public int Quantity { get; set; }
    }
}
