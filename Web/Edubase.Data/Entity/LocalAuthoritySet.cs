using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.IO;

namespace Edubase.Data.Entity
{
    public class LocalAuthoritySet : TableEntity
    {
        public string Title { get; set; }
        
        public byte[] IdData { get; set; }

        /// <summary>
        /// The Get never returns null; always an empty array if anything.
        /// </summary>
        [IgnoreProperty]
        public int[] Ids
        {
            get
            {
                if (IdData != null)
                {
                    using (var ms = new MemoryStream(IdData))
                    {
                        using (var reader = new BsonDataReader(ms))
                        {
                            reader.ReadRootValueAsArray = true; // very important
                            var serializer = new JsonSerializer();
                            return serializer.Deserialize<int[]>(reader);
                        }
                    }
                }
                else return new int[0];
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    IdData = null;
                }
                else
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var writer = new BsonDataWriter(ms))
                        {
                            var serializer = new JsonSerializer();
                            serializer.Serialize(writer, value);
                            ms.Seek(0, SeekOrigin.Begin);
                            IdData = ms.ToArray();
                        }
                    }
                }
            }
        }

              

        public LocalAuthoritySet()
        {
            PartitionKey = string.Empty;
            RowKey = Guid.NewGuid().ToString("N").Substring(0, 8);
        }
    }
}
