using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Edubase.Services.Downloads.Models
{
    public class FileDownloadRequest
    {
        public FileDownloadRequest(string tag, DateTime? generated)
        {
            Id = tag;
            FileGeneratedDate = generated;
        }
        public string Id { get; set; }

        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? FileGeneratedDate { get; set; }
    }
}
