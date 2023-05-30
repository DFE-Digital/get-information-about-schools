using Edubase.Common;
using Edubase.Services.Establishments.DisplayPolicies;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Edubase.Services.Enums;
using Edubase.Web.UI.Areas.Establishments.Models;
using Edubase.Web.UI.Models.Search;
using Newtonsoft.Json;

namespace Edubase.Web.UI.Models
{
    using Edubase.Services.Establishments.Models;
    using Edubase.Services.Groups.Models;
    using Services.Domain;
    using System.ComponentModel;
    using System.Linq;
    using ET = Services.Enums.eLookupEstablishmentType;
    
    public class EditEstablishmentModel : IEstablishmentPageViewModel
    {
        /// <summary>
        /// Action Specifiers (AS)
        /// </summary>
        public const string ASRemoveAddress = "RemoveAddress"; // suffixed with hyphen as it's parameterised in its use.
        public const string ASAddAddress = "AddAddress";
        public const string ASSave = "Save"; // suffixed with hyphen as it's parameterised in its use.
        public const string ASConfirm = "Confirm";
        public const string ASCancel = "Cancel";

        public const string ASSaveDetail = "details";
        public const string ASSaveLocation = "location";
        public const string ASSaveIEBT = "iebt";
        public const string ASSaveHelpdesk = "helpdesk";
        public const string ASSaveEmails = "SaveEmails";
        public const string ASAmendEmails = "AmendEmails";
        public const string ASEmailBack = "AmendEmailsCancel";

        public const string ASUpdateProprietors = "UpdateProprietors";
        public const string ASAddProprietor = "AddProprietor";
        public const string ASRemoveProprietor = "RemoveProprietor";

        public enum eLinkType
        {
            Successor,
            Predecessor
        }

        public EstablishmentDisplayEditPolicy EditPolicy { get; set; }
        public EstablishmentDisplayEditPolicy DisplayPolicy { get; set; }

        public Dictionary<string, string> SelectedTab2DetailPageTabNameMapping { get; private set; } = new Dictionary<string, string>
        {
            ["details"] = "#school-dashboard",
            ["location"] = "#school-location",
            ["iebt"] = "#school-iebt",
            ["helpdesk"] = "#helpdesk"
        };

        public int? Urn { get; set; }
        public int? LocalAuthorityId { get; set; }
        public string LocalAuthorityCode { get; set; }
        public string Name { get; set; }
        public int? StatusId { get; set; }
        public int? ReasonEstablishmentOpenedId { get; set; }
        public int? ReasonEstablishmentClosedId { get; set; }
        public int? EducationPhaseId { get; set; }
        public int? StatutoryLowAge { get; set; }
        public int? StatutoryHighAge { get; set; }
        public int? ProvisionBoardingId { get; set; }

        [Display(Name = "Boarding establishment")]
        public int? BoardingEstablishmentId { get; set; }
        public int? ProvisionNurseryId { get; set; }
        public int? ProvisionOfficialSixthFormId { get; set; }
        public int? GenderId { get; set; }
        public int? ReligiousCharacterId { get; set; }
        public int? ReligiousEthosId { get; set; }
        public int? DioceseId { get; set; }
        public int? AdmissionsPolicyId { get; set; }
        public int? Capacity { get; set; }
        public int? NumberOfPupilsOnRoll { get; set; }
        public int? ProvisionSpecialClassesId { get; set; }
        public int? UKPRN { get; set; }
        public int? EstablishmentTypeGroupId { get; set; }

        [Display(Name = "Quality assurance body name")]
        public int? QualityAssuranceBodyNameId { get; set; }

        [Display(Name = "Establishment accredited")]
        public int? EstablishmentAccreditedId { get; set; }

        /// <summary>
        /// Flags whether there are unsaved changes in the viewmodel
        /// </summary>
        public bool IsDirty { get; set; }


        public string Address_Line1 { get; set; }
        public string Address_Line2 { get; set; }
        public string Address_Line3 { get; set; }
        public string Address_CityOrTown { get; set; }
        public int? Address_CountyId { get; set; }
        public int? Address_CountryId { get; set; }
        public string Address_Locality { get; set; }
        public string Address_PostCode { get; set; }
        public string Address_UPRN { get; set; }

