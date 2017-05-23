using System;
using System.Linq;

namespace Edubase.Services.Domain
{
    public class BulkUpdateProgressModel
    {
        public Guid? Id { get; set; }
        public bool IsComplete { get; set; }
        public string Status { get; set; }
        public FileDownloadDto ErrorLogFile { get; set; }
        public ApiError[] Errors { get; set; } = new ApiError[0];
        public bool IsCompleted() => (Status ?? "").Equals("FAILED", StringComparison.OrdinalIgnoreCase) || IsComplete;
        public bool IsFailure() => Status == "FAILED" || (Errors != null && Errors.Any());
    }
}
