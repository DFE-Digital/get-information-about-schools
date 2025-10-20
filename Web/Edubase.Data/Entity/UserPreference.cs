using Azure;
using Azure.Data.Tables;
using System;
using System.Runtime.Serialization;

namespace Edubase.Data.Entity;

public class UserPreference : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public string SavedSearchToken { get; set; }

    [IgnoreDataMember]
    public string UserId
    {
        get => RowKey;
        set => RowKey = value;
    }

    public UserPreference() : this(string.Empty)
    {
        
    }
    public UserPreference(string userId)
    {
        PartitionKey = string.Empty;
        UserId = userId;
    }
}
