using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Common;

namespace Edubase.Services.Establishments.Models
{
    public class ProprietorModel
    {
        public int? Id { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Street")]
        public string Street { get; set; }

        [DisplayName("Locality")]
        public string Locality { get; set; }

        [DisplayName("Address 3")]
        public string Address3 { get; set; }

        [DisplayName("Town")]
        public string Town { get; set; }

        [DisplayName("County")]
        public int? CountyId { get; set; }

        [DisplayName("Postcode")]
        public string Postcode { get; set; }

        [DisplayName("Telephone number")]
        public string TelephoneNumber { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        public string GetAddress() => StringUtil.ConcatNonEmpties(", ",
            Street,
            Locality,
            Address3,
            Town,
            Postcode);

    }
}
