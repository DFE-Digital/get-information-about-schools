using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models.Search
{
    public class AdvancedSearchDownloadViewModel : AdvancedSearchViewModel
    {
        public enum eFileFormat
        {
            CSV,
            XLSX
        }

        public eFileFormat? FileFormat { get; set; }

        public Services.Establishments.Downloads.EstablishmentDownloadService.eDataSet? Dataset { get; set; }
    }
}