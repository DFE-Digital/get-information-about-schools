using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Web.UI.Models.Search;
using System;

namespace Edubase.Web.UI.Areas.Groups.Models
{
    public class GroupSearchDownloadViewModel : GroupSearchViewModel, IDownloadGenerationProgressModel
    {
        public eFileFormat? FileFormat { get; set; }
        public ProgressDto Progress { get { throw new Exception("Progress object not available"); } }
        public int? Step { get; set; } = 0;
        public int? TotalSteps => 3;
        public string DownloadName => "group";
        eFileFormat IDownloadGenerationProgressModel.FileFormat => FileFormat.Value;
    }
}
