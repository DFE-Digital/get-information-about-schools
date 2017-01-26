using Edubase.Common;
using Edubase.Common.Spatial;
using System;

namespace Edubase.Services.Establishments.Models
{
    public abstract class EstablishmentModelBase
    {
        public int? Urn { get; set; }

        public int? LocalAuthorityId { get; set; }

        public int? EstablishmentNumber { get; set; }

        public string Name { get; set; }
        public string NameDistilled { get; set; }

        public int? StatusId { get; set; }

        public int? ReasonEstablishmentOpenedId { get; set; }

        public int? ReasonEstablishmentClosedId { get; set; }

        public DateTime? OpenDate { get; set; }

        public DateTime? CloseDate { get; set; }

        public int? EducationPhaseId { get; set; }

        public int? StatutoryLowAge { get; set; }

        public int? StatutoryHighAge { get; set; }

        public int? ProvisionBoardingId { get; set; }

        public int? ProvisionNurseryId { get; set; }

        public int? ProvisionOfficialSixthFormId { get; set; }

        public int? GenderId { get; set; }

        public int? ReligiousCharacterId { get; set; }

        public int? ReligiousEthosId { get; set; }

        public int? DioceseId { get; set; }

        public int? AdmissionsPolicyId { get; set; }

        public int? Capacity { get; set; }

        public int? ProvisionSpecialClassesId { get; set; }

        public int? UKPRN { get; set; }

        public DateTime? LastChangedDate { get; set; }

        public string Address_Line1 { get; set; }

        public string Address_Line2 { get; set; }

        public string Address_Line3 { get; set; }

        public string Address_CityOrTown { get; set; }

        public string Address_County { get; set; }

        public string Address_Country { get; set; }

        public string Address_Locality { get; set; }

        public string Address_PostCode { get; set; }

        public string HeadFirstName { get; set; }

        public string HeadLastName { get; set; }

        public int? HeadTitleId { get; set; }

        public string HeadEmailAddress { get; set; }

        public string HeadPreferredJobTitle { get; set; }

        public string Contact_TelephoneNumber { get; set; }

        public string Contact_EmailAddress { get; set; }

        public string Contact_WebsiteAddress { get; set; }

        public string Contact_FaxNumber { get; set; }

        public string ContactAlt_TelephoneNumber { get; set; }

        public string ContactAlt_EmailAddress { get; set; }

        public string ContactAlt_WebsiteAddress { get; set; }

        public string ContactAlt_FaxNumber { get; set; }

        public int? TypeId { get; set; }

        public int? Easting { get; set; }

        public int? Northing { get; set; }
        
        public abstract LatLon Coordinate { get; }
        
        public int? EstablishmentTypeGroupId { get; set; }

        public byte? OfstedRating { get; set; }

        public DateTime? OfstedInspectionDate { get; set; }

        public int? InspectorateId { get; set; }

        public int? Section41ApprovedId { get; set; }

        public string ProprietorName { get; set; }

        public int? SENStat { get; set; }

        public int? SENNoStat { get; set; }

        public int? SEN1Id { get; set; }

        public int? SEN2Id { get; set; }

        public int? SEN3Id { get; set; }

        public int? SEN4Id { get; set; }

        public int? TeenageMothersProvisionId { get; set; }

        public int? TeenageMothersCapacity { get; set; }

        public int? ChildcareFacilitiesId { get; set; }

        public int? PRUSENId { get; set; }

        public int? PRUEBDId { get; set; }

        public int? PlacesPRU { get; set; }

        public int? PruFulltimeProvisionId { get; set; }

        public int? PruEducatedByOthersId { get; set; }

        public int? TypeOfResourcedProvisionId { get; set; }

        public int? ResourcedProvisionOnRoll { get; set; }

        public int? ResourcedProvisionCapacity { get; set; }

        public int? GovernmentOfficeRegionId { get; set; }

        public int? AdministrativeDistrictId { get; set; }

        public int? AdministrativeWardId { get; set; }

        public int? ParliamentaryConstituencyId { get; set; }

        public int? UrbanRuralId { get; set; }

        public int? GSSLAId { get; set; }

        public int? CASWardId { get; set; }

        public int? MSOAId { get; set; }

        public int? LSOAId { get; set; }

        public int? FurtherEducationTypeId { get; set; }

        public int? CCGovernanceId { get; set; }

        public string CCGovernanceDetail { get; set; }

        public int? CCOperationalHoursId { get; set; }

        public int? CCPhaseTypeId { get; set; }

        public int? CCGroupLeadId { get; set; }

        public int? CCDisadvantagedAreaId { get; set; }

        public int? CCDirectProvisionOfEarlyYearsId { get; set; }

        public int? CCDeliveryModelId { get; set; }

        public int? CCUnder5YearsOfAgeCount { get; set; }

        public int? SenUnitOnRoll { get; set; }

        public int? SenUnitCapacity { get; set; }

        /// <summary>
        /// Local Authority Id
        /// </summary>
        public int? RSCRegionId { get; set; }

        public int? BSOInspectorateId { get; set; }

        public string BSOInspectorateReportUrl { get; set; }

        public DateTime? BSODateOfLastInspectionVisit { get; set; }

        public DateTime? BSODateOfNextInspectionVisit { get; set; }

        public DateTime? CreatedUtc { get; set; }

        public DateTime? LastUpdatedUtc { get; set; }

        public bool? IsDeleted { get; set; }
        
        public string GetAddress() => StringUtil.ConcatNonEmpties(", ", Address_Line1, Address_Line2, Address_Line3, Address_Locality, Address_CityOrTown, Address_County, Address_PostCode);
        public string GetLAESTAB() => string.Concat(LocalAuthorityId, EstablishmentNumber.GetValueOrDefault().ToString("D4"));
    }
}
