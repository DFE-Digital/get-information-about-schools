using Newtonsoft.Json;
using System;

namespace Edubase.Services.Establishments.Models
{
    public class EstablishmentSearchResultModel : EstablishmentModel
    {
        public string PredecessorName { get; set; }
        public string PredecessorUrn { get; set; }

        [JsonProperty("nextActionRequiredByWEL")]
        public DateTime? NextActionRequiredByWEL { get; set; }
        
        [JsonProperty("nextGeneralActionRequired")]
        public DateTime? NextGeneralActionRequired { get; set; }
    }
}
