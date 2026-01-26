using System.Collections.Generic;
using Edubase.Common.Logging;

namespace Edubase.Services.Domain;

/// <summary>
/// Represents a paginated collection of log messages returned from storage.
/// </summary>
public class LogMessagesDto
{
    /// <summary>
    /// A continuation token used to retrieve the next page of results.
    /// If null, no further pages are available.
    /// </summary>
    public string SkipToken { get; set; }

    /// <summary>
    /// The collection of log messages returned for the current page.
    /// </summary>
    public IEnumerable<WebLogMessage> Items { get; set; }
}
