using System.Collections.Generic;

namespace Retail.Data.Abstractions.OrderCreation
{
    public interface IOrderSearch
    {
        IEnumerable<ProductDto> GetAvailableProducts(ICustomerIdentifier customer, IStoreIdentifier store);

        void CreateOrder(OrderDto order);

        void DecrementProductInventory(IProductIdentifier product, int quantity);
    }
}
