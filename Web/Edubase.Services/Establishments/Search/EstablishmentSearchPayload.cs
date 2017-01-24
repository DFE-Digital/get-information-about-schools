using Edubase.Common.Spatial;
using Edubase.Services.Establishments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Establishments.Search
{
    public class EstablishmentSearchPayload
    {
        public EstablishmentSearchPayload()
        {

        }

        public EstablishmentSearchPayload(string orderBy, int skip, int take)
        {
            OrderBy = new List<string> { orderBy };
            Skip = skip;
            Take = take;
        }

        public string Text { get; set; }

        public EstablishmentSearchFilters Filters { get; set; } = new EstablishmentSearchFilters();

        public int Skip { get; set; }

        public int Take { get; set; } = 10;

        public int? GeoSearchMaxRadiusInKilometres { get; set; } = 10;

        public LatLon GeoSearchLocation { get; set; }

        public bool GeoSearchOrderByDistance { get; set; }
        

        public IList<string> OrderBy { get; set; } = new List<string>();

        public int[] SENIds { get; set; } = new int[0];
    }
}
