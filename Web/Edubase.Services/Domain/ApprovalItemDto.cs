using System;

namespace Edubase.Services.Domain
{
    public class ApprovalItemDto
    {
        public int Id { get; set; }
        public string EstablishmentName { get; set; }
        public string UpdatedFieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime DateOfChange { get; set; }
        public string OriginatorName { get; set; }
        public int EstablishmentUrn { get; set; }

    }
}
