using System.Collections.Generic;
using Customer = Retail.Services.CustomerServices.Customer;
using Store = Retail.Services.StoreLocator.Store;

namespace Retail.Services.OrderCreation
{
    public class Order
    {
        public Order(Customer customer, Store store)
        {
            Customer = customer;
            Store = store;
        }

        public Customer Customer { get; }
        public Store Store { get; }

        public List<OrderLineItem> LineItems { get; set; } = new List<OrderLineItem>();
    }

    public class OrderLineItem
    {
        public OrderLineItem(int quantity, Product product)
        {
            Quantity = quantity;
            Product = product;
        }

        public Product Product { get; }
        public int Quantity { get; }

        public decimal UnitPrice => Product.DiscountPrice;
        public decimal TotalPrice => UnitPrice * Quantity;
    }
}
