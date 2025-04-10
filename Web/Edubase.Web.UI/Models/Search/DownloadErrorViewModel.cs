using Edubase.Services.Enums;

namespace Edubase.Web.UI.Models.Search
{
    public class DownloadErrorViewModel
    {
        public string SearchQueryString { get; set; }
        public eLookupSearchSource? SearchSource { get; set; }
        public bool NeedsRegenerating { get; set; }
        public eDownloadReturnSource? ReturnSource { get; set; }

        public bool FriendlyMessage { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorType { get; set; }
        public string ApiDetails { get; set; }
        public string NextSteps { get; set; }
    }
}
