using Retail.Data.Abstractions.OrderCreation;
using Retail.Data.SqlDb.EfModels.Models;

namespace Retail.Data.SqlDb.Tests.OrderCreation
{
    internal class ProductIdentifier : IProductIdentifier
    {
        public ProductIdentifier(int productId)
        {
            ProductId = productId;
        }

        public int ProductId { get; }

        public static implicit operator ProductIdentifier(Product product) =>
            new ProductIdentifier(product.ProductId);
    }
}
