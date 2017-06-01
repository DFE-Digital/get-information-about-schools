using Edubase.Common;
using Edubase.Common.Spatial;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Edubase.Services.Establishments.Models
{
    public class EstablishmentModel
    {
        [JsonProperty("ccLAContactDetail")]
        public ChildrensCentreLocalAuthorityDto CCLAContactDetail { get; set; }

        [JsonProperty("latLon")]
        public LatLon Location { get; set; }
        
        [JsonIgnore] // TODO: texchange: NOT DEFINED in YAML
        public eGovernanceMode? GovernanceMode { get; set; }

        [JsonProperty("iebtDetail")]
        public IEBTModel IEBTModel { get; set; }

        public int? Urn { get; set; }

        public int? LocalAuthorityId { get; set; }
        
        public int? EstablishmentNumber { get; set; }

        public string Name { get; set; }

        public int? StatusId { get; set; }

        public int? ReasonEstablishmentOpenedId { get; set; }

        public int? ReasonEstablishmentClosedId { get; set; }

        public DateTime? OpenDate { get; set; }

        public DateTime? CloseDate { get; set; }

        public int? EducationPhaseId { get; set; }

        public int? StatutoryLowAge { get; set; }

        public int? StatutoryHighAge { get; set; }

        [DisplayName("Boarders")]
        public int? ProvisionBoardingId { get; set; }

        [DisplayName("Nursery provision")]
        public int? ProvisionNurseryId { get; set; }

        [DisplayName("Official sixth form")]
        public int? ProvisionOfficialSixthFormId { get; set; }

        [DisplayName("Gender of entry")]
        public int? GenderId { get; set; }

        public int? ReligiousCharacterId { get; set; }

        public int? ReligiousEthosId { get; set; }

        public int? DioceseId { get; set; }

        public int? AdmissionsPolicyId { get; set; }

        public int? Capacity { get; set; }

        [DisplayName("Special classes")]
        public int? ProvisionSpecialClassesId { get; set; }

        [DisplayName("UKPRN"), JsonProperty("UKPRN")]
        public int? UKPRN { get; set; }

        public DateTime? LastChangedDate { get; set; }

        public string Address_Line1 { get; set; }

        public string Address_Line2 { get; set; }

        public string Address_Line3 { get; set; }

        public string Address_CityOrTown { get; set; }

        public int? Address_CountyId { get; set; }
        
        public int? Address_CountryId { get; set; }

        public string Address_Locality { get; set; }

        public string Address_PostCode { get; set; }


        #region Alt Address

        public string AltSiteName { get; set; }
        public int? AltCountryId { get; set; }
        public string AltUPRN { get; set; }
        public string AltStreet { get; set; }
        public string AltLocality { get; set; }
        public string AltAddress3 { get; set; }
        public string AltTown { get; set; }
        public int? AltCountyId { get; set; }
        public string AltPostCode { get; set; }

        #endregion


        [DisplayName("Headteacher/Principal first name")]
        public string HeadFirstName { get; set; }

        [DisplayName("Headteacher/Principal last name")]
        public string HeadLastName { get; set; }

        [DisplayName("Headteacher/Principal title")]
        public int? HeadTitleId { get; set; }

        public string HeadEmailAddress { get; set; }

        public string HeadPreferredJobTitle { get; set; }

        //public DateTime? HeadAppointmentDate { get; set; }

        public string Contact_TelephoneNumber { get; set; }

        public string Contact_EmailAddress { get; set; }

        public string Contact_WebsiteAddress { get; set; }

        public string Contact_FaxNumber { get; set; }

        public string ContactAlt_TelephoneNumber { get; set; }

        public string ContactAlt_EmailAddress { get; set; }

        public string ContactAlt_WebsiteAddress { get; set; }

        public string ContactAlt_FaxNumber { get; set; }

        [DisplayName("Establishment type")]
        public int? TypeId { get; set; }

        public int? Easting { get; set; }

        public int? Northing { get; set; }
        
        public int? EstablishmentTypeGroupId { get; set; }
        
        public int? OfstedRatingId { get; set; }

        public DateTime? OfstedInspectionDate { get; set; }

        public int? InspectorateId { get; set; }

        public int? Section41ApprovedId { get; set; }

        public string ProprietorName { get; set; }

        [DisplayName("Number of special pupils under a SEN statement/EHCP"), JsonProperty("SENStat")]
        public int? SENStat { get; set; }

        [DisplayName("Number of special pupils not under a SEN statement/EHCP"), JsonProperty("SENNoStat")]
        public int? SENNoStat { get; set; }

        // todo: TEXCHANGE: support new combined SENIds property
        //[DisplayName("Type of SEN provision 1")]

        //public int? SEN1Id { get; set; }

        //[DisplayName("Type of SEN provision 2")]

        //public int? SEN2Id { get; set; }

        //[DisplayName("Type of SEN provision 3")]

        //public int? SEN3Id { get; set; }

        //[DisplayName("Type of SEN provision 4")]

        //public int? SEN4Id { get; set; }

        [DisplayName("Teenage mothers provision")]
        public int? TeenageMothersProvisionId { get; set; }

        [DisplayName("Teenage mothers capacity")]
        public int? TeenageMothersCapacity { get; set; }

        [DisplayName("Childcare facilities provision")]
        public int? ChildcareFacilitiesId { get; set; }

        [DisplayName("SEN facilities"), JsonProperty("PRUSENId")]
        public int? PRUSENId { get; set; }

        [DisplayName("Pupils with EBD"), JsonProperty("PRUEBDId")]
        public int? PRUEBDId { get; set; }

        [DisplayName("Number of places"), JsonProperty("placesPRU")]
        public int? PlacesPRU { get; set; }

        [DisplayName("Full time provision")]
        public int? PruFulltimeProvisionId { get; set; }

        [DisplayName("Pupils educated by other providers")]
        public int? PruEducatedByOthersId { get; set; }

        public int? TypeOfResourcedProvisionId { get; set; }

        public int? ResourcedProvisionOnRoll { get; set; }

        public int? ResourcedProvisionCapacity { get; set; }

        public int? GovernmentOfficeRegionId { get; set; }

        public int? AdministrativeDistrictId { get; set; }

        public int? AdministrativeWardId { get; set; }

        public int? ParliamentaryConstituencyId { get; set; }

        [DisplayName("Urban / Rural description")]
        public int? UrbanRuralId { get; set; }

        [DisplayName("GSS LA code"), JsonProperty("GSSLAId")]
        public int? GSSLAId { get; set; }

        [DisplayName("Census ward"), JsonProperty("casWardId")]
        public int? CASWardId { get; set; }

        [DisplayName("Middle Super Output Area (MSOA)"), JsonProperty("MSOAId")]
        public int? MSOAId { get; set; }

        [DisplayName("Lower Super Output Area (LSOA)"), JsonProperty("LSOAId")]
        public int? LSOAId { get; set; }

        public int? FurtherEducationTypeId { get; set; }

        [JsonProperty("ccGovernanceId")]
        public int? CCGovernanceId { get; set; }

        [JsonProperty("ccGovernanceDetail")]
        public string CCGovernanceDetail { get; set; }

        [JsonProperty("ccOperationalHoursId")]
        public int? CCOperationalHoursId { get; set; }

        [JsonProperty("ccPhaseTypeId")]
        public int? CCPhaseTypeId { get; set; }

        [JsonProperty("ccGroupLeadId")]
        public int? CCGroupLeadId { get; set; }

        [JsonProperty("ccDisadvantagedAreaId")]
        public int? CCDisadvantagedAreaId { get; set; }

        [JsonProperty("ccDirectProvisionOfEarlyYearsId")]
        public int? CCDirectProvisionOfEarlyYearsId { get; set; }

        [JsonProperty("ccDeliveryModelId")]
        public int? CCDeliveryModelId { get; set; }

        [JsonProperty("ccUnder5YearsOfAgeCount")]
        public int? CCUnder5YearsOfAgeCount { get; set; }

        public int? SenUnitOnRoll { get; set; }

        public int? SenUnitCapacity { get; set; }

        /// <summary>
        /// Local Authority Id
        /// </summary>
        [DisplayName("RSC Region"), JsonProperty("rscRegionId")]
        public int? RSCRegionId { get; set; }

        [JsonProperty("bsoInspectorateId")]
        public int? BSOInspectorateId { get; set; }

        [JsonProperty("bsoInspectorateReportUrl")]
        public string BSOInspectorateReportUrl { get; set; }

        [JsonProperty("bsoDateOfLastInspectionVisit")]
        public DateTime? BSODateOfLastInspectionVisit { get; set; }

        [JsonProperty("bsoDateOfNextInspectionVisit")]
        public DateTime? BSODateOfNextInspectionVisit { get; set; }

        public DateTime? CreatedUtc { get; set; }

        public DateTime? LastUpdatedUtc { get; set; }
        
        public string GetAddress() => StringUtil.ConcatNonEmpties(", ", Address_Line1, Address_Line2, Address_Line3, Address_Locality, Address_CityOrTown, Address_CountyId.ToString(), Address_PostCode);
        public AddressDto GetAddressDto() => new AddressDto { Line1 = Address_Line1, Line2 = Address_Line2, Line3 = Address_Line3, CityOrTown = Address_CityOrTown, County = Address_CountyId.ToString(), PostCode = Address_PostCode, Country = Address_CountryId.ToString() };
        public string GetLAESTAB() => string.Concat(LocalAuthorityId, "/", EstablishmentNumber.GetValueOrDefault().ToString("D4"));

        public string HelpdeskNotes { get; set; }
        public DateTime? HelpdeskLastUpdate { get; set; }
        public string HelpdeskTrigger1 { get; set; }
        public int? HelpdeskPreviousLocalAuthorityId { get; set; }
        public string HelpdeskPreviousEstablishmentNumber { get; set; }
    }
}
