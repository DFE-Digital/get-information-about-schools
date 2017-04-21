using Edubase.Web.UI.Helpers.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models.Search
{
    public class SearchViewModel
    {
        public const string BIND_ALIAS_GOONERES = Areas.Establishments.Models.Search.EstablishmentSearchViewModel.BIND_ALIAS_GOONERES;
        public const string BIND_ALIAS_STATUSIDS = Areas.Establishments.Models.Search.EstablishmentSearchViewModel.BIND_ALIAS_STATUSIDS;
        public const string BIND_ALIAS_LAIDS = Areas.Establishments.Models.Search.EstablishmentSearchViewModel.BIND_ALIAS_LAIDS;

        public IEnumerable<LookupItemViewModel> LocalAuthorities { get; set; }

        [BindAlias(BIND_ALIAS_LAIDS)]
        public List<int> SelectedLocalAuthorityIds { get; set; } = new List<int>();

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
            SelectedLocalAuthorityIds.Add(id);
            return this;
        }

        public SearchViewModel RemoveLocalAuthorityId(int id)
        {
            SelectedLocalAuthorityIds.Remove(id);
            return this;
        }

    }
}