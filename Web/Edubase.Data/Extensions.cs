using Edubase.Common.Spatial;
using MoreLinq;
using System.Data.Entity.Spatial;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;

namespace Edubase.Data
{
    public static class Extensions
    {
        public static string ExtractValidationErrorsReport(this DbEntityValidationException exception) 
            => string.Join("\r\n", exception.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.PropertyName + ", " + x.ErrorMessage));

        public static DbGeography ToDBGeography(this LatLon coord) 
            => coord != null ? DbGeography.PointFromText(string.Format("POINT({0} {1})", coord.Longitude, coord.Latitude), 4326) : null;

        public static LatLon ToLatLon(this DbGeography geo) 
            => geo != null && geo.Latitude.HasValue && geo.Longitude.HasValue 
            ? new LatLon(geo.Latitude.Value, geo.Longitude.Value) 
            : null as LatLon;
    }
    
}
