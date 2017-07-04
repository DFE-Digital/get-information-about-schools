namespace Edubase.Services.Texuna.ChangeHistory.Models
{
    public class SearchChangeHistoryBrowsePayload : SearchChangeHistoryPayload
    {
        public int Skip { get; set; }
        public int Take { get; set; }

        public SearchChangeHistoryBrowsePayload()
        {

        }

        public SearchChangeHistoryBrowsePayload(int skip, int take)
        {
            Skip = skip;
            Take = take;
        }
    }
}
