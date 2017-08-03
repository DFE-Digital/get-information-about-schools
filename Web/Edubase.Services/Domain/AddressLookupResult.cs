﻿using Edubase.Common;
using Edubase.Common.Text;

namespace Edubase.Services.Domain
{
    public class AddressLookupResult
    {
        public AddressLookupResult(AddressBaseResult result)
        {
            Street = StringUtil.ConcatNonEmpties(" ", result.BuildingNumber, result.ThoroughfareName?.ToLower()?.ToTitleCase());
            Town = result.PostTown?.ToLower()?.ToTitleCase();
            UPRN = result.UPRN;
            PostCode = result.Postcode;
            Easting = result.xCoordinate.ToInteger();
            Northing = result.yCoordinate.ToInteger();
        }

        public AddressLookupResult()
        {

        }

        public string Street { get; set; }
        public string Town { get; set; }
        public string UPRN { get; set; }
        public string PostCode { get; set; }
        public int? Easting { get; set; }
        public int? Northing { get; set; }

        public override string ToString() => Street + ", " + Town;
    }
}
