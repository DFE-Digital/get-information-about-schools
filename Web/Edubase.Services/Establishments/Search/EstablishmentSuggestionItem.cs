using Edubase.Common;
using Newtonsoft.Json;

namespace Edubase.Services.Establishments.Search
{
    public class EstablishmentSuggestionItem
    {
        public int Id => Urn;
        public int Urn { get; set; }
        public string Name { get; set; }

        [JsonProperty("addressCityOrTown")]
        public string Address_CityOrTown { get; set; }

        [JsonProperty("addressPostCode")]
        public string Address_PostCode { get; set; }

        public bool Closed { get; set; }

        public string Text => Name + (Address_CityOrTown.Clean() == null && Address_PostCode.Clean() == null 
            ? string.Empty 
            : string.Concat(" (", StringUtil.ConcatNonEmpties(", ", Address_CityOrTown, Address_PostCode), ")"));
    }
}
