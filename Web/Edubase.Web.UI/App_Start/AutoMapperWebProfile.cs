using AutoMapper;
using Edubase.Services.Establishments.Models;
using Edubase.Web.UI.Models;
using System;

namespace Edubase.Web.UI
{
    public class AutoMapperWebProfile : Profile
    {
        public AutoMapperWebProfile()
        {
            CreateMap<EditEstablishmentModel, EstablishmentModel>();
            CreateMap<EstablishmentModel, EditEstablishmentModel>()
                .ForMember(dst => dst.OldHeadFirstName, mapping => mapping.MapFrom(src => src.HeadFirstName))
                .ForMember(dst => dst.OldHeadLastName, mapping => mapping.MapFrom(src => src.HeadLastName));
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