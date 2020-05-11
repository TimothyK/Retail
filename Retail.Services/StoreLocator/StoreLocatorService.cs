using Retail.Data.Abstractions.StoreLocator;
using System.Collections.Generic;
using System.Linq;

namespace Retail.Services.StoreLocator
{
    public class StoreLocatorService
    {
        private readonly IStoreRepository _repo;

        public StoreLocatorService(IStoreRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<Store> GetStores() =>
            _repo.GetStores()
                .Select(DtoToStore)
                .ToList();

        public Store GetStoreById(int storeId)
        {
            var dto = _repo.GetStoreById(storeId);
            if (dto == null) return null;

            return DtoToStore(dto);
        }

        private Store DtoToStore(StoreDto dto) =>
            new Store
            {
                StoreId = dto.StoreId,
                StoreName = dto.StoreName
            };


    }

}
