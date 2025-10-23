using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Helpers.ModelBinding;

namespace Edubase.Web.UI.Models.Tools
{
    public class PredefinedLASetViewModel
    {
        public string Id { get; set; }
        public string PageTitle => (Id == null ? "Create" : "Edit") + " local authority set";

        [BindAlias(Areas.Establishments.Models.Search.EstablishmentSearchViewModel.BIND_ALIAS_LAIDS), CannotBeEmpty(ErrorMessage = "Please enter local authorities")]
        public List<int> SelectedLocalAuthorityIds { get; set; } = new List<int>();

        public IEnumerable<LookupItemViewModel> LocalAuthorities { get; set; }

        [Required(ErrorMessage = "Please enter the set name")]
        public string Title { get; set; }
        public string Referrer { get; set; }
        public string Mode { get; set; }
        public bool IsNewEntity => Id == null;

        public bool WarningNameClash { get; set; } // when true, we should warn the user
        public bool SuppressWarning { get; set; } // when true, if there's a name clash, the other one will be deleted.

    }
}
