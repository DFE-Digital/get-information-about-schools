using Edubase.Services.Enums;
using Edubase.Services.Establishments.Search;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edubase.Services.Establishments.Downloads
{
    public class EstablishmentSearchDownloadPayload
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public eDataSet DataSet { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public eFileFormat FileFormat { get; set; }
        
        public EstablishmentSearchPayload SearchPayload { get; set; }
    }
}
