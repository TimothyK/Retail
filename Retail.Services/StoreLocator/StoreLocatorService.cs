using Retail.Data.Abstractions.StoreLocator;
using System.Collections.Generic;
using System.Linq;

namespace Retail.Services.StoreLocator
{
    public class StoreLocatorService
    {
        private readonly IStoreSearch _repo;

        public StoreLocatorService(IStoreSearch repo)
        {
            _repo = repo;
        }

        public IEnumerable<Store> GetStores()
        {
            return Enumerable.Empty<Store>();
        }

        public Store GetStoreById(int storeId) 
        {
            return null;
        }

    }

}
