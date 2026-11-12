using Edubase.Web.UI.Helpers.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace Edubase.Web.UI.Models.Search
{
    public class SearchViewModel
    {
        public enum Tab
        {
            Establishments,
            Groups,
            Governors
        }

        private List<int> selectedLocalAuthorityIds = new List<int>();

        // Keep these alias values aligned with Establishments search model aliases.
        public const string BIND_ALIAS_GOONERES = "f";
        public const string BIND_ALIAS_STATUSIDS = "b";
        public const string BIND_ALIAS_LAIDS = "d";

        public IEnumerable<LookupItemViewModel> LocalAuthorities { get; set; }

        [BindAlias(BIND_ALIAS_LAIDS)]
        public LocalAuthorityIdList SelectedLocalAuthorityIds
        {
            get { return new LocalAuthorityIdList(this.selectedLocalAuthorityIds.Distinct()); }
            set { this.selectedLocalAuthorityIds = value; }
        }

        public Tab SelectedTab { get; set; }

        public IEnumerable<LookupItemViewModel> GovernorRoles { get; internal set; }

        public GovernorSearchPayloadViewModel GovernorSearchModel { get; set; } = new GovernorSearchPayloadViewModel();
        public SearchPayloadViewModel TextSearchModel { get; set; } = new SearchPayloadViewModel();
        public SearchPayloadViewModel LocationSearchModel { get; set; } = new SearchPayloadViewModel();
        public SearchPayloadViewModel GroupSearchModel { get; set; } = new SearchPayloadViewModel();

        public string LocalAuthorityToAdd { get; set; }

        public int? LocalAuthorityToRemove { get; set; }

        public eSearchType? SearchType { get; set; }

        public bool OpenOnly { get; set; }

        public SearchViewModel AddLocalAuthorityId(int id)
        {
            if (!this.selectedLocalAuthorityIds.Contains(id))
            {
                this.selectedLocalAuthorityIds.Add(id);
            }
            return this;
        }

        public SearchViewModel RemoveLocalAuthorityId(int id)
        {
            if (this.selectedLocalAuthorityIds.Contains(id))
            {
                this.selectedLocalAuthorityIds.RemoveAll(la => la == id);
            }

            return this;
        }

        public bool NoResults { get; set; }

        public string ErrorPanel { get; set; }
    }

    public class LocalAuthorityIdList : List<int>
    {
        public LocalAuthorityIdList()
        {
        }

        public LocalAuthorityIdList(IEnumerable<int> ids)
        {
            this.AddRange(ids);
        }

        public override string ToString()
        {
            return $"d={string.Join("&d=", this)}";
        }
    }
}
