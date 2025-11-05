using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Edubase.Services.Establishments.Models;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI
{
    public class AutoMapperWebProfile : Profile
    {
        public AutoMapperWebProfile()
        {
            CreateMap<ProprietorViewModel, ProprietorModel>();
            CreateMap<ProprietorModel, ProprietorViewModel>();

            CreateMap<EditEstablishmentModel, EstablishmentModel>()
                .ForMember(dst => dst.AdditionalAddresses, mapping => mapping.MapFrom(src => src.AdditionalAddresses.ToArray()));

            CreateMap<EditEstablishmentModel, IEBTModel>();
            CreateMap<IEBTModel, EstablishmentModel>();
            CreateMap<IEBTModel, EditEstablishmentModel>()
                .ForMember(dst => dst.Proprietors,
                    mapping => mapping.MapFrom(src =>
                        src.Proprietors.Any() ? src.Proprietors : new List<ProprietorModel>() { new ProprietorModel() }));

            CreateMap<EstablishmentModel, EditEstablishmentModel>()
                .ForMember(dst => dst.OldHeadFirstName, mapping => mapping.MapFrom(src => src.HeadFirstName))
                .ForMember(dst => dst.OldHeadLastName, mapping => mapping.MapFrom(src => src.HeadLastName));

            CreateMap<DateTimeViewModel, DateTime?>().ConvertUsing<DateTimeTypeConverter>();
            CreateMap<DateTime?, DateTimeViewModel>().ConvertUsing<DateTimeViewModelTypeConverter>();
        }

    }

    public class ProprietorModelConverter : ITypeConverter<ProprietorModel, ProprietorViewModel>
    {
        public ProprietorViewModel Convert(ProprietorModel source, ProprietorViewModel destination, ResolutionContext context)
        {
            return new ProprietorViewModel
            {
                Id = source.Id,
                Locality = source.Locality,
                Town = source.Town,
                Postcode = source.Postcode,
                TelephoneNumber = source.TelephoneNumber,
                Street = source.Street,
                Address3 = source.Address3,
                CountyId = source.CountyId,
                Email = source.Email,
                Name = source.Name
            };
        }
    }

    public class ProprietorViewModelConverter : ITypeConverter<ProprietorViewModel, ProprietorModel>
    {
        public ProprietorModel Convert(ProprietorViewModel source, ProprietorModel destination, ResolutionContext context)
        {
            return new ProprietorModel
            {
                Id = source.Id,
                Locality = source.Locality,
                Town = source.Town,
                Postcode = source.Postcode,
                TelephoneNumber = source.TelephoneNumber,
                Street = source.Street,
                Address3 = source.Address3,
                CountyId = source.CountyId,
                Email = source.Email,
                Name = source.Name
            };
        }
    }

    public class DateTimeTypeConverter : ITypeConverter<DateTimeViewModel, DateTime?>
    {
        public DateTime? Convert(DateTimeViewModel source, DateTime? destination, ResolutionContext context) => source.ToDateTime();
    }

    public class DateTimeViewModelTypeConverter : ITypeConverter<DateTime?, DateTimeViewModel>
    {
        public DateTimeViewModel Convert(DateTime? source, DateTimeViewModel destination, ResolutionContext context)
        {
            return source.HasValue
                ? new DateTimeViewModel
            {
                Day = source.Value.Day,
                Month = source.Value.Month,
                Year = source.Value.Year
            }
                : new DateTimeViewModel();
        }
    }
}
