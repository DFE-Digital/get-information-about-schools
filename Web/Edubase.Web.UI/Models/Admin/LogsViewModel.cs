using System;
using System.Collections.Generic;
using System.ComponentModel;
using Edubase.Common.Logging;

namespace Edubase.Web.UI.Models.Admin;

/// <summary>
/// Represents the data required to display and filter log messages
/// within the Admin UI, including date range, search query, and
/// optional filtering of purge‑related log entries.
/// </summary>
public class LogsViewModel
{
    /// <summary>
    /// The collection of log messages returned for the current query and filters.
    /// </summary>
    public List<WebLogMessage> Messages { get; set; }

    /// <summary>
    /// A free‑text search query used to filter log messages across multiple fields.
    /// </summary>
    public string Query { get; set; }

    /// <summary>
    /// The start date used to filter log messages. Defaults to 28 days ago.
    /// </summary>
    public DateTimeViewModel StartDate { get; set; } =
        new DateTimeViewModel(DateTime.Today.AddDays(-28));

    /// <summary>
    /// The end date used to filter log messages. Defaults to today.
    /// </summary>
    public DateTimeViewModel EndDate { get; set; } =
        new DateTimeViewModel(DateTime.Today);

    /// <summary>
    /// Indicates whether log messages related to "zero logs purged" events
    /// should be included in the results.
    /// </summary>
    [DisplayName("Include log messages about purging zero logs?")]
    public bool IncludePurgeZeroLogsMessage { get; set; }
}
