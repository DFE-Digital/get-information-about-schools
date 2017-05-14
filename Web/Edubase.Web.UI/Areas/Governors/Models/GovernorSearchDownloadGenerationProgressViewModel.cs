using System;
using Edubase.Services.Domain;
using Edubase.Web.UI.Models.Search;
using Edubase.Services.Enums;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    public class GovernorSearchDownloadGenerationProgressViewModel : IDownloadGenerationProgressModel
    {
        public eFileFormat? FileFormat { get; set; }
        public SearchDownloadGenerationProgressDto Progress { get; set; }
        public int Step { get; private set; }
        public int TotalSteps => 3;
        public string DownloadName => "governor";

        eFileFormat IDownloadGenerationProgressModel.FileFormat => FileFormat.Value;

        public GovernorSearchDownloadGenerationProgressViewModel(SearchDownloadGenerationProgressDto progressDto, int step)
        {
            Progress = progressDto;
            Step = step;
        }
    }
}