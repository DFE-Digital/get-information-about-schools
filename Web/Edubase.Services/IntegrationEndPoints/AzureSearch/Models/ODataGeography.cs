using Edubase.Common.Spatial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.IntegrationEndPoints.AzureSearch.Models
{
    public class ODataGeographyExpression
    {
        private LatLon _coord;
        private string _fieldName = null;

        public ODataGeographyExpression(LatLon coordinate)
        {
            _coord = coordinate;
        }

        public ODataGeographyExpression(LatLon coordinate, string fieldName)
            : this(coordinate)
        {
            _fieldName = fieldName;
        }

        public string ToFilterODataExpression(double maxKilometers, string fieldName = null)
        {
            return string.Concat(ToODataExpression(fieldName), " le " + maxKilometers);
        }

        public string ToODataExpression(string fieldName = null)
        {
            return $"geo.distance({fieldName ?? _fieldName}, geography'POINT({_coord.Longitude} {_coord.Latitude})')";
        }
    }
}
