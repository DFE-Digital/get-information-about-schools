using System;

namespace Edubase.Common.Spatial
{
    /// <summary>
    /// Converts OS grid refs to WGS84 Lat/Long coordinates.
    /// </summary>
    public class OSGB36Converter
    {
        private const double a = 6377563.396;
        private const double b = 6356256.91;
        private const double e2 = (a - b) / a;
        private const double n0 = -100000;
        private const double e0 = 400000;
        private const double f0 = 0.999601272;
        private const double phi0 = 0.855211333;
        private const double lambda0 = -0.034906585;
        private const double n = (a - b) / (a + b);
        private RefEll airy1830;
        private RefEll wgs84;
        
        public OSGB36Converter()
        {
            airy1830 = new RefEll(6377563.396, 6356256.909);
            wgs84 = new RefEll(6378137.000, 6356752.3141);
        }

        public LatLon ToWGS84(string easting, string northing)
        {
            int? e = easting.ToInteger(), n = northing.ToInteger();
            if (e.HasValue && n.HasValue) return ToWGS84(e.Value, n.Value);
            else return null;
        }

        public LatLon ToWGS84(int? easting, int? northing)
        {
            if (easting.HasValue && northing.HasValue) return ToWGS84(easting.Value, northing.Value);
            else return null;
        }

        public LatLon ToWGS84(int easting, int northing)
        {
            double OSGB_F0 = 0.9996012717;
            double N0 = -100000.0;
            double E0 = 400000.0;
            double phi0 = Deg2Rad(49.0);
            double lambda0 = Deg2Rad(-2.0);
            double a = airy1830.maj;
            double b = airy1830.min;
            double eSquared = airy1830.ecc;
            double phi = 0.0;
            double lambda = 0.0;
            double E = easting;
            double N = northing;
            double n = (a - b) / (a + b);
            double M = 0.0;
            double phiPrime = ((N - N0) / (a * OSGB_F0)) + phi0;
            do
            {
                M =
                  (b * OSGB_F0)
                    * (((1 + n + ((5.0 / 4.0) * n * n) + ((5.0 / 4.0) * n * n * n))
                      * (phiPrime - phi0))
                      - (((3 * n) + (3 * n * n) + ((21.0 / 8.0) * n * n * n))
                        * Math.Sin(phiPrime - phi0)
                        * Math.Cos(phiPrime + phi0))
                      + ((((15.0 / 8.0) * n * n) + ((15.0 / 8.0) * n * n * n))
                        * Math.Sin(2.0 * (phiPrime - phi0))
                        * Math.Cos(2.0 * (phiPrime + phi0)))
                      - (((35.0 / 24.0) * n * n * n)
                        * Math.Sin(3.0 * (phiPrime - phi0))
                        * Math.Cos(3.0 * (phiPrime + phi0))));
                phiPrime += (N - N0 - M) / (a * OSGB_F0);
            } while ((N - N0 - M) >= 0.001);
            var v = a * OSGB_F0 * Math.Pow(1.0 - eSquared * SinSquared(phiPrime), -0.5);
            var rho =
              a
                * OSGB_F0
                * (1.0 - eSquared)
                * Math.Pow(1.0 - eSquared * SinSquared(phiPrime), -1.5);
            var etaSquared = (v / rho) - 1.0;
            var VII = Math.Tan(phiPrime) / (2 * rho * v);
            var VIII =
              (Math.Tan(phiPrime) / (24.0 * rho * Math.Pow(v, 3.0)))
                * (5.0
                  + (3.0 * TanSquared(phiPrime))
                  + etaSquared
                  - (9.0 * TanSquared(phiPrime) * etaSquared));
            var IX =
              (Math.Tan(phiPrime) / (720.0 * rho * Math.Pow(v, 5.0)))
                * (61.0
                  + (90.0 * TanSquared(phiPrime))
                  + (45.0 * TanSquared(phiPrime) * TanSquared(phiPrime)));
            var X = Sec(phiPrime) / v;
            var XI =
              (Sec(phiPrime) / (6.0 * v * v * v))
                * ((v / rho) + (2 * TanSquared(phiPrime)));
            var XII =
              (Sec(phiPrime) / (120.0 * Math.Pow(v, 5.0)))
                * (5.0
                  + (28.0 * TanSquared(phiPrime))
                  + (24.0 * TanSquared(phiPrime) * TanSquared(phiPrime)));
            var XIIA =
              (Sec(phiPrime) / (5040.0 * Math.Pow(v, 7.0)))
                * (61.0
                  + (662.0 * TanSquared(phiPrime))
                  + (1320.0 * TanSquared(phiPrime) * TanSquared(phiPrime))
                  + (720.0
                    * TanSquared(phiPrime)
                    * TanSquared(phiPrime)
                    * TanSquared(phiPrime)));
            phi =
              phiPrime
                - (VII * Math.Pow(E - E0, 2.0))
                + (VIII * Math.Pow(E - E0, 4.0))
                - (IX * Math.Pow(E - E0, 6.0));
            lambda =
              lambda0
                + (X * (E - E0))
                - (XI * Math.Pow(E - E0, 3.0))
                + (XII * Math.Pow(E - E0, 5.0))
                - (XIIA * Math.Pow(E - E0, 7.0));


            return OSGB36ToWGS84(new LatLon(Rad2Deg(phi), Rad2Deg(lambda)));
        }

        private LatLon OSGB36ToWGS84(LatLon loc)
        {
            var a = airy1830.maj;
            var b = airy1830.min;
            var eSquared = airy1830.ecc;
            var phi = Deg2Rad(loc.Latitude);
            var lambda = Deg2Rad(loc.Longitude);
            var v = a / (Math.Sqrt(1 - eSquared * SinSquared(phi)));
            var H = 0; // height
            var x = (v + H) * Math.Cos(phi) * Math.Cos(lambda);
            var y = (v + H) * Math.Cos(phi) * Math.Sin(lambda);
            var z = ((1 - eSquared) * v + H) * Math.Sin(phi);

            var tx = 446.448;
            var ty = -124.157;
            var tz = 542.060;
            var s = -0.0000204894;
            var rx = Deg2Rad(0.00004172222);
            var ry = Deg2Rad(0.00006861111);
            var rz = Deg2Rad(0.00023391666);

            var xB = tx + (x * (1 + s)) + (-rx * y) + (ry * z);
            var yB = ty + (rz * x) + (y * (1 + s)) + (-rx * z);
            var zB = tz + (-ry * x) + (rx * y) + (z * (1 + s));

            a = wgs84.maj;
            b = wgs84.min;
            eSquared = wgs84.ecc;

            var lambdaB = Rad2Deg(Math.Atan(yB / xB));
            var p = Math.Sqrt((xB * xB) + (yB * yB));
            var phiN = Math.Atan(zB / (p * (1 - eSquared)));
            for (var i = 1; i < 10; i++)
            {
                v = a / (Math.Sqrt(1 - eSquared * SinSquared(phiN)));
                double phiN1 = Math.Atan((zB + (eSquared * v * Math.Sin(phiN))) / p);
                phiN = phiN1;
            }

            var phiB = Rad2Deg(phiN);

            return new LatLon(phiB, lambdaB);
        }
        
        private double Deg2Rad(double x) => x * (Math.PI / 180);

        private double Rad2Deg(double x) => x * (180 / Math.PI);

        private double SinSquared(double x) => Math.Sin(x) * Math.Sin(x);

        private double TanSquared(double x) => Math.Tan(x) * Math.Tan(x);

        private double Sec(double x) => 1.0 / Math.Cos(x);
    }
}
