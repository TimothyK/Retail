namespace Retail.Services.OrderCreation
{
    public class Product
    {
        internal int ProductId { get; set; }
        public string ProductName { get; }
        public decimal OriginalPrice { get; }
        public decimal DiscountPrice { get; }
        public int Quantity { get; }
    }
}
