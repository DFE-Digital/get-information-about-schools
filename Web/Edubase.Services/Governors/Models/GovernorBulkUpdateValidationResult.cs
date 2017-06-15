using Edubase.Services.Domain;
using Newtonsoft.Json;
using System;

namespace Edubase.Services.Governors.Models
{
    public class GovernorBulkUpdateValidationResult
    {
        [JsonProperty("uuid")]
        public string Id { get; set; }
        [JsonProperty("fileDownload")]
        public FileDownloadDto ErrorLogFile { get; set; }
        public bool? Success { get; set; }
        public ApiError[] Errors { get; set; }
    }
}
