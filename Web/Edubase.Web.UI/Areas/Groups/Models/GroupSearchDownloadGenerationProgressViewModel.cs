using System;
using Edubase.Services.Domain;
using Edubase.Web.UI.Models.Search;

namespace Edubase.Web.UI.Areas.Groups.Models
{
    public class GroupSearchDownloadGenerationProgressViewModel : IDownloadGenerationProgressModel
    {
        public SearchDownloadGenerationProgressDto Progress { get; set; }
        public int Step { get; private set; }
        public int TotalSteps => 3;
        public string DownloadName => "group";
        

        public GroupSearchDownloadGenerationProgressViewModel(SearchDownloadGenerationProgressDto progressDto, int step)
        {
            Progress = progressDto;
            Step = step;
        }
    }
}