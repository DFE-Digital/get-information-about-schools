using Edubase.Services.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edubase.Services.Establishments.Models
{
    public class EstablishmentChangeHistoryDownloadFilters : EstablishmentChangeHistoryFilters
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public DownloadType FileFormat { get; set; }
    }
}