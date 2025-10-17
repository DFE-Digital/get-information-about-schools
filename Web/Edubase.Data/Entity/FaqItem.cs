using System;
using Azure;
using Azure.Data.Tables;

namespace Edubase.Data.Entity;


public class FaqItem : ITableEntity
{
    public string Title { get; set; }
    public string Content { get; set; }
    public int DisplayOrder { get; set; }
    public string GroupId { get; set; }
    public string PartitionKey { get; set; } = string.Empty;
    public string RowKey { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8);
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public FaqItem ShallowCopy()
    {
        return (FaqItem) this.MemberwiseClone();
    }
}
