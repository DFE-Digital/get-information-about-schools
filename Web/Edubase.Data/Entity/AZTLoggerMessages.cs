using Azure;
using Azure.Data.Tables;
using System;

namespace Edubase.Data.Entity;

public class AZTLoggerMessages : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public DateTime DateUtc { get; set; }

    public string Environment { get; set; }

    public string Exception { get; set; }

    public string Level { get; set; }

    public string Message { get; set; }

    public string Id => RowKey + PartitionKey;

    public string ClientIpAddress { get; set; }

    public string HttpMethod { get; set; }

    public string ReferrerUrl { get; set; }

    public string RequestJsonBody { get; set; }

    public string Url { get; set; }

    public string UserAgent { get; set; }

    public string UserId { get; set; }

    public string UserName { get; set; }
}
