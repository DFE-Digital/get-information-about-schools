using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Common.Logging;

namespace Edubase.Services;

/// <summary>
/// Defines operations for collecting, buffering, and persisting application log messages.
/// </summary>
public interface ILoggingService
{
    /// <summary>
    /// Retrieves all log messages that have been collected but not yet flushed
    /// to the underlying storage mechanism.
    /// </summary>
    /// <returns>
    /// A collection of <see cref="WebLogMessage"/> instances representing pending log entries.
    /// </returns>
    IEnumerable<WebLogMessage> GetPending();

    /// <summary>
    /// Gets a unique identifier for the current logging service instance.
    /// Useful for distinguishing logs produced by different application instances.
    /// </summary>
    string InstanceId { get; }

    /// <summary>
    /// Persists all pending log messages to the underlying storage system.
    /// </summary>
    /// <returns>A task representing the asynchronous flush operation.</returns>
    Task FlushAsync();

    /// <summary>
    /// Records a log message with an optional category.
    /// </summary>
    /// <param name="message">The log message text.</param>
    /// <param name="category">
    /// The category of the log message. Defaults to <c>"General"</c>.
    /// </param>
    /// <returns>A task representing the asynchronous logging operation.</returns>
    Task LogAsync(string message, string category = "General");
}
