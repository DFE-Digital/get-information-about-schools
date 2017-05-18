using Newtonsoft.Json;

namespace Edubase.Services.Establishments.Search
{
    public class EstablishmentSearchFilters : EstablishmentSearchFiltersLookups
    {
        [JsonIgnore, JsonProperty("UKPRN")]
        public int? UKPRN { get; set; }

        [JsonIgnore]
        public int? EstablishmentNumber { get; set; }

    }
}
