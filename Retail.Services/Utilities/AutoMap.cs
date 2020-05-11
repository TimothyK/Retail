using AutoMapper;
using System.Reflection;

namespace Retail.Services.Utilities
{
    internal static class AutoMap
    {
        private static MapperConfiguration _configuration;
        public static MapperConfiguration Configuration
        {
            get
            {
                if (_configuration != null) return _configuration;

                _configuration = new MapperConfiguration(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));
                _configuration.AssertConfigurationIsValid();
                return _configuration;
            }
        }

        public static IMapper Mapper => Configuration.CreateMapper();
    }
}
