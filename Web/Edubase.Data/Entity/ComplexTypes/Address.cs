using Edubase.Common;
using Edubase.Data.Entity.Permissions;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edubase.Data.Entity.ComplexTypes
{
    [ComplexType, Serializable]
    public class Address
    {
        [RequiresApproval]
        public string Line1 { get; set; }

        [RequiresApproval]
        public string Line2 { get; set; }

        [RequiresApproval]
        public string Line3 { get; set; }

        [RequiresApproval]
        public string CityOrTown { get; set; }

        [RequiresApproval]
        public string County { get; set; }

        [RequiresApproval]
        public string Country { get; set; }

        [RequiresApproval]
        public string Locality { get; set; }

        [RequiresApproval]
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
