using System.ComponentModel.DataAnnotations;
using Edubase.Data.Entity;

namespace Edubase.Web.UI.Models.DataQuality
{
    public class DataQualityDataOwnerItem
    {
        public DataQualityStatus.DataQualityEstablishmentType EstablishmentType { get; set; }

        [Display(Name = "Data Owner Name")]
        [Required(ErrorMessage = "The data owner's name is required")]
        public string Name { get; set; }

        [Display(Name = "Email address")]
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
    }
}
