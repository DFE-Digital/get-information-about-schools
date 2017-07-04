using Edubase.Services.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edubase.Services.Domain
{
    public class SearchDownloadDto<T>
    {
        [JsonIgnore]
        public eFileFormat FileFormat { get; set; }

        [JsonProperty("fileFormat")]
        public string FileFormatDescription => FileFormat == eFileFormat.CSV ? "csv" : "xlsx";
        
        public T SearchPayload { get; set; }
    }
}
