using Edubase.Services.Domain;
using Edubase.Services.Enums;

namespace Edubase.Web.UI.Models.Search
{
    public interface IDownloadGenerationProgressModel
    {
        int? Step { get; }
        int? TotalSteps { get; }
        string DownloadName { get; }
        string DownloadSource { get; }
        eFileFormat FileFormat { get; }
        ProgressDto Progress { get; }
        string SearchQueryString { get; set; }
        eLookupSearchSource? SearchSource { get; set; }
    }
}
