using System;
using Edubase.Services.Domain;
using Edubase.Web.UI.Models.Search;

namespace Edubase.Web.UI.Areas.Establishments.Models.Search
{
    public class EstablishmentSearchDownloadGenerationProgressViewModel : IDownloadGenerationProgressModel
    {
        public SearchDownloadGenerationProgressDto Progress { get; set; }
        public int Step { get; private set; }
        public int TotalSteps => 4;
        public string DownloadName => "establishment";
        

        public EstablishmentSearchDownloadGenerationProgressViewModel(SearchDownloadGenerationProgressDto progressDto, int step)
        {
            Progress = progressDto;
            Step = step;
        }
    }
}