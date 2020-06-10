using Edubase.Common;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Groups.Models;
using Edubase.Web.UI.Areas.Governors.Models;
using System.Collections.Generic;
using Edubase.Web.UI.Areas.Establishments.Models;
using Edubase.Services.Core;
using Edubase.Web.UI.Helpers;
using System;
using System.Linq;
using Edubase.Services.ExternalLookup;

namespace Edubase.Web.UI.Models
{
    public class EstablishmentDetailViewModel
    {
        private readonly IFBService fbService;
        private readonly ICSCPService cscpService;

        public EstablishmentDetailViewModel(IFBService fbService = null, ICSCPService cscpService = null)
        {
            this.fbService = fbService;
            this.cscpService = cscpService;
            LegalParentGroupRouteDto = new Lazy<RouteDto>(() => LegalParentGroup != null ? new RouteDto("GroupDetails", new System.Web.Routing.RouteValueDictionary(new { id = LegalParentGroup.GroupUId }), LegalParentGroup.Name) : null);
        }
            
        private static Dictionary<int, string> _groupType2FieldLabelMappings = new Dictionary<int, string>
        {
            [(int)eLookupGroupType.SingleacademyTrust] = "Single-academy trust",
            [(int)eLookupGroupType.MultiacademyTrust] = "Academy trust",
            [(int)eLookupGroupType.SchoolSponsor] = "Academy sponsor",
            [(int)eLookupGroupType.Trust] = "Trust",
            [(int)eLookupGroupType.Federation] = "Federation",

            [(int)eLookupGroupType.UmbrellaTrust] = "Umbrella trust",
            [(int)eLookupGroupType.ChildrensCentresCollaboration] = "Childrens' centres collaboration",
            [(int)eLookupGroupType.ChildrensCentresGroup] = "Childrens' centre group"
        };

        public EstablishmentDisplayEditPolicy DisplayPolicy { get; set; }

        public TabDisplayPolicy TabDisplayPolicy { get; set; }

        public enum GovRole
        {
            AccountingOfficer = 1,
            ChairOfGovernors,
            ChairOfLocalGoverningBody,
            ChairOfTrustees,
            ChiefFinancialOfficer,
            Governor,
            LocalGovernor,
            Member,
            Trustee
        }


        public EstablishmentModel Establishment { get; set; }

        public IEnumerable<GroupModel> Groups { get; set; }

        public PaginatedResult<EstablishmentChangeDto> ChangeHistory { get; set; }

        public IEnumerable<LinkedEstabViewModel> LinkedEstablishments { get; set; }

        public GroupModel LegalParentGroup { get; set; }

        public Lazy<RouteDto> LegalParentGroupRouteDto { get; private set; }

        public RouteDto EstabDetailRouteDto => new RouteDto("EstabDetails", new System.Web.Routing.RouteValueDictionary(new { id = Establishment.Urn }), Establishment.Name);

        public bool IsUserLoggedOn { get; set; }

        public bool UserCanEdit { get; set; }

        public bool IsClosed => Establishment.StatusId == (int)eLookupEstablishmentStatus.Closed;

        public string SearchQueryString { get; set; }

        public eLookupSearchSource? SearchSource { get; set; }

        public string OfstedRatingReportUrl => (Establishment.OfstedRatingId.HasValue
            ? new OfstedRatingUrl(Establishment.Urn).ToString() : null as string);

        public string GetGroupFieldLabel(GroupModel model) => _groupType2FieldLabelMappings[model.GroupTypeId.Value];

        public string AgeRangeToolTip { get; set; }
        public string AgeRangeToolTipLink { get; set; }
        public string SchoolCapacityToolTip { get; set; }
        public string SchoolCapacityToolTipLink { get; set; }

        #region Lookup names

        public string ReligiousCharacterName { get; set; }
        public string DioceseName { get; set; }
        public string ReligiousEthosName { get; set; }
        public string ProvisionBoardingName { get; set; }
        public string BoardingEstabName { get; set; }
        public string AccommodationChangedName { get; set; }
        public string ProvisionNurseryName { get; set; }
        public string ProvisionOfficialSixthFormName { get; set; }
        public string Section41ApprovedName { get; set; }
        public string ReasonEstablishmentOpenedName { get; set; }
        public string ReasonEstablishmentClosedName { get; set; }
        public string CCOperationalHoursName { get; set; }
        public string CCGovernanceName { get; set; }
        public string CCDeliveryModelName { get; set; }
        public string CCGroupLeadName { get; set; }
        public string CCPhaseTypeName { get; set; }
        public string CCDisadvantagedAreaName { get; set; }
        public string CCDirectProvisionOfEarlyYearsName { get; set; }
        public string ProvisionSpecialClassesName { get; set; }
        public string SENNames { get; set; }
        public string TeenageMothersProvisionName { get; set; }
        public string ChildcareFacilitiesName { get; set; }
        public string PRUSENName { get; set; }
        public string PRUEBDName { get; set; }
        public string PruFulltimeProvisionName { get; set; }
        public string PruEducatedByOthersName { get; set; }
        public string TypeOfResourcedProvisionName { get; set; }
        public string BSOInspectorateName { get; set; }
        public string InspectorateName { get; set; }
        public string IndependentSchoolTypeName { get; set; }
        public string RSCRegionName { get; set; }
        public string GovernmentOfficeRegionName { get; set; }
        public string AdministrativeDistrictName { get; set; }
        public string AdministrativeWardName { get; set; }
        public string ParliamentaryConstituencyName { get; set; }
        public string UrbanRuralName { get; set; }
        public string GSSLAName { get; set; }
        public string CASWardName { get; set; }
        public string MSOAName { get; set; }
        public string LSOAName { get; set; }
        public string LocalAuthorityName { get; set; }
        public string LocalAuthorityCode { get; set; }
        public string HeadTitleName { get; set; }
        public string EducationPhaseName { get; set; }
        public string TypeName { get; set; }
        public string FurtherEducationTypeName { get; set; }
        public string GenderName { get; set; }
        public string StatusName { get; set; }
        public string AdmissionsPolicyName { get; set; }
        public string AddressCountryName { get; set; }
        public string AddressCountyName { get; set; }
        public string OfstedRatingName { get; set; }
        public string HelpdeskPreviousLocalAuthorityName { get; set; }

