using Edubase.Common;
using Edubase.Common.Spatial;
using Edubase.Data.Entity;
using Edubase.Services;
using System.Linq;
using Edubase.Data.Entity.ComplexTypes;
using AutoMapper;
using Edubase.Data;
using System.Collections.Generic;

namespace Edubase.Import.Mapping
{
    internal class MappingConfiguration
    {
        private static readonly string[] _dtFormats = new[] { "dd-MM-yyyy", "dd/MM/yyyy", "dd/MM/yy" };

        private static CachedLookupService L { get; set; } = new CachedLookupService();
        private static IMapper _mapper = null;

        public static IMapper Create(Dictionary<int, Ofstedratings> ofstedRatings)
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
                    cfg2.CreateMap<GroupData, Person>()
                        .ForMember(x => x.FirstName, opt => opt.MapFrom(m => m.HeadofGroupFirstName.Clean()))
                        .ForMember(x => x.LastName, opt => opt.MapFrom(m => m.HeadofGroupLastName.Clean()))
                        .ForMember(x => x.Title, opt => opt.MapFrom(m => m.HeadofGroupTitle.Remove("Not-applicable", "Unknown").Clean()))
                        .ForAllOtherMembers(opt => opt.Ignore());
                }).CreateMapper();

                var govenorPersonMapper = new MapperConfiguration(cfg2 =>
                {
                    cfg2.CreateMap<Governors, Person>()
                        .ForMember(x => x.FirstName, opt => opt.MapFrom(m => m.Forename1.Clean()))
                        .ForMember(x => x.LastName, opt => opt.MapFrom(m => m.Surname.Clean()))
                        .ForMember(x => x.MiddleName, opt => opt.MapFrom(m => m.Forename2.Clean()))
                        .ForMember(x => x.Title, opt => opt.MapFrom(m => m.Title.Remove("Not-applicable", "Unknown").Clean()))
                        .ForAllOtherMembers(opt => opt.Ignore());
                }).CreateMapper();

                var govenorPreviousPersonMapper = new MapperConfiguration(cfg2 =>
                {
                    cfg2.CreateMap<Governors, Person>()
                         .ForMember(x => x.FirstName, opt => opt.MapFrom(m => m.PreviousForename1.Clean()))
                         .ForMember(x => x.LastName, opt => opt.MapFrom(m => m.PreviousSurname.Clean()))
                         .ForMember(x => x.MiddleName, opt => opt.MapFrom(m => m.PreviousForename2.Clean()))
                         .ForMember(x => x.Title, opt => opt.MapFrom(m => m.PreviousTitle.Remove("Not-applicable", "Unknown").Clean()))
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
                    .ForMember(x => x.InspectorateId, opt => opt.MapFrom(m => L.InspectoratesGetAll().Id(m.Inspectoratecode)))
                    .ForMember(x => x.EstablishmentTypeGroupId, opt => opt.MapFrom(m => L.EstablishmentGroupTypesGetAll().Id(m.EstablishmentTypeGroupcode)))
                    .ForMember(x => x.Section41ApprovedId, opt => opt.MapFrom(m => L.Section41ApprovedGetAll().Id(m.Section41Approvedcode)))
                    .ForMember(x => x.UKPRN, opt => opt.MapFrom(m => m.UKPRN.ToInteger()))
                    .ForMember(x => x.ProprietorName, opt => opt.MapFrom(m => m.PropsName))
                    .ForMember(x => x.SENStat, opt => opt.MapFrom(m => m.SENStat.ToInteger()))
                    .ForMember(x => x.SENNoStat, opt => opt.MapFrom(m => m.SENNoStat.ToInteger()))
                    .ForMember(x => x.SEN1Id, opt => opt.MapFrom(m => m.SEN1code.ToInteger()))
                    .ForMember(x => x.SEN2Id, opt => opt.MapFrom(m => m.SEN2code.ToInteger()))
                    .ForMember(x => x.SEN3Id, opt => opt.MapFrom(m => m.SEN3code.ToInteger()))
                    .ForMember(x => x.SEN4Id, opt => opt.MapFrom(m => m.SEN4code.ToInteger()))
                    .ForMember(x => x.TeenageMothersProvisionId, opt => opt.MapFrom(m => m.TeenMothcode.ToInteger()))
                    .ForMember(x => x.TeenageMothersCapacity, opt => opt.MapFrom(m => m.TeenMothPlaces.ToInteger()))
                    .ForMember(x => x.ChildcareFacilitiesId, opt => opt.MapFrom(m => L.ChildcareFacilitiesGetAll().Id(m.CCFcode)))
                    .ForMember(x => x.PRUSENId, opt => opt.MapFrom(m => L.PRUSENsGetAll().Id(m.SENPRUcode)))
                    .ForMember(x => x.PRUEBDId, opt => opt.MapFrom(m => L.PRUEBDsGetAll().Id(m.EBDcode)))
                    .ForMember(x => x.PlacesPRU, opt => opt.MapFrom(m => m.PlacesPRU.ToInteger()))
                    .ForMember(x => x.PruFulltimeProvisionId, opt => opt.MapFrom(m => L.PruFulltimeProvisionsGetAll().Id(m.FTProvcode)))
                    .ForMember(x => x.PruEducatedByOthersId, opt => opt.MapFrom(m => L.PruEducatedByOthersGetAll().Id(m.EdByOthercode)))
                    .ForMember(x => x.TypeOfResourcedProvisionId, opt => opt.MapFrom(m => L.TypeOfResourcedProvisionsGetAll().Id(m.TypeOfResourcedProvisioncode)))
                    .ForMember(x => x.Urn, opt => opt.MapFrom(m => m.URN.ToInteger()))
                    .ForMember(x => x.ResourcedProvisionCapacity, opt => opt.MapFrom(m => m.ResourcedProvisionCapacity.ToInteger()))
                    .ForMember(x => x.ResourcedProvisionOnRoll, opt => opt.MapFrom(m => m.ResourcedProvisionOnRoll.ToInteger()))
                    .ForMember(x => x.SenUnitCapacity, opt => opt.MapFrom(m => m.SenUnitCapacity.ToInteger()))
                    .ForMember(x => x.SenUnitOnRoll, opt => opt.MapFrom(m => m.SenUnitOnRoll.ToInteger()))
                    .ForMember(x => x.GovernmentOfficeRegionId, opt => opt.MapFrom(m => L.GovernmentOfficeRegionsGetAll().Id(m.GORcode)))
                    .ForMember(x => x.AdministrativeDistrictId, opt => opt.MapFrom(m => L.AdministrativeDistrictsGetAll().Id(m.DistrictAdministrativecode)))
                    .ForMember(x => x.AdministrativeWardId, opt => opt.MapFrom(m => L.AdministrativeWardsGetAll().Id(m.AdministrativeWardcode)))
                    .ForMember(x => x.ParliamentaryConstituencyId, opt => opt.MapFrom(m => L.ParliamentaryConstituenciesGetAll().Id(m.ParliamentaryConstituencycode)))
                    .ForMember(x => x.UrbanRuralId, opt => opt.MapFrom(m => L.UrbanRuralGetAll().Id(m.UrbanRuralcode)))
                    .ForMember(x => x.GSSLAId, opt => opt.MapFrom(m => L.GSSLAGetAll().Id(m.GSSLACodecode)))
                    .ForMember(x => x.CASWardId, opt => opt.MapFrom(m => L.CASWardsGetAll().Id(m.CensusAreaStatisticWardcode)))
                    .ForMember(x => x.MSOAId, opt => opt.MapFrom(m => L.MSOAsGetAll().Id(m.MSOAcode)))
                    .ForMember(x => x.LSOAId, opt => opt.MapFrom(m => L.LSOAsGetAll().Id(m.LSOAcode)))
                    .ForMember(x => x.FurtherEducationTypeId, opt => opt.MapFrom(m => L.FurtherEducationTypesGetAll().Id(m.FurtherEducationTypecode)))
                    .ForMember(x => x.BSOInspectorateId, opt => opt.MapFrom(m => L.InspectorateNamesGetAll().Id(m.InspectorateNamecode)))
                    .ForMember(x => x.BSODateOfLastInspectionVisit, opt => opt.MapFrom(m => m.DateOfLastInspectionVisit.ToDateTime(_dtFormats)))
                    .ForMember(x => x.BSODateOfNextInspectionVisit, opt => opt.MapFrom(m => m.NextInspectionVisit.ToDateTime(_dtFormats)))
                    .ForMember(x => x.BSOInspectorateReportUrl, opt => opt.MapFrom(m => m.InspectorateReport))
                    .AfterMap((source, dest) =>
                    {
                        var rating = ofstedRatings.Get(dest.Urn);
                        if (rating != null)
                        {
                            dest.OfstedInspectionDate = rating.Inspectiondate.ToDateTime(_dtFormats);
                            dest.OfstedRating = System.Convert.ToByte(rating.Overalleffectiveness);
                        }
                    })
                    .ForAllOtherMembers(opt => opt.Ignore());


                cfg.CreateMap<LocalAuthority, Data.Entity.LocalAuthority>()
                    .ForMember(x => x.Id, opt => opt.MapFrom(m => m.Code.ToInteger().Value))
                    .ForMember(x => x.Name, opt => opt.MapFrom(m => m.Name.Clean()))
                    .ForMember(x => x.Group, opt => opt.MapFrom(m => m.C_Group))
                    .ForMember(x => x.Order, opt => opt.MapFrom(m => m.C_Order.ToInteger()))
                    .ForAllOtherMembers(opt => opt.Ignore());

                cfg.CreateMap<GroupData, Trust>()
                    .ForMember(x => x.GroupUID, opt => opt.MapFrom(m => m.UID.ToInteger().Value))
                    .ForMember(x => x.Name, opt => opt.MapFrom(m => m.GroupName.Clean()))
                    .ForMember(x => x.CompaniesHouseNumber, opt => opt.MapFrom(m => m.CompaniesHouseNumber.Clean()))
                    .ForMember(x => x.GroupTypeId, opt => opt.MapFrom(m => L.GroupTypesGetAll().IdFromName(m.GroupType)))
                    .ForMember(x => x.ClosedDate, opt => opt.MapFrom(m => m.ClosedDate.ToDateTime(_dtFormats)))
                    .ForMember(x => x.StatusId, opt => opt.MapFrom(m => L.EstablishmentStatusesGetAll().IdFromName(m.Status)))
                    .ForMember(x => x.Head, opt => opt.MapFrom(m => groupPersonMapper.Map<GroupData, Person>(m)))
                    .ForAllOtherMembers(opt => opt.Ignore());

                cfg.CreateMap<Governors, Governor>()
                    .ForMember(x => x.Id, opt => opt.MapFrom(x => x.GID.ToInteger()))
                    .ForMember(x => x.EstablishmentUrn, opt => opt.MapFrom(m => m.URN.ToInteger()))
                    .ForMember(x => x.AppointingBodyId, opt => opt.MapFrom(m => L.GovernorAppointingBodiesGetAll().IdFromName(m.Appointingbody)))
                    .ForMember(x => x.AppointmentEndDate, opt => opt.MapFrom(m => m.Datetermofofficeendsended.ToDateTime(_dtFormats)))
                    .ForMember(x => x.AppointmentStartDate, opt => opt.MapFrom(m => m.Dateofappointment.ToDateTime(_dtFormats)))
                    .ForMember(x => x.DOB, opt => opt.MapFrom(m => m.DateofBirth.ToDateTime(_dtFormats)))
                    .ForMember(x => x.EmailAddress, opt => opt.MapFrom(m => m.Directemailaddress.ToCleanEmail()))
                    .ForMember(x => x.UID, opt => opt.MapFrom(m => m.UID.ToInteger()))
                    .ForMember(x => x.IsDeleted, opt => opt.MapFrom(m => m.Deleted == "yes"))
                    .ForMember(x => x.Nationality, opt => opt.MapFrom(m => m.Nationality.Remove("N/A")))
                    .ForMember(x => x.PostCode, opt => opt.MapFrom(m => m.Homepostcode))
                    .ForMember(x => x.RoleId, opt => opt.MapFrom(m => L.GovernorRolesGetAll().IdFromName(m.Role)))
                    .ForMember(x => x.Person, opt => opt.MapFrom(m => govenorPersonMapper.Map<Governors, Person>(m)))
                    .ForMember(x => x.PreviousPerson, opt => opt.MapFrom(m => govenorPreviousPersonMapper.Map<Governors, Person>(m)))
                    .ForAllOtherMembers(opt => opt.Ignore());


                cfg.CreateMap<Establishmentlinks, EstablishmentLink>()
                    .ForMember(x => x.EstablishmentUrn, opt => opt.MapFrom(m => m.URN.ToInteger()))
                    .ForMember(x => x.LinkedEstablishmentUrn, opt => opt.MapFrom(m => m.LinkURN.ToInteger()))
                    .ForMember(x => x.LinkName, opt => opt.MapFrom(m => m.LinkName))
                    .ForMember(x => x.LinkTypeId, opt => opt.MapFrom(m => L.EstablishmentLinkTypesGetAll().IdFromName(m.LinkType)))
                    .ForMember(x => x.LinkEstablishedDate, opt => opt.MapFrom(m => m.LinkEstablishedDate.ToDateTime(_dtFormats)))
                    .ForAllOtherMembers(opt => opt.Ignore());

                cfg.CreateMap<GroupLinks, EstablishmentTrust>()
                    .ForMember(x => x.EstablishmentUrn, opt => opt.MapFrom(m => m.URN.ToInteger()))
                    .ForMember(x => x.TrustGroupUID, opt => opt.MapFrom(m => m.LinkedUID.ToInteger()))
                    .ForAllOtherMembers(opt => opt.Ignore());

            }).CreateMapper();
        }
        
        
    }
}
