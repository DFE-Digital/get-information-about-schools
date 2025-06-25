using System.Linq;
using System.Threading.Tasks;
using Edubase.Services.ExternalLookup;
using Edubase.Services.Governors.Models;

namespace Edubase.Web.UI.Areas.Groups.Models
{
    using Common;
    using Edubase.Web.UI.Areas.Governors.Models;
    using Services.Core;
    using Services.Domain;
    using Services.Enums;
    using Services.Groups.Models;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using GT = Services.Enums.eLookupGroupType;

    public class GroupDetailViewModel
    {
        private readonly IExternalLookupService extService;

        public GroupDetailViewModel(IExternalLookupService extService = null)
        {
            this.extService = extService;
        }

        private Dictionary<int, string> _groupTypes2Name = new Dictionary<int, string>
        {
            [(int)GT.ChildrensCentresCollaboration] = "children's centre collaboration",
            [(int)GT.ChildrensCentresGroup] = "children's centre group",
            [(int)GT.Federation] = "federation",
            [(int)GT.MultiacademyTrust] = "multi-academy trust",
            [(int)GT.SchoolSponsor] = "school sponsor",
            [(int)GT.SingleacademyTrust] = "single-academy trust",
            [(int)GT.SecureSingleAcademyTrust] = "secure single-academy trust",
            [(int)GT.Trust] = "trust"
        };

        private static readonly int[] FscpdGroupTypes =
        {
            (int) eLookupEstablishmentType.Academy1619SponsorLed,
            (int) eLookupEstablishmentType.Academy1619Converter,
            (int) eLookupEstablishmentType.AcademyConverter,
            (int) eLookupEstablishmentType.AcademySpecialConverter,
            (int) eLookupEstablishmentType.AcademySpecialSponsorLed,
            (int) eLookupEstablishmentType.AcademySponsorLed,
            (int) eLookupEstablishmentType.CityTechnologyCollege,
            (int) eLookupEstablishmentType.CommunitySchool,
            (int) eLookupEstablishmentType.CommunitySpecialSchool,
            (int) eLookupEstablishmentType.FoundationSchool,
            (int) eLookupEstablishmentType.FoundationSpecialSchool,
            (int) eLookupEstablishmentType.FreeSchools,
            (int) eLookupEstablishmentType.FreeSchools1619,
            (int) eLookupEstablishmentType.FreeSchoolsSpecial,
            (int) eLookupEstablishmentType.FurtherEducation,
            (int) eLookupEstablishmentType.NonmaintainedSpecialSchool,
            (int) eLookupEstablishmentType.OtherIndependentSchool,
            (int) eLookupEstablishmentType.OtherIndependentSpecialSchool,
            (int) eLookupEstablishmentType.ServiceChildrensEducation,
            (int) eLookupEstablishmentType.SixthFormCentres,
            (int) eLookupEstablishmentType.StudioSchools,
            (int) eLookupEstablishmentType.UniversityTechnicalCollege,
            (int) eLookupEstablishmentType.VoluntaryAidedSchool,
            (int) eLookupEstablishmentType.VoluntaryControlledSchool
        };

        public bool CanUserEdit { get; set; }
        public bool CanUserEditGovernance { get; set; }
        public bool IsUserLoggedOn { get; set; }
        public GroupModel Group { get; set; }
        public string Address { get; set; }
        public string GroupTypeName { get; set; }
        public string GroupStatusName { get; set; }
        public string LocalAuthorityName { get; set; }
        public string SearchQueryString { get; set; }
        public eLookupSearchSource? SearchSource { get; set; }
        public bool IsClosed { get; set; }
        public bool IsClosedInError { get; set; }
        public DateTime? CloseDate { get; set; }
        public int GroupTypeId { get; set; }
        public int? UKPRN { get; set; }

        public string OpenDateLabel => Group.GroupTypeId.OneOfThese(GT.MultiacademyTrust, GT.SingleacademyTrust, GT.SecureSingleAcademyTrust) ? "Incorporated on (open date)" : "Open date";
        public string EstablishmentsPluralName => Group.GroupTypeId.OneOfThese(GT.MultiacademyTrust, GT.SingleacademyTrust, GT.SchoolSponsor, GT.SecureSingleAcademyTrust) ? "Academies" :
            (Group.GroupTypeId.OneOfThese(GT.ChildrensCentresCollaboration, GT.ChildrensCentresGroup) ? "Children's centres" : "Schools");

        public List<EstablishmentGroupViewModel> Establishments { get; private set; } = new List<EstablishmentGroupViewModel>();

        public string CompaniesHouseUrl => ConfigurationManager.AppSettings["CompaniesHouseBaseUrl"].Append(Group.CompaniesHouseNumber);

        public PaginatedResult<GroupChangeDto> ChangeHistory { get; set; }

        public string GroupTypeNameForClosureLabel => _groupTypes2Name.ContainsKey(GroupTypeId) ? _groupTypes2Name[GroupTypeId] : string.Empty;

        public IEnumerable<LinkedGroupModel> Links { get; set; }
        public GovernorPermissions GovernorPermissions { get; set; }

        public string FscpdServiceName => ConfigurationManager.AppSettings["FscpdServiceName"];
        public string FscpdURL => extService.FscpdURL(Group.GroupUId, Group.Name, GroupTypeId.OneOfThese(eLookupGroupType.MultiacademyTrust, eLookupGroupType.SchoolSponsor));

        public bool ShowFscpd =>
            Group?.GroupTypeId.HasValue == true &&
            FscpdGroupTypes.Contains(Group.GroupTypeId.Value);

        private Tuple<int?, FbType> FinancialBenchmarkingLookups
        {
            get
            {
                var lookupId = Group.GroupUId;
                var lookupType = FbType.Federation;

                if (Group.GroupTypeId.OneOfThese(GT.MultiacademyTrust, GT.SingleacademyTrust))
                {
                    lookupId = Group.CompaniesHouseNumber.ToInteger();
                    lookupType = FbType.Trust;
                }

                return new Tuple<int?, FbType>(lookupId, lookupType);
            }
        }

        public string FinancialBenchmarkingURL => extService.SfbURL(FinancialBenchmarkingLookups.Item1, FinancialBenchmarkingLookups.Item2);

        private bool? showFinancialBenchmarking;

        public bool ShowFinancialBenchmarking
        {
            get => showFinancialBenchmarking.GetValueOrDefault();
            private set => showFinancialBenchmarking = value;
        }

        //code originally inside the property, moved here to allow it to be async
        public async Task SetShowFinancialBenchmarkingAsync()
        {
            if (FinancialBenchmarkingLookups == null)
            {
                return;
            }
            if (!showFinancialBenchmarking.HasValue)
            {
                showFinancialBenchmarking = extService != null && await extService.SfbCheckExists(FinancialBenchmarkingLookups.Item1, FinancialBenchmarkingLookups.Item2);
            }
        }
        public GovernorsGridViewModel GovernorsGridViewModel { get; set; }
    }
}
