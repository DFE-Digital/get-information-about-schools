using Azure;
using Edubase.Common;
using System;
using Azure.Data.Tables;
using ITableEntity = Azure.Data.Tables.ITableEntity;

namespace Edubase.Data.Entity;

public class ApiRecorderSessionItem : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public string HttpMethod { get; set; }
    public string Path { get; set; }
    public string RequestHeaders { get; set; }
    public string ResponseHeaders { get; set; }
    public string RawRequestBody { get; set; }
    public string RawResponseBody { get; set; }
    public string ElapsedTimeSpan { get; set; }

    /// <summary>
    /// Elapsed milliseconds
    /// </summary>
    public double ElapsedMS { get; set; }

    public ApiRecorderSessionItem() { }

    public ApiRecorderSessionItem(string sessionId, string requestPath)
    {
        PartitionKey = sessionId;
        RowKey = string.Concat(
            DateTime.UtcNow.ToString("yyyyMMdd-HHmmss"),
            "~~",
            requestPath.CleanOfNonChars(false, "-"),
            "~~",
            RandomNumber.Next(1, 10000)
        );
        Path = requestPath;
    }
}
