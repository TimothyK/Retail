using Retail.Data.Abstractions.OrderCreation;
using Retail.Data.SqlDb.EfModels.Models;

namespace Retail.Data.SqlDb.Tests.OrderCreation
{
    internal class StoreIdentifier : IStoreIdentifier
    {
        public StoreIdentifier(int storeId)
        {
            StoreId = storeId;
        }

        public int StoreId { get; }

        public static implicit operator StoreIdentifier(Store store) =>
            new StoreIdentifier(store.StoreId);
    }
}
