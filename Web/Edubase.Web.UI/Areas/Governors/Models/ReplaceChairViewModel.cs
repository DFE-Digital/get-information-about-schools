using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Edubase.Services.Enums;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Establishments;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    public class ReplaceChairViewModel : IEstablishmentPageViewModel
    {
        public enum ChairType { SharedChair, LocalChair }
        public string GovernorFullName { get; set; }
        public int ExistingGovernorId { get; set; }
        public ChairType ExistingChairType { get; set; }
        public eLookupGovernorRole Role { get; set; }

        [Display(Name = "Date term ends")]
        public DateTimeViewModel DateTermEnds { get; set; }

        public List<SharedGovernorViewModel> SharedGovernors { get; set; }
        public GovernorViewModel NewLocalGovernor { get; set; }
        public ChairType NewChairType { get; set; }
        public int SelectedGovernorId { get; set; }

        public string SelectedTab { get; set; }
        public int? Urn { get; set; }
        public string Name { get; set; }
        public TabDisplayPolicy TabDisplayPolicy { get; set; }
        public string Layout { get; set; }
    }
}