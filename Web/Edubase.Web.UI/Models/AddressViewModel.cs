using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Edubase.Web.UI.Models
{
    public class AddressViewModel
    {
        [Display(Name="Street (required to save record)")]
        public string Line1 { get; set; }

        [Display(Name = "Locality")]
        public string Line2 { get; set; }

        [Display(Name = "Address 3")]
        public string Line3 { get; set; }

        [Display(Name = "Town (required to save record)")]
        public string CityOrTown { get; set; }

        [Display(Name = "County (required to save record)")]
        public int? County { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Postcode (required to save record)")]
        public string PostCode { get; set; }

        public IEnumerable<SelectListItem> Counties { get; set; }
    }
}
