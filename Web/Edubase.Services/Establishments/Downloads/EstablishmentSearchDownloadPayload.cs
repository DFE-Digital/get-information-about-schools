using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.Search;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edubase.Services.Establishments.Downloads
{
    public class EstablishmentSearchDownloadPayload : SearchDownloadDto<EstablishmentSearchPayload>
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public eDataSet DataSet { get; set; }
    }
}
