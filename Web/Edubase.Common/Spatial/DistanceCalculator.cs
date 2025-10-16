using System;
using GeoCoordinatePortable;

namespace Edubase.Common.Spatial
{
    public static class DistanceCalculator
    {
        public static Distance Calculate(LatLon coord1, LatLon coord2)
        {
            Guard.IsNotNull(coord1, () => new ArgumentNullException(nameof(coord1)));
            Guard.IsNotNull(coord2, () => new ArgumentNullException(nameof(coord2)));

            var p1 = new GeoCoordinate(coord1.Latitude, coord1.Longitude);
            var p2 = new GeoCoordinate(coord2.Latitude, coord2.Longitude);
            return new Distance(p1.GetDistanceTo(p2), Distance.eUnit.Metres);
        }
    }
}
