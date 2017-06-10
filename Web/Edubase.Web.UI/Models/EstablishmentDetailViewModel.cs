using Edubase.Common;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Groups.Models;
using Edubase.Web.UI.Models.Establishments;
using System.Collections.Generic;

namespace Edubase.Web.UI.Models
{
    public class EstablishmentDetailViewModel
    {
        private static Dictionary<int, string> _groupType2FieldLabelMappings = new Dictionary<int, string>
        {
            [(int)eLookupGroupType.SingleacademyTrust] = "Single academy trust",
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

        public IEnumerable<EstablishmentChangeDto> ChangeHistory { get; set; }

        public IEnumerable<LinkedEstabViewModel> LinkedEstablishments { get; set; }

        public GroupModel LegalParentGroup { get; set; }

        public bool IsUserLoggedOn { get; set; }

        public bool UserCanEdit { get; set; }
        
        public bool IsClosed => Establishment.StatusId == (int)eLookupEstablishmentStatus.Closed;

        public string SearchQueryString { get; set; }

        public eLookupSearchSource? SearchSource { get; set; }

        public EstablishmentDetailViewModel()
        {

        }

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
        #endregion

        public string GetAddress() => StringUtil.ConcatNonEmpties(", ", 
            Establishment.Address_Line1, 
            Establishment.Address_Line2, 
            Establishment.Address_Line3, 
            Establishment.Address_Locality, 
            Establishment.Address_CityOrTown, 
            AddressCountyName);

    }
}