using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Establishments.Models
{
    public class ProprietorModel
    {
        public string Name { get; set; }
        public string Street { get; set; }
        public string Locality { get; set; }
        public string Address3 { get; set; }
        public string Town { get; set; }
        public int? CountyId { get; set; }
        public string Postcode { get; set; }
        public string TelephoneNumber { get; set; }
        public string Email { get; set; }

    }
}
