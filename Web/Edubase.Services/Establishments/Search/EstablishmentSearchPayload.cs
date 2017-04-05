using Edubase.Common.Spatial;
using Edubase.Services.Core.Search;
using Edubase.Services.Establishments.Models;
using Newtonsoft.Json;
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

        public EstablishmentSearchPayload(int skip, int take)
        {
            Skip = skip;
            Take = take;
        }

        public string Text { get; set; }

        public EstablishmentSearchFilters Filters { get; set; } = new EstablishmentSearchFilters();

        public int Skip { get; set; }

        public int Take { get; set; } = 10;

        public int? RadiusInMiles { get; set; }

        public LatLon GeoSearchLocation { get; set; }

        [JsonIgnore]
        public eSortBy SortBy { get; set; }

        [JsonProperty("sortBy")]
        internal string SortByDescriptor => SortBy.ToString();


    }
}
