using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Edubase.Web.UI.Models.Establishments
{
    public class CreateChildrensCentreViewModel : CreateEstablishmentViewModel
    {
        [Display(Name = "Open date (required tp save record)")]
        public DateTimeViewModel OpenDate { get; set; }

        [Display(Name= "Address details")]
        public AddressViewModel Address { get; set; }

        [Display(Name = "Manager first name (required to save record)")]
        public string ManagerFirstName { get; set; }

        [Display(Name = "Manager last name (required to save record)")]
        public string ManagerLastName { get; set; }

        [Display(Name = "Manager email (required to save record)")]
        public string ManagerEmail { get; set; }

        [Display(Name = "Telephone (required to save record)")]
        public string Telephone { get; set; }

        [Display(Name = "Operational hours (required to save record)")]
        public int? OperationalHoursId { get; set; }

        [Display(Name = "Number of under 5s (required to save record)")]
        public int? NumberOfUnderFives { get; set; }

        [Display(Name = "Governance (required to save record)")]
        public int? GovernanceId { get; set; }

        [Display(Name = "Governance detail (required to save record)")]
        public string GovernanceDetail { get; set; }

        [Display(Name = "Disadvantaged area (required to save record)")]
        public int? DisadvantagedAreaId { get; set; }

        [Display(Name = "Direct provision of early years (required to save record)")]
        public int? DirectProvisionOfEarlyYears { get; set; }

        [Display(Name = "Establishment status")]
        public int? EstablishmentStatusId { get; set; }

        public IEnumerable<SelectListItem> OperationalHoursOptions { get; set; }
        public IEnumerable<SelectListItem> GovernanceOptions { get; set; }
        public IEnumerable<SelectListItem> DisadvantagedAreaOptions { get; set; }
        public IEnumerable<SelectListItem> DirectProvisionOfEarlyYearsOptions { get; set; }
        public IEnumerable<SelectListItem> EstablishmentStatusOptions { get; set; }
    }
}