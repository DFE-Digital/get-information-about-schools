using Edubase.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Establishments.Models
{
    [Serializable]
    public class EstablishmentAddressModel
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string CityOrTown { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public string Locality { get; set; }
        public string PostCode { get; set; }
        public override string ToString() => StringUtil.ConcatNonEmpties(", ",
            Line1,
            Line2,
            Line3,
            Locality,
            CityOrTown,
            County,
            PostCode);
    }
}
