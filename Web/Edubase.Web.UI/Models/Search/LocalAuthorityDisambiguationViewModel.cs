using Edubase.Services.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models.Search
{
    public class LocalAuthorityDisambiguationViewModel
    {
        public List<int> SelectedLocalAuthorityIds { get; set; } = new List<int>();
        public List<LookupDto> MatchingLocalAuthorities { get; set; } = new List<LookupDto>();
        public string SearchText { get; set; }
        public bool OpenOnly { get; set; }

        public LocalAuthorityDisambiguationViewModel()
        {

        }

        public LocalAuthorityDisambiguationViewModel(List<int> selectedLocalAuthorityIds, string searchText, List<LookupDto> matchingLocalAuthorities, bool openOnly)
        {
            SelectedLocalAuthorityIds = selectedLocalAuthorityIds;
            SearchText = searchText;
            MatchingLocalAuthorities = matchingLocalAuthorities;
            OpenOnly = openOnly;
        }
    }
}
