using System.Collections.Generic;

namespace Retail.Data.Abstractions.OrderCreation
{
    public class OrderDto
    {
        public IStoreIdentifier Store { get; set; }

        public decimal TotalPrice { get; set; }

        public ICustomerIdentifier Customer { get; set; }
        
        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingProvince { get; set; }
        public string ShippingCountry { get; set; }
        public string ShippingPostalCode { get; set; }

        public string CustomerPhoneNumber { get; set; }

        public List<OrderLineItemDto> LineItems { get; set; } = new List<OrderLineItemDto>();
    }

    public class OrderLineItemDto
    {
        public IProductIdentifier Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
