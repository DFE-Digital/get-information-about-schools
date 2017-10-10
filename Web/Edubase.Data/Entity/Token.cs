using Edubase.Common;
using Edubase.Common.Formatting;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Edubase.Data.Entity
{
    public class Token : TableEntity
    {
        public string Data { get; set; }

        [IgnoreProperty]
        public string Id => string.Concat(PartitionKey, RowKey);

        public Token()
        {
            PartitionKey = Base62.FromCurrentDate();
            RowKey = Base62.Encode(RandomNumber.Next(1, 10_000_000));
        }

        public Token(string data) : this()
        {
            Data = data;
        }
    }
}
