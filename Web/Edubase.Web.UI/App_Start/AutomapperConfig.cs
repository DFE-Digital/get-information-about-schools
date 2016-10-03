using AutoMapper;
using Edubase.Data.Entity;
using Edubase.Data.Entity.ComplexTypes;
using Edubase.Web.UI.Models;
using System;
using Edubase.Common;

namespace Edubase.Web.UI
{
    public static class AutoMapperConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<AddressViewModel, Address>().ReverseMap();
                cfg.CreateMap<CreateEditEstablishmentModel, Establishment>()
                    .AfterMap((s, d) =>
                    {
                        if (s.LAESTAB.HasValue)
                        {
                            var laestab = s.LAESTAB.Value.ToString();
                            if (laestab.Length == 7) d.EstablishmentNumber = laestab.Substring(3, 4).ToInteger();
                        }
                    }).ReverseMap().AfterMap((s, d) =>
                    {
                        if (s.EstablishmentNumber.HasValue && s.LocalAuthorityId.HasValue)
                            d.LAESTAB = int.Parse(string.Concat(s.EstablishmentNumber, s.LocalAuthorityId));
                    });
                cfg.CreateMap<ContactDetailsViewModel, ContactDetail>().ReverseMap();
                cfg.CreateMap<DateTimeViewModel, DateTime?>().ConvertUsing<DateTimeTypeConverter>();
                cfg.CreateMap<DateTime?, DateTimeViewModel>().ConvertUsing<DateTimeViewModelTypeConverter>();
            });   
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
            else return new Models.DateTimeViewModel();
        }
    }

}