using AutoMapper;
using Retail.Data.Abstractions.OrderCreation;
using Retail.Data.Abstractions.StoreLocator;

namespace Retail.Services.StoreLocator
{
    public class Store : IStoreIdentifier
    {
        public int StoreId { get; internal set; }
        public string StoreName { get; internal set; }
    }

    internal class StoreProfile : Profile
    {
        public StoreProfile()
        {
            CreateMap<StoreDto, Store>();
        }
    }

}
