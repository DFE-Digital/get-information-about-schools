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
        public int RowErrors { get; set; }
        public bool IsCompleted() => (Status ?? "").Equals("FAILED", StringComparison.OrdinalIgnoreCase) || IsComplete;
        public bool IsFailure() => Status == "FAILED" || (Errors != null && Errors.Any());
        public bool CompletedWithErrors() => string.Equals(Status, "COMPLETED WITH ERRORS") || RowErrors > 0;
    }
}