        public List<AdditionalAddressModel> AdditionalAddresses { get; set; } = new List<AdditionalAddressModel>();

        public string TypeName { get; set; }

        public GroupModel LegalParentGroup
        {
            get
            {
                return UriHelper.TryDeserializeUrlToken<GroupModel>(LegalParentGroupToken);
            }
            set
            {
                LegalParentGroupToken = UriHelper.SerializeToUrlToken(value);
            }
        }

        public string LegalParentGroupToken { get; set; }

        public string OldHeadFirstName { get; set; }
        public string HeadFirstName { get; set; }
        public string OldHeadLastName { get; set; }
        public string HeadLastName { get; set; }
        public int? HeadTitleId { get; set; }
        public string HeadEmailAddress { get; set; }
        public DateTimeViewModel HeadAppointmentDate { get; set; } = new DateTimeViewModel();
        public ContactDetailsViewModel Contact { get; set; } = new ContactDetailsViewModel();
        public ContactDetailsViewModel ContactAlt { get; set; } = new ContactDetailsViewModel();
        public int? EstablishmentNumber { get; set; }

        public int? TypeId { get; set; }

        [Display(Name = "Open date")]
        public DateTimeViewModel OpenDate { get; set; } = new DateTimeViewModel();

        [Display(Name = "Close date")]
        public DateTimeViewModel CloseDate { get; set; } = new DateTimeViewModel();

        public DateTimeViewModel AccreditationExpiryDate { get; set; } = new DateTimeViewModel();

        public string ActionSpecifier { get; set; }
        public string ActionSpecifierCommand => ActionSpecifier.GetPart("-", 0);
        public string ActionSpecifierParam => ActionSpecifier.GetPart("-", 1);
        public static string GetActionSpecifier(string command, object parameter) => string.Concat(command, "-", parameter);



        public bool ScrollToLinksSection { get; set; }

        public string GetAddress() => StringUtil.ConcatNonEmpties(", ", Address_Line1, Address_Line2, Address_Line3, Address_Locality, Address_CityOrTown, Counties.FirstOrDefault(x=>x.Value == Address_CountyId?.ToString())?.Text, Address_PostCode);

        public int? FurtherEducationTypeId { get; set; }
        public string Contact_WebsiteAddress { get; set; }
        public string Contact_TelephoneNumber { get; set; }
        public int? OfstedRatingId { get; set; }

        [Display(Name = "Ofsted last inspection")]
        public DateTimeViewModel OfstedInspectionDate { get; set; } = new DateTimeViewModel();

        [Display(Name = "Inspectorate")]
        public int? InspectorateId { get; set; }

        [Display(Name = "Name")]
        public string ProprietorBodyName { get; set; }

        public int? ProprietorTypeId { get; set; }

        public List<ProprietorViewModel> Proprietors { get; set; } = new List<ProprietorViewModel>();

        public ProprietorViewModel ChairOfProprietor { get; set; }

        public int? Section41ApprovedId { get; set; }
        public int[] SENIds { get; set; } = new int[0];
        public int? TypeOfResourcedProvisionId { get; set; }
        public int? ResourcedProvisionOnRoll { get; set; }
        public int? ResourcedProvisionCapacity { get; set; }
        public int? SenUnitOnRoll { get; set; }
        public int? SenUnitCapacity { get; set; }
        public int? BSOInspectorateId { get; set; }
        public string BSOInspectorateReportUrl { get; set; }
        public int? RSCRegionId { get; set; }
        public int? Easting { get; set; }
        public int? Northing { get; set; }
        public int? GovernmentOfficeRegionId { get; set; }
        public int? AdministrativeDistrictId { get; set; }
        public string AdministrativeDistrictName { get; set; }
        public int? AdministrativeWardId { get; set; }
        public string AdministrativeWardName { get; set; }
        public int? ParliamentaryConstituencyId { get; set; }
        public string ParliamentaryConstituencyName { get; set; }
        public int? UrbanRuralId { get; set; }
        public int? GSSLAId { get; set; }
        public string GSSLAName { get; set; }
        public string MSOAName { get; set; }
        public int? MSOAId { get; set; }
        public string LSOAName { get; set; }
        public int? LSOAId { get; set; }

