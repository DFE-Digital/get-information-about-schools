using System;
using System.Collections.Generic;
using System.ComponentModel;
using Edubase.Data.Entity;

namespace Edubase.Web.UI.Models.Admin
{
    public class LogsViewModel
    {
        public List<AZTLoggerMessages> Messages { get; set; }

        public string Query { get; set; }

        public DateTimeViewModel StartDate { get; set; } = new DateTimeViewModel(DateTime.Today.AddDays(-28)); // Default to 28 days ago

        public DateTimeViewModel EndDate { get; set; } = new DateTimeViewModel(DateTime.Today);


        [DisplayName("Include log messages about purging zero logs?")]
        public bool IncludePurgeZeroLogsMessage { get; set; }

    }
}
