using System.Collections.Generic;
using Edubase.Services.Enums;
using Edubase.Web.UI.Models.Establishments;

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

        public int SelectedGovernorId { get; set; }
    }
}