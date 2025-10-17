using System;
using Azure;
using Azure.Data.Tables;

namespace Edubase.Data.Entity;

public class FaqGroup : ITableEntity
{
    public string PartitionKey { get; set; } = string.Empty;
    public string RowKey { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8);
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public string GroupName { get; set; }
    public int DisplayOrder { get; set; }

    public FaqGroup() { }

    public FaqGroup ShallowCopy()
    {
        return (FaqGroup) this.MemberwiseClone();
    }
}
