using Edubase.Common;
using Edubase.Data.Entity.Permissions;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edubase.Data.Entity.ComplexTypes
{
    [ComplexType, Serializable]
    public class Address
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
