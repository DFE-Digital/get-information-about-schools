using static Edubase.Services.Core.FileDownloadFactoryService;
using static Edubase.Services.Establishments.Downloads.EstablishmentDownloadService;

namespace Edubase.Web.UI.Models.Search
{
    public class AdvancedSearchDownloadViewModel : AdvancedSearchViewModel
    {
        public eFileFormat? FileFormat { get; set; }

        public eDataSet? Dataset { get; set; }
    }
}