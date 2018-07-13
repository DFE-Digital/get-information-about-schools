using Newtonsoft.Json;
using System.Collections.Generic;

namespace Edubase.Services.Establishments.Models
{
    /// <summary>
    /// Represents the list of fields within an Establishment.
    /// </summary>
    public abstract class EstablishmentFieldListBase
    {
        public virtual bool Urn { get; set; }

        public virtual bool LocalAuthorityId { get; set; }

        public virtual bool EstablishmentNumber { get; set; }

        public virtual bool Name { get; set; }

        public virtual bool StatusId { get; set; }

        public virtual bool ReasonEstablishmentOpenedId { get; set; }

        public virtual bool ReasonEstablishmentClosedId { get; set; }

        public virtual bool OpenDate { get; set; }

        public virtual bool CloseDate { get; set; }

        public virtual bool EducationPhaseId { get; set; }

        public virtual bool StatutoryLowAge { get; set; }

        public virtual bool StatutoryHighAge { get; set; }

        public virtual bool ProvisionBoardingId { get; set; }

        public virtual bool ProvisionNurseryId { get; set; }

        public virtual bool ProvisionOfficialSixthFormId { get; set; }

        public virtual bool GenderId { get; set; }

        public virtual bool ReligiousCharacterId { get; set; }

        public virtual bool ReligiousEthosId { get; set; }

        public virtual bool DioceseId { get; set; }

        public virtual bool AdmissionsPolicyId { get; set; }

        public virtual bool Capacity { get; set; }

        public virtual bool ProvisionSpecialClassesId { get; set; }
        public virtual bool NumberOfPupilsOnRoll { get; set; }

        public virtual bool UKPRN { get; set; }

        public virtual bool LastChangedDate { get; set; }

        public virtual bool Address_Line1 { get; set; }

        public virtual bool Address_Line2 { get; set; }

        public virtual bool Address_Line3 { get; set; }

        public virtual bool Address_CityOrTown { get; set; }

        public virtual bool Address_CountyId { get; set; }

        public virtual bool Address_CountryId { get; set; }
        public virtual bool Address_UPRN { get; set; }

        public virtual bool Address_Locality { get; set; }

        public virtual bool Address_PostCode { get; set; }

        public virtual bool HeadFirstName { get; set; }

        public virtual bool HeadLastName { get; set; }

        public virtual bool HeadTitleId { get; set; }

        public virtual bool HeadEmailAddress { get; set; }

        public virtual bool HeadAppointmentDate { get; set; }

        public virtual bool Contact_TelephoneNumber { get; set; }

        public virtual bool Contact_EmailAddress { get; set; }

        public virtual bool Contact_WebsiteAddress { get; set; }

        public virtual bool Contact_FaxNumber { get; set; }

        public virtual bool ContactAlt_TelephoneNumber { get; set; }

        public virtual bool ContactAlt_EmailAddress { get; set; }

        public virtual bool ContactAlt_WebsiteAddress { get; set; }

        public virtual bool ContactAlt_FaxNumber { get; set; }

        public virtual bool TypeId { get; set; }

        


        public virtual bool EstablishmentTypeGroupId { get; set; }

        public virtual bool OfstedRatingId { get; set; }

        public virtual bool OfstedInspectionDate { get; set; }

        public virtual bool InspectorateId { get; set; }

        public virtual bool Section41ApprovedId { get; set; }

        public virtual bool ProprietorName { get; set; }

        public virtual bool SENStat { get; set; }

        public virtual bool SENNoStat { get; set; }

        [JsonProperty("SEN1Ids")]
        public virtual bool SENIds { get; set; }

        public virtual bool TeenageMothersProvisionId { get; set; }

        public virtual bool TeenageMothersCapacity { get; set; }

        public virtual bool ChildcareFacilitiesId { get; set; }

        public virtual bool PRUSENId { get; set; }

        public virtual bool PRUEBDId { get; set; }

        public virtual bool PlacesPRU { get; set; }

        public virtual bool PruFulltimeProvisionId { get; set; }

        public virtual bool PruEducatedByOthersId { get; set; }

        public virtual bool TypeOfResourcedProvisionId { get; set; }

        public virtual bool ResourcedProvisionOnRoll { get; set; }

        public virtual bool ResourcedProvisionCapacity { get; set; }


        public virtual bool RSCRegionId { get; set; }
        public virtual bool GovernmentOfficeRegionId { get; set; }
        public virtual bool AdministrativeDistrictId { get; set; }
        public virtual bool AdministrativeWardId { get; set; }
        public virtual bool ParliamentaryConstituencyId { get; set; }
        public virtual bool UrbanRuralId { get; set; }
        public virtual bool GSSLAId { get; set; }
        public virtual bool CASWardId { get; set; }
        public virtual bool MSOAId { get; set; }
        public virtual bool LSOAId { get; set; }
        public virtual bool Easting { get; set; }
        public virtual bool Northing { get; set; }

        public virtual bool FurtherEducationTypeId { get; set; }

        public virtual bool CCGovernanceId { get; set; }

        public virtual bool CCGovernanceDetail { get; set; }

        public virtual bool CCOperationalHoursId { get; set; }

        public virtual bool CCPhaseTypeId { get; set; }

        public virtual bool CCGroupLeadId { get; set; }

        public virtual bool CCDisadvantagedAreaId { get; set; }

        public virtual bool CCDirectProvisionOfEarlyYearsId { get; set; }

        public virtual bool CCDeliveryModelId { get; set; }

        public virtual bool CCUnder5YearsOfAgeCount { get; set; }

        public virtual bool SenUnitOnRoll { get; set; }

        public virtual bool SenUnitCapacity { get; set; }
        
        public virtual bool BSOInspectorateId { get; set; }

        public virtual bool BSOInspectorateReportUrl { get; set; }

        public virtual bool BSODateOfLastInspectionVisit { get; set; }

        public virtual bool BSODateOfNextInspectionVisit { get; set; }

        public virtual bool HeadPreferredJobTitle { get; set; }

        public bool HelpdeskNotes { get; set; }
        public bool HelpdeskLastUpdate { get; set; }
        public bool HelpdeskTrigger1 { get; set; }
        public bool HelpdeskPreviousLocalAuthorityId { get; set; }
        public bool HelpdeskPreviousEstablishmentNumber { get; set; }
        
        public bool AdditionalAddresses { get; set; }

        public bool FreeSchoolMealsNumber { get; set; }
        public bool FreeSchoolMealsPercentage { get; set; }

    }
}
