namespace Edubase.Services.Texuna.ChangeHistory.Models
{
    public class SearchChangeHistoryBrowsePayload : SearchChangeHistoryPayload
    {
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}
