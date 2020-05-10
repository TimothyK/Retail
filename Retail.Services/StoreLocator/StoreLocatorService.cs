using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Retail.Services.StoreLocator
{
    public class StoreLocatorService
    {
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
