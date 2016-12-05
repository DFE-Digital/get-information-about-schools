using AutoMapper;
using Edubase.Common.Spatial;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Data;
using Edubase.Services.Domain;
using Edubase.Data.Entity.ComplexTypes;
using Edubase.Services.Establishments.Models;
using Edubase.Data.Entity;
using Edubase.Services.Groups.Models;

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
            CreateMap<Establishment, EstablishmentModel>();
            CreateMap<GroupCollection, GroupModel>();
        }
    }
}
