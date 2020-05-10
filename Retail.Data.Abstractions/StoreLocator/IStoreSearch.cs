using System.Collections.Generic;

namespace Retail.Data.Abstractions.StoreLocator
{
    public interface IStoreSearch
    {
        IEnumerable<StoreDto> GetStores();
        StoreDto GetStoreById(int storeId);
    }
}
