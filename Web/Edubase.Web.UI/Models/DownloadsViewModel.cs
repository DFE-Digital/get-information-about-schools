using Edubase.Services.Downloads.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models
{
    public class DownloadsViewModel
    {
        public FileDownload[] Downloads { get; set; }

        public ScheduledExtractsResult ScheduledExtracts { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public int ScheduledExtractsCount => (ScheduledExtracts?.Count).GetValueOrDefault();

        public bool AreScheduledExtractsAvailable => ScheduledExtractsCount > 0;
    }
}