using AutoMapper.QueryableExtensions;
using Retail.Data.Abstractions.StoreLocator;
using Retail.Data.SqlDb.EfModels;
using Retail.Data.SqlDb.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Retail.Data.SqlDb.StoreLocator
{
    public class StoreRepository : IStoreRepository
    {
        private readonly RetailDbContext _dbRetail;

        public StoreRepository(RetailDbContext dbRetail)
        {
            _dbRetail = dbRetail;
        }

        public StoreDto GetStoreById(int storeId)
        {
            return _dbRetail.Stores
                .Where(store => store.StoreId == storeId)
                .ProjectTo<StoreDto>(AutoMap.Configuration)
                .SingleOrDefault();
        }

        public IEnumerable<StoreDto> GetStores()
        {
            return _dbRetail.Stores
                .ProjectTo<StoreDto>(AutoMap.Configuration)
                .ToList();
        }
    }
}
