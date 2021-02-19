using Edubase.Services.Governors.Models;

namespace Edubase.Web.UI.Areas.Groups.Models
{
    using Common;
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
        private Dictionary<int, string> _groupTypes2Name = new Dictionary<int, string>
        {
            [(int)GT.ChildrensCentresCollaboration] = "children's centre collaboration",
            [(int)GT.ChildrensCentresGroup] = "children's centre group",
            [(int)GT.Federation] = "federation",
            [(int)GT.MultiacademyTrust] = "multi-academy trust",
            [(int)GT.SchoolSponsor] = "school sponsor",
            [(int)GT.SingleacademyTrust] = "single-academy trust",
            [(int)GT.Trust] = "trust"
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

        public string OpenDateLabel => Group.GroupTypeId.OneOfThese(GT.MultiacademyTrust, GT.SingleacademyTrust) ? "Incorporated on (open date)" : "Open date";
        public string EstablishmentsPluralName => Group.GroupTypeId.OneOfThese(GT.MultiacademyTrust, GT.SingleacademyTrust, GT.SchoolSponsor) ? "Academies" :
            (Group.GroupTypeId.OneOfThese(GT.ChildrensCentresCollaboration, GT.ChildrensCentresGroup) ? "Children's centres" : "Schools");

        public List<EstablishmentGroupViewModel> Establishments { get; private set; } = new List<EstablishmentGroupViewModel>();

        public string CompaniesHouseUrl => ConfigurationManager.AppSettings["CompaniesHouseBaseUrl"].Append(Group.CompaniesHouseNumber);

        public PaginatedResult<GroupChangeDto> ChangeHistory { get; set; }

        public string GroupTypeNameForClosureLabel => _groupTypes2Name.ContainsKey(GroupTypeId) ? _groupTypes2Name[GroupTypeId] : string.Empty;

        public IEnumerable<LinkedGroupModel> Links { get; set; }
        public GovernorPermissions GovernorPermissions { get; set; }

    }
}
