//using Edubase.Common;
//using Edubase.Common.Spatial;
//using Edubase.Data.DbContext;
//using MoreLinq;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity.Spatial;
//using System.Data.Entity.Validation;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Edubase.Data
//{
//    public static class Extensions
//    {
//        public static string ExtractValidationErrorsReport(this DbEntityValidationException exception) 
//            => string.Join("\r\n", exception.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.PropertyName + ", " + x.ErrorMessage));

//        public static DbGeography ToDBGeography(this LatLon coord) 
//            => coord != null ? DbGeography.PointFromText(string.Format("POINT({0} {1})", coord.Longitude, coord.Latitude), 4326) : null;

//        public static LatLon ToLatLon(this DbGeography geo) 
//            => geo != null && geo.Latitude.HasValue && geo.Longitude.HasValue 
//            ? new LatLon(geo.Latitude.Value, geo.Longitude.Value) 
//            : null as LatLon;

//        public static DataTable Get<T>(this Dictionary<Type, DataTable> dictionary) => dictionary.Get(typeof(T));
//    }
    
//}
