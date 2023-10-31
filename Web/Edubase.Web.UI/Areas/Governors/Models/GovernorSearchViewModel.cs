using Edubase.Common;
using Edubase.Services.Enums;
using Edubase.Services.Governors.Models;
using Edubase.Services.Governors.Search;
using Edubase.Web.UI.Helpers.ModelBinding;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Search;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    public class GovernorSearchViewModel
    {
        public const string BIND_ALIAS_ROLE_ID = "t";
        public const string BIND_ALIAS_GOVERNOR_TYPE_FLAG_ID = "g";
        public const string BIND_ALIAS_LAIDS = "d";

        public GovernorSearchPayloadViewModel GovernorSearchModel { get; set; } = new GovernorSearchPayloadViewModel();
        public IList<SearchGovernorModel> Results { get; set; } = new List<SearchGovernorModel>();
        public List<LookupItemViewModel> AppointingBodies { get; internal set; }
        public List<LookupItemViewModel> GovernorRoles { get; internal set; }
        public List<LookupItemViewModel> GovernorTypeFlags => new List<LookupItemViewModel>
        {
            new LookupItemViewModel((int)eGovernorTypesFlag.MultiAcademyTrusts, "Multi-academy trusts"),
            new LookupItemViewModel((int)eGovernorTypesFlag.AcademiesWithinMAT, "Academies in a multi-academy trust"),
            new LookupItemViewModel((int)eGovernorTypesFlag.AcademiesWithinSAT, "Academies in a single-academy trust"),
            new LookupItemViewModel((int)eGovernorTypesFlag.GovsOfLAMaintained, "Local authority maintained schools"),
            new LookupItemViewModel((int)eGovernorTypesFlag.CTC, "City technology colleges"),
            new LookupItemViewModel((int)eGovernorTypesFlag.FreeSchools, "Free schools"),
            new LookupItemViewModel((int)eGovernorTypesFlag.AcadsWithSchoolSponsor, "Academies with a school sponsor"),
            new LookupItemViewModel((int)eGovernorTypesFlag.SecureSingleAcademyTrusts, "Secure single-academy trusts"),
        };

        public IEnumerable<LookupItemViewModel> LocalAuthorities { get; set; }

        public long Count { get; set; }

        public int PageCount => (int)Math.Ceiling(Count / (double)PageSize);

        public int PageSize { get; set; } = 50;

        public int StartIndex { get; set; }

        public string Error { get; set; }

        public bool HasError => !Error.IsNullOrEmpty();

        public char? SortBy { get; set; }

        public string SearchQueryString { get; set; }

        public eLookupSearchSource? SearchSource { get; set; }

        public eSortBy SortOption => SortBy == 'z' ? eSortBy.NameAlphabeticalZA : eSortBy.NameAlphabeticalAZ;

        [BindAlias(BIND_ALIAS_ROLE_ID)]
        public List<int> SelectedRoleIds { get; set; } = new List<int>();

        [BindAlias(BIND_ALIAS_GOVERNOR_TYPE_FLAG_ID)]
        public List<int> SelectedGovernorTypeFlagIds { get; set; } = new List<int>();

        [BindAlias(BIND_ALIAS_LAIDS)]
        public List<int> SelectedLocalAuthorityIds { get; set; } = new List<int>();

        public string RoleNames => StringUtil.Sentenceify(SelectedRoleIds.Select(x => GovernorRoles.First(r => r.Id == x).Name).ToArray(), StringUtil.SentenceifyOptions.AND);

        public eSearchType SearchType { get; set; }
    }
}
