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
        public ChildrensCentreLocalAuthorityDto CCLAContactDetail { get; set; }

        [JsonProperty("latLon")]
        public LatLon Location { get; set; }
        
        public eGovernanceMode? GovernanceMode { get; set; }

        #region IEBT properties
        public string Notes { get; set; }
        public DateTime? DateOfTheLastBridgeVisit { get; set; }

        [DisplayName("Date of the last ISI visit")]
        public DateTime? DateOfTheLastISIVisit { get; set; }

        public DateTime? DateOfTheLastWelfareVisit { get; set; }

        [DisplayName("Date of the last FP visit")]
        public DateTime? DateOfTheLastFPVisit { get; set; }

        [DisplayName("Date of the last SIS visit")]
        public DateTime? DateOfTheLastSISVisit { get; set; }
        public DateTime? NextOfstedVisit { get; set; }
        public DateTime? NextGeneralActionRequired { get; set; }

        [DisplayName("Next action required by WEL")]
        public DateTime? NextActionRequiredByWEL { get; set; }

        [DisplayName("Next action required by FP")]
        public DateTime? NextActionRequiredByFP { get; set; }
        
        public int? IndependentSchoolTypeId { get; set; } 
        public string CharityOrganisation { get; set; }
        public int? CharityRegistrationNumber { get; set; }
        public int? TotalNumberOfFullTimePupils { get; set; }
        public int? TotalNumberOfPartTimePupils { get; set; }
        public int? TotalNumberOfPupilsOfCompulsorySchoolAge { get; set; }

        [Obsolete("Use SENStat/NoStat")]
        [DisplayName("Total number of special pupils under a SEN statement/ECHP")]
        public int? NumberOfSpecialPupilsUnderASENStatementEHCP { get; set; }

        [Obsolete("Use SENStat/NoStat")]
        [DisplayName("Number of special pupils not under a SEN statement/EHCP")]
        public int? NumberOfSpecialPupilsNotUnderASENStatementEHCP { get; set; }


        public int? TotalNumberOfPupilsInPublicCare { get; set; }

        [DisplayName("PT boys (aged 2 and under)")]
        public int? PTBoysAged2AndUnder { get; set; }

        [DisplayName("PT boys (aged 3)")]
        public int? PTBoysAged3 { get; set; }

        [DisplayName("PT boys (aged 4a)")]
        public int? PTBoysAged4A { get; set; }

        [DisplayName("PT boys (aged 4b)")]
        public int? PTBoysAged4B { get; set; }

        [DisplayName("PT boys (aged 4c)")]
        public int? PTBoysAged4C { get; set; }

        public int? TotalNumberOfBoysInBoardingSchools { get; set; }

        [DisplayName("PT girls (aged 2 and under)")]
        public int? PTGirlsAged2AndUnder { get; set; }

        [DisplayName("PT girls (aged 3)")]
        public int? PTGirlsAged3 { get; set; }

        [DisplayName("PT girls (aged 4a)")]
        public int? PTGirlsAged4A { get; set; }

        [DisplayName("PT girls (aged 4b)")]
        public int? PTGirlsAged4B { get; set; }

        [DisplayName("PT girls (aged 4c)")]
        public int? PTGirlsAged4C { get; set; }

        public int? TotalNumberOfGirlsInBoardingSchools { get; set; }
        public int? TotalNumberOfFullTimeStaff { get; set; }
        public int? TotalNumberOfPartTimeStaff { get; set; }
        public int? LowestAnnualRateForDayPupils { get; set; }
        public int? HighestAnnualRateForDayPupils { get; set; }
        public int? LowestAnnualRateForBoardingPupils { get; set; }
        public int? HighestAnnualRateForBoardingPupils { get; set; }
        //public Lookup BoardingEstablishment { get; set; } //ProvisionBoardingId
        //public string ProprietorsName { get; set; } //ProprietorName
        public string ProprietorsStreet { get; set; }
        public string ProprietorsLocality { get; set; }
        public string ProprietorsAddress3 { get; set; }
        public string ProprietorsTown { get; set; }
        public string ProprietorsCounty { get; set; }
        public string ProprietorsPostcode { get; set; }
        public string ProprietorsTelephoneNumber { get; set; }
        public string ProprietorsFaxNumber { get; set; }
        public string ProprietorsEmail { get; set; }
        public string ProprietorsPreferredJobTitle { get; set; }
        public string ChairOfProprietorsBodyName { get; set; }
        public string ChairOfProprietorsBodyStreet { get; set; }
        public string ChairOfProprietorsBodyLocality { get; set; }
        public string ChairOfProprietorsBodyAddress3 { get; set; }
        public string ChairOfProprietorsBodyTown { get; set; }
        public string ChairOfProprietorsBodyCounty { get; set; }
        public string ChairOfProprietorsBodyPostcode { get; set; }
        public string ChairOfProprietorsBodyTelephoneNumber { get; set; }
        public string ChairOfProprietorsBodyFaxNumber { get; set; }
        public string ChairOfProprietorsBodyEmail { get; set; }
        public string ChairOfProprietorsBodyPreferredJobTitle { get; set; }
        public int? AccommodationChangedId { get; set; }

        #endregion


        public int? Urn { get; set; }

        public int? LocalAuthorityId { get; set; }
        
        public int? EstablishmentNumber { get; set; }

        public string Name { get; set; }

        public string NameDistilled { get; internal set; }

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

        [DisplayName("UKPRN")]
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

        public byte? OfstedRating { get; set; }

        public DateTime? OfstedInspectionDate { get; set; }

        public int? InspectorateId { get; set; }

        public int? Section41ApprovedId { get; set; }

        public string ProprietorName { get; set; }

        [DisplayName("Number of special pupils under a SEN statement/EHCP")]
        public int? SENStat { get; set; }

        [DisplayName("Number of special pupils not under a SEN statement/EHCP")]
        public int? SENNoStat { get; set; }

        [DisplayName("Type of SEN provision 1")]

        public int? SEN1Id { get; set; }

        [DisplayName("Type of SEN provision 2")]

        public int? SEN2Id { get; set; }

        [DisplayName("Type of SEN provision 3")]

        public int? SEN3Id { get; set; }

        [DisplayName("Type of SEN provision 4")]

        public int? SEN4Id { get; set; }

        [DisplayName("Teenage mothers provision")]
        public int? TeenageMothersProvisionId { get; set; }

        [DisplayName("Teenage mothers capacity")]
        public int? TeenageMothersCapacity { get; set; }

        [DisplayName("Childcare facilities provision")]
        public int? ChildcareFacilitiesId { get; set; }

        [DisplayName("SEN facilities")]
        public int? PRUSENId { get; set; }

        [DisplayName("Pupils with EBD")]
        public int? PRUEBDId { get; set; }

        [DisplayName("Number of places")]
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

        [DisplayName("GSS LA code")]
        public int? GSSLAId { get; set; }

        [DisplayName("Census ward")]
        public int? CASWardId { get; set; }

        [DisplayName("Middle Super Output Area (MSOA)")]
        public int? MSOAId { get; set; }

        [DisplayName("Lower Super Output Area (LSOA)")]
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
        [DisplayName("RSC Region")]
        public int? RSCRegionId { get; set; }

        public int? BSOInspectorateId { get; set; }

        public string BSOInspectorateReportUrl { get; set; }

        public DateTime? BSODateOfLastInspectionVisit { get; set; }

        public DateTime? BSODateOfNextInspectionVisit { get; set; }

        public DateTime? CreatedUtc { get; set; }

        public DateTime? LastUpdatedUtc { get; set; }

        public bool? IsDeleted { get; set; }
        
        public string GetAddress() => StringUtil.ConcatNonEmpties(", ", Address_Line1, Address_Line2, Address_Line3, Address_Locality, Address_CityOrTown, Address_County, Address_PostCode);
        public AddressDto GetAddressDto() => new AddressDto { Line1 = Address_Line1, Line2 = Address_Line2, Line3 = Address_Line3, CityOrTown = Address_CityOrTown, County = Address_County, PostCode = Address_PostCode, Country = Address_Country };
        public string GetLAESTAB() => string.Concat(LocalAuthorityId, "/", EstablishmentNumber.GetValueOrDefault().ToString("D4"));
    }
}
