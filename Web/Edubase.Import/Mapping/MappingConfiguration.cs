using Edubase.Common;
using Edubase.Common.Spatial;
using Edubase.Data.Entity;
using Edubase.Services;
using System.Linq;
using Edubase.Data.Entity.ComplexTypes;
using AutoMapper;
using Edubase.Data;

namespace Edubase.Import.Mapping
{
    internal class MappingConfiguration
    {
        private static readonly string[] _dtFormats = new[] { "dd-MM-yyyy", "dd/MM/yyyy", "dd/MM/yy" };

        private static CachedLookupService L { get; set; } = new CachedLookupService();
        private static IMapper _mapper = null;

        public static IMapper Create()
        {
            return _mapper = new MapperConfiguration(cfg =>
            {
                var contactAltMapper = new MapperConfiguration(cfg2 =>
                {
                    cfg2.CreateMap<Establishments, ContactDetail>()
                        .ForMember(x => x.EmailAddress, opt => opt.MapFrom(m => m.AlternativeEmail.ToCleanEmail()))
                        .ForAllOtherMembers(opt => opt.Ignore());
                }).CreateMapper();

                var groupPersonMapper = new MapperConfiguration(cfg2 =>
                {
                    cfg2.CreateMap<Groupdata, Person>()
                        .ForMember(x => x.FirstName, opt => opt.MapFrom(m => m.HeadofGroupFirstName.Clean()))
                        .ForMember(x => x.LastName, opt => opt.MapFrom(m => m.HeadofGroupLastName.Clean()))
                        .ForMember(x => x.Title, opt => opt.MapFrom(m => m.HeadofGroupTitle.Remove("Not-applicable", "Unknown").Clean()))
                        .ForAllOtherMembers(opt => opt.Ignore());
                }).CreateMapper();

                cfg.CreateMap<Establishments, Address>()
                    .ForMember(x => x.Line1, opt => opt.MapFrom(m => m.Street.Clean()))
                    .ForMember(x => x.Line2, opt => opt.MapFrom(m => m.Locality.Clean()))
                    .ForMember(x => x.Line3, opt => opt.MapFrom(m => m.Address3.Clean()))
                    .ForMember(x => x.CityOrTown, opt => opt.MapFrom(m => m.Town.Clean()))
                    .ForMember(x => x.County, opt => opt.MapFrom(m => m.Countyname.Clean()))
                    .ForMember(x => x.PostCode, opt => opt.MapFrom(m => m.Postcode.Clean()))
                    .ForAllOtherMembers(opt => opt.Ignore());

                cfg.CreateMap<Establishments, ContactDetail>()
                    .ForMember(x => x.EmailAddress, opt => opt.MapFrom(m => m.MainEmail.ToCleanEmail()))
                    .ForMember(x => x.TelephoneNumber, opt => opt.MapFrom(m => m.TelephoneNum.Clean()))
                    .ForMember(x => x.WebsiteAddress, opt => opt.MapFrom(m => m.SchoolWebsite.Clean()))
                    .ForAllOtherMembers(opt => opt.Ignore());

                cfg.CreateMap<Establishments, Establishment>()
                    .ForMember(x => x.Name, opt => opt.MapFrom(m => m.EstablishmentName.Clean()))
                    .ForMember(x => x.AdmissionsPolicyId, opt => opt.MapFrom(m => L.AdmissionsPoliciesGetAll().Id(m.AdmissionsPolicycode)))
                    .ForMember(x => x.Address, opt => opt.MapFrom(m => _mapper.Map<Establishments, Address>(m)))
                    .ForMember(x => x.Contact, opt => opt.MapFrom(m => _mapper.Map<Establishments, ContactDetail>(m)))
                    .ForMember(x => x.ContactAlt, opt => opt.MapFrom(m => contactAltMapper.Map<Establishments, ContactDetail>(m)))
                    .ForMember(x => x.Capacity, opt => opt.MapFrom(m => m.SchoolCapacity.ToInteger()))
                    .ForMember(x => x.CloseDate, opt => opt.MapFrom(m => m.CloseDate.ToDateTime(_dtFormats)))
                    .ForMember(x => x.DioceseId, opt => opt.MapFrom(m => L.DiocesesGetAll().Id(m.Diocesecode)))
                    .ForMember(x => x.Easting, opt => opt.MapFrom(m => m.Easting.ToInteger()))
                    .ForMember(x => x.Northing, opt => opt.MapFrom(m => m.Northing.ToInteger()))
                    .ForMember(x => x.Location, opt => opt.MapFrom(m => new OSGB36Converter().ToWGS84(m.Easting.ToInteger(), m.Northing.ToInteger()).ToDBGeography()))
                    .ForMember(x => x.EducationPhaseId, opt => opt.MapFrom(m => L.EducationPhasesGetAll().Id(m.PhaseOfEducationcode)))
                    .ForMember(x => x.EstablishmentNumber, opt => opt.MapFrom(m => m.EstablishmentNumber))
                    .ForMember(x => x.GenderId, opt => opt.MapFrom(m => L.GendersGetAll().Id(m.Gendercode)))
                    .ForMember(x => x.HeadFirstName, opt => opt.MapFrom(m => m.HeadFirstName.Clean()))
                    .ForMember(x => x.HeadLastName, opt => opt.MapFrom(m => m.HeadLastName.Clean()))
                    .ForMember(x => x.HeadTitleId, opt => opt.MapFrom(m => L.HeadTitlesGetAll().Id(m.HeadTitlecode)))
                    .ForMember(x => x.LastChangedDate, opt => opt.MapFrom(m => m.LastChangedDate.ToDateTime(_dtFormats)))
                    .ForMember(x => x.LocalAuthorityId, opt => opt.MapFrom(m => L.LocalAuthorityGetAll().FirstOrDefault(l => l.Id == m.LAcode.ToInteger()).Id))
                    .ForMember(x => x.OpenDate, opt => opt.MapFrom(m => m.OpenDate.ToDateTime(_dtFormats)))
                    .ForMember(x => x.ProvisionBoardingId, opt => opt.MapFrom(m => L.ProvisionBoardingGetAll().Id(m.Boarderscode)))
                    .ForMember(x => x.ProvisionNurseryId, opt => opt.MapFrom(m => L.ProvisionNurseriesGetAll().Id(m.NurseryProvisioncode)))
                    .ForMember(x => x.ProvisionOfficialSixthFormId, opt => opt.MapFrom(m => L.ProvisionOfficialSixthFormsGetAll().Id(m.OfficialSixthFormcode)))
                    .ForMember(x => x.ProvisionSpecialClassesId, opt => opt.MapFrom(m => L.ProvisionSpecialClassesGetAll().Id(m.SpecialClassescode)))
                    .ForMember(x => x.ReasonEstablishmentClosedId, opt => opt.MapFrom(m => L.ReasonEstablishmentClosedGetAll().Id(m.ReasonEstablishmentClosedcode)))
                    .ForMember(x => x.ReasonEstablishmentOpenedId, opt => opt.MapFrom(m => L.ReasonEstablishmentOpenedGetAll().Id(m.ReasonEstablishmentOpenedcode)))
                    .ForMember(x => x.ReligiousCharacterId, opt => opt.MapFrom(m => L.ReligiousCharactersGetAll().Id(m.ReligiousCharactercode)))
                    .ForMember(x => x.ReligiousEthosId, opt => opt.MapFrom(m => L.ReligiousEthosGetAll().Id(m.ReligiousEthoscode)))
                    .ForMember(x => x.StatusId, opt => opt.MapFrom(m => L.EstablishmentStatusesGetAll().Id(m.EstablishmentStatuscode)))
                    .ForMember(x => x.StatutoryHighAge, opt => opt.MapFrom(m => m.StatutoryHighAge.ToInteger()))
                    .ForMember(x => x.StatutoryLowAge, opt => opt.MapFrom(m => m.StatutoryLowAge.ToInteger()))
                    .ForMember(x => x.TypeId, opt => opt.MapFrom(m => L.EstablishmentTypesGetAll().Id(m.TypeOfEstablishmentcode)))
                    .ForMember(x => x.UKPRN, opt => opt.MapFrom(m => m.UKPRN.ToInteger()))
                    .ForMember(x => x.Urn, opt => opt.MapFrom(m => m.URN.ToInteger()))
                    .ForAllOtherMembers(opt => opt.Ignore());


                cfg.CreateMap<LocalAuthority, Data.Entity.LocalAuthority>()
                    .ForMember(x => x.Id, opt => opt.MapFrom(m => m.Code.ToInteger().Value))
                    .ForMember(x => x.Name, opt => opt.MapFrom(m => m.Name.Clean()))
                    .ForMember(x => x.Group, opt => opt.MapFrom(m => m.C_Group))
                    .ForMember(x => x.Order, opt => opt.MapFrom(m => m.C_Order.ToInteger()))
                    .ForAllOtherMembers(opt => opt.Ignore());

                cfg.CreateMap<Groupdata, Trust>()
                    .ForMember(x => x.GroupUID, opt => opt.MapFrom(m => m.UID.ToInteger().Value))
                    .ForMember(x => x.Name, opt => opt.MapFrom(m => m.GroupName.Clean()))
                    .ForMember(x => x.CompaniesHouseNumber, opt => opt.MapFrom(m => m.CompaniesHouseNumber.Clean()))
                    .ForMember(x => x.GroupTypeId, opt => opt.MapFrom(m => L.GroupTypesGetAll().IdFromName(m.GroupType)))
                    .ForMember(x => x.ClosedDate, opt => opt.MapFrom(m => m.ClosedDate.ToDateTime(_dtFormats)))
                    .ForMember(x => x.StatusId, opt => opt.MapFrom(m => L.EstablishmentStatusesGetAll().IdFromName(m.Status)))
                    .ForMember(x => x.Head, opt => opt.MapFrom(m => groupPersonMapper.Map<Groupdata, Person>(m)))
                    .ForAllOtherMembers(opt => opt.Ignore());

            }).CreateMapper();
        }
        
        
    }
}
