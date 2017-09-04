using Edubase.Services.Enums;
using Edubase.Web.UI.Areas.Establishments.Models;
using Edubase.Web.UI.Areas.Groups.Models.CreateEdit;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    public class CreateEditGovernorViewModel : GovernorViewModel, IGroupPageViewModel, IEstablishmentPageViewModel
    {
        public enum EditMode { Create, Edit, Replace}

        public EditMode Mode { get; set; }
        public string GovernorRoleName { get; set; }
        public int? EstablishmentUrn { get; set; }
        public int? GroupUId { get; set; }
        public string ParentControllerName => EstablishmentUrn.HasValue ? "Establishment" : "Group";
        public string ParentAreaName => EstablishmentUrn.HasValue ? "" : "Groups";
        public string Layout { get; set; }
        public string ListOfEstablishmentsPluralName { get; set; }
        public string GroupName { get; set; }
        public int? GroupTypeId { get; set; }
        public string SelectedTabName { get; set; }
        string IEstablishmentPageViewModel.SelectedTab { get; set; }
        int? IEstablishmentPageViewModel.Urn { get; set; }
        string IEstablishmentPageViewModel.Name { get; set; }
        TabDisplayPolicy IEstablishmentPageViewModel.TabDisplayPolicy { get; set; }
        public bool IsHistoric { get; set; }

        public ReplaceGovernorViewModel ReplaceGovernorViewModel { get; set; } = new ReplaceGovernorViewModel();

        public string FormPostRouteName
        {
            get
            {
                var part1 = EstablishmentUrn.HasValue ? "Estab" : "Group";
                var part2 = ReplaceGovernorViewModel.GID.HasValue ? "Replace" : (GID.HasValue ? "Edit" : "Add");
                return string.Concat(part1, part2, "Governor");
            }
        }

        public string GroupTypeName { get; set; }
        public string TypeName { get; set; }
    }
}