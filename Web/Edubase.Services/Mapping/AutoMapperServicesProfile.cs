using AutoMapper;
using Edubase.Common.Formatting.Json;
using Edubase.Common.Spatial;
using Edubase.Data;
using Edubase.Data.Entity;
using Edubase.Data.Entity.ComplexTypes;
using Edubase.Services.Domain;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Groups.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.Entity.Spatial;

namespace Edubase.Services.Mapping
{
    public class AutoMapperServicesProfile : Profile
    {
        public AutoMapperServicesProfile()
        {
            CreateMap<DbGeography, LatLon>().ConvertUsing(x => x.ToLatLon());
            CreateMap<LatLon, DbGeography>().ConvertUsing(x => x.ToDBGeography());

            CreateMap<string, List<AdditionalAddressModel>>()
                .ConvertUsing<FromJsonTypeConverter<List<AdditionalAddressModel>>>();

            CreateMap<List<AdditionalAddressModel>, string>()
                .ConvertUsing<ToJsonTypeConverter<List<AdditionalAddressModel>>>();
            
            CreateMap<ContactDetail, ContactDetailDto>();
            CreateMap<Address, EstablishmentAddressModel>();
            CreateMap<Person, PersonDto>();

            CreateMap<Establishment, EstablishmentModel>() // out
                .ForMember(x => x.Location, opt => opt.Ignore())
                .AfterMap((s, d) =>
                {
                    d.Location = s.Location.ToLatLon();
                    if (d.AdditionalAddresses == null) d.AdditionalAddresses = new List<AdditionalAddressModel>();
                })
                .ReverseMap() // in
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Name))
                .ForMember(x => x.AdditionalAddresses, opt => opt.MapFrom(x => x.AdditionalAddresses))
                .ForAllOtherMembers(opt => opt.Ignore());
            

            CreateMap<GroupCollection, GroupModel>();
        }
    }
}
