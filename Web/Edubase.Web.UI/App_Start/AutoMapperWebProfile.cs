using AutoMapper;
using Edubase.Data.Entity.ComplexTypes;
using Edubase.Services.Domain;
using Edubase.Services.Establishments.Models;
using Edubase.Web.UI.Models;
using System;

namespace Edubase.Web.UI
{
    public class AutoMapperWebProfile : Profile
    {
        public AutoMapperWebProfile()
        {
            CreateMap<AddressViewModel, Address>().ReverseMap();
            CreateMap<AddressViewModel, EstablishmentAddressModel>().ReverseMap();
            CreateMap<CreateEditEstablishmentModel, EstablishmentModel>().ReverseMap();
            CreateMap<CreateEditEstablishmentModel, Data.Entity.Establishment>().ReverseMap(); // TODO: delete one day
            CreateMap<ContactDetailsViewModel, ContactDetail>().ReverseMap();
            CreateMap<ContactDetailsViewModel, ContactDetailDto>().ReverseMap();
            CreateMap<DateTimeViewModel, DateTime?>().ConvertUsing<DateTimeTypeConverter>();
            CreateMap<DateTime?, DateTimeViewModel>().ConvertUsing<DateTimeViewModelTypeConverter>();
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
            if (source.HasValue) return new DateTimeViewModel
            {
                Day = source.Value.Day,
                Month = source.Value.Month,
                Year = source.Value.Year
            };
            else return new DateTimeViewModel();
        }
    }

}