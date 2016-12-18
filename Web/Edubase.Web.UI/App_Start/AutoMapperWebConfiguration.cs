using AutoMapper;
using Edubase.Services.Mapping;

namespace Edubase.Web.UI
{
    public static class AutoMapperWebConfiguration
    {
        public static IMapper CreateMapper()
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperWebProfile>();
                cfg.AddProfile<AutoMapperServicesProfile>();
            }).CreateMapper();

            return mapper;
        }
    }
}