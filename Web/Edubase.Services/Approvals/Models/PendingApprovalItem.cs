using System;

namespace Edubase.Services.Approvals.Models
{
    public class PendingApprovalItem
    {
        public int Id { get; set; }
        public string OriginatorUserName { get; set; }
        public string ApproverUserName { get; set; }
        public DateTime? EffectiveDateUtc { get; set; }
        public DateTime? RequestedDateUtc { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string OriginatorFullName { get; set; }
        public int EstablishmentUrn { get; set; }
        public string EstablishmentName { get; set; }

    }
}
