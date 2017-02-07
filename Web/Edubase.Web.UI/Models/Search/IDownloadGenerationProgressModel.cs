using Edubase.Services.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Web.UI.Models.Search
{
    public interface IDownloadGenerationProgressModel
    {
        int Step { get; }
        int TotalSteps { get; }
        string DownloadName { get; }
        SearchDownloadGenerationProgressDto Progress { get; }
    }
}
