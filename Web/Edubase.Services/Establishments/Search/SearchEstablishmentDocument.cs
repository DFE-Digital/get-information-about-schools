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
    }
}
