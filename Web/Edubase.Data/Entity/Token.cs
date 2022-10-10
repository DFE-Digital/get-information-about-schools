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

        public Token(string data) : this()
        {
            Data = data;
        }

        public Token() : this(DateTime.Now)
        {

        }

        public Token(DateTime tokenDate)
        {
            var partitionKey = Base62.FromDate(tokenDate);
            if (partitionKey.Length < 4)
            {
                partitionKey = partitionKey.PadLeft(4, '0');
            }
            else
            {
                partitionKey = partitionKey.Substring(0, 4);
            }


            PartitionKey = partitionKey;
            RowKey = Base62.Encode(RandomNumber.Next(1, 10_000_000));
        }
    }
}
