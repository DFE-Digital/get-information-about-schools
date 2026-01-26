using System.Collections.Generic;
using Edubase.Common.Logging;

namespace Edubase.Web.UI.Models.Admin;

/// <summary>
/// Represents the data required to display log messages in the Admin UI,
/// including pagination and optional date filtering.
/// </summary>
public class LogMessagesViewModel
{
    /// <summary>
    /// The collection of log messages to display.
    /// </summary>
    public IEnumerable<WebLogMessage> Messages { get; set; }

    /// <summary>
    /// A continuation token used for retrieving the next page of log messages.
    /// If null, no further pages are available.
    /// </summary>
    public string SkipToken { get; set; }

    /// <summary>
    /// A date filter value (typically user‑entered) used to constrain the log results.
    /// </summary>
    public string DateFilter { get; set; }
}
