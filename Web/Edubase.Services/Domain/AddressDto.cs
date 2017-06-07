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
        public int? CountyId { get; set; }
        public int? CountryId { get; set; }
        public string PostCode { get; set; }

        public override string ToString() => Common.StringUtil.ConcatNonEmpties(", ", Line1, Line2, Line3, CityOrTown, PostCode);
    }
}
