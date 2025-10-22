using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Web.UI.Models.Search;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    public class GovernorSearchDownloadGenerationProgressViewModel : IDownloadGenerationProgressModel
    {
        public eFileFormat? FileFormat { get; set; }
        public ProgressDto Progress { get; set; }
        public string SearchQueryString { get; set; }
        public eLookupSearchSource? SearchSource { get; set; }
        public int? Step { get; set; }
        public int? TotalSteps { get; set; }
        public string DownloadName => "governor";
        eFileFormat IDownloadGenerationProgressModel.FileFormat => FileFormat.Value;

        public GovernorSearchDownloadGenerationProgressViewModel(ProgressDto progressDto)
        {
            Progress = progressDto;
        }


    }
}
