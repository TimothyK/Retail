using AutoMapper;
using Retail.Data.Abstractions.StoreLocator;
using Retail.Data.SqlDb.EfModels.Models;

namespace Retail.Data.SqlDb.StoreLocator
{
    internal class StoreLocatorMapProfile : Profile
    {
        public StoreLocatorMapProfile()
        {
            CreateMap<Store, StoreDto>();
        }
    }
}