        public string LocationEditField { get; set; }

        [Display(Name = "Quality assurance body report")]
        public string QualityAssuranceBodyReport { get; set; }

        [Display(Name = "Companies House number")]
        public string CompaniesHouseNumber { get; set; }

        [Display(Name = "Total number of full time teachers or tutors")]
        public int? TotalFTTeachersTutors { get; set; }

        [Display(Name = "Total number of part time teachers or tutors")]
        public int? TotalPTTeachersTutors { get; set; }

        [Display(Name = "BSO: Date of last inspection")]
        public DateTimeViewModel BSODateOfLastInspectionVisit { get; set; } = new DateTimeViewModel();

        [Display(Name = "BSO: Next inspection visit")]
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
        public IEnumerable<SelectListItem> BoardingEstablishment { get; set; }
        public IEnumerable<SelectListItem> NurseryProvisions { get; set; }
        public IEnumerable<SelectListItem> OfficialSixthFormProvisions { get; set; }
        public IEnumerable<SelectListItem> Section41ApprovedItems { get; set; }
        public IEnumerable<SelectListItem> ReasonsEstablishmentOpened { get; set; }
        public IEnumerable<SelectListItem> ReasonsEstablishmentClosed { get; set; }
        public IEnumerable<SelectListItem> SpecialClassesProvisions { get; set; }

        public IEnumerable<SelectListItem> TypeOfResourcedProvisions { get; set; }
        public IEnumerable<SelectListItem> TeenageMothersProvisions { get; set; }
        public IEnumerable<SelectListItem> ChildcareFacilitiesProvisions { get; set; }
        public IEnumerable<SelectListItem> RSCRegions { get; internal set; }
        public IEnumerable<SelectListItem> GovernmentOfficeRegions { get; internal set; }
        public IEnumerable<LookupItemViewModel> AdministrativeDistricts { get; internal set; }
        public IEnumerable<LookupItemViewModel> AdministrativeWards { get; internal set; }
        public IEnumerable<LookupItemViewModel> ParliamentaryConstituencies { get; internal set; }
        public IEnumerable<SelectListItem> UrbanRuralLookup { get; internal set; }
        public IEnumerable<LookupItemViewModel> GSSLALookup { get; internal set; }
        public IEnumerable<LookupItemViewModel> MSOAs { get; set; }
        public IEnumerable<LookupItemViewModel> LSOAs { get; set; }

        public IEnumerable<SelectListItem> PRUSENOptions { get; internal set; }
        public IEnumerable<SelectListItem> PRUEBDOptions { get; internal set; }

        public IEnumerable<SelectListItem> QualityAssuranceBodyName { get; set; }
        public IEnumerable<SelectListItem> EstablishmentAccredited { get; set; }


        public TabDisplayPolicy TabDisplayPolicy { get; set; }


        public List<ChangeDescriptorDto> ChangesSummary { get; set; }

        /// <summary>
        /// Field names of fields whose changes will require approval
        /// </summary>
        public string[] ApprovalFields { get; set; } = new string[0];

        public int ChangesRequireApprovalCount { get; set; }

        public int ChangesInstantCount { get; set; }

        public int TotalChangesCount => ChangesInstantCount + ChangesRequireApprovalCount;

        public int GetChangesRequiringApprovalCount() => OverrideCRProcess ? 0 : ChangesRequireApprovalCount;

        public int GetChangesNotRequiringApprovalCount() => OverrideCRProcess ? TotalChangesCount : ChangesInstantCount;


        public bool IsLAMaintained => TypeId.OneOfThese(ET.CommunitySchool, ET.FoundationSchool, ET.LANurserySchool, ET.PupilReferralUnit, ET.VoluntaryAidedSchool, ET.VoluntaryControlledSchool, ET.CommunitySpecialSchool, ET.FoundationSpecialSchool);

