using System;

namespace Edubase.Services.Texuna.ChangeHistory.Models
{
    public class ChangeHistorySearchItem
    {
        public string EstablishmentName { get; set; }
        public string GroupName { get; set; }
        public int? GroupUId { get; set; }
        public string SuggesterName { get; set; }
        public  string ApproverName { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? RequestedDate { get; set; }
        public DateTime? DateChanged { get; set; }
        public DateTime? EstablishmentUrn { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
