using System;
using System.Collections.Generic;

namespace Edubase.Services.Downloads.Models
{
    public class FileDownloadRequestArray
    {
        public List<FileDownloadRequest> Files { get; set; } = new List<FileDownloadRequest>();
    }

    public class FileDownloadRequest
    {
        public FileDownloadRequest(string tag, DateTime? generated)
        {
            Id = tag;
            FileGeneratedDate = generated;
        }
        public string Id { get; set; }
        public DateTime? FileGeneratedDate { get; set; }
    }
}
