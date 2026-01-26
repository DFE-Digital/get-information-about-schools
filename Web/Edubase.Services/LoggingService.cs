using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Edubase.Common.Logging;

namespace Edubase.Services;

/// <summary>
/// Provides functionality for collecting, buffering, and persisting log messages
/// into Azure Table Storage. Logs are queued in memory and flushed asynchronously.
/// </summary>
public class LoggingService : ILoggingService
{
    private readonly TableClient _table;
    private readonly ConcurrentQueue<WebLogMessage> _pending = new();

    /// <summary>
    /// Gets a unique identifier for this logging service instance.
    /// Useful for distinguishing logs produced by different application instances.
    /// </summary>
    public string InstanceId { get; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingService"/> class.
    /// </summary>
    /// <param name="tableServiceClient">The Azure Table Service client.</param>
    /// <param name="tableName">The name of the table where log entries will be stored.</param>
    public LoggingService(
        TableServiceClient tableServiceClient,
        string tableName)
    {
        _table = tableServiceClient.GetTableClient(tableName);
        _table.CreateIfNotExists();
    }

    /// <summary>
    /// Retrieves all log messages that have been queued but not yet flushed to storage.
    /// </summary>
    /// <returns>A collection of pending <see cref="WebLogMessage"/> instances.</returns>
    public IEnumerable<WebLogMessage> GetPending() => _pending;

    /// <summary>
    /// Records a log message and immediately attempts to flush it to storage.
    /// </summary>
    /// <param name="message">The log message text.</param>
    /// <param name="category">The category of the log message. Defaults to "General".</param>
    /// <returns>A task representing the asynchronous logging operation.</returns>
    public async Task LogAsync(string message, string category = "General")
    {
        WebLogMessage log = new()
        {
            PartitionKey = category,
            RowKey = Guid.NewGuid().ToString(),
            TimestampUtc = DateTime.UtcNow,
            Message = message,
            Category = category
        };

        _pending.Enqueue(log);
        await FlushAsync();
    }

    /// <summary>
    /// Flushes all pending log messages to Azure Table Storage.
    /// </summary>
    /// <returns>A task representing the asynchronous flush operation.</returns>
    public async Task FlushAsync()
    {
        while (_pending.TryDequeue(out WebLogMessage log))
        {
            TableEntity entity = new(log.PartitionKey, log.RowKey)
            {
                [LogEntityColumns.Message] = log.Message,
                [LogEntityColumns.TimestampUtc] = log.TimestampUtc,
                [LogEntityColumns.Category] = log.Category
            };

            await _table.AddEntityAsync(entity);
        }
    }
}

/// <summary>
/// Defines the Azure Table Storage column names used for log entities.
/// </summary>
public static class LogEntityColumns
{
    /// <summary>The message text of the log entry.</summary>
    public const string Message = "Message";

    /// <summary>The UTC timestamp when the log entry was created.</summary>
    public const string TimestampUtc = "TimestampUtc";

    /// <summary>The category associated with the log entry.</summary>
    public const string Category = "Category";
}