        public bool IsAcademy => TypeId.OneOfThese(ET.Academy1619Converter, ET.Academy1619SponsorLed, ET.AcademyAlternativeProvisionConverter, ET.AcademyAlternativeProvisionSponsorLed, ET.AcademyConverter, ET.AcademySpecialConverter, ET.AcademySpecialSponsorLed, ET.AcademySponsorLed, ET.FreeSchools, ET.FreeSchools1619, ET.FreeSchoolsAlternativeProvision, ET.FreeSchoolsSpecial, ET.StudioSchools, ET.UniversityTechnicalCollege, ET.CityTechnologyCollege);

        [Display(Name = "Effective date (optional)")]
        public DateTimeViewModel ChangeEffectiveDate { get; set; } = new DateTimeViewModel();

        public string OriginalEstablishmentName { get; set; }
        public string OriginalTypeName { get; set; }

        [DisplayName("Email address")]
        public string Contact_EmailAddress { get; set; }

        [DisplayName("Alternative email address")]
        public string ContactAlt_EmailAddress { get; set; }

        [DisplayName("Number of pupils eligible for free school meals")]
        public int? FreeSchoolMealsNumber { get; set; }

        [DisplayName("Percentage of children eligible for free school meals")]
        public double? FreeSchoolMealsPercentage { get; set; }


        [DisplayName("Number of special pupils under a special educational needs (SEN) statement/education, health and care (EHC) plan")]
        public int? SENStat { get; set; }

        [DisplayName("Number of special pupils not under a special educational needs (SEN) statement/education, health and care (EHC) plan")]
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
        [MaxLength(4000)]
        public string Notes { get; set; }

        [Display(Name = "Associations"), MaxLength(1000)]
        public string Associations { get; set; }
        public DateTimeViewModel DateOfTheLastBridgeVisit { get; set; } = new DateTimeViewModel();
        public DateTimeViewModel DateOfLastOfstedVisit { get; set; } = new DateTimeViewModel();
        public DateTimeViewModel DateOfTheLastISIVisit { get; set; } = new DateTimeViewModel();
        public DateTimeViewModel DateOfTheLastWelfareVisit { get; set; } = new DateTimeViewModel();
        public DateTimeViewModel DateOfTheLastFPVisit { get; set; } = new DateTimeViewModel();
        public DateTimeViewModel DateOfTheLastSISVisit { get; set; } = new DateTimeViewModel();
        public DateTimeViewModel NextOfstedVisit { get; set; } = new DateTimeViewModel();
        public DateTimeViewModel NextGeneralActionRequired { get; set; } = new DateTimeViewModel();
        public DateTimeViewModel NextActionRequiredByWEL { get; set; } = new DateTimeViewModel();
        public DateTimeViewModel NextActionRequiredByFP { get; set; } = new DateTimeViewModel();

        [DisplayName("Independent school type")]
        public int? IndependentSchoolTypeId { get; set; }

        [DisplayName("Charity organisation")]
        public string CharityOrganisation { get; set; }

        [DisplayName("Charity registration number")]
        public int? CharityRegistrationNumber { get; set; }

        [DisplayName("Total number of full time pupils")]
        public int? TotalNumberOfFullTimePupils { get; set; }

        [DisplayName("Total number of part time pupils")]
        public int? TotalNumberOfPartTimePupils { get; set; }

        [DisplayName("Total number of pupils of compulsory school age")]
        public int? TotalNumberOfPupilsOfCompulsorySchoolAge { get; set; }

        [DisplayName("Total number of pupils in public care")]
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

        [DisplayName("Total number of boys in boarding schools")]
        public int? TotalNumberOfBoysInBoardingSchools { get; set; }

        [DisplayName("PT girls (aged 2 and under) ")]
        public int? PTGirlsAged2AndUnder { get; set; }

        [DisplayName("PT girls (aged 3)")]
        public int? PTGirlsAged3 { get; set; }

        [DisplayName("PT girls (aged 4a)")]
        public int? PTGirlsAged4A { get; set; }

        [DisplayName("PT girls (aged 4b)")]
        public int? PTGirlsAged4B { get; set; }

