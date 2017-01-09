using Edubase.Common;
using Edubase.Services.Establishments.Models;
using System;
using Edubase.Common.Spatial;

namespace Edubase.Services.Establishments.Search
{
    public class SearchEstablishmentDocument : EstablishmentModelBase
    {
        public override LatLon Coordinate => Location == null || Location.IsEmpty ? null as LatLon : new LatLon(Location.Latitude, Location.Longitude);

        public Microsoft.Spatial.GeographyPoint Location { get; set; }

        //public string GetAddress() => StringUtil.ConcatNonEmpties(", ", Address_Line1, Address_Line2, Address_Line3, Address_Locality, Address_CityOrTown, Address_County, Address_PostCode);

        //public string GetLAESTAB() => string.Concat(LocalAuthorityId, EstablishmentNumber.GetValueOrDefault().ToString("D4"));
    }
}
