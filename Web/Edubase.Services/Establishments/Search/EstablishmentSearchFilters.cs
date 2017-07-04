using Newtonsoft.Json;
using System;

namespace Edubase.Services.Establishments.Search
{
    public class EstablishmentSearchFilters : EstablishmentSearchFiltersLookups
    {
        [JsonProperty("UKPRN")]
        public string UKPRN { get; set; }
        public int? EstablishmentNumber { get; set; }
        public DateTime? OpenDateMin { get; set; }
        public DateTime? OpenDateMax { get; set; }
    }
}
