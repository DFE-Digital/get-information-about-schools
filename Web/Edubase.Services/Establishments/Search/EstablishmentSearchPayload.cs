using Edubase.Common.Spatial;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

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

        public double? RadiusInMiles { get; set; }

        public LatLon GeoSearchLocation { get; set; }

        [JsonIgnore]
        public eSortBy SortBy { get; set; }

        [JsonProperty("sortBy")]
        internal string SortByDescriptor => SortBy.ToString();

        [JsonIgnore]
        public List<string> Select { get; set; } = new List<string>();

        [JsonProperty("select")]
        public IEnumerable<string> SelectFields => Select.Select(x => Establishment2JsonPropertyMap.Convert(x));

    }
}
