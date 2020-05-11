﻿using AutoMapper;
using Retail.Data.Abstractions.StoreLocator;
using System.Collections.Generic;
using System.Linq;

namespace Retail.Services.StoreLocator
{
    public class StoreLocatorService
    {
        private readonly IStoreRepository _repo;
        private readonly IMapper _mapper;

        public StoreLocatorService(IStoreRepository repo)
        {
            _repo = repo;

            var config = new MapperConfiguration(cfg => cfg.CreateMap<StoreDto, Store>());
            _mapper = config.CreateMapper();
        }

        public IEnumerable<Store> GetStores() =>
            _repo.GetStores()
                .Select(_mapper.Map<Store>)
                .ToList();

        public Store GetStoreById(int storeId)
        {
            var dto = _repo.GetStoreById(storeId);
            if (dto == null) return null;

            return _mapper.Map<Store>(dto);
        }
    }

}
