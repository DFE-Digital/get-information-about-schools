using System;
using System.Runtime.Serialization;
using Azure;
using Azure.Data.Tables;
using Edubase.Common;
using Edubase.Common.Formatting;

namespace Edubase.Data.Entity;

public class Token : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public string Data { get; set; }

    [IgnoreDataMember]
    public string Id => string.Concat(PartitionKey, RowKey);

    public Token() : this(null, null)
    {
    }

    public Token(string data, DateTime? date = null)
    {
        Data = data ?? string.Empty;

        string partitionKey = Base62.FromDate(date ?? DateTime.Now);

        PartitionKey = partitionKey.Length < 4
            ? partitionKey.PadLeft(4, '0')
            : partitionKey[..4];

        RowKey = Base62.Encode(RandomNumber.Next(1, 10_000_000));
    }
}
