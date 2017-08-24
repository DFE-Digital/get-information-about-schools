using Edubase.Common;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Edubase.Data.Entity
{
    public class GlossaryItem : TableEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }

        public GlossaryItem()
        {
            PartitionKey = string.Empty;
            RowKey = Guid.NewGuid().ToString("N").Substring(0, 8);
        }
    }
}
