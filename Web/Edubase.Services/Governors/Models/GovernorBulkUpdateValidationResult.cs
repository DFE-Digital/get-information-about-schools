using Edubase.Services.Domain;
using System;

namespace Edubase.Services.Governors.Models
{
    public class GovernorBulkUpdateValidationResult
    {
        public Guid? Id { get; set; }
        public FileDownloadDto ErrorLogFile { get; set; }
        public bool? Success { get; set; }
    }
}
