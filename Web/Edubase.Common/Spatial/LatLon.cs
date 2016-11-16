namespace Edubase.Common.Spatial
{
    public class LatLon
    {
        public double Latitude;
        public double Longitude;

        public LatLon(double lat, double lon)
        {
            Latitude = lat;
            Longitude = lon;
        }

        /// <summary>
        /// Attempts to parse the text as "{lat},{long}"; otherwise returns null.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static LatLon Parse(string text)
        {
            var lat = text.GetPart(",", 0).ToDouble();
            var lng = text.GetPart(",", 1).ToDouble();
            if (lat.HasValue && lng.HasValue) return new LatLon(lat.Value, lng.Value);
            else return null;
        }
    }
}
