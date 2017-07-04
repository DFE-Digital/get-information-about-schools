using System;
using Edubase.Services.Enums;

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
        public int? EstablishmentUrn { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public GroupChangeRequestType? RequestType { get; set; }
        public GroupChangeLinkType? LinkType { get; set; }
        public DateTime? LinkDateUtc { get; set; }
        public int? LinkUrn { get; set; }
        public string LinkEstablishmentName { get; set; }
    }
}
