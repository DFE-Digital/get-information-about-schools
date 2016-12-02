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
    public class AutoMapperConfiguration
    {
        public static IMapper CreateMapper()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DbGeography, LatLon>().ConvertUsing(x => x.ToLatLon());
                cfg.CreateMap<LatLon, DbGeography>().ConvertUsing(x => x.ToDBGeography());
                cfg.CreateMap<ContactDetail, ContactDetailDto>();
                cfg.CreateMap<Address, EstablishmentAddressModel>();
                cfg.CreateMap<Person, PersonDto>();
                cfg.CreateMap<Establishment, EstablishmentModel>();
                cfg.CreateMap<GroupCollection, GroupModel>();
            }).CreateMapper();
        }
    }
}
