using Retail.Data.Abstractions.StoreLocator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Retail.Services.UnitTests.StoreLocator.TestDoubles
{
    class StoreRepository : IStoreRepository
    {
        public StoreDto GetStoreById(int storeId)
        {
            return _stores.SingleOrDefault(store => store.StoreId == storeId);
        }

        public IEnumerable<StoreDto> GetStores() => _stores;

        private List<StoreDto> _stores = new List<StoreDto>();

        public StoreDto AddStore(int storeId, string storeName)
        {
            var store = new StoreDto
            {
                StoreId = storeId,
                StoreName = storeName
            };

            _stores.Add(store);
            return store;
        }
    }
}
