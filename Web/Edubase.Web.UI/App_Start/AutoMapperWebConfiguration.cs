using AutoMapper;
using Edubase.Services.Mapping;

namespace Edubase.Web.UI
{
    public static class AutoMapperWebConfiguration
    {
        public static IMapper CreateMapper()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperWebProfile>();
                cfg.AddProfile<AutoMapperServicesProfile>();
            }).CreateMapper();
        }
    }
}