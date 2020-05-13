using AutoMapper;
using AutoMapper.QueryableExtensions;
using Retail.Data.Abstractions.StoreLocator;
using Retail.Data.SqlDb.EfModels;
using Retail.Data.SqlDb.Utilities;
using System;
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
            throw new NotImplementedException();
        }

        public IEnumerable<StoreDto> GetStores()
        {
            return _dbRetail.Stores
                .ProjectTo<StoreDto>(AutoMap.Configuration)
                .ToList();
        }
    }
}