        public string IEBTProprietorsAddressCountyName { get; set; }
        public string IEBTChairOfProprietorsBodyAddressCountyName { get; set; }
        #endregion

        public string GetAddress() => StringUtil.ConcatNonEmpties(", ",
            Establishment.Address_Line1,
            Establishment.Address_Locality,
            Establishment.Address_Line3,
            Establishment.Address_CityOrTown,
            AddressCountyName?.Replace("Not recorded", string.Empty),
            Establishment.Address_PostCode);

        public IEnumerable<AdditionalAddressViewModel> AdditionalAddressList { get; set; } = Enumerable.Empty<AdditionalAddressViewModel>();

        public string GetProprietorsAddress() => StringUtil.ConcatNonEmpties(", ",
            Establishment.IEBTModel.ProprietorsStreet,
            Establishment.IEBTModel.ProprietorsLocality,
            Establishment.IEBTModel.ProprietorsAddress3,
            Establishment.IEBTModel.ProprietorsTown,
            IEBTProprietorsAddressCountyName?.Replace("Not recorded", string.Empty),
            Establishment.IEBTModel.ProprietorsPostcode);

        public string GetChairOfProprietorsBodyAddress() => StringUtil.ConcatNonEmpties(", ",
            Establishment.IEBTModel.ChairOfProprietorsBodyStreet,
            Establishment.IEBTModel.ChairOfProprietorsBodyLocality,
            Establishment.IEBTModel.ChairOfProprietorsBodyAddress3,
            Establishment.IEBTModel.ChairOfProprietorsBodyTown,
            IEBTChairOfProprietorsBodyAddressCountyName?.Replace("Not recorded", string.Empty),
            Establishment.IEBTModel.ChairOfProprietorsBodyPostcode);

        public GovernorsGridViewModel GovernorsGridViewModel { get; set; }

        /// <summary>
        /// Whether the current user needs to confirm either the establishment or governance records are up-to-date; at _MEDIUM_ priority.
        /// </summary>
        public bool MediumPriorityConfirmationsPending => MediumPriorityEstablishmentConfirmationPending || MediumPriorityGovernanceConfirmationPending;

        /// <summary>
        /// Whether the current user has a HIGH PRIORITY obligation to confirm their Establishment or Governance records are up-to-date.
        /// </summary>
        public bool HighPriorityConfirmationsPending => HighPriorityEstablishmentConfirmationPending || HighPriorityGovernanceConfirmationPending;

        public bool MediumPriorityEstablishmentConfirmationPending => Establishment.ConfirmationUpToDateRequired && !Establishment.UrgentConfirmationUpToDateRequired;
        public bool MediumPriorityGovernanceConfirmationPending => Establishment.ConfirmationUpToDateGovernanceRequired && !Establishment.UrgentConfirmationUpToDateGovernanceRequired;
        public bool HighPriorityEstablishmentConfirmationPending => (Establishment?.UrgentConfirmationUpToDateRequired).GetValueOrDefault();
        public bool HighPriorityGovernanceConfirmationPending => (Establishment?.UrgentConfirmationUpToDateGovernanceRequired).GetValueOrDefault();

        public string CscpURL => cscpService.SchoolURL(Establishment.Urn);
        private bool? showCscp;
        public bool ShowCscp
        {
            get
            {
                if (!showCscp.HasValue) showCscp = cscpService != null && cscpService.CheckExists(Establishment.Urn);
                return showCscp.Value;
            }
        }

        public string FinancialBenchmarkingURL => fbService.SchoolURL(Establishment.Urn);

        private bool? showFinancialBenchmarking;
        public bool ShowFinancialBenchmarking
        {
            get
            {
                if (!showFinancialBenchmarking.HasValue) showFinancialBenchmarking = fbService != null && fbService.CheckExists(Establishment.Urn);
                return showFinancialBenchmarking.Value;
            }
        }
    }
}
