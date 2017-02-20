using Edubase.Common;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.Models;
using Edubase.Web.UI.Models.Establishments;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Edubase.Web.UI.Models
{
    using Services.Domain;
    using System.ComponentModel;
    using ET = Services.Enums.eLookupEstablishmentType;

    public class EditEstablishmentModel
    {
        private static readonly IDictionary<byte?, string> _ofstedRatingsLookup =
            new Dictionary<byte?, string>
            {
                [0] = "No Ofsted assessment published",
                [1] = "Outstanding",
                [2] = "Good",
                [3] = "Requires Improvement",
                [4] = "Inadequate"
            };

        public enum eAction
        {
            Edit,
            FindEstablishment,
            Save,
            SaveIEBT,
            Confirm,
            AddLinkedSchool,
            RemoveLinkedSchool,
            AddAddress,
            RemoveAddress,
            CancelChanges
        }

        public enum eLinkType
        {
            Successor,
            Predecessor
        }
        
        public EstablishmentDisplayPolicy DisplayPolicy { get; set; }

        public int? Urn { get; set; }
        public int? LocalAuthorityId { get; set; }
        public string Name { get; set; }
        public int? StatusId { get; set; }
        public int? ReasonEstablishmentOpenedId { get; set; }
        public int? ReasonEstablishmentClosedId { get; set; }
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
        public int? EstablishmentTypeGroupId { get; set; }


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
        public ContactDetailsViewModel Contact { get; set; } = new ContactDetailsViewModel();
        public ContactDetailsViewModel ContactAlt { get; set; } = new ContactDetailsViewModel();
        //public int? LAESTAB { get; set; }
        public int? EstablishmentNumber { get; set; }

        public int? TypeId { get; set; }
        public DateTimeViewModel OpenDate { get; set; } = new DateTimeViewModel();
        public DateTimeViewModel CloseDate { get; set; } = new DateTimeViewModel();
        public eAction Action { get; set; }

        public int? LinkedSearchUrn { get; set; }
        public int? LinkedUrnToAdd { get; set; }
        public string LinkedEstabNameToAdd { get; set; }
        public eLinkType? LinkTypeToAdd { get; set; }
        public DateTimeViewModel LinkedDateToAdd { get; set; }
        public int? LinkedItemPositionToRemove { get; set; }
        public List<LinkedEstabViewModel> Links { get; internal set; } = new List<LinkedEstabViewModel>();
        public bool ScrollToLinksSection { get; set; }

        public Dictionary<string, string> SimplifiedLAESTABRules { get; set; }

        public string GetAddress() => StringUtil.ConcatNonEmpties(", ", Address_Line1, Address_Line2, Address_Line3, Address_Locality, Address_CityOrTown, Address_County, Address_PostCode);


        public int? FurtherEducationTypeId { get; set; }
        public string Contact_WebsiteAddress { get; set; }
        public string Contact_TelephoneNumber { get; set; }
        public byte? OfstedRating { get; set; }
        public string OfstedRatingText => _ofstedRatingsLookup.Get(OfstedRating);
        public DateTimeViewModel OfstedInspectionDate { get; set; } = new DateTimeViewModel();
        public int? InspectorateId { get; set; }
        public string ProprietorName { get; set; }
        public int? Section41ApprovedId { get; set; }
        public int? SEN1Id { get; set; }
        public int? SEN2Id { get; set; }
        public int? SEN3Id { get; set; }
        public int? SEN4Id { get; set; }
        public int? TypeOfResourcedProvisionId { get; set; }
        public int? ResourcedProvisionOnRoll { get; set; }
        public int? SenUnitOnRoll { get; set; }
        public int? SenUnitCapacity { get; set; }
        public int? BSOInspectorateId { get; set; }
        public string BSOInspectorateReportUrl { get; set; }
        public int? RSCRegionId { get; set; }
        public int? Easting { get; set; }
        public int? Northing { get; set; }
        public int? GovernmentOfficeRegionId { get; set; }
        public int? AdministrativeDistrictId { get; set; }
        public int? AdministrativeWardId { get; set; }
        public int? ParliamentaryConstituencyId { get; set; }
        public int? UrbanRuralId { get; set; }
        public int? GSSLAId { get; set; }
        public int? CASWardId { get; set; }
        public int? MSOAId { get; set; }
        public int? LSOAId { get; set; }

        public string MSOACode { get; set; }
        public string LSOACode { get; set; }
        public List<AdditionalAddressModel> AdditionalAddresses { get; set; } = new List<AdditionalAddressModel>();
        public int AdditionalAddressesCount { get; set; }
        public DateTimeViewModel BSODateOfLastInspectionVisit { get; set; } = new DateTimeViewModel();
        public DateTimeViewModel BSODateOfNextInspectionVisit { get; set; } = new DateTimeViewModel();

        public IEnumerable<SelectListItem> AccommodationChanges { get; set; }
        public IEnumerable<SelectListItem> FurtherEducationTypes { get; set; }
        public IEnumerable<SelectListItem> Genders { get; set; }
        public IEnumerable<SelectListItem> LocalAuthorities { get; set; }
        public IEnumerable<SelectListItem> EstablishmentTypes { get; set; }
        public IEnumerable<SelectListItem> EducationPhases { get; set; }
        public IEnumerable<SelectListItem> HeadTitles { get; set; }
        public IEnumerable<SelectListItem> Statuses { get; set; }
        public IEnumerable<SelectListItem> AdmissionsPolicies { get; set; }
        public IEnumerable<SelectListItem> Inspectorates { get; set; }
        public IEnumerable<SelectListItem> IndependentSchoolTypes { get; set; }
        public IEnumerable<SelectListItem> BSOInspectorates { get; set; }
        public IEnumerable<SelectListItem> ReligiousCharacters { get; set; }
        public IEnumerable<SelectListItem> ReligiousEthoses { get; set; }
        public IEnumerable<SelectListItem> Dioceses { get; set; }
        public IEnumerable<SelectListItem> BoardingProvisions { get; set; }
        public IEnumerable<SelectListItem> NurseryProvisions { get; set; }
        public IEnumerable<SelectListItem> OfficialSixthFormProvisions { get; set; }
        public IEnumerable<SelectListItem> Section41ApprovedItems { get; set; }
        public IEnumerable<SelectListItem> ReasonsEstablishmentOpened { get; set; }
        public IEnumerable<SelectListItem> ReasonsEstablishmentClosed { get; set; }
        public IEnumerable<SelectListItem> SpecialClassesProvisions { get; set; }
        public IEnumerable<SelectListItem> SENProvisions1 { get; set; }
        public IEnumerable<SelectListItem> SENProvisions2 { get; set; }
        public IEnumerable<SelectListItem> SENProvisions3 { get; set; }
        public IEnumerable<SelectListItem> SENProvisions4 { get; set; }
        public IEnumerable<SelectListItem> TypeOfResourcedProvisions { get; set; }
        public IEnumerable<SelectListItem> TeenageMothersProvisions { get; set; }
        public IEnumerable<SelectListItem> ChildcareFacilitiesProvisions { get; set; }
        public IEnumerable<SelectListItem> RSCRegionLocalAuthorites { get; internal set; }
        public IEnumerable<SelectListItem> GovernmentOfficeRegions { get; internal set; }
        public IEnumerable<SelectListItem> AdministrativeDistricts { get; internal set; }
        public IEnumerable<SelectListItem> AdministrativeWards { get; internal set; }
        public IEnumerable<SelectListItem> ParliamentaryConstituencies { get; internal set; }
        public IEnumerable<SelectListItem> UrbanRuralLookup { get; internal set; }
        public IEnumerable<SelectListItem> GSSLALookup { get; internal set; }
        public IEnumerable<SelectListItem> CASWards { get; internal set; }

        public IEnumerable<SelectListItem> PRUSENOptions { get; internal set; }
        public IEnumerable<SelectListItem> PRUEBDOptions { get; internal set; }

        public TabDisplayPolicy TabDisplayPolicy { get; internal set; }

        public Guid? AddressToRemoveId { get; set; }

        public bool AllowHidingOfAddress { get; set; }

        public List<ChangeDescriptorDto> ChangesSummary { get; set; }

        public bool RequireConfirmationOfChanges => true;

        public bool IsLAMaintained => TypeId.OneOfThese(ET.CommunitySchool, ET.FoundationSchool, ET.LANurserySchool, ET.PupilReferralUnit, ET.VoluntaryAidedSchool, ET.VoluntaryControlledSchool, ET.CommunitySpecialSchool, ET.FoundationSpecialSchool);

        public bool IsAcademy => TypeId.OneOfThese(ET.Academy1619Converter, ET.Academy1619SponsorLed, ET.AcademyAlternativeProvisionConverter, ET.AcademyAlternativeProvisionSponsorLed, ET.AcademyConverter, ET.AcademySpecialConverter, ET.AcademySpecialSponsorLed, ET.AcademySponsorLed, ET.FreeSchools, ET.FreeSchools1619, ET.FreeSchoolsAlternativeProvision, ET.FreeSchoolsSpecial, ET.StudioSchools, ET.UniversityTechnicalCollege, ET.CityTechnologyCollege);

        public DateTimeViewModel ChangeEffectiveDate { get; set; } = new DateTimeViewModel();

        public string OriginalEstablishmentName { get; set; }

        public string Contact_EmailAddress { get; set; }
        public string ContactAlt_EmailAddress { get; set; }

        [DisplayName("Number of special pupils under a SEN statement/EHCP")]
        public int? SENStat { get; set; }

        [DisplayName("Number of special pupils not under a SEN statement/EHCP")]
        public int? SENNoStat { get; set; }

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

        #region IEBT properties
        public string Notes { get; set; }
        public DateTimeViewModel DateOfTheLastBridgeVisit { get; set; } = new DateTimeViewModel();
        //public DateTime? DateOfTheLastOfstedVisit { get; set; }//OfstedInspectionDate
        public DateTimeViewModel DateOfTheLastISIVisit { get; set; } = new DateTimeViewModel();
        public DateTimeViewModel DateOfTheLastWelfareVisit { get; set; } = new DateTimeViewModel();
        public DateTimeViewModel DateOfTheLastFPVisit { get; set; } = new DateTimeViewModel();
        public DateTimeViewModel DateOfTheLastSISVisit { get; set; } = new DateTimeViewModel();
        public DateTimeViewModel NextOfstedVisit { get; set; } = new DateTimeViewModel();
        public DateTimeViewModel NextGeneralActionRequired { get; set; } = new DateTimeViewModel();
        public DateTimeViewModel NextActionRequiredByWEL { get; set; } = new DateTimeViewModel();
        public DateTimeViewModel NextActionRequiredByFP { get; set; } = new DateTimeViewModel();
        //public Lookup Inspectorate { get; set; } //InspectorateId
        public int? IndependentSchoolTypeId { get; set; } // LookupIndependentSchoolType
        public string CharityOrganisation { get; set; }
        public int? CharityRegistrationNumber { get; set; }
        public int? TotalNumberOfFullTimePupils { get; set; }
        public int? TotalNumberOfPartTimePupils { get; set; }
        public int? TotalNumberOfPupilsOfCompulsorySchoolAge { get; set; }
        public int? NumberOfSpecialPupilsUnderASENStatementEHCP { get; set; }
        public int? NumberOfSpecialPupilsNotUnderASENStatementEHCP { get; set; }
        public int? TotalNumberOfPupilsInPublicCare { get; set; }
        public int? PTBoysAged2AndUnder { get; set; }
        public int? PTBoysAged3 { get; set; }
        public int? PTBoysAged4A { get; set; }
        public int? PTBoysAged4B { get; set; }
        public int? PTBoysAged4C { get; set; }
        public int? TotalNumberOfBoysInBoardingSchools { get; set; }
        public int? PTGirlsAged2AndUnder { get; set; }
        public int? PTGirlsAged3 { get; set; }
        public int? PTGirlsAged4A { get; set; }
        public int? PTGirlsAged4B { get; set; }
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
        public IEnumerable<SelectListItem> PruFulltimeProvisionOptions { get; internal set; }
        public IEnumerable<SelectListItem> PruEducatedByOthersOptions { get; internal set; }


        #endregion

        public EditEstablishmentModel()
        {

        }
    }
}