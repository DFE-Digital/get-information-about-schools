using System;

namespace Edubase.Common.Spatial
{
    public static class DistanceCalculator
    {
        private const double EarthRadiusMetres = 6_371_000d;

        public static Distance Calculate(LatLon coord1, LatLon coord2)
        {
            Guard.IsNotNull(coord1, () => new ArgumentNullException(nameof(coord1)));
            Guard.IsNotNull(coord2, () => new ArgumentNullException(nameof(coord2)));

            var latitude1 = DegreesToRadians(coord1.Latitude);
            var latitude2 = DegreesToRadians(coord2.Latitude);
            var deltaLatitude = DegreesToRadians(coord2.Latitude - coord1.Latitude);
            var deltaLongitude = DegreesToRadians(coord2.Longitude - coord1.Longitude);

            var a = Math.Sin(deltaLatitude / 2) * Math.Sin(deltaLatitude / 2) +
                    Math.Cos(latitude1) * Math.Cos(latitude2) *
                    Math.Sin(deltaLongitude / 2) * Math.Sin(deltaLongitude / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return new Distance(EarthRadiusMetres * c, Distance.eUnit.Metres);
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180d);
        }
    }
}
