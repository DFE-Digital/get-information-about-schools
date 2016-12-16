using AutoMapper;
using Edubase.Common;
using Edubase.Common.Spatial;
using Edubase.Data;
using Edubase.Data.Entity;
using Edubase.Data.Entity.ComplexTypes;
using Edubase.Services.Domain;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Groups.Models;
using System.Data.Entity.Spatial;

namespace Edubase.Services.Mapping
{
    public class AutoMapperServicesProfile : Profile
    {
        public AutoMapperServicesProfile()
        {
            CreateMap<DbGeography, LatLon>().ConvertUsing(x => x.ToLatLon());
            CreateMap<LatLon, DbGeography>().ConvertUsing(x => x.ToDBGeography());

            CreateMap<ContactDetail, ContactDetailDto>();
            CreateMap<Address, EstablishmentAddressModel>();
            CreateMap<Person, PersonDto>();
            CreateMap<Establishment, EstablishmentModel>()
                .ForMember(x => x.Location, opt => opt.Ignore())
                .AfterMap((s, d) =>
                {
                    d.Location = s.Location.ToLatLon();
                });
            

            CreateMap<GroupCollection, GroupModel>();
        }
    }
}
