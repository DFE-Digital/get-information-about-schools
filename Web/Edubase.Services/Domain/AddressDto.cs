using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Domain
{
    public class AddressDto
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string CityOrTown { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public string PostCode { get; set; }
    }
}
