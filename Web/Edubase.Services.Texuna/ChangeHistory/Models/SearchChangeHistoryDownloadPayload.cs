using Edubase.Services.Enums;
using Newtonsoft.Json;

namespace Edubase.Services.Texuna.ChangeHistory.Models
{
    public class SearchChangeHistoryDownloadPayload : SearchChangeHistoryPayload
    {
        [JsonIgnore]
        public eFileFormat FileFormat { get; set; }

        [JsonProperty("fileFormat")]
        public string FileFormatString => FileFormat.ToString().ToLower();

        public SearchChangeHistoryDownloadPayload()
        {

        }

        public SearchChangeHistoryDownloadPayload(eFileFormat fileFormat)
        {
            FileFormat = fileFormat;
        }
    }
}
