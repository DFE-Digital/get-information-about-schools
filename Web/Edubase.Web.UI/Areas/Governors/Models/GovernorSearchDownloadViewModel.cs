using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Web.UI.Models.Search;
using System;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    public class GovernorSearchDownloadViewModel : GovernorSearchViewModel, IDownloadGenerationProgressModel
    {
        public eFileFormat? FileFormat { get; set; }
        public ProgressDto Progress { get { throw new Exception("Progress object not available"); } }
        public int? Step { get; set; }
        public int? TotalSteps { get; set; }
        public string DownloadName => "governor";
        eFileFormat IDownloadGenerationProgressModel.FileFormat => FileFormat.Value;
        public bool? IncludeNonPublicData { get; set; }
    }
}