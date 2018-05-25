using Edubase.Common;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Edubase.Data.Entity
{
    public class FaqItem : TableEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int DisplayOrder { get; set; }
        public string TitleFontSize { get; set; }

        public FaqItem()
        {
            PartitionKey = string.Empty;
            RowKey = Guid.NewGuid().ToString("N").Substring(0, 8);
        }
    }
}
