using Azure;
using Azure.Data.Tables;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.IO;
using System.Runtime.Serialization;

namespace Edubase.Data.Entity;

public class LocalAuthoritySet : ITableEntity
{
    public string PartitionKey { get; set; } = string.Empty;
    public string RowKey { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public string Title { get; set; }
    public byte[] IdData { get; set; }

    [IgnoreDataMember]
    public int[] Ids
    {
        get
        {
            if (IdData != null)
            {
                using MemoryStream ms = new(IdData);
                using BsonDataReader reader = new(ms)
                {
                    ReadRootValueAsArray = true
                };
                JsonSerializer serializer = new();
                return serializer.Deserialize<int[]>(reader);
            }

            return [];
        }
        set
        {
            if (value == null || value.Length == 0)
            {
                IdData = null;
            }
            else
            {
                using MemoryStream ms = new();
                using BsonDataWriter writer = new(ms);
                JsonSerializer serializer = new();
                serializer.Serialize(writer, value);
                ms.Seek(0, SeekOrigin.Begin);
                IdData = ms.ToArray();
            }
        }
    }
}
