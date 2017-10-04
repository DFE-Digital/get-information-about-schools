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
        public DateTime? CloseDateMin { get; set; }
        public DateTime? CloseDateMax { get; set; }
        public int? StatutoryLowAgeMin { get; set; }
        public int? StatutoryLowAgeMax { get; set; }
        public int? StatutoryHighAgeMin { get; set; }
        public int? StatutoryHighAgeMax { get; set; }
        public int[] OfstedRatingIds { get; set; }

        [JsonProperty("nextActionRequiredByWELMin")]
        public DateTime? NextActionRequiredByWELMin { get; set; }

        [JsonProperty("nextActionRequiredByWELMax")]
        public DateTime? NextActionRequiredByWELMax { get; set; }

        [JsonProperty("nextGeneralActionRequiredMin")]
        public DateTime? NextGeneralActionRequiredMin { get; set; }

        [JsonProperty("nextGeneralActionRequiredMax")]
        public DateTime? NextGeneralActionRequiredMax { get; set; }
    }
}
