using Edubase.Common;
using System;

namespace Edubase.Services.Domain
{
    public class ProgressDto
    {
        public Guid? Id { get; set; }
        public long? TotalRecordsCount { get; set; }
        public int? ProcessedCount { get; set; }
        public string Status { get; set; }
        public bool IsComplete { get; set; }
        public string FileLocationUri { get; set; }
        public string Error { get; set; }
        public bool HasErrored => Error.Clean() != null;
        public string Name { get; set; }
        public long? FileSizeInBytes { get; set; }

        public ProgressDto()
        {

        }

        public ProgressDto(Guid id)
        {
            Id = id;
        }
    }
}
