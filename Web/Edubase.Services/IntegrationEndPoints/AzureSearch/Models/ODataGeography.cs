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

        public ODataGeographyExpression(LatLon coordinate)
        {
            _coord = coordinate;
        }

        public string ToFilterODataExpression(string indexFieldName, int maxKilometers)
        {
            return string.Concat(ToODataExpression(indexFieldName), " le " + maxKilometers);
        }

        public string ToODataExpression(string indexFieldName)
        {
            return $"geo.distance({indexFieldName}, geography'POINT({_coord.Longitude} {_coord.Latitude})')";
        }
    }
}
