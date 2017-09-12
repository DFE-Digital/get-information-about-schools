using Edubase.Services.Groups.Models;
using Edubase.Web.UI.Helpers.ModelBinding;

namespace Edubase.Web.UI.Areas.Establishments.Models
{
    public class SearchForEstablishmentViewModel : IEstablishmentPageViewModel
    {
        public string SelectedTab { get; set; }
        public int? Urn { get; set; }
        public string Name { get; set; }
        public TabDisplayPolicy TabDisplayPolicy { get; set; }
        public string Layout { get; set; }

        
        public string SearchUrn { get; set; }

        public const string BIND_ALIAS_DOSEARCH = "s";

        [BindAlias(BIND_ALIAS_DOSEARCH)]
        public bool DoSearch { get; set; }
        public string TypeName { get; set; }

        GroupModel IEstablishmentPageViewModel.LegalParentGroup { get; set; }
        string IEstablishmentPageViewModel.LegalParentGroupToken { get; set; }
    }
}