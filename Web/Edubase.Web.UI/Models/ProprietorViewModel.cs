using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Edubase.Common;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Edubase.Web.UI.Models
{
    public class ProprietorViewModel
    {
        [DisplayName("Id")] public int? Id { get; set; }

        [DisplayName("Name")] public string Name { get; set; }

        [DisplayName("Street")] public string Street { get; set; }

        [DisplayName("Locality")] public string Locality { get; set; }

        [DisplayName("Address 3")] public string Address3 { get; set; }

        [DisplayName("Town")] public string Town { get; set; }

        // hack as the backend cant return an empty model although that is what has been saved (43744)
        public int? CountyIdDefault { get; set; }
        private int? _countyId;
        [DisplayName("County")]
        public int? CountyId
        {
            get => _countyId ?? CountyIdDefault;
            set => _countyId = value;
        }

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