        [DisplayName("PT girls (aged 4c)")]
        public int? PTGirlsAged4C { get; set; }

        [DisplayName("Total number of girls in boarding schools ")]
        public int? TotalNumberOfGirlsInBoardingSchools { get; set; }

        [DisplayName("Total number of full time staff")]
        public int? TotalNumberOfFullTimeStaff { get; set; }

        [DisplayName("Total number of part time staff")]
        public int? TotalNumberOfPartTimeStaff { get; set; }

        [DisplayName("Lowest annual rate for day pupils")]
        public int? LowestAnnualRateForDayPupils { get; set; }

        [DisplayName("Highest annual rate for day pupils")]
        public int? HighestAnnualRateForDayPupils { get; set; }

        [DisplayName("Lowest annual rate for boarding pupils")]
        public int? LowestAnnualRateForBoardingPupils { get; set; }

        [DisplayName("Highest annual rate for boarding pupils")]
        public int? HighestAnnualRateForBoardingPupils { get; set; }

        [DisplayName("Accommodation changes")]
        public int? AccommodationChangedId { get; set; }

        public IEnumerable<SelectListItem> PruFulltimeProvisionOptions { get; internal set; }
        public IEnumerable<SelectListItem> PruEducatedByOthersOptions { get; internal set; }

        public string SelectedTab { get; set; }

        public string Layout { get; set; }


        #endregion

        public bool CanOverrideCRProcess { get; set; }

        public bool OverrideCRProcess { get; set; }
        public IEnumerable<SelectListItem> Counties { get; internal set; }
        public IEnumerable<SelectListItem> Countries { get; internal set; }
        public IEnumerable<SelectListItem> OfstedRatings { get; internal set; }
        public List<LookupDto> SENProvisions { get; internal set; }

        [DisplayName("Helpdesk notes")]
        public string HelpdeskNotes { get; set; }

        [DisplayName("Edubase last update")]
        public DateTimeViewModel HelpdeskLastUpdate { get; set; } = new DateTimeViewModel();

        [DisplayName("Previous local authority")]
        public int? HelpdeskPreviousLocalAuthorityId { get; set; }

        [DisplayName("Previous establishment number")]
        public int? HelpdeskPreviousEstablishmentNumber { get; set; }

        public bool? HasEmptyEmailFields { get; set; }

        public bool? IsUpdatingEmailFields { get; set; }

        public List<string> EmptyEmailFields { get; set; }

        #region Children's Centre fields
        public int? CCOperationalHoursId { get; set; }
        public int? CCUnder5YearsOfAgeCount { get; set; }
        public int? CCGovernanceId { get; set; }
        public string CCGovernanceDetail { get; set; }
        public int? CCDeliveryModelId { get; set; }
        public int? CCGroupLeadId { get; set; }
        public int? CCPhaseTypeId { get; set; }
        public int? CCDisadvantagedAreaId { get; set; }
        public int? CCDirectProvisionOfEarlyYearsId { get; set; }

        public IEnumerable<SelectListItem> CCOperationalHours { get; internal set; }
        public IEnumerable<SelectListItem> CCGovernanceList { get; internal set; }
        public IEnumerable<SelectListItem> CCDeliveryModels { get; internal set; }
        public IEnumerable<SelectListItem> CCGroupLead { get; internal set; }
        public IEnumerable<SelectListItem> CCPhaseTypes { get; internal set; }
        public IEnumerable<SelectListItem> CCDisadvantagedAreas { get; internal set; }
        public IEnumerable<SelectListItem> CCDirectProvisionOfEarlyYears { get; internal set; }

        #endregion

        public Dictionary<int, int[]> Type2PhaseMap { get; set; }

        public bool CCIsDemoting { get; set; }
        public bool CCIsPromoting { get; set; }

        /// <summary>
        /// When the name, LA and post code match another record
        /// </summary>
        public bool ShowDuplicateRecordError { get; set; }

        /// <summary>
        /// When the name and LA match another record
        /// </summary>
        public bool ShowDuplicateRecordWarning { get; set; }

        public EditEstablishmentModel()
        {

        }
    }
}
