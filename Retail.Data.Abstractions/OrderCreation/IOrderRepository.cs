using System.Collections.Generic;

namespace Retail.Data.Abstractions.OrderCreation
{
    public interface IOrderRepository
    {
        double GetCustomerDiscount(ICustomerIdentifier customer);

        IEnumerable<ProductDto> GetAvailableProducts(IStoreIdentifier store);

        void CreateOrder(OrderDto order);

        void DecrementProductInventory(IProductIdentifier product, IStoreIdentifier store, int quantity);
    }
}
