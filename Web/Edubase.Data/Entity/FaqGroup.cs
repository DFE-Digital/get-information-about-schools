using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Data.Entity
{
    public class FaqGroup : TableEntity
    {
        public string GroupName { get; set; }
        public int DisplayOrder { get; set; }

        public FaqGroup()
        {
            PartitionKey = string.Empty;
            RowKey = Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        public FaqGroup ShallowCopy()
        {
            return (FaqGroup) this.MemberwiseClone();
        }
    }
}
