using System.ComponentModel.DataAnnotations;

namespace Edubase.Web.UI.Models
{
    public class AddressViewModel
    {
        [Display(Name="Street")]
        public string Line1 { get; set; }

        [Display(Name = "Locality")]
        public string Line2 { get; set; }

        [Display(Name = "Address 3")]
        public string Line3 { get; set; }

        [Display(Name = "Town")]
        public string CityOrTown { get; set; }

        [Display(Name = "County")]
        public string County { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Postcode")]
        public string PostCode { get; set; }
    }
}