using System;
using Newtonsoft.Json;

namespace Edubase.Services.Downloads.Models
{
    public class FileDownload
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public long FileSizeInBytes { get; set; }
        public string Tag { get; set; }
        public bool RequiresGeneration { get; set; }
        public bool AuthenticationRequired { get; set; }
        public DateTime? FileGeneratedDate { get; set; } = DateTime.Now;

        [JsonIgnore]
        public bool Selected { get; set; }
    }
}
