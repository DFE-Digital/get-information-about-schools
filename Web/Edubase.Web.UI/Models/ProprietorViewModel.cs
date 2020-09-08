using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Edubase.Common;

namespace Edubase.Web.UI.Models
{
    public class ProprietorViewModel
    {
        [Display(Name = "Name")] public string Name { get; set; }

        [DisplayName("Street")] public string Street { get; set; }

        [DisplayName("Locality")] public string Locality { get; set; }

        [DisplayName("Address 3")] public string Address3 { get; set; }

        [DisplayName("Town")] public string Town { get; set; }

        [DisplayName("County")] public int? CountyId { get; set; }

        [DisplayName("Postcode")] public string Postcode { get; set; }

        [DisplayName("Telephone number")] public string TelephoneNumber { get; set; }

        [DisplayName("Email")] public string Email { get; set; }

        public IEnumerable<SelectListItem> Counties { get; internal set; }

        public string GetAddress() => StringUtil.ConcatNonEmpties(", ",
            Street,
            Locality,
            Address3,
            Town,
            CountyName?.Replace("Not recorded", string.Empty),
            Postcode);

        public string CountyName => Counties.Where(x => x.Value.Equals(CountyId)).Select(x => x.Text).ToString();

        public Guid Index  => Guid.NewGuid();
    }
}
