using Edubase.Services.Domain;
using Edubase.Web.UI.Models.Search;
using System;
using static Edubase.Services.Core.FileDownloadFactoryService;
using static Edubase.Services.Establishments.Downloads.EstablishmentDownloadService;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    public class GovernorSearchDownloadViewModel : GovernorSearchViewModel, IDownloadGenerationProgressModel
    {
        public eFileFormat? FileFormat { get; set; }
        public SearchDownloadGenerationProgressDto Progress { get { throw new Exception("Progress object not available"); } }
        public int Step => 2;
        public int TotalSteps => 3;
        public string DownloadName => "governor";
    }
}