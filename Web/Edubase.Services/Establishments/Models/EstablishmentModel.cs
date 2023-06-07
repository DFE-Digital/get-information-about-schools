using Edubase.Common;
using Edubase.Common.Reflection;
using Edubase.Common.Spatial;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Lookup;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Edubase.Services.Establishments.Models
{
    public class EstablishmentModel
    {
        [JsonProperty("ccLAContactDetail")]
        public ChildrensCentreLocalAuthorityDto CCLAContactDetail { get; set; }

        [JsonProperty("latLon")]
        public LatLon Location { get; set; }
        
        public int? GovernanceModeId { get; set; }

        [JsonIgnore]
        public eGovernanceMode? GovernanceMode => GovernanceModeId.HasValue ? (eGovernanceMode) GovernanceModeId.Value : null as eGovernanceMode?;

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

        public DateTime? AccreditationExpiryDate { get; set; }

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

        [DisplayName("Number of pupils")]
        public int? NumberOfPupilsOnRoll { get; set; }


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

        public string Address_UPRN { get; set; }

        public AdditionalAddressModel[] AdditionalAddresses { get; set; } = new AdditionalAddressModel[0];

        [DisplayName("Headteacher/principal/manager first name")]
        public string HeadFirstName { get; set; }

        [DisplayName("Headteacher/principal/manager last name")]
        public string HeadLastName { get; set; }

        [DisplayName("Headteacher/principal/manager title")]
        public int? HeadTitleId { get; set; }

        [DisplayName("Headteacher/principal/manager appointment date")]
        public DateTime? HeadAppointmentDate { get; set; }

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

        [DisplayName("Establishment type")]
        public int? TypeId { get; set; }

        public int? Easting { get; set; }

        public int? Northing { get; set; }
        
        public int? EstablishmentTypeGroupId { get; set; }
        
        public int? OfstedRatingId { get; set; }

        public DateTime? OfstedInspectionDate { get; set; }

        public int? InspectorateId { get; set; }

        public int? Section41ApprovedId { get; set; }

        public string ProprietorBodyName { get; set; }

        [DisplayName("Number of special pupils under a special educational needs (SEN) statement/education, health and care (EHC) plan"), JsonProperty("SENStat")]
        public int? SENStat { get; set; }

        [DisplayName("Number of special pupils not under a special educational needs (SEN) statement/education, health and care (EHC) plan"), JsonProperty("SENNoStat")]
        public int? SENNoStat { get; set; }
        
        [JsonProperty("SEN1Ids")]
        public int[] SENIds { get; set; }
        
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

        [DisplayName("Middle Super Output Area (MSOA)"), JsonProperty("MSOAId")]
        public int? MSOAId { get; set; }

        [DisplayName("Lower Super Output Area (LSOA)"), JsonProperty("LSOAId")]
        public int? LSOAId { get; set; }

        public int? FurtherEducationTypeId { get; set; }

        [DisplayName("Governance"), JsonProperty("ccGovernanceId")]
        public int? CCGovernanceId { get; set; }

        [DisplayName("Governance detail"), JsonProperty("ccGovernanceDetail")]
        public string CCGovernanceDetail { get; set; }

        [DisplayName("Operational hours"), JsonProperty("ccOperationalHoursId")]
        public int? CCOperationalHoursId { get; set; }

        [DisplayName("Phase"), JsonProperty("ccPhaseTypeId")]
        public int? CCPhaseTypeId { get; set; }

        [DisplayName("Group lead centre"), JsonProperty("ccGroupLeadId")]
        public int? CCGroupLeadId { get; set; }

        [DisplayName("Disadvantaged area"), JsonProperty("ccDisadvantagedAreaId")]
        public int? CCDisadvantagedAreaId { get; set; }

        [DisplayName("Direct provision of early years"), JsonProperty("ccDirectProvisionOfEarlyYearsId")]
        public int? CCDirectProvisionOfEarlyYearsId { get; set; }

        [DisplayName("Delivery model"), JsonProperty("ccDeliveryModelId")]
        public int? CCDeliveryModelId { get; set; }

        [DisplayName("Number of under 5s"), JsonProperty("ccUnder5YearsOfAgeCount")]
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
        public async Task<string> GetAddressAsync(ICachedLookupService lookup) => StringUtil.ConcatNonEmpties(", ", Address_Line1, Address_Locality, Address_Line3, Address_CityOrTown, await lookup.GetNameAsync("CountyId", Address_CountyId), Address_PostCode);
        public string GetLAESTAB(string laCode) => string.Concat(laCode, "/", EstablishmentNumber.GetValueOrDefault().ToString("D4"));
        public string HelpdeskNotes { get; set; }
        public DateTime? HelpdeskLastUpdate { get; set; }
        public string HelpdeskTrigger1 { get; set; }
        public int? HelpdeskPreviousLocalAuthorityId { get; set; }
        public int? HelpdeskPreviousEstablishmentNumber { get; set; }
        
        [DisplayName("Number of pupils eligible for free school meals")]
        public int? FreeSchoolMealsNumber { get; set; }

        [DisplayName("Percentage of children eligible for free school meals")]
        public double? FreeSchoolMealsPercentage { get; set; }

        public bool ConfirmationUpToDateRequired { get; set; }
        public bool ConfirmationUpToDateGovernanceRequired { get; set; }
        public DateTime? ConfirmationUpToDate_LastConfirmationDate { get; set; }
        public DateTime? ConfirmationUpToDateGovernance_LastConfirmationDate { get; set; }
        public bool UrgentConfirmationUpToDateRequired { get; set; }
        public bool UrgentConfirmationUpToDateGovernanceRequired { get; set; }

        public int? QualityAssuranceBodyNameId { get; set; }
        public string QualityAssuranceBodyReport { get; set; }
        public string CompaniesHouseNumber { get; set; }
        public int? EstablishmentAccreditedId { get; set; }
    }
}
