using System.Collections.Generic;
using System.Linq;
using Edubase.Common;
using Edubase.Services.Enums;
using Edubase.Services.Governors.Models;
using Edubase.Services.Groups.Models;
using Edubase.Web.UI.Areas.Establishments.Models;
using Edubase.Web.UI.Areas.Groups.Models.CreateEdit;
using Edubase.Web.UI.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    public class CreateEditGovernorViewModel : GovernorViewModel, IGroupPageViewModel, IEstablishmentPageViewModel
    {
        public enum EditMode { Create, Edit, Replace}

        public EditMode Mode { get; set; }
        public string GovernorRoleName { get; set; }
        public string GovernorRoleNameMidSentence { get; set; }
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
        GroupModel IEstablishmentPageViewModel.LegalParentGroup { get; set; }
        string IEstablishmentPageViewModel.LegalParentGroupToken { get; set; }

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

        public IEnumerable<SelectListItem> ExistingGovernors { get; set; } = Enumerable.Empty<SelectListItem>();

        public bool AllowReinstateAsGovernor => EstablishmentUrn.HasValue &&
                                                GovernorRole.OneOfThese(eLookupGovernorRole.ChairOfTrustees,
                                                    eLookupGovernorRole.ChairOfGovernors, eLookupGovernorRole.ChairOfLocalGoverningBody) &&
                                                !GovernorRole.IsSharedChairOfLocalGoverningBody();

        public GovernorModel SelectedGovernor { get; set; }

        public int? SelectedPreviousGovernorId { get; set; }
        public bool ReinstateAsGovernor{ get; set; }

        public string ReinstateAsGovernorCheckboxLabel =>
            "Re-instate as " + (GovernorRole == eLookupGovernorRole.ChairOfTrustees ? "trustee" : "governor");

        public string SelectPreviousGovernorLabel => "Choose " +
                                                     (GovernorRole == eLookupGovernorRole.ChairOfTrustees
                                                         ? "trustee"
                                                         : "governor");

    }
}
