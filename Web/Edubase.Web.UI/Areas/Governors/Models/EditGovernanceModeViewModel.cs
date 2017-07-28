using Edubase.Services.Enums;
using Edubase.Web.UI.Areas.Establishments.Models;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    public class EditGovernanceModeViewModel : IEstablishmentPageViewModel
    {
        public string SelectedTab { get; set; }
        public int? Urn { get; set; }
        public string Name { get; set; }
        public TabDisplayPolicy TabDisplayPolicy { get; set; }
        public string Layout { get; set; }
        public eGovernanceMode? GovernanceMode { get; set; }
        public eGovernanceMode[] PermissibleGovernanceModes { get; internal set; }
    }
}