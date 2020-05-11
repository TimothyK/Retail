using System.Collections.Generic;

namespace Retail.Data.Abstractions.StoreLocator
{
    public interface IStoreRepository
    {
        IEnumerable<StoreDto> GetStores();
        StoreDto GetStoreById(int storeId);
    }
}
