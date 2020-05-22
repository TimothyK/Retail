using AutoMapper.QueryableExtensions;
using Retail.Data.Abstractions.StoreLocator;
using Retail.Data.SqlDb.EfModels;
using Retail.Data.SqlDb.Utilities;
using System.Collections.Generic;
using System.Linq;
using TimothyK.Data.UnitOfWork;

namespace Retail.Data.SqlDb.StoreLocator
{
    public class StoreRepository : IStoreRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public StoreRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private RetailDbContext CreateDbContext() => _unitOfWork.CreateDbContext<RetailDbContext>();

        public StoreDto GetStoreById(int storeId)
        {            
            return CreateDbContext().Stores
                .Where(store => store.StoreId == storeId)
                .ProjectTo<StoreDto>(AutoMap.Configuration)
                .SingleOrDefault();
        }

        public IEnumerable<StoreDto> GetStores()
        {
            return CreateDbContext().Stores
                .ProjectTo<StoreDto>(AutoMap.Configuration)
                .ToList();
        }
    }
}
