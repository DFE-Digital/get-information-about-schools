using System;

namespace Edubase.Services.Texuna.ChangeHistory.Models
{
    public abstract class SearchChangeHistoryPayload
    {
        public string EntityName { get; set; }
        public string SuggesterUserGroupCode { get; set; }
        public string ApproverUserGroupCode { get; set; }
        public int[] EstablishmentTypeIds { get; set; }
        public int[] GroupTypeIds { get; set; }
        public string[] EstablishmentFieldIds { get; set; }
        public DateTime? AppliedDateFrom { get; set; }
        public DateTime? AppliedDateTo { get; set; }
        public DateTime? EffectiveDateFrom { get; set; }
        public DateTime? EffectiveDateTo { get; set; }
        public DateTime? ApprovedDateFrom { get; set; }
        public DateTime? ApprovedDateTo { get; set; }
        public string SortBy { get; set; }
    }
}
