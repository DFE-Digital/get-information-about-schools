using System;
using Edubase.Services.Domain;
using Edubase.Web.UI.Models.Search;
using Edubase.Services.Enums;

namespace Edubase.Web.UI.Areas.Groups.Models
{
    public class GroupSearchDownloadGenerationProgressViewModel : IDownloadGenerationProgressModel
    {
        public eFileFormat? FileFormat { get; set; }
        public ProgressDto Progress { get; set; }
        public string SearchQueryString { get; set; }
        public eLookupSearchSource? SearchSource { get; set; }
        public int? Step { get; set; } = 0;
        public int? TotalSteps => 3;
        public string DownloadName => "group";
        eFileFormat IDownloadGenerationProgressModel.FileFormat => FileFormat.Value;

        public GroupSearchDownloadGenerationProgressViewModel(ProgressDto progressDto)
        {
            Progress = progressDto;
        }

        public GroupSearchDownloadGenerationProgressViewModel SetStep(int step)
        {
            Step = step;
            return this;
        }
    }
}
