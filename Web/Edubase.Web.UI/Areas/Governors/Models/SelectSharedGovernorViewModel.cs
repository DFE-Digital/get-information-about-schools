using System.Collections.Generic;
using Edubase.Services.Enums;
using Edubase.Web.UI.Areas.Establishments.Models;
using Edubase.Services.Groups.Models;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    public class SelectSharedGovernorViewModel : IEstablishmentPageViewModel
    {
        public string GovernorType { get; set; }

        public eLookupGovernorRole Role { get; set; }

        public IList<SharedGovernorViewModel> Governors { get; set; }

        public string SelectedTab { get; set; }

        public int? Urn { get; set; }

        public string Name { get; set; }

        public TabDisplayPolicy TabDisplayPolicy { get; set; }

        public string Layout { get; set; }

        public string SelectedGovernorId { get; set; }
        public string TypeName { get; set; }
        GroupModel IEstablishmentPageViewModel.LegalParentGroup { get; set; }
        string IEstablishmentPageViewModel.LegalParentGroupToken { get; set; }
    }
}