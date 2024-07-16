using System;
using System.Collections.Generic;
using Edubase.Data.Entity;

namespace Edubase.Web.UI.Models.Admin
{
    public class LogsViewModel
    {
        public List<AZTLoggerMessages> Messages { get; set; }

        public string Query { get; set; }

        public int? StartDateDay { get; set; }
        public int? StartDateMonth { get; set; }
        public int? StartDateYear { get; set; }

        public int? EndDateDay { get; set; }
        public int? EndDateMonth { get; set; }
        public int? EndDateYear { get; set; }

        [System.ComponentModel.DisplayName("Include log messages about purging zero logs?")]
        public bool IncludePurgeZeroLogsMessage { get; set; }

        // We can create properties for StartDate and EndDate that use the separate day, month and year values
        public DateTime? StartDate
        {
            get
            {
                if (StartDateDay.HasValue && StartDateMonth.HasValue && StartDateYear.HasValue)
                {
                    return new DateTime(StartDateYear.Value, StartDateMonth.Value, StartDateDay.Value);
                }

                return null;
            }
        }

        public DateTime? EndDate
        {
            get
            {
                if (EndDateDay.HasValue && EndDateMonth.HasValue && EndDateYear.HasValue)
                {
                    return new DateTime(EndDateYear.Value, EndDateMonth.Value, EndDateDay.Value);
                }

                return null;
            }
        }
    }
}
