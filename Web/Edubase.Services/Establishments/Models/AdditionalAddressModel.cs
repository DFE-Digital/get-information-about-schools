using Edubase.Common;
using Edubase.Common.Reflection;
using Edubase.Common.Spatial;
using Edubase.Services.Lookup;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Edubase.Services.Establishments.Models
{
    public class AdditionalAddressModel
    {
        public int? Id { get; set; }
        public string SiteName { get; set; }
        public int? CountryId { get; set; }
        [IgnoreChanges, JsonProperty("UPRN")]
        public string UPRN { get; set; }
        public string Street { get; set; }
        public string Locality { get; set; }
        public string Address3 { get; set; }
        public string Town { get; set; }
        public int? CountyId { get; set; }
        public string PostCode { get; set; }
        [JsonProperty("latLon")]
        public LatLon Location { get; set; }
    }
}
