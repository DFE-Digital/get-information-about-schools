using Azure;
using Azure.Data.Tables;
using System;

namespace Edubase.Data.Entity;

public class GlossaryItem : ITableEntity
{
    public string PartitionKey { get; set; } = string.Empty;
    public string RowKey { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public string Title { get; set; }
    public string Content { get; set; }
}
