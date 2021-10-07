using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Edubase.Data.Entity
{

    public class FaqItem : TableEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int DisplayOrder { get; set; }
        public string GroupId { get; set; }

        public FaqItem()
        {
            PartitionKey = string.Empty;
            RowKey = Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        public FaqItem ShallowCopy()
        {
            return (FaqItem) this.MemberwiseClone();
        }
    }
}
