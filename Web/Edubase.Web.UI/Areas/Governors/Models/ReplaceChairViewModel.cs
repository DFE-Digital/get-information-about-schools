using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Edubase.Common;
using Edubase.Services.Enums;
using Edubase.Services.Governors.Models;
using Edubase.Web.UI.Areas.Establishments.Models;
using Edubase.Web.UI.Models;
using Edubase.Services.Groups.Models;
using Edubase.Web.UI.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    public class ReplaceChairViewModel : IEstablishmentPageViewModel
    {
        public enum ChairType { SharedChair, LocalChair }
        public string GovernorFullName { get; set; }
        public int ExistingGovernorId { get; set; }
        public ChairType ExistingChairType { get; set; }
        public eLookupGovernorRole Role { get; set; }
        public string RoleName { get; set; }

        [Display(Name = "Date term ends")]
        public DateTimeViewModel DateTermEnds { get; set; } = new DateTimeViewModel();

        public List<SharedGovernorViewModel> SharedGovernors { get; set; }
        public GovernorViewModel NewLocalGovernor { get; set; }
        public ChairType NewChairType { get; set; }

        private int _selectedGovernorId;

        public int SelectedGovernorId
        {
            get => _selectedGovernorId;
            set
            {
                _selectedGovernorId = value;
                var first = SharedGovernors.FirstOrDefault(x => x.Id == value);
                if (first != null)
                {
                    first.Selected = true;
                }
            }
        }

        public string SelectedTab { get; set; }
        public int? Urn { get; set; }
        public string Name { get; set; }
        public TabDisplayPolicy TabDisplayPolicy { get; set; }
        public string Layout { get; set; }
        public string TypeName { get; set; }
        GroupModel IEstablishmentPageViewModel.LegalParentGroup { get; set; }
        string IEstablishmentPageViewModel.LegalParentGroupToken { get; set; }

        public bool AllowReinstatement => Urn.HasValue &&
                                          Role.OneOfThese(eLookupGovernorRole.ChairOfTrustees,
                                              eLookupGovernorRole.ChairOfGovernors, eLookupGovernorRole.ChairOfLocalGoverningBody) && !Role.IsSharedChairOfLocalGoverningBody();
        public bool Reinstate { get; set; }
        public IEnumerable<SelectListItem> ExistingNonChairs { get; set; } = Enumerable.Empty<SelectListItem>();
        public int? SelectedPreviousExistingNonChairId { get; set; }
        public GovernorModel SelectedNonChair { get; set; }
    }
}
