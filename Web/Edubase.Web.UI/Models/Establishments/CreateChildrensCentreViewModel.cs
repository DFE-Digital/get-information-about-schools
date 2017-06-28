using System.ComponentModel.DataAnnotations;

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
    }
}