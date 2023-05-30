using Newtonsoft.Json;

namespace Edubase.Services.Establishments.Models
{
    /// <summary>
    /// Represents the list of fields within an Establishment.
    /// </summary>
    public abstract class EstablishmentFieldListBase<T>
    {
        public virtual T Urn { get; set; }

        public virtual T LocalAuthorityId { get; set; }

        public virtual T EstablishmentNumber { get; set; }

        public virtual T Name { get; set; }

        public virtual T StatusId { get; set; }

        public virtual T ReasonEstablishmentOpenedId { get; set; }

        public virtual T ReasonEstablishmentClosedId { get; set; }

        public virtual T OpenDate { get; set; }

        public virtual T CloseDate { get; set; }

        public virtual T AccreditationExpiryDate { get; set; }

        public virtual T EducationPhaseId { get; set; }

        public virtual T StatutoryLowAge { get; set; }

        public virtual T StatutoryHighAge { get; set; }

        public virtual T ProvisionBoardingId { get; set; }

        public virtual T ProvisionNurseryId { get; set; }

        public virtual T ProvisionOfficialSixthFormId { get; set; }

        public virtual T GenderId { get; set; }

        public virtual T ReligiousCharacterId { get; set; }

        public virtual T ReligiousEthosId { get; set; }

        public virtual T DioceseId { get; set; }

        public virtual T AdmissionsPolicyId { get; set; }

        public virtual T Capacity { get; set; }

        public virtual T ProvisionSpecialClassesId { get; set; }
        public virtual T NumberOfPupilsOnRoll { get; set; }

        public virtual T UKPRN { get; set; }

        public virtual T LastChangedDate { get; set; }

        public virtual T Address_Line1 { get; set; }

        public virtual T Address_Line2 { get; set; }

        public virtual T Address_Line3 { get; set; }

        public virtual T Address_CityOrTown { get; set; }

        public virtual T Address_CountyId { get; set; }

        public virtual T Address_CountryId { get; set; }
        public virtual T Address_UPRN { get; set; }

        public virtual T Address_Locality { get; set; }

        public virtual T Address_PostCode { get; set; }

        public virtual T HeadFirstName { get; set; }

        public virtual T HeadLastName { get; set; }

        public virtual T HeadTitleId { get; set; }

        public virtual T HeadEmailAddress { get; set; }

        public virtual T HeadAppointmentDate { get; set; }

        public virtual T Contact_TelephoneNumber { get; set; }

        public virtual T Contact_EmailAddress { get; set; }

        public virtual T Contact_WebsiteAddress { get; set; }

        public virtual T Contact_FaxNumber { get; set; }

        public virtual T ContactAlt_TelephoneNumber { get; set; }

        public virtual T ContactAlt_EmailAddress { get; set; }

        public virtual T ContactAlt_WebsiteAddress { get; set; }

        public virtual T ContactAlt_FaxNumber { get; set; }

        public virtual T TypeId { get; set; }

        public virtual T EstablishmentTypeGroupId { get; set; }

        public virtual T OfstedRatingId { get; set; }

        public virtual T OfstedInspectionDate { get; set; }

        public virtual T InspectorateId { get; set; }

        public virtual T Section41ApprovedId { get; set; }

        public virtual T ProprietorBodyName { get; set; }

        public virtual T SENStat { get; set; }

        public virtual T SENNoStat { get; set; }

        [JsonProperty("SEN1Ids")]
        public virtual T SENIds { get; set; }

        public virtual T TeenageMothersProvisionId { get; set; }

        public virtual T TeenageMothersCapacity { get; set; }

        public virtual T ChildcareFacilitiesId { get; set; }

        public virtual T PRUSENId { get; set; }

        public virtual T PRUEBDId { get; set; }

        public virtual T PlacesPRU { get; set; }

        public virtual T PruFulltimeProvisionId { get; set; }

        public virtual T PruEducatedByOthersId { get; set; }

        public virtual T TypeOfResourcedProvisionId { get; set; }

        public virtual T ResourcedProvisionOnRoll { get; set; }

        public virtual T ResourcedProvisionCapacity { get; set; }

        public virtual T RSCRegionId { get; set; }
        public virtual T GovernmentOfficeRegionId { get; set; }
        public virtual T AdministrativeDistrictId { get; set; }
        public virtual T AdministrativeWardId { get; set; }
        public virtual T ParliamentaryConstituencyId { get; set; }
        public virtual T UrbanRuralId { get; set; }
        public virtual T GSSLAId { get; set; }
        public virtual T MSOAId { get; set; }
        public virtual T LSOAId { get; set; }
        public virtual T MSOACode => MSOAId;
        public virtual T LSOACode => LSOAId;
        public virtual T Easting { get; set; }
        public virtual T Northing { get; set; }

        public virtual T FurtherEducationTypeId { get; set; }

        public virtual T CCGovernanceId { get; set; }

        public virtual T CCGovernanceDetail { get; set; }

        public virtual T CCOperationalHoursId { get; set; }

        public virtual T CCPhaseTypeId { get; set; }

        public virtual T CCGroupLeadId { get; set; }

        public virtual T CCDisadvantagedAreaId { get; set; }

        public virtual T CCDirectProvisionOfEarlyYearsId { get; set; }

        public virtual T CCDeliveryModelId { get; set; }

        public virtual T CCUnder5YearsOfAgeCount { get; set; }

        public virtual T SenUnitOnRoll { get; set; }

        public virtual T SenUnitCapacity { get; set; }

        public virtual T BSOInspectorateId { get; set; }

        public virtual T BSOInspectorateReportUrl { get; set; }

        public virtual T BSODateOfLastInspectionVisit { get; set; }

        public virtual T BSODateOfNextInspectionVisit { get; set; }

        public virtual T HeadPreferredJobTitle { get; set; }

        public T HelpdeskNotes { get; set; }
        public T HelpdeskLastUpdate { get; set; }
        public T HelpdeskTrigger1 { get; set; }
        public T HelpdeskPreviousLocalAuthorityId { get; set; }
        public T HelpdeskPreviousEstablishmentNumber { get; set; }

        public T AdditionalAddresses { get; set; }

        public T FreeSchoolMealsNumber { get; set; }
        public T FreeSchoolMealsPercentage { get; set; }

        public T QualityAssuranceBodyNameId { get; set; }
        public T QualityAssuranceBodyReport { get; set; }
        public T CompaniesHouseNumber { get; set; }
        public T EstablishmentAccreditedId { get; set; }
    }
}
