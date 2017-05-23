using System;
using Edubase.Services.Domain;
using Edubase.Web.UI.Models.Search;
using Edubase.Services.Enums;

namespace Edubase.Web.UI.Areas.Establishments.Models.Search
{
    public class EstablishmentSearchDownloadGenerationProgressViewModel : IDownloadGenerationProgressModel
    {
        public ProgressDto Progress { get; set; }
        public string SearchQueryString { get; set; }
        public eLookupSearchSource? SearchSource { get; set; }
        public int Step { get; private set; }
        public int TotalSteps => 4;
        public string DownloadName => "establishment";

        public eFileFormat FileFormat { get; set; }


        public EstablishmentSearchDownloadGenerationProgressViewModel(ProgressDto progressDto, int step)
        {
            Progress = progressDto;
            Step = step;
        }
    }
}